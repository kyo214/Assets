using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "id", Folder = "Special", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerId")]
public class BGFieldId : BGFieldCachedStructA<BGId>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 48;

	public const int Size = 16;

	public override ushort TypeCode => 48;

	protected override int ValueSize => 16;

	public BGFieldId(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldId(BGMetaEntity meta, BGId id, string name)
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
			StoreItems[i] = new BGId(array, offset + 16 * i);
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldId(meta, id, name);
	}

	public static byte[] ValueToBytes(BGId id)
	{
		return id.ToByteArray();
	}

	public static BGId ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 16)
		{
			return BGId.Empty;
		}
		return new BGId(segment.Array, segment.Offset);
	}

	public static string ValueToString(BGId id)
	{
		return id.ToString();
	}

	public static BGId ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return new BGId(value);
		}
		return BGId.Empty;
	}
}
