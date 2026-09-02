using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedStructListA<T> : BGFieldCachedListA<T>, BGBinaryBulkLoaderClass where T : struct
{
	protected abstract int ValueSize { get; }

	public override List<T> this[int entityIndex]
	{
		set
		{
			if (base.events.On)
			{
				List<T> list = this[entityIndex];
				if (!BGUtil.ListsValuesEqual(value, list))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, list, value);
					StoreSet(entityIndex, value);
					FireValueChanged(entity, list, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value);
			}
		}
	}

	protected BGFieldCachedStructListA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedStructListA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		List<T> list = this[entityIndex];
		if (BGUtil.IsEmpty(list))
		{
			return null;
		}
		int valueSize = ValueSize;
		byte[] array = new byte[list.Count * valueSize];
		for (int i = 0; i < list.Count; i++)
		{
			Buffer.BlockCopy(ValueToBytes(list[i]), 0, array, i * valueSize, valueSize);
		}
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		int count = segment.Count;
		if (count == 0)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		int valueSize = ValueSize;
		if (count % valueSize != 0)
		{
			throw new BGException("Can not convert byte array to value. Wrong byte array size $. Should be dividable by $", count, valueSize);
		}
		List<T> list = BGFieldCachedListA<T>.EnsureValueCleared(this, entityIndex, count / valueSize);
		for (int i = 0; i < count; i += valueSize)
		{
			list.Add(ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset + i, valueSize)));
		}
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			int entityIndex = cellRequest.EntityIndex;
			int offset = cellRequest.Offset;
			try
			{
				int valueSize = ValueSize;
				if (cellRequest.Count % valueSize != 0)
				{
					throw new BGException("Can not convert byte array to value. Wrong byte array size $. Should be dividable by $", cellRequest.Count, valueSize);
				}
				int num2 = cellRequest.Count / valueSize;
				if (num2 == 0)
				{
					StoreItems[entityIndex] = null;
					continue;
				}
				List<T> list = StoreItems[entityIndex];
				if (list == null)
				{
					list = new List<T>(num2);
				}
				else
				{
					list.Clear();
					if (list.Capacity < num2)
					{
						list.Capacity = num2;
					}
				}
				StoreItems[entityIndex] = list;
				int num3 = valueSize * num2;
				if (BitConverter.IsLittleEndian)
				{
					for (int j = 0; j < num3; j += valueSize)
					{
						list.Add(ValueFromBytes(array, offset + j));
					}
				}
				else
				{
					for (int k = 4; k < num3; k += valueSize)
					{
						list.Add(ValueFromBytes(new ArraySegment<byte>(array, k, valueSize)));
					}
				}
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	protected abstract T ValueFromBytes(byte[] array, int offset);

	public override string ToString(int entityIndex)
	{
		List<T> list = this[entityIndex];
		if (BGUtil.IsEmpty(list))
		{
			return null;
		}
		char c = StringValueSeparator[0];
		string text = "";
		for (int i = 0; i < list.Count; i++)
		{
			T value = list[i];
			if (i != 0)
			{
				text += c;
			}
			text += ValueToString(value);
		}
		return text;
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		string[] array = value.Split(StringValueSeparator, StringSplitOptions.RemoveEmptyEntries);
		List<T> list = BGFieldCachedListA<T>.EnsureValueCleared(this, entityIndex, array.Length);
		string[] array2 = array;
		foreach (string value2 in array2)
		{
			list.Add(ValueFromString(value2));
		}
	}

	protected abstract byte[] ValueToBytes(T value);

	protected abstract T ValueFromBytes(ArraySegment<byte> segment);

	protected abstract string ValueToString(T value);

	protected abstract T ValueFromString(string value);
}
