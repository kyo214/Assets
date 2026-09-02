using System;
using System.IO;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.CryptoAPI;

public class CryptoAPIDecryptor : Decryptor
{
	private class SeekableMemoryStream : MemoryStream
	{
		private Cipher cipher;

		private byte[] oneByte = new byte[1];

		public void Seek(int pos)
		{
			if (pos > Length)
			{
				throw new IndexOutOfRangeException($"seek position({pos}) is greater than stream length({Length})");
			}
			throw new NotImplementedException();
		}

		public void SetBlock(int block)
		{
			throw new NotImplementedException();
		}

		public int Read()
		{
			int num = ReadByte();
			if (num == -1)
			{
				return -1;
			}
			oneByte[0] = (byte)num;
			try
			{
				cipher.Update(oneByte, 0, 1, oneByte);
			}
			catch (Exception cause)
			{
				throw new EncryptedDocumentException(cause);
			}
			return oneByte[0];
		}

		public override int Read(byte[] b, int off, int len)
		{
			int num = base.Read(b, off, len);
			if (num == -1)
			{
				return -1;
			}
			try
			{
				cipher.Update(b, off, num, b, off);
				return num;
			}
			catch (Exception cause)
			{
				throw new EncryptedDocumentException(cause);
			}
		}

		public SeekableMemoryStream(byte[] buf)
			: base(buf)
		{
			throw new NotImplementedException();
		}
	}

	internal class StreamDescriptorEntry
	{
		internal static BitField flagStream = BitFieldFactory.GetInstance(1);

		internal int streamOffset;

		internal int streamSize;

		internal int block;

		internal int flags;

		internal int reserved2;

		internal string streamName;
	}

	private long _length;

	protected internal CryptoAPIDecryptor(CryptoAPIEncryptionInfoBuilder builder)
		: base(builder)
	{
		_length = -1L;
	}

	public override bool VerifyPassword(string password)
	{
		EncryptionVerifier encryptionVerifier = builder.GetVerifier();
		ISecretKey skey = GenerateSecretKey(password, encryptionVerifier);
		try
		{
			Cipher cipher = InitCipherForBlock(null, 0, builder, skey, Cipher.DECRYPT_MODE);
			byte[] encryptedVerifier = encryptionVerifier.EncryptedVerifier;
			byte[] array = new byte[encryptedVerifier.Length];
			cipher.Update(encryptedVerifier, 0, encryptedVerifier.Length, array);
			SetVerifier(array);
			byte[] encryptedVerifierHash = encryptionVerifier.EncryptedVerifierHash;
			byte[] b = cipher.DoFinal(encryptedVerifierHash);
			if (Arrays.Equals(CryptoFunctions.GetMessageDigest(encryptionVerifier.HashAlgorithm).Digest(array), b))
			{
				SetSecretKey(skey);
				return true;
			}
		}
		catch (Exception cause)
		{
			throw new EncryptedDocumentException(cause);
		}
		return false;
	}

	public Cipher InitCipherForBlock(Cipher cipher, int block)
	{
		return InitCipherForBlock(cipher, block, builder, GetSecretKey(), Cipher.DECRYPT_MODE);
	}

	protected internal static Cipher InitCipherForBlock(Cipher cipher, int block, IEncryptionInfoBuilder builder, ISecretKey skey, int encryptMode)
	{
		HashAlgorithm hashAlgorithm = builder.GetVerifier().HashAlgorithm;
		byte[] array = new byte[4];
		LittleEndian.PutUInt(array, 0, block);
		MessageDigest messageDigest = CryptoFunctions.GetMessageDigest(hashAlgorithm);
		messageDigest.Update(skey.GetEncoded());
		byte[] hash = messageDigest.Digest(array);
		EncryptionHeader header = builder.GetHeader();
		int keySize = header.KeySize;
		hash = CryptoFunctions.GetBlock0(hash, keySize / 8);
		if (keySize == 40)
		{
			hash = CryptoFunctions.GetBlock0(hash, 16);
		}
		ISecretKey key = new SecretKeySpec(hash, skey.GetAlgorithm());
		if (cipher == null)
		{
			cipher = CryptoFunctions.GetCipher(key, header.CipherAlgorithm, null, null, encryptMode);
		}
		else
		{
			cipher.Init(encryptMode, key);
		}
		return cipher;
	}

