using System;
using System.Globalization;
using System.IO;

namespace NPOI.Util;

public class ShortField
{
	private short _value;

	private int _offset;

	public short Value
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

	public ShortField(int offset)
	{
		if (offset < 0)
		{
			throw new IndexOutOfRangeException("Illegal offset: " + offset);
		}
		_offset = offset;
	}

	public ShortField(int offset, short value)
		: this(offset)
	{
		_value = value;
	}

	public ShortField(int offset, byte[] data)
		: this(offset)
	{
		ReadFromBytes(data);
	}

	public ShortField(int offset, short value, ref byte[] data)
		: this(offset)
	{
		Set(value, ref data);
	}

	public void Set(short value, ref byte[] data)
	{
		_value = value;
		WriteToBytes(data);
	}

	public void ReadFromBytes(byte[] data)
	{
		_value = LittleEndian.GetShort(data, _offset);
	}

	public void ReadFromStream(Stream stream)
	{
		_value = LittleEndian.ReadShort(stream);
	}

	public void WriteToBytes(byte[] data)
	{
		LittleEndian.PutShort(data, _offset, _value);
	}

	public static void Write(int offset, short value, ref byte[] data)
	{
		LittleEndian.PutShort(data, offset, value);
	}

	public override string ToString()
	{
		return Convert.ToString(_value, CultureInfo.CurrentCulture);
	}
}
