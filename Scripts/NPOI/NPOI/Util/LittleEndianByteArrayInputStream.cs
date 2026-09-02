using System;

namespace NPOI.Util;

public class LittleEndianByteArrayInputStream : ILittleEndianInput
{
	private byte[] _buf;

	private int _endIndex;

	private int _ReadIndex;

	public LittleEndianByteArrayInputStream(byte[] buf, int startOffset, int maxReadLen)
	{
		_buf = buf;
		_ReadIndex = startOffset;
		_endIndex = startOffset + maxReadLen;
	}

	public LittleEndianByteArrayInputStream(byte[] buf, int startOffset)
		: this(buf, startOffset, buf.Length - startOffset)
	{
	}

	public LittleEndianByteArrayInputStream(byte[] buf)
		: this(buf, 0, buf.Length)
	{
	}

	public int Available()
	{
		return _endIndex - _ReadIndex;
	}

	private void CheckPosition(int i)
	{
		if (i > _endIndex - _ReadIndex)
		{
			throw new RuntimeException("Buffer overrun");
		}
	}

	public int GetReadIndex()
	{
		return _ReadIndex;
	}

	public int ReadByte()
	{
		CheckPosition(1);
		return _buf[_ReadIndex++];
	}

	public int ReadInt()
	{
		CheckPosition(4);
		int readIndex = _ReadIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		int num3 = _buf[readIndex++] & 0xFF;
		int num4 = _buf[readIndex++] & 0xFF;
		_ReadIndex = readIndex;
		return (num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public long ReadLong()
	{
		CheckPosition(8);
		int readIndex = _ReadIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		int num3 = _buf[readIndex++] & 0xFF;
		int num4 = _buf[readIndex++] & 0xFF;
		int num5 = _buf[readIndex++] & 0xFF;
		int num6 = _buf[readIndex++] & 0xFF;
		int num7 = _buf[readIndex++] & 0xFF;
		int num8 = _buf[readIndex++] & 0xFF;
		_ReadIndex = readIndex;
		return ((long)num8 << 56) + ((long)num7 << 48) + ((long)num6 << 40) + ((long)num5 << 32) + ((long)num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public short ReadShort()
	{
		return (short)ReadUShort();
	}

	public int ReadUByte()
	{
		CheckPosition(1);
		return _buf[_ReadIndex++] & 0xFF;
	}

	public int ReadUShort()
	{
		CheckPosition(2);
		int readIndex = _ReadIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		_ReadIndex = readIndex;
		return (num2 << 8) + num;
	}

	public void ReadFully(byte[] buf, int off, int len)
	{
		CheckPosition(len);
		Array.Copy(_buf, _ReadIndex, buf, off, len);
		_ReadIndex += len;
	}

	public void ReadFully(byte[] buf)
	{
		ReadFully(buf, 0, buf.Length);
	}

	public double ReadDouble()
	{
		return BitConverter.Int64BitsToDouble(ReadLong());
	}
}
