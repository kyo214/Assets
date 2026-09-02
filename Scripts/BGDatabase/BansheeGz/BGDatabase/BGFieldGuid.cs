using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "guid", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerGuid")]
public class BGFieldGuid : BGFieldCachedStructA<Guid>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 30;

	public const int SizeOfTheValue = 16;

	public override ushort TypeCode => 30;

	protected override int ValueSize => 16;

	public override bool CanBeUsedAsKey => true;

	public BGFieldGuid(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldGuid(BGMetaEntity meta, BGId id, string name)
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
		byte[] array2 = new byte[16];
		for (int i = 0; i < entitiesCount; i++)
		{
			int srcOffset = offset + 16 * i;
			Buffer.BlockCopy(array, srcOffset, array2, 0, 16);
			StoreItems[i] = new Guid(array2);
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldGuid(meta, id, name);
	}

	public static byte[] ValueToBytes(Guid guid)
	{
		return guid.ToByteArray();
	}

	public static Guid ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count == 16)
		{
			return new Guid(BGUtil.ToArray(segment));
		}
		return Guid.Empty;
	}

	public static string ValueToString(Guid guid)
	{
		return guid.ToString();
	}

	public static Guid ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return new Guid(value);
		}
		return Guid.Empty;
	}
}
