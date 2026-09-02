using System;

namespace BansheeGz.BGDatabase;

public struct BGId : IEquatable<BGId>
{
	public static readonly BGId Empty;

	private readonly ulong key1;

	private readonly ulong key2;

	public static BGId NewId => new BGId(Guid.NewGuid().ToByteArray());

	public bool IsEmpty => (key1 | key2) == 0;

	public static BGId Parse(string value)
	{
		if (value == null || value.Length != 22)
		{
			return Empty;
		}
		try
		{
			return new BGId(value);
		}
		catch
		{
			return Empty;
		}
	}

	public static bool TryParse(string value, out BGId id)
	{
		id = Empty;
		if (value == null || value.Length != 22)
		{
			return false;
		}
		try
		{
			id = new BGId(value);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public BGId(string value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Length != 22)
		{
			throw new FormatException("Invalid BGID: value should be 22 symbols long, invalid value is inside brackets [" + value + "]");
		}
		try
		{
			byte[] array = Convert.FromBase64String(value + "==");
			key1 = (ulong)((uint)(array[0] | (array[1] << 8) | (array[2] << 16) | (array[3] << 24)) | ((long)(array[4] | (array[5] << 8) | (array[6] << 16) | (array[7] << 24)) << 32));
			key2 = (ulong)((uint)(array[8] | (array[9] << 8) | (array[10] << 16) | (array[11] << 24)) | ((long)(array[12] | (array[13] << 8) | (array[14] << 16) | (array[15] << 24)) << 32));
		}
		catch
		{
			throw new FormatException("Invalid BGID: invalid value is inside brackets [" + value + "]");
		}
	}

	public BGId(byte[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Length != 16)
		{
			throw new FormatException("Invalid BGID: value should be 16 bytes long");
		}
		key1 = (ulong)((uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24)) | ((long)(value[4] | (value[5] << 8) | (value[6] << 16) | (value[7] << 24)) << 32));
		key2 = (ulong)((uint)(value[8] | (value[9] << 8) | (value[10] << 16) | (value[11] << 24)) | ((long)(value[12] | (value[13] << 8) | (value[14] << 16) | (value[15] << 24)) << 32));
	}

	public BGId(byte[] value, int startIndex)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (startIndex + 16 > value.Length)
		{
			throw new FormatException($"Invalid BGID: startIndex + 16 < value.Length, startIndex={startIndex}, value.Length={value.Length}");
		}
		key1 = (ulong)((uint)(value[startIndex] | (value[startIndex + 1] << 8) | (value[startIndex + 2] << 16) | (value[startIndex + 3] << 24)) | ((long)(value[startIndex + 4] | (value[startIndex + 5] << 8) | (value[startIndex + 6] << 16) | (value[startIndex + 7] << 24)) << 32));
		key2 = (ulong)((uint)(value[startIndex + 8] | (value[startIndex + 9] << 8) | (value[startIndex + 10] << 16) | (value[startIndex + 11] << 24)) | ((long)(value[startIndex + 12] | (value[startIndex + 13] << 8) | (value[startIndex + 14] << 16) | (value[startIndex + 15] << 24)) << 32));
	}

	public BGId(ulong key1, ulong key2)
	{
		this.key1 = key1;
		this.key2 = key2;
	}

	public BGId(long key1, long key2)
	{
		this.key1 = (ulong)key1;
		this.key2 = (ulong)key2;
	}

	public BGId(Guid value)
		: this(value.ToByteArray())
	{
	}

	private static long ToInt64(byte[] value, int offset)
	{
		if (offset > value.Length - 8)
		{
			throw new FormatException("start index more than value.Length - 8" + offset + ">=" + value.Length);
		}
		return (uint)(value[offset] | (value[offset + 1] << 8) | (value[offset + 2] << 16) | (value[offset + 3] << 24)) | ((long)(value[offset + 4] | (value[offset + 5] << 8) | (value[offset + 6] << 16) | (value[offset + 7] << 24)) << 32);
	}

	public override bool Equals(object obj)
	{
		if (obj is BGId other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(BGId other)
	{
		if (key1 == other.key1)
		{
			return key2 == other.key2;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)key1 ^ (int)(key1 >> 32) ^ (int)key2 ^ (int)(key2 >> 32);
	}

	public static bool operator ==(BGId a, BGId b)
	{
		if (a.key1 == b.key1)
		{
			return a.key2 == b.key2;
		}
		return false;
	}

	public static bool operator !=(BGId a, BGId b)
	{
		return !(a == b);
	}

	public override string ToString()
	{
		return Convert.ToBase64String(ToByteArray()).Substring(0, 22);
	}

	public byte[] ToByteArray()
	{
		byte[] result = new byte[16];
		ToBytes(result, 0, key1);
		ToBytes(result, 8, key2);
		return result;
	}

	public void ToByteArray(byte[] result, int start)
	{
		ToBytes(result, start, key1);
		ToBytes(result, start + 8, key2);
	}

	public void ToULongKeys(out ulong key1, out ulong key2)
	{
		key1 = this.key1;
		key2 = this.key2;
	}

	public void ToLongKeys(out long key1, out long key2)
	{
		key1 = (long)this.key1;
		key2 = (long)this.key2;
	}

	private static void ToBytes(byte[] result, int offset, ulong data)
	{
		result[offset] = (byte)data;
		result[offset + 1] = (byte)(data >> 8);
		result[offset + 2] = (byte)(data >> 16);
		result[offset + 3] = (byte)(data >> 24);
		result[offset + 4] = (byte)(data >> 32);
		result[offset + 5] = (byte)(data >> 40);
		result[offset + 6] = (byte)(data >> 48);
		result[offset + 7] = (byte)(data >> 56);
	}

	public Guid ToGuid()
	{
		return new Guid(ToByteArray());
	}
}
