using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "int", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerInt")]
public class BGFieldInt : BGFieldCachedStructA<int>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 31;

	public const int SizeOfTheValue = 4;

	public override ushort TypeCode => 31;

	protected override int ValueSize => 4;

	public override bool CanBeUsedAsKey => true;

	public BGFieldInt(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldInt(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
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
		for (int i = 0; i < entitiesCount; i++)
		{
			int num = offset + 4 * i;
			StoreItems[i] = (array[num + 3] << 24) | (array[num + 2] << 16) | (array[num + 1] << 8) | array[num];
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldInt(meta, id, name);
	}

	public static byte[] ValueToBytes(int value)
	{
		return new byte[4]
		{
			(byte)value,
			(byte)((value >>> 8) & 0xFF),
			(byte)((value >>> 16) & 0xFF),
			(byte)((value >>> 24) & 0xFF)
		};
	}

	public static int ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 4)
		{
			return 0;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		return (array[offset + 3] << 24) | (array[offset + 2] << 16) | (array[offset + 1] << 8) | array[offset];
	}

	public static string ValueToString(int i)
	{
		return i.ToString(CultureInfo.InvariantCulture);
	}

	public static int ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return int.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0;
	}
}
