using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "bool", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerBool")]
public class BGFieldBool : BGFieldCachedStructA<bool>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 25;

	public const int SizeOfTheValue = 1;

	public override ushort TypeCode => 25;

	protected override int ValueSize => 1;

	public override bool CanBeUsedAsKey => true;

	public BGFieldBool(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldBool(BGMetaEntity meta, BGId id, string name)
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
			StoreItems[i] = array[offset + i] != 0;
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldBool(meta, id, name);
	}

	public static byte[] ValueToBytes(bool value)
	{
		return new byte[1] { value ? ((byte)1) : ((byte)0) };
	}

	public static bool ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count == 1)
		{
			return segment.Array[segment.Offset] != 0;
		}
		return false;
	}

	public static string ValueToString(bool b)
	{
		if (!b)
		{
			return "0";
		}
		return "1";
	}

	public static bool ValueFromString(string value)
	{
		return "1".Equals(value);
	}
}
