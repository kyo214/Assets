using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "enum", Folder = "Enum", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerEnum")]
public class BGFieldEnum : BGFieldEnumA<int>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 9;

	public override ushort TypeCode => 9;

	protected override int ValueSize => 4;

	public BGFieldEnum(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name, enumType)
	{
	}

	internal BGFieldEnum(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldEnum(meta, id, name);
	}

	protected override Enum StoredValueToEnum(int value)
	{
		return (Enum)Enum.ToObject(base.EnumType, value);
	}

	protected override int EnumToStoredValue(Enum value)
	{
		return Convert.ToInt32(value);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return BGFieldInt.ValueToBytes(GetStoredValue(entityIndex));
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 4)
		{
			SetStoredValue(entityIndex, BGFieldInt.ValueFromBytes(segment));
		}
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
}
