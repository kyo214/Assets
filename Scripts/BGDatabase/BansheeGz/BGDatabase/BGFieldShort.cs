using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "short", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerShort")]
public class BGFieldShort : BGFieldCachedStructA<short>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 33;

	public const int SizeOfTheValue = 2;

	public override ushort TypeCode => 33;

	protected override int ValueSize => 2;

	public override bool CanBeUsedAsKey => true;

	public BGFieldShort(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldShort(BGMetaEntity meta, BGId id, string name)
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
			int num = offset + 2 * i;
			StoreItems[i] = (short)((array[num + 1] << 8) | array[num]);
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldShort(meta, id, name);
	}

	public static byte[] ValueToBytes(short value)
	{
		return new byte[2]
		{
			(byte)value,
			(byte)((value >>> 8) & 0xFF)
		};
	}

	public static short ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 2)
		{
			return 0;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		return (short)((array[offset + 1] << 8) | array[offset]);
	}

	public static string ValueToString(short i)
	{
		return i.ToString(CultureInfo.InvariantCulture);
	}

	public static short ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return short.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0;
	}
}
