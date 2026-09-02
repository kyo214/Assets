using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldStringA : BGFieldCachedClassA<string>, BGBinaryBulkLoaderClass
{
	public override bool CanBeUsedAsKey => true;

	public override string this[int entityIndex]
	{
		set
		{
			if (base.events.On)
			{
				string text = this[entityIndex];
				if (!string.Equals(text, value))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, text, value);
					StoreSet(entityIndex, value);
					FireValueChanged(entity, text, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value);
			}
		}
	}

	protected BGFieldStringA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldStringA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex != -1 && !fromField.IsDeleted)
		{
			int num = base.Meta.FindEntityIndex(toEntityId);
			if (num != -1)
			{
				BGField<string> bGField = (BGField<string>)fromField;
				StoreSet(num, bGField[fromEntityIndex]);
			}
		}
	}

	public override void ForEachValue(Action<int> action)
	{
		int storeCount = StoreCount;
		for (int i = 0; i < storeCount; i++)
		{
			string value = StoreItems[i];
			if (!string.IsNullOrEmpty(value))
			{
				action(i);
			}
		}
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return ValueToBytes(this[entityIndex]);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		this[entityIndex] = ValueFromBytes(segment);
	}

	public virtual void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		Encoding uTF = Encoding.UTF8;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			try
			{
				StoreItems[cellRequest.EntityIndex] = uTF.GetString(array, cellRequest.Offset, cellRequest.Count);
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
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

	public static byte[] ValueToBytes(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return Encoding.UTF8.GetBytes(value);
		}
		return null;
	}

	public static string ValueFromBytes(ArraySegment<byte> segment)
	{
		if (segment.Count != 0)
		{
			return Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
		}
		return null;
	}

	public static string ValueToString(string value)
	{
		return value ?? "";
	}

	public static string ValueFromString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value;
		}
		return null;
	}
}
