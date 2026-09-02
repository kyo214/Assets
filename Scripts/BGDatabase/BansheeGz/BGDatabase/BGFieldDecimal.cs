using System;
using System.Globalization;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "decimal", Folder = "Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerDecimal")]
public class BGFieldDecimal : BGFieldCachedStructA<decimal>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 27;

	public const int SizeOfTheValue = 16;

	public override ushort TypeCode => 27;

	protected override int ValueSize => 16;

	public override bool CanBeUsedAsKey => true;

	public BGFieldDecimal(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldDecimal(BGMetaEntity meta, BGId id, string name)
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
		int[] array2 = new int[4];
		for (int i = 0; i < entitiesCount; i++)
		{
			int num = offset + 16 * i;
			array2[0] = (array[num + 3] << 24) | (array[num + 2] << 16) | (array[num + 1] << 8) | array[num];
			array2[1] = (array[num + 7] << 24) | (array[num + 6] << 16) | (array[num + 5] << 8) | array[num + 4];
			array2[2] = (array[num + 11] << 24) | (array[num + 10] << 16) | (array[num + 9] << 8) | array[num + 8];
			array2[3] = (array[num + 15] << 24) | (array[num + 14] << 16) | (array[num + 13] << 8) | array[num + 12];
			StoreItems[i] = new decimal(array2);
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldDecimal(meta, id, name);
	}

	public static byte[] ValueToBytes(decimal value)
	{
		byte[] array = new byte[16];
		int[] bits = decimal.GetBits(value);
		Buffer.BlockCopy(BGFieldInt.ValueToBytes(bits[0]), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldInt.ValueToBytes(bits[1]), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldInt.ValueToBytes(bits[2]), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldInt.ValueToBytes(bits[3]), 0, array, 12, 4);
		return array;
	}

	public static decimal ValueFromBytes(ArraySegment<byte> segment)
	{
		int num = 0;
		if (segment.Count != 16)
		{
			return num;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		return new decimal(new int[4]
		{
			BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, offset, 4)),
			BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4)),
			BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4)),
			BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4))
		});
	}

	public static string ValueToString(decimal f)
	{
		return f.ToString(CultureInfo.InvariantCulture);
	}

	public static decimal ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return decimal.Parse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
		}
		return 0m;
	}
}
