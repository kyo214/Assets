using System;
using System.IO;
using System.Security;
using NPOI.Util;

namespace NPOI.POIFS.Crypt;

public abstract class ChunkedCipherInputStream : LittleEndianInputStream
{
	private int chunkSize;

	private int chunkMask;

	private int chunkBits;

	private int _lastIndex;

	private long _pos;

	private long _size;

	private byte[] _chunk;

	private Cipher _cipher;

	protected IEncryptionInfoBuilder builder;

	protected Decryptor decryptor;

	public ChunkedCipherInputStream(ILittleEndianInput stream, long size, int chunkSize, IEncryptionInfoBuilder builder, Decryptor decryptor)
		: base((Stream)stream)
	{
		_size = size;
		this.chunkSize = chunkSize;
		chunkMask = chunkSize - 1;
		chunkBits = Number.BitCount(chunkMask);
		this.builder = builder;
		this.decryptor = decryptor;
		_cipher = InitCipherForBlock(null, 0);
	}

	protected abstract Cipher InitCipherForBlock(Cipher existing, int block);

	public int Read()
	{
		byte[] array = new byte[1];
		if (Read(array) == 1)
		{
			return array[0];
		}
		return -1;
	}

	public new int Read(byte[] b, int off, int len)
	{
		int num = 0;
		if (Available() <= 0)
		{
			return -1;
		}
		while (len > 0)
		{
			if (_chunk == null)
			{
				try
				{
					_chunk = NextChunk();
				}
				catch (SecurityException ex)
				{
					throw new EncryptedDocumentException(ex.Message, ex);
				}
			}
			int val = (int)(chunkSize - (_pos & chunkMask));
			int num2 = Available();
			if (num2 == 0)
			{
				return num;
			}
			val = Math.Min(num2, Math.Min(val, len));
			Array.Copy(_chunk, (int)(_pos & chunkMask), b, off, val);
			off += val;
			len -= val;
			_pos += val;
			if ((_pos & chunkMask) == 0L)
			{
				_chunk = null;
			}
			num += val;
		}
		return num;
	}

	public new long Skip(long n)
	{
		long pos = _pos;
		long num = Math.Min(Available(), n);
		if ((((_pos + num) ^ pos) & ~chunkMask) != 0L)
		{
			_chunk = null;
		}
		_pos += num;
		return num;
	}

	public new int Available()
	{
		return (int)(_size - _pos);
	}

	public bool MarkSupported()
	{
		return false;
	}

	public new void Mark(int Readlimit)
	{
		throw new InvalidOperationException();
	}

	public new void Reset()
	{
		throw new InvalidOperationException();
	}

	private byte[] NextChunk()
	{
		int num = (int)(_pos >> chunkBits);
		InitCipherForBlock(_cipher, num);
		if (_lastIndex != num)
		{
			base.Skip((long)(num - _lastIndex << chunkBits));
		}
		byte[] array = new byte[Math.Min(base.Available(), chunkSize)];
		base.Read(array, 0, array.Length);
		_lastIndex = num + 1;
		return _cipher.DoFinal(array);
	}
}
