using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedStructNullableA<T> : BGFieldCachedA<T?>, BGStructNullableI, BGBinaryBulkLoaderClass where T : struct
{
	protected abstract int ValueSize { get; }

	public override T? this[int entityIndex]
	{
		set
		{
			if (base.events.On)
			{
				T? val = this[entityIndex];
				if (!Nullable.Equals(val, value))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, val, value);
					StoreSet(entityIndex, value);
					FireValueChanged(entity, val, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value);
			}
		}
	}

	protected BGFieldCachedStructNullableA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedStructNullableA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		T? val = StoreItems[entityIndex];
		if (val.HasValue)
		{
			return ValueToBytes(val.Value);
		}
		return null;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == ValueSize)
		{
			this[entityIndex] = ValueFromBytes(segment);
		}
		else
		{
			ClearValueNoEvent(entityIndex);
		}
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < num; i++)
			{
				BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
				try
				{
					StoreItems[cellRequest.EntityIndex] = ValueFromBytes(array, cellRequest.Offset);
				}
				catch (Exception obj)
				{
					request.OnError?.Invoke(obj);
				}
			}
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				BGBinaryBulkRequestClass.CellRequest cellRequest2 = cellRequests[j];
				FromBytes(cellRequest2.EntityIndex, new ArraySegment<byte>(array, cellRequest2.Offset, ValueSize));
			}
		}
	}

	protected abstract T ValueFromBytes(byte[] array, int offset);

	public override string ToString(int entityIndex)
	{
		T? val = StoreItems[entityIndex];
		if (val.HasValue)
		{
			return ValueToString(val.Value);
		}
		return "";
	}

	public override void FromString(int entityIndex, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			T? val = ValueFromString(value);
			if (val.HasValue)
			{
				this[entityIndex] = val.Value;
			}
			else
			{
				ClearValueNoEvent(entityIndex);
			}
		}
		else
		{
			ClearValueNoEvent(entityIndex);
		}
	}

	protected abstract byte[] ValueToBytes(T value);

	protected abstract T ValueFromBytes(ArraySegment<byte> segment);

	protected abstract string ValueToString(T value);

	protected abstract T? ValueFromString(string value);
}
