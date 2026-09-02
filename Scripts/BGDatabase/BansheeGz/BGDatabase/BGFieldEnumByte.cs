using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "enumByte", Folder = "Enum", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerEnumByte")]
public class BGFieldEnumByte : BGFieldEnumA<byte>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 10;

	public override ushort TypeCode => 10;

	protected override int ValueSize => 1;

	public BGFieldEnumByte(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name, enumType)
	{
	}

	internal BGFieldEnumByte(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldEnumByte(meta, id, name);
	}

	protected override Enum StoredValueToEnum(byte value)
	{
		return (Enum)Enum.ToObject(base.EnumType, value);
	}

	protected override byte EnumToStoredValue(Enum value)
	{
		return Convert.ToByte(value);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return new byte[1] { GetStoredValue(entityIndex) };
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == ValueSize)
		{
			byte[] array = segment.Array;
			int offset = segment.Offset;
			byte value = array[offset];
			SetStoredValue(entityIndex, value);
		}
	}

	public void FromBytes(BGBinaryBulkRequestStruct request)
	{
		byte[] array = request.Array;
		int offset = request.Offset;
		int entitiesCount = request.EntitiesCount;
		for (int i = 0; i < entitiesCount; i++)
		{
			StoreItems[i] = array[offset + i];
		}
	}
}
