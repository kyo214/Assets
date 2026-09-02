using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGBinaryReader
{
	private byte[] array;

	private int cursor;

	public int Cursor => cursor;

	public int Length => array.Length;

	public BGBinaryReader(byte[] array)
	{
		this.array = array;
		cursor = 0;
	}

	public BGBinaryReader(ArraySegment<byte> array)
	{
		Reset(array);
	}

	public BGBinaryReader NewReader(int position = 0)
	{
		return new BGBinaryReader(new ArraySegment<byte>(array, position, array.Length - position));
	}

	public void Reset(ArraySegment<byte> array)
	{
		this.array = array.Array;
		cursor = array.Offset;
	}

	public void ShiftCursor(int delta)
	{
		cursor += delta;
	}

	public void SetCursor(int position)
	{
		cursor = position;
	}

	public int ReadInt()
	{
		int result = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, cursor, 4));
		cursor += 4;
		return result;
	}

	public float ReadFloat()
	{
		float result = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, cursor, 4));
		cursor += 4;
		return result;
	}

	public bool ReadBool()
	{
		bool result = BGFieldBool.ValueFromBytes(new ArraySegment<byte>(array, cursor, 1));
		cursor++;
		return result;
	}

	public byte ReadByte()
	{
		byte result = array[cursor];
		cursor++;
		return result;
	}

	public BGId ReadId()
	{
		BGId result = new BGId(array, cursor);
		cursor += 16;
		return result;
	}

	public string ReadString()
	{
		ArraySegment<byte> arraySegment = ReadByteArray();
		if (arraySegment.Count != 0)
		{
			return Encoding.UTF8.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
		return null;
	}

	public ArraySegment<byte> ReadByteArray()
	{
		int num = ReadInt();
		ArraySegment<byte> result = new ArraySegment<byte>(array, cursor, num);
		cursor += num;
		return result;
	}

	public void ReadArray(Action action)
	{
		int num = ReadInt();
		for (int i = 0; i < num; i++)
		{
			action();
		}
	}

	public void Dispose()
	{
		array = null;
		cursor = 0;
	}

	public short ReadShort()
	{
		short result = BGFieldShort.ValueFromBytes(new ArraySegment<byte>(array, cursor, 2));
		cursor += 2;
		return result;
	}

	public long ReadLong()
	{
		long result = BGFieldLong.ValueFromBytes(new ArraySegment<byte>(array, cursor, 8));
		cursor += 8;
		return result;
	}

	public sbyte ReadSByte()
	{
		return (sbyte)ReadByte();
	}

	public ushort ReadUShort()
	{
		return (ushort)ReadShort();
	}

	public uint ReadUInt()
	{
		return (uint)ReadInt();
	}

	public ulong ReadULong()
	{
		return (ulong)ReadLong();
	}

	public ArraySegment<byte> ReadByteArrayRaw(int length)
	{
		ArraySegment<byte> result = new ArraySegment<byte>(array, cursor, length);
		cursor += length;
		return result;
	}
}
