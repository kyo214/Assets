using System;
using System.Globalization;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "float", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerFloat")]
public class BGFieldFloat : BGFieldCachedStructA<float>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 29;

	public const int SizeOfTheValue = 4;

	public override ushort TypeCode => 29;

	protected override int ValueSize => 4;

	public BGFieldFloat(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldFloat(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(float myValue, float otherValue)
	{
		return Mathf.Approximately(myValue, otherValue);
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
				int startIndex = offset + 4 * i;
				StoreItems[i] = BitConverter.ToSingle(array, startIndex);
			}
		}
		else
		{
			for (int j = 0; j < entitiesCount; j++)
			{
				FromBytes(j, new ArraySegment<byte>(array, offset + 4 * j, 4));
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldFloat(meta, id, name);
	}

	public static byte[] ValueToBytes(float value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		return bytes;
	}

	public static float ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 4)
		{
			return 0f;
		}
		if (BitConverter.IsLittleEndian)
		{
			return BitConverter.ToSingle(segment.Array, segment.Offset);
		}
		byte[] array = new byte[4];
		byte[] array2 = segment.Array;
		int offset = segment.Offset;
		for (int i = 0; i < 4; i++)
		{
			array[i] = array2[offset + i];
		}
		Array.Reverse((Array)array);
		return BitConverter.ToSingle(array, 0);
	}

	public static string ValueToString(float f)
	{
		return f.ToString("G9", CultureInfo.InvariantCulture);
	}

	public static float ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return float.Parse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0f;
	}
}
