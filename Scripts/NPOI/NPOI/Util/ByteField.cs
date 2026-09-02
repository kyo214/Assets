using System;
using System.Globalization;
using System.IO;

namespace NPOI.Util;

public class ByteField : FixedField
{
	private const byte _default_value = 0;

	private int _offset;

	private byte _value;

	public virtual byte Value
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

	public ByteField(int offset)
		: this(offset, 0)
	{
	}

	public ByteField(int offset, byte value)
	{
		if (offset < 0)
		{
			throw new IndexOutOfRangeException("offset cannot be negative");
		}
		_offset = offset;
		Value = value;
	}

	public ByteField(int offset, byte[] data)
		: this(offset)
	{
		ReadFromBytes(data);
	}

	public ByteField(int offset, byte _value, byte[] data)
		: this(offset, _value)
	{
		WriteToBytes(data);
	}

	public virtual void ReadFromBytes(byte[] data)
	{
		_value = data[_offset];
	}

	public virtual void ReadFromStream(Stream stream)
	{
		int num = stream.ReadByte();
		if (num < 0)
		{
			throw new BufferUnderflowException();
		}
		_value = (byte)num;
	}

	public virtual void Set(byte value, byte[] data)
	{
		Value = value;
		WriteToBytes(data);
	}

	public override string ToString()
	{
		return Convert.ToString(_value, CultureInfo.CurrentCulture);
	}

	public virtual void WriteToBytes(byte[] data)
	{
		data[_offset] = _value;
	}
}
