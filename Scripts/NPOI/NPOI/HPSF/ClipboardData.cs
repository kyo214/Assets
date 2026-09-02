using System;
using System.IO;
using NPOI.Util;

namespace NPOI.HPSF;

public class ClipboardData
{
	private int _format;

	private byte[] _value;

	public int Size => 8 + _value.Length;

	public byte[] Value => _value;

	public ClipboardData(byte[] data, int offset)
	{
		int num = LittleEndian.GetInt(data, offset);
		if (num < 4)
		{
			_format = 0;
			_value = new byte[0];
		}
		else
		{
			_format = LittleEndian.GetInt(data, offset + 4);
			_value = LittleEndian.GetByteArray(data, offset + 8, num - 4);
		}
	}

	public byte[] ToByteArray()
	{
		byte[] array = new byte[Size];
		LittleEndian.PutInt(array, 0, 4 + _value.Length);
		LittleEndian.PutInt(array, 4, _format);
		System.Array.Copy(_value, 0, array, 8, _value.Length);
		return array;
	}

	public int Write(Stream out1)
	{
		LittleEndian.PutInt(4 + _value.Length, out1);
		LittleEndian.PutInt(_format, out1);
		out1.Write(_value, 0, _value.Length);
		return 8 + _value.Length;
	}
}
