using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedStructA<T> : BGFieldCachedA<T>, BGStructI where T : struct
{
	public override int ConstantSize => ValueSize;

	protected abstract int ValueSize { get; }

	public override T this[int entityIndex]
	{
		set
		{
			if (base.events.On)
			{
				T val = this[entityIndex];
				if (!EqualityComparer<T>.Default.Equals(val, value))
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

	protected BGFieldCachedStructA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedStructA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}
}
