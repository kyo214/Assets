using System;
using System.IO;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public abstract class ChunkedCipherOutputStream : LittleEndianOutputStream
{
	private class EncryptedPackageWriter : POIFSWriterListener
	{
		private ChunkedCipherOutputStream stream;

		public EncryptedPackageWriter(ChunkedCipherOutputStream stream)
		{
			this.stream = stream;
		}

		public void ProcessPOIFSWriterEvent(POIFSWriterEvent event1)
		{
			try
			{
				DocumentOutputStream documentOutputStream = event1.Stream;
				byte[] array = new byte[stream.chunkSize];
				LittleEndian.PutLong(array, 0, stream._pos);
				documentOutputStream.Write(array, 0, 8);
				FileStream fileStream = stream.fileOut.Create();
				int count;
				while ((count = fileStream.Read(array, 0, array.Length)) != -1)
				{
					documentOutputStream.Write(array, 0, count);
				}
				fileStream.Close();
				documentOutputStream.Close();
				stream.fileOut.Delete();
			}
			catch (IOException cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}
	}

	protected int chunkSize;

	protected int chunkMask;

	protected int chunkBits;

	private byte[] _chunk;

	private FileInfo fileOut;

	private DirectoryNode dir;

	private long _pos;

	private Cipher _cipher;

	protected IEncryptionInfoBuilder builder;

	protected Encryptor encryptor;

	public Stream GetStream()
	{
		return out1;
	}

	public ChunkedCipherOutputStream(DirectoryNode dir, int chunkSize, IEncryptionInfoBuilder builder, Encryptor encryptor)
		: base(null)
	{
		this.chunkSize = chunkSize;
		chunkMask = chunkSize - 1;
		chunkBits = Number.BitCount(chunkMask);
		_chunk = new byte[chunkSize];
		fileOut = TempFile.CreateTempFile("encrypted_package", "crypt");
		out1 = fileOut.Create();
		this.dir = dir;
		this.builder = builder;
		this.encryptor = encryptor;
		_cipher = InitCipherForBlock(null, 0, lastChunk: false);
	}

	protected abstract Cipher InitCipherForBlock(Cipher existing, int block, bool lastChunk);

	protected abstract void CalculateChecksum(FileInfo fileOut, int oleStreamSize);

	protected abstract void CreateEncryptionInfoEntry(DirectoryNode dir, FileInfo tmpFile);

	public void Write(int b)
	{
		Write(new byte[1] { (byte)b });
	}

	public new void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public new void Write(byte[] b, int off, int len)
	{
		if (len == 0)
		{
			return;
		}
		if (len < 0 || b.Length < off + len)
		{
			throw new IOException("not enough bytes in your input buffer");
		}
		while (len > 0)
		{
			int num = (int)(_pos & chunkMask);
			int num2 = Math.Min(chunkSize - num, len);
			Array.Copy(b, off, _chunk, num, num2);
			_pos += num2;
			off += num2;
			len -= num2;
			if ((_pos & chunkMask) == 0L)
			{
				try
				{
					WriteChunk();
				}
				catch (Exception ex)
				{
					throw new IOException(ex.Message);
				}
			}
		}
	}

	protected void WriteChunk()
	{
		int num = (int)(_pos & chunkMask);
		int num2 = (int)(_pos >> chunkBits);
		bool lastChunk;
		if (num == 0)
		{
			num2--;
			num = chunkSize;
			lastChunk = false;
		}
		else
		{
			lastChunk = true;
		}
		_cipher = InitCipherForBlock(_cipher, num2, lastChunk);
		int count = _cipher.DoFinal(_chunk, 0, num, _chunk);
		out1.Write(_chunk, 0, count);
	}

	public new void Close()
	{
		try
		{
			WriteChunk();
			base.Close();
			int size = (int)(fileOut.Length + 8);
			CalculateChecksum(fileOut, (int)_pos);
			dir.CreateDocument(Decryptor.DEFAULT_POIFS_ENTRY, size, new EncryptedPackageWriter(this));
			CreateEncryptionInfoEntry(dir, fileOut);
		}
		catch (Exception ex)
		{
			throw new IOException(ex.Message);
		}
	}
}
