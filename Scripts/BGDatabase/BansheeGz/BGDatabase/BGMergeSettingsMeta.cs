using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGMergeSettingsMeta : BGMergeSettingsA
{
	[Serializable]
	public class MetaSettings
	{
		[SerializeField]
		private BGIdList fields = new BGIdList();

		[SerializeField]
		private bool included;

		public bool Included
		{
			get
			{
				return included;
			}
			set
			{
				included = value;
			}
		}

		public int Count => fields.Count;

		public event Action OnChange;

		public void AddField(BGId fieldId)
		{
			if (!fields.Contains(fieldId))
			{
				fields.Add(fieldId);
				FireOnChange();
			}
		}

		public void RemoveField(BGId fieldId)
		{
			if (fields.Contains(fieldId))
			{
				fields.Remove(fieldId);
				FireOnChange();
			}
		}

		public bool HasField(BGId fieldId)
		{
			return fields.Contains(fieldId);
		}

		private void FireOnChange()
		{
			OnChange?.Invoke();
		}
	}

	[Serializable]
	public class HashtableId2MetaSettings : BGHashtableIdKey<MetaSettings>
	{
	}

	[SerializeField]
	private HashtableId2MetaSettings id2Meta = new HashtableId2MetaSettings();

	public bool HasAny
	{
		get
		{
			if (!addMissing && !updateMatching)
			{
				return removeOrphaned;
			}
			return true;
		}
	}

	public int CountMeta => id2Meta.Count;

	public bool Has(BGId metaId)
	{
		return id2Meta.ContainsKey(metaId);
	}

	public MetaSettings GetSettings(BGId metaId)
	{
		return BGUtil.Get(id2Meta, metaId);
	}

	public void Remove(BGId metaId)
	{
		if (Has(metaId))
		{
			id2Meta.Remove(metaId);
			FireOnChange();
		}
	}

	public bool IsMetaIncluded(BGId metaId)
	{
		if (!HasAny)
		{
			return false;
		}
		if (!Has(metaId))
		{
			return base.IncludedByDefault;
		}
		return GetSettings(metaId).Included;
	}

	public bool IsFieldIncluded(BGField field)
	{
		if (!HasAny)
		{
			return false;
		}
		BGId metaId = field.MetaId;
		if (!Has(metaId))
		{
			return base.IncludedByDefault;
		}
		MetaSettings settings = GetSettings(metaId);
		if (!settings.Included)
		{
			return false;
		}
		return settings.HasField(field.Id);
	}

	public void ForEachSetting(Action<MetaSettings> action)
	{
		foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
		{
			action(id2Metum.Value);
		}
	}

	public void Ensure(BGId metaId)
	{
		if (!id2Meta.ContainsKey(metaId))
		{
			id2Meta[metaId] = new MetaSettings();
		}
	}

	public void ForEachMeta(BGRepo repo, Action<BGMetaEntity> action)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (IsMetaIncluded(meta.Id))
			{
				action(meta);
			}
		});
	}

	public void ForEachField(BGRepo repo, Action<BGField> action)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (IsMetaIncluded(meta.Id))
			{
				meta.ForEachField((BGField field) =>
				{
					if (IsFieldIncluded(field))
					{
						action(field);
					}
				});
			}
		});
	}
}
