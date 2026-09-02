using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedEnumA<T> : BGFieldCachedStructA<T>, BGBinaryBulkLoaderStruct where T : struct, IComparable, IConvertible, IFormattable
{
	private const int Size = 4;

	private readonly Type enumType = typeof(T);

	protected override int ValueSize => 4;

	public BGFieldCachedEnumA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	public BGFieldCachedEnumA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return BGFieldInt.ValueToBytes(Convert.ToInt32(this[entityIndex]));
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 4)
		{
			this[entityIndex] = (T)(object)BGFieldInt.ValueFromBytes(segment);
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
			StoreItems[i] = (T)(object)((array[num + 3] << 24) | (array[num + 2] << 16) | (array[num + 1] << 8) | array[num]);
		}
	}

	public override string ToString(int entityIndex)
	{
		return Enum.GetName(enumType, this[entityIndex]);
	}

	public override void FromString(int entityIndex, string value)
	{
		T val;
		if (string.IsNullOrEmpty(value))
		{
			val = default;
		}
		else
		{
			try
			{
				val = (T)Enum.Parse(enumType, value);
				if (!Enum.IsDefined(enumType, val))
				{
					val = default;
				}
			}
			catch
			{
				val = default;
			}
		}
		this[entityIndex] = val;
	}
}
