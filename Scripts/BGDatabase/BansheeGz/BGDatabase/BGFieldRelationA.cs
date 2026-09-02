using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldRelationA<T, TStoreType> : BGFieldCachedA<T, TStoreType>, BGAbstractRelationI, BGFieldWithCustomConfigI
{
	protected class ReverseRelationCache
	{
		private Dictionary<BGId, ReverseRelationCacheValueI> reverseEntityId2Related;

		private readonly BGFieldRelationA<T, TStoreType> relation;

		private bool allowDuplicates;

		public bool Enabled => reverseEntityId2Related != null;

		public bool AllowDuplicates
		{
			set
			{
				if (allowDuplicates != value)
				{
					allowDuplicates = value;
					Enable(enabled: false);
				}
			}
		}

		public ReverseRelationCache(BGFieldRelationA<T, TStoreType> relation)
		{
			this.relation = relation;
		}

		public void Enable(bool enabled)
		{
			if (enabled)
			{
				if (reverseEntityId2Related != null)
				{
					return;
				}
				reverseEntityId2Related = new Dictionary<BGId, ReverseRelationCacheValueI>(16);
				relation.BuildReverseCache();
				{
					foreach (KeyValuePair<BGId, ReverseRelationCacheValueI> item in reverseEntityId2Related)
					{
						item.Value.Flush();
					}
					return;
				}
			}
			reverseEntityId2Related = null;
		}

		public void RemoveRelated(BGEntity entity, BGId relatedId)
		{
			if (reverseEntityId2Related != null && !relatedId.IsEmpty && reverseEntityId2Related.TryGetValue(relatedId, out var value))
			{
				value.Remove(entity);
			}
		}

		public void RemoveRelated(int entityIndex, BGId relatedId)
		{
			if (reverseEntityId2Related != null && !relatedId.IsEmpty && reverseEntityId2Related.TryGetValue(relatedId, out var value))
			{
				value.Remove(relation.Meta[entityIndex]);
			}
		}

		public void AddRelated(int entityIndex, BGId relatedId)
		{
			if (reverseEntityId2Related != null && !relatedId.IsEmpty)
			{
				Ensure(relatedId).Add(relation.Meta.GetEntity(entityIndex));
			}
		}

		public void AddRelated(BGEntity entity, BGId relatedId)
		{
			if (reverseEntityId2Related != null && entity != null)
			{
				Ensure(relatedId).Add(entity);
			}
		}

		public List<BGEntity> Get(BGId relatedId)
		{
			if (reverseEntityId2Related == null)
			{
				return null;
			}
			if (reverseEntityId2Related.TryGetValue(relatedId, out var value))
			{
				return value.List;
			}
			return null;
		}

		public void Remove(BGId relatedId)
		{
			reverseEntityId2Related?.Remove(relatedId);
		}

		public void MarkDirty(BGId relatedId)
		{
			if (reverseEntityId2Related != null && reverseEntityId2Related.TryGetValue(relatedId, out var value))
			{
				value.MarkDirty();
			}
		}

		public void MarkDirty()
		{
			if (reverseEntityId2Related == null)
			{
				return;
			}
			foreach (KeyValuePair<BGId, ReverseRelationCacheValueI> item in reverseEntityId2Related)
			{
				item.Value.MarkDirty();
			}
		}

		public ReverseRelationCacheValueI Ensure(BGId entityId)
		{
			if (reverseEntityId2Related.TryGetValue(entityId, out var value))
			{
				return value;
			}
			ReverseRelationCacheValueI reverseRelationCacheValueI2;
			if (!allowDuplicates)
			{
				ReverseRelationCacheValueI reverseRelationCacheValueI = new ReverseRelationCacheValue();
				reverseRelationCacheValueI2 = reverseRelationCacheValueI;
			}
			else
			{
				ReverseRelationCacheValueI reverseRelationCacheValueI = new ReverseRelationCacheValueMulti();
				reverseRelationCacheValueI2 = reverseRelationCacheValueI;
			}
			value = reverseRelationCacheValueI2;
			reverseEntityId2Related[entityId] = value;
			return value;
		}
	}

	protected interface ReverseRelationCacheValueI
	{
		List<BGEntity> List { get; }

		void Add(BGEntity entity);

		void Remove(BGEntity entity);

		void MarkDirty();

		void Flush();
	}

	protected class ReverseRelationCacheValue : ReverseRelationCacheValueI
	{
		private readonly HashSet<BGEntity> set = new HashSet<BGEntity>();

		private readonly List<BGEntity> list = new List<BGEntity>();

		private bool isDirty;

		public List<BGEntity> List
		{
			get
			{
				if (!isDirty)
				{
					return list;
				}
				Flush();
				return list;
			}
		}

		internal ReverseRelationCacheValue()
		{
		}

		public void Add(BGEntity entity)
		{
			if (entity != null && set.Add(entity))
			{
				MarkDirty();
			}
		}

		public void Remove(BGEntity entity)
		{
			if (entity != null && set.Remove(entity))
			{
				MarkDirty();
			}
		}

		public void MarkDirty()
		{
			isDirty = true;
			list.Clear();
		}

		public void Flush()
		{
			if (isDirty)
			{
				isDirty = false;
				list.Clear();
				list.AddRange(set);
				list.Sort();
			}
		}
	}

	protected class ReverseRelationCacheValueMulti : ReverseRelationCacheValueI
	{
		private readonly Dictionary<BGEntity, int> id2Count = new Dictionary<BGEntity, int>();

		private readonly List<BGEntity> list = new List<BGEntity>();

		private bool isDirty;

		public List<BGEntity> List
		{
			get
			{
				if (!isDirty)
				{
					return list;
				}
				Flush();
				return list;
			}
		}

		internal ReverseRelationCacheValueMulti()
		{
		}

		public void Add(BGEntity entity)
		{
			if (entity != null)
			{
				if (!id2Count.TryGetValue(entity, out var value))
				{
					id2Count[entity] = 1;
				}
				else
				{
					id2Count[entity] = value + 1;
				}
				MarkDirty();
			}
		}

		public void Remove(BGEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (id2Count.TryGetValue(entity, out var value))
			{
				if (value <= 1)
				{
					id2Count.Remove(entity);
				}
				else
				{
					id2Count[entity] = value - 1;
				}
			}
			MarkDirty();
		}

		public void MarkDirty()
		{
			isDirty = true;
			list.Clear();
		}

		public void Flush()
		{
			if (isDirty)
			{
				isDirty = false;
				list.Clear();
				list.AddRange(id2Count.Keys);
				list.Sort();
			}
		}
	}

	public const char ValueIdSeparator = '_';

	protected readonly ReverseRelationCache ReverseCache;

	public BGMetaEntity From => base.Meta;

	public override bool StoredValueIsTheSameAsValueType => false;

	protected BGFieldRelationA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
		ReverseCache = new ReverseRelationCache(this);
	}

	protected BGFieldRelationA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
		ReverseCache = new ReverseRelationCache(this);
	}

	protected abstract void BuildReverseCache();

	public override void ClearValues()
	{
		ReverseCache.Enable(enabled: false);
		base.ClearValues();
	}

	public override void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues)
	{
		ReverseCache.MarkDirty();
		base.MoveEntitiesValues(fromIndex, toIndex, numberOfValues);
	}

	public abstract List<BGEntity> GetRelatedIn(BGId entityId, List<BGEntity> result = null);

	public abstract List<BGEntity> GetRelatedIn(HashSet<BGId> entityIds, List<BGEntity> result = null);

	public abstract void ClearToValue(BGId entityId);

	public abstract void ClearToValue(HashSet<BGId> entityIds);
}
