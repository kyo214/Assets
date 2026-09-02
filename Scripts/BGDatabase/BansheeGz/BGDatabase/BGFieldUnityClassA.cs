using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldUnityClassA<T> : BGFieldCachedClassA<T> where T : class, new()
{
	public abstract int MinValueSize { get; }

	protected BGFieldUnityClassA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldUnityClassA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		T val = this[entityIndex];
		if (val == null)
		{
			return null;
		}
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(128);
		ToBytes(bGBinaryWriter, val);
		return bGBinaryWriter.ToArray();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count < MinValueSize)
		{
			ClearValueNoEvent(entityIndex);
		}
		else
		{
			this[entityIndex] = FromBytes(segment);
		}
	}

	protected abstract T FromBytes(ArraySegment<byte> segment);

	protected abstract void ToBytes(BGBinaryWriter writer, T value);

	public override string ToString(int entityIndex)
	{
		T val = this[entityIndex];
		if (val == null)
		{
			return null;
		}
		return ToString(val);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (value == null || value.Trim().Length == 0)
		{
			ClearValueNoEvent(entityIndex);
		}
		else
		{
			this[entityIndex] = FromString(value);
		}
	}

	protected abstract T FromString(string value);

	protected abstract string ToString(T value);

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || fromField.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num != -1)
		{
			BGField<T> bGField = (BGField<T>)fromField;
			T val = bGField[fromEntityIndex];
			if (val == null)
			{
				ClearValueNoEvent(num);
			}
			else
			{
				StoreSet(num, CloneValue(val));
			}
		}
	}

	public abstract T CloneValue(T value);

	protected override bool AreStoredValuesEqual(T myValue, T otherValue)
	{
		return AreEqual(myValue, otherValue);
	}

	public abstract bool AreEqual(T myValue, T otherValue);
}
