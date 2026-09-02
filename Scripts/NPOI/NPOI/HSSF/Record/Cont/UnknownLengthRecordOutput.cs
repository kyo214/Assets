using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.Cont;

internal class UnknownLengthRecordOutput : ILittleEndianOutput
{
	private const int MAX_DATA_SIZE = 8224;

	private ILittleEndianOutput _originalOut;

	private ILittleEndianOutput _dataSizeOutput;

	private byte[] _byteBuffer;

	private ILittleEndianOutput _out;

	private int _size;

	public int TotalSize => 4 + _size;

	public int AvailableSpace
	{
		get
		{
			if (_out == null)
			{
				throw new InvalidOperationException("Record already terminated");
			}
			return 8224 - _size;
		}
	}

	public UnknownLengthRecordOutput(ILittleEndianOutput out1, int sid)
	{
		_originalOut = out1;
		out1.WriteShort(sid);
		if (out1 is IDelayableLittleEndianOutput)
		{
			IDelayableLittleEndianOutput delayableLittleEndianOutput = (IDelayableLittleEndianOutput)out1;
			_dataSizeOutput = delayableLittleEndianOutput.CreateDelayedOutput(2);
			_byteBuffer = null;
			_out = out1;
		}
		else
		{
			_dataSizeOutput = out1;
			_byteBuffer = new byte[8224];
			_out = new LittleEndianByteArrayOutputStream(_byteBuffer, 0);
		}
	}

	public void Terminate()
	{
		if (_out == null)
		{
			throw new InvalidOperationException("Record already terminated");
		}
		_dataSizeOutput.WriteShort(_size);
		if (_byteBuffer != null)
		{
			_originalOut.Write(_byteBuffer, 0, _size);
			_out = null;
		}
		else
		{
			_out = null;
		}
	}

	public void Write(byte[] b)
	{
		_out.Write(b);
		_size += b.Length;
	}

	public void Write(byte[] b, int offset, int len)
	{
		_out.Write(b, offset, len);
		_size += len;
	}

	public void WriteByte(int v)
	{
		_out.WriteByte(v);
		_size++;
	}

	public void WriteDouble(double v)
	{
		_out.WriteDouble(v);
		_size += 8;
	}

	public void WriteInt(int v)
	{
		_out.WriteInt(v);
		_size += 4;
	}

	public void WriteLong(long v)
	{
		_out.WriteLong(v);
		_size += 8;
	}

	public void WriteShort(int v)
	{
		_out.WriteShort(v);
		_size += 2;
	}
}
