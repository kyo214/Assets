using System;
using System.Globalization;
using System.IO;

namespace NPOI.Util;

public class IntegerField : FixedField
{
	private int _value;

	private int _offset;

	public int Value
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

	public IntegerField(int offset)
	{
		if (offset < 0)
		{
			throw new IndexOutOfRangeException("negative offset");
		}
		_offset = offset;
	}

	public IntegerField(int offset, int value)
		: this(offset)
	{
		_value = value;
	}

	public IntegerField(int offset, byte[] data)
		: this(offset)
	{
		ReadFromBytes(data);
	}

	public IntegerField(int offset, int value, byte[] data)
		: this(offset)
	{
		Set(value, data);
	}

	public void Set(int value, byte[] data)
	{
		_value = value;
		WriteToBytes(data);
	}

	public void ReadFromBytes(byte[] data)
	{
		_value = LittleEndian.GetInt(data, _offset);
	}

	public void ReadFromStream(Stream stream)
	{
		_value = LittleEndian.ReadInt(stream);
	}

	public void WriteToBytes(byte[] data)
	{
		LittleEndian.PutInt(data, _offset, _value);
	}

	public static void Write(int offset, int value, byte[] data)
	{
		LittleEndian.PutInt(data, offset, value);
	}

	public override string ToString()
	{
		return Convert.ToString(_value, CultureInfo.CurrentCulture);
	}
}
