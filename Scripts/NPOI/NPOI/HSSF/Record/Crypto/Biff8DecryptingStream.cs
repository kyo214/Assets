using System;
using System.IO;
using NPOI.Util;

namespace NPOI.HSSF.Record.Crypto;

public class Biff8DecryptingStream : BiffHeaderInput, ILittleEndianInput
{
	private ILittleEndianInput _le;

	private Biff8RC4 _rc4;

	public Biff8DecryptingStream(Stream in1, int InitialOffSet, Biff8EncryptionKey key)
	{
		_rc4 = new Biff8RC4(InitialOffSet, key);
		if (in1 is ILittleEndianInput)
		{
			_le = (ILittleEndianInput)in1;
		}
		else
		{
			_le = new LittleEndianInputStream(in1);
		}
	}

	public int Available()
	{
		return _le.Available();
	}

	public int ReadRecordSID()
	{
		int num = _le.ReadUShort();
		_rc4.SkipTwoBytes();
		_rc4.StartRecord(num);
		return num;
	}

	public int ReadDataSize()
	{
		int result = _le.ReadUShort();
		_rc4.SkipTwoBytes();
		return result;
	}

	public double ReadDouble()
	{
		double num = BitConverter.Int64BitsToDouble(ReadLong());
		if (double.IsNaN(num))
		{
			throw new Exception("Did not expect to read NaN");
		}
		return num;
	}

	public void ReadFully(byte[] buf)
	{
		ReadFully(buf, 0, buf.Length);
	}

	public void ReadFully(byte[] buf, int off, int len)
	{
		_le.ReadFully(buf, off, len);
		_rc4.Xor(buf, off, len);
	}

	public int ReadUByte()
	{
		return ReadByte() & 0xFF;
	}

	public int ReadByte()
	{
		return _rc4.XorByte(_le.ReadUByte());
	}

	public int ReadUShort()
	{
		return ReadShort() & 0xFFFF;
	}

	public short ReadShort()
	{
		return (short)_rc4.Xorshort(_le.ReadUShort());
	}

	public int ReadInt()
	{
		return _rc4.XorInt(_le.ReadInt());
	}

	public long ReadLong()
	{
		return _rc4.XorLong(_le.ReadLong());
	}
}
