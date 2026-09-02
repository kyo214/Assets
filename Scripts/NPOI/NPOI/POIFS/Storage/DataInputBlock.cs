using System;

namespace NPOI.POIFS.Storage;

public class DataInputBlock
{
	private byte[] _buf;

	private int _readIndex;

	private int _maxIndex;

	internal DataInputBlock(byte[] data, int startOffset)
	{
		_buf = data;
		_readIndex = startOffset;
		_maxIndex = _buf.Length;
	}

	public int Available()
	{
		return _maxIndex - _readIndex;
	}

	public int ReadUByte()
	{
		return _buf[_readIndex++] & 0xFF;
	}

	public int ReadUshortLE()
	{
		int readIndex = _readIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		_readIndex = readIndex;
		return (num2 << 8) + num;
	}

	public int ReadUshortLE(DataInputBlock prevBlock)
	{
		int num = prevBlock._buf.Length - 1;
		int num2 = prevBlock._buf[num] & 0xFF;
		return ((_buf[_readIndex++] & 0xFF) << 8) + num2;
	}

	public int ReadIntLE()
	{
		int readIndex = _readIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		int num3 = _buf[readIndex++] & 0xFF;
		int num4 = _buf[readIndex++] & 0xFF;
		_readIndex = readIndex;
		return (num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public int ReadIntLE(DataInputBlock prevBlock, int prevBlockAvailable)
	{
		byte[] array = new byte[4];
		ReadSpanning(prevBlock, prevBlockAvailable, array);
		int num = array[0] & 0xFF;
		int num2 = array[1] & 0xFF;
		int num3 = array[2] & 0xFF;
		return ((array[3] & 0xFF) << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public long ReadLongLE()
	{
		int readIndex = _readIndex;
		int num = _buf[readIndex++] & 0xFF;
		int num2 = _buf[readIndex++] & 0xFF;
		int num3 = _buf[readIndex++] & 0xFF;
		int num4 = _buf[readIndex++] & 0xFF;
		int num5 = _buf[readIndex++] & 0xFF;
		int num6 = _buf[readIndex++] & 0xFF;
		int num7 = _buf[readIndex++] & 0xFF;
		int num8 = _buf[readIndex++] & 0xFF;
		_readIndex = readIndex;
		return ((long)num8 << 56) + ((long)num7 << 48) + ((long)num6 << 40) + ((long)num5 << 32) + ((long)num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public long ReadLongLE(DataInputBlock prevBlock, int prevBlockAvailable)
	{
		byte[] array = new byte[8];
		ReadSpanning(prevBlock, prevBlockAvailable, array);
		int num = array[0] & 0xFF;
		int num2 = array[1] & 0xFF;
		int num3 = array[2] & 0xFF;
		int num4 = array[3] & 0xFF;
		int num5 = array[4] & 0xFF;
		int num6 = array[5] & 0xFF;
		int num7 = array[6] & 0xFF;
		return ((long)(array[7] & 0xFF) << 56) + ((long)num7 << 48) + ((long)num6 << 40) + ((long)num5 << 32) + ((long)num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	private void ReadSpanning(DataInputBlock prevBlock, int prevBlockAvailable, byte[] buf)
	{
		Array.Copy(prevBlock._buf, prevBlock._readIndex, buf, 0, prevBlockAvailable);
		int num = buf.Length - prevBlockAvailable;
		Array.Copy(_buf, 0, buf, prevBlockAvailable, num);
		_readIndex = num;
	}

	public void ReadFully(byte[] buf, int off, int len)
	{
		Array.Copy(_buf, _readIndex, buf, off, len);
		_readIndex += len;
	}
}
