using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "byte", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerByte")]
public class BGFieldByte : BGFieldCachedStructA<byte>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 26;

	public const int SizeOfTheValue = 1;

	public override ushort TypeCode => 26;

	protected override int ValueSize => 1;

	public override bool CanBeUsedAsKey => true;

	public BGFieldByte(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldByte(BGMetaEntity meta, BGId id, string name)
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
			StoreItems[i] = array[offset + i];
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldByte(meta, id, name);
	}

	public static byte[] ValueToBytes(byte value)
	{
		return new byte[1] { value };
	}

	public static byte ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 1)
		{
			return 0;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		return array[offset];
	}

	public static string ValueToString(byte i)
	{
		return i.ToString(CultureInfo.InvariantCulture);
	}

	public static byte ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return byte.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0;
	}
}
