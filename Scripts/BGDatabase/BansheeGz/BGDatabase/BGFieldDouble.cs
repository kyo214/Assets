using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "double", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerDouble")]
public class BGFieldDouble : BGFieldCachedStructA<double>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 28;

	public const int SizeOfTheValue = 8;

	public override ushort TypeCode => 28;

	protected override int ValueSize => 8;

	public BGFieldDouble(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldDouble(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(double myValue, double otherValue)
	{
		double num = Math.Max(Math.Abs(myValue), Math.Abs(otherValue)) * 1E-15;
		return Math.Abs(myValue - otherValue) <= num;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return ValueToBytes(this[entityIndex]);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		this[entityIndex] = ValueFromBytes(segment);
	}

	public void FromBytes(BGBinaryBulkRequestStruct request)
	{
		byte[] array = request.Array;
		int offset = request.Offset;
		int entitiesCount = request.EntitiesCount;
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < entitiesCount; i++)
			{
				int startIndex = offset + 8 * i;
				StoreItems[i] = BitConverter.ToDouble(array, startIndex);
			}
		}
		else
		{
			for (int j = 0; j < entitiesCount; j++)
			{
				FromBytes(j, new ArraySegment<byte>(array, offset + 8 * j, 8));
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		return ValueToString(this[entityIndex]);
	}

	public override void FromString(int entityIndex, string value)
	{
		this[entityIndex] = ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldDouble(meta, id, name);
	}

	public static byte[] ValueToBytes(double value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		return bytes;
	}

	public static double ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 8)
		{
			return 0.0;
		}
		if (BitConverter.IsLittleEndian)
		{
			return BitConverter.ToDouble(segment.Array, segment.Offset);
		}
		byte[] array = new byte[8];
		byte[] array2 = segment.Array;
		int offset = segment.Offset;
		for (int i = 0; i < 8; i++)
		{
			array[i] = array2[offset + i];
		}
		Array.Reverse((Array)array);
		return BitConverter.ToDouble(array, 0);
	}

	public static string ValueToString(double d)
	{
		return d.ToString("G17", CultureInfo.InvariantCulture);
	}

	public static double ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0.0;
	}
}
