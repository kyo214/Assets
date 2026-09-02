using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldReferenceA<T> : BGFieldCachedA<T, BGId>, BGSceneObjectReferenceI, BGBinaryBulkLoaderStruct
{
	private const int Size = 16;

	public override int ConstantSize => 16;

	public override bool StoredValueIsTheSameAsValueType => false;

	protected BGFieldReferenceA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldReferenceA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return GetStoredValue(entityIndex).ToByteArray();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		StoreItems[entityIndex] = new BGId(segment.Array, segment.Offset);
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
		return GetStoredValue(entityIndex).ToString();
	}

	public override void FromString(int entityIndex, string value)
	{
		StoreItems[entityIndex] = new BGId(value);
	}
}
