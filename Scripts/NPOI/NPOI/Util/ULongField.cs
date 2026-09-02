using System;
using System.IO;

namespace NPOI.Util;

[Obsolete]
public class ULongField
{
	private ulong _value;

	private int _offset;

	public ulong Value
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

	public ULongField(int offset)
	{
		if (offset < 0)
		{
			throw new IndexOutOfRangeException("Illegal offset: " + offset);
		}
		_offset = offset;
	}

	public ULongField(int offset, ulong value)
		: this(offset)
	{
		Value = value;
	}

	public ULongField(int offset, byte[] data)
	{
		_offset = offset;
		ReadFromBytes(data);
	}

	public ULongField(int offset, ulong value, byte[] data)
	{
		_offset = offset;
		Set(value, data);
	}

	public void Set(ulong value, byte[] data)
	{
		_value = value;
		WriteToBytes(data);
	}

	public void ReadFromBytes(byte[] data)
	{
		_value = LittleEndian.GetULong(data, _offset);
	}

	public void ReadFromStream(Stream stream)
	{
		_value = LittleEndian.ReadULong(stream);
	}

	public void WriteToBytes(byte[] data)
	{
		LittleEndian.PutULong(data, _offset, _value);
	}

	public override string ToString()
	{
		return Convert.ToString(_value);
	}
}
