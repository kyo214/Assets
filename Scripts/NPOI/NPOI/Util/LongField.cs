using System;
using System.Globalization;
using System.IO;

namespace NPOI.Util;

public class LongField
{
	private long _value;

	private int _offset;

	public long Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public LongField(int offset)
	{
		if (offset < 0)
		{
			throw new IndexOutOfRangeException("Illegal offset: " + offset);
		}
		_offset = offset;
	}

	public LongField(int offset, long value)
		: this(offset)
	{
		Value = value;
	}

	public LongField(int offset, byte[] data)
		: this(offset)
	{
		ReadFromBytes(data);
	}

	public LongField(int offset, long value, byte[] data)
		: this(offset)
	{
		Set(value, data);
	}

	public void Set(long value, byte[] data)
	{
		_value = value;
		WriteToBytes(data);
	}

	public void ReadFromBytes(byte[] data)
	{
		_value = LittleEndian.GetLong(data, _offset);
	}

	public void ReadFromStream(Stream stream)
	{
		_value = LittleEndian.ReadLong(stream);
	}

	public void WriteToBytes(byte[] data)
	{
		LittleEndian.PutLong(data, _offset, _value);
	}

	public static void Write(int offset, long value, byte[] data)
	{
		LittleEndian.PutLong(data, offset, value);
	}

	public override string ToString()
	{
		return Convert.ToString(_value, CultureInfo.CurrentCulture);
	}
}
