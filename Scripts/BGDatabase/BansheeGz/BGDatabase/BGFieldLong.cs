using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "long", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerLong")]
public class BGFieldLong : BGFieldCachedStructA<long>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 32;

	public const int SizeOfTheValue = 8;

	public override ushort TypeCode => 32;

	protected override int ValueSize => 8;

	public override bool CanBeUsedAsKey => true;

	public BGFieldLong(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldLong(BGMetaEntity meta, BGId id, string name)
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
			int num = offset + 8 * i;
			StoreItems[i] = (long)(((ulong)array[num + 7] << 56) | ((ulong)array[num + 6] << 48) | ((ulong)array[num + 5] << 40) | ((ulong)array[num + 4] << 32) | ((ulong)array[num + 3] << 24) | ((ulong)array[num + 2] << 16) | ((ulong)array[num + 1] << 8) | array[num]);
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldLong(meta, id, name);
	}

	public static byte[] ValueToBytes(long value)
	{
		return new byte[8]
		{
			(byte)value,
			(byte)((value >>> 8) & 0xFF),
			(byte)((value >>> 16) & 0xFF),
			(byte)((value >>> 24) & 0xFF),
			(byte)((value >>> 32) & 0xFF),
			(byte)((value >>> 40) & 0xFF),
			(byte)((value >>> 48) & 0xFF),
			(byte)((value >>> 56) & 0xFF)
		};
	}

	public static long ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 8)
		{
			return 0L;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		return (long)(((ulong)array[offset + 7] << 56) | ((ulong)array[offset + 6] << 48) | ((ulong)array[offset + 5] << 40) | ((ulong)array[offset + 4] << 32) | ((ulong)array[offset + 3] << 24) | ((ulong)array[offset + 2] << 16) | ((ulong)array[offset + 1] << 8) | array[offset]);
	}

	public static string ValueToString(long l)
	{
		return l.ToString(CultureInfo.InvariantCulture);
	}

	public static long ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return long.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0L;
	}
}
