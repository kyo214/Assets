using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "enumShort", Folder = "Enum", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerEnumShort")]
public class BGFieldEnumShort : BGFieldEnumA<short>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 12;

	public override ushort TypeCode => 12;

	protected override int ValueSize => 2;

	public BGFieldEnumShort(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name, enumType)
	{
	}

	internal BGFieldEnumShort(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldEnumShort(meta, id, name);
	}

	protected override Enum StoredValueToEnum(short value)
	{
		return (Enum)Enum.ToObject(base.EnumType, value);
	}

	protected override short EnumToStoredValue(Enum value)
	{
		return Convert.ToInt16(value);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		short storedValue = GetStoredValue(entityIndex);
		byte[] array = new byte[ValueSize];
		array[0] = (byte)(storedValue & 0xFF);
		array[1] = (byte)(storedValue >> 8);
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == ValueSize)
		{
			byte[] array = segment.Array;
			int offset = segment.Offset;
			short value = (short)((array[offset + 1] << 8) + array[offset]);
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
			int num = offset + 2 * i;
			StoreItems[i] = (short)((array[num + 1] << 8) | array[num]);
		}
	}
}