	protected internal static ISecretKey GenerateSecretKey(string password, EncryptionVerifier ver)
	{
		if (password.Length > 255)
		{
			password = password.Substring(0, 255);
		}
		MessageDigest messageDigest = CryptoFunctions.GetMessageDigest(ver.HashAlgorithm);
		messageDigest.Update(ver.Salt);
		return new SecretKeySpec(messageDigest.Digest(StringUtil.GetToUnicodeLE(password)), ver.CipherAlgorithm.jceId);
	}

	public override InputStream GetDataStream(DirectoryNode dir)
	{
		NPOIFSFileSystem nPOIFSFileSystem = new NPOIFSFileSystem();
		DocumentNode document = (DocumentNode)dir.GetEntry("EncryptedSummary");
		DocumentInputStream documentInputStream = dir.CreateDocumentInputStream(document);
		MemoryStream memoryStream = new MemoryStream();
		IOUtils.Copy(documentInputStream, memoryStream);
		documentInputStream.Close();
		SeekableMemoryStream seekableMemoryStream = new SeekableMemoryStream(memoryStream.ToArray());
		LittleEndianInputStream littleEndianInputStream = new LittleEndianInputStream(seekableMemoryStream);
		int num = (int)littleEndianInputStream.ReadUInt();
		littleEndianInputStream.ReadUInt();
		seekableMemoryStream.Seek(num - 8, SeekOrigin.Current);
		seekableMemoryStream.SetBlock(0);
		int num2 = (int)littleEndianInputStream.ReadUInt();
		StreamDescriptorEntry[] array = new StreamDescriptorEntry[num2];
		for (int i = 0; i < num2; i++)
		{
			StreamDescriptorEntry streamDescriptorEntry = (array[i] = new StreamDescriptorEntry());
			streamDescriptorEntry.streamOffset = (int)littleEndianInputStream.ReadUInt();
			streamDescriptorEntry.streamSize = (int)littleEndianInputStream.ReadUInt();
			streamDescriptorEntry.block = littleEndianInputStream.ReadUShort();
			int nChars = littleEndianInputStream.ReadUByte();
			streamDescriptorEntry.flags = littleEndianInputStream.ReadUByte();
			StreamDescriptorEntry.flagStream.IsSet(streamDescriptorEntry.flags);
			streamDescriptorEntry.reserved2 = littleEndianInputStream.ReadInt();
			streamDescriptorEntry.streamName = StringUtil.ReadUnicodeLE(littleEndianInputStream, nChars);
			littleEndianInputStream.ReadShort();
		}
		StreamDescriptorEntry[] array2 = array;
		foreach (StreamDescriptorEntry streamDescriptorEntry2 in array2)
		{
			seekableMemoryStream.Seek(streamDescriptorEntry2.streamOffset);
			seekableMemoryStream.SetBlock(streamDescriptorEntry2.block);
			Stream stream = new BufferedStream(seekableMemoryStream, streamDescriptorEntry2.streamSize);
			nPOIFSFileSystem.CreateDocument(stream, streamDescriptorEntry2.streamName);
		}
		littleEndianInputStream.Close();
		seekableMemoryStream = null;
		memoryStream.Seek(0L, SeekOrigin.Begin);
		nPOIFSFileSystem.WriteFileSystem(memoryStream);
		nPOIFSFileSystem.Close();
		_length = memoryStream.Length;
		new ByteArrayInputStream(memoryStream.ToArray());
		throw new NotImplementedException("ByteArrayInputStream should be derived from InputStream");
	}

	public override long GetLength()
	{
		if (_length == -1)
		{
			throw new InvalidOperationException("Decryptor.DataStream was not called");
		}
		return _length;
	}
}
