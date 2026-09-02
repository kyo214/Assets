using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGBinaryWriter
{
	private readonly List<byte> list;

	public int Count => list.Count;

	public BGBinaryWriter()
		: this(65536)
	{
	}

	public BGBinaryWriter(int size)
	{
		list = new List<byte>(size);
	}

	public void Clear()
	{
		list.Clear();
	}

	public byte[] ToArray()
	{
		return list.ToArray();
	}

	public void AddString(string value)
	{
		AddByteArray(string.IsNullOrEmpty(value) ? null : Encoding.UTF8.GetBytes(value));
	}

	public void AddInt(int value)
	{
		byte[] array = BGFieldInt.ValueToBytes(value);
		list.Add(array[0]);
		list.Add(array[1]);
		list.Add(array[2]);
		list.Add(array[3]);
	}

	public void AddFloat(float value)
	{
		byte[] array = BGFieldFloat.ValueToBytes(value);
		list.Add(array[0]);
		list.Add(array[1]);
		list.Add(array[2]);
		list.Add(array[3]);
	}

	public void AddBool(bool value)
	{
		list.AddRange(BGFieldBool.ValueToBytes(value));
	}

	public void AddByte(byte value)
	{
		list.Add(value);
	}

	public void AddId(BGId value)
	{
		list.AddRange(value.ToByteArray());
	}

	public void AddByteArray(byte[] value)
	{
		if (value == null || value.Length == 0)
		{
			AddInt(0);
			return;
		}
		AddInt(value.Length);
		list.AddRange(value);
	}

	public void AddArray(Action action, int count)
	{
		AddInt(count);
		if (count > 0)
		{
			action();
		}
	}

	public void AddBytesRaw(byte[] value)
	{
		list.AddRange(value);
	}

	public static int GetBytesCount(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 4;
		}
		return 4 + Encoding.UTF8.GetByteCount(value);
	}

	public static int GetBytesCount(byte[] value)
	{
		if (value != null)
		{
			return 4 + value.Length;
		}
		return 4;
	}

	public void AddShort(short value)
	{
		byte[] array = BGFieldShort.ValueToBytes(value);
		list.Add(array[0]);
		list.Add(array[1]);
	}

	public void AddLong(long value)
	{
		byte[] array = BGFieldLong.ValueToBytes(value);
		list.Add(array[0]);
		list.Add(array[1]);
		list.Add(array[2]);
		list.Add(array[3]);
		list.Add(array[4]);
		list.Add(array[5]);
		list.Add(array[6]);
		list.Add(array[7]);
	}

	public void AddSByte(sbyte value)
	{
		AddByte((byte)value);
	}

	public void AddUShort(ushort value)
	{
		AddShort((short)value);
	}

	public void AddUInt(uint value)
	{
		AddInt((int)value);
	}

	public void AddULong(ulong value)
	{
		AddLong((long)value);
	}

	public override string ToString()
	{
		return "[" + list.Count + "]";
	}

	public void ReplaceInt(int value, int position)
	{
		byte[] array = BGFieldInt.ValueToBytes(value);
		list[position] = array[0];
		list[position + 1] = array[1];
		list[position + 2] = array[2];
		list[position + 3] = array[3];
	}
}
