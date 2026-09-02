using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldReferenceSingleA<T> : BGFieldReferenceA<T> where T : MonoBehaviour
{
	public override T this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			BGId bGId = StoreItems[entityIndex];
			if (bGId.IsEmpty)
			{
				return null;
			}
			return GetById(bGId);
		}
		set
		{
			BGId bGId = StoreGet(entityIndex);
			bool flag = !bGId.IsEmpty;
			if (value == null)
			{
				StoreSet(entityIndex, BGId.Empty);
				if (base.events.On & flag)
				{
					FireStoredValueChanged(base.Meta[entityIndex], bGId, BGId.Empty);
				}
				return;
			}
			BGId bGId2 = IdProvider(value);
			if (bGId2.IsEmpty)
			{
				if (flag)
				{
					StoreSet(entityIndex, BGId.Empty);
					FireStoredValueChanged(base.Meta[entityIndex], bGId2, BGId.Empty);
				}
			}
			else if (!(bGId == bGId2))
			{
				SetStoredValue(entityIndex, bGId2);
			}
		}
	}

	protected BGFieldReferenceSingleA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldReferenceSingleA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected abstract BGId IdProvider(T component);

	protected abstract T GetById(BGId id);
}
