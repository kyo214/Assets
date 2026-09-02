using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "MultiThreading", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerMT")]
public class BGAddonMT : BGAddon
{
	public enum MetaModeEnum
	{
		Copy = 0
	}

	[Serializable]
	private class Settings
	{
		public bool MultithreadedUpdates;

		public MetaId[] MetaIds;

		public bool MergeOnSave;

		public BGMergeSettingsEntity MergeSettings = new BGMergeSettingsEntity();
	}

	[Serializable]
	private class MetaId
	{
		public string Id;

		public MetaModeEnum Mode;
	}

	public class MetaSetting
	{
		public BGId MetaId;

		public MetaModeEnum Mode;
	}

	private bool multithreadedUpdates;

	private readonly BGIdDictionary<MetaSetting> id2MetaSetting = new BGIdDictionary<MetaSetting>();

	private bool mergeOnSave;

	public BGMergeSettingsEntity MergeSettings = new BGMergeSettingsEntity();

	public bool MultithreadedUpdates
	{
		get
		{
			return multithreadedUpdates;
		}
		set
		{
			if (multithreadedUpdates != value)
			{
				multithreadedUpdates = value;
				FireChange();
			}
		}
	}

	public bool MergeOnSave
	{
		get
		{
			return mergeOnSave;
		}
		set
		{
			if (mergeOnSave != value)
			{
				mergeOnSave = value;
				FireChange();
			}
		}
	}

	public BGAddonMT()
	{
		MergeSettings.OnChange += SettingsChanged;
	}

	public BGMTService CreateService()
	{
		List<BGMTMeta> metaList = new List<BGMTMeta>();
		int index = 0;
		Repo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (id2MetaSetting.TryGetValue(meta.Id, out var _))
			{
				BGMTMeta item = new BGMTMeta(meta, index++);
				metaList.Add(item);
			}
		});
		return new BGMTService(multithreadedUpdates, new BGMTRepo(metaList.ToArray()));
	}

	public bool HasMeta(BGId metaId)
	{
		return id2MetaSetting.ContainsKey(metaId);
	}

	public void AddMeta(BGId metaId)
	{
		id2MetaSetting[metaId] = new MetaSetting
		{
			MetaId = metaId
		};
	}

	public void RemoveMeta(BGId metaId)
	{
		id2MetaSetting.Remove(metaId);
	}

	public override string ConfigToString()
	{
		Settings settings = new Settings
		{
			MultithreadedUpdates = multithreadedUpdates,
			MergeOnSave = mergeOnSave,
			MergeSettings = MergeSettings
		};
		if (id2MetaSetting.Count > 0)
		{
			settings.MetaIds = new MetaId[id2MetaSetting.Count];
			int num = 0;
			foreach (KeyValuePair<BGId, MetaSetting> item in id2MetaSetting)
			{
				MetaSetting value = item.Value;
				settings.MetaIds[num++] = new MetaId
				{
					Mode = value.Mode,
					Id = value.MetaId.ToString()
				};
			}
		}
		return JsonUtility.ToJson(settings);
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		multithreadedUpdates = settings.MultithreadedUpdates;
		mergeOnSave = settings.MergeOnSave;
		MergeSettings = settings.MergeSettings;
		MergeSettings.OnChange += SettingsChanged;
		id2MetaSetting.Clear();
		if (settings.MetaIds != null && settings.MetaIds.Length != 0)
		{
			for (int i = 0; i < settings.MetaIds.Length; i++)
			{
				MetaId metaId = settings.MetaIds[i];
				BGId bGId = BGId.Parse(metaId.Id);
				id2MetaSetting[bGId] = new MetaSetting
				{
					Mode = metaId.Mode,
					MetaId = bGId
				};
			}
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter(6);
		writer.AddInt(1);
		writer.AddBool(multithreadedUpdates);
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, MetaSetting> item in id2MetaSetting)
			{
				MetaSetting value = item.Value;
				writer.AddId(value.MetaId);
				writer.AddInt((int)value.Mode);
			}
		}, id2MetaSetting.Count);
		writer.AddBool(mergeOnSave);
		writer.AddByteArray(MergeSettings.ConfigToBytes());
		return writer.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		if (num == 1)
		{
			multithreadedUpdates = reader.ReadBool();
			id2MetaSetting.Clear();
			reader.ReadArray(() =>
			{
				MetaSetting metaSetting = new MetaSetting
				{
					MetaId = reader.ReadId(),
					Mode = (MetaModeEnum)reader.ReadInt()
				};
				id2MetaSetting[metaSetting.MetaId] = metaSetting;
			});
			mergeOnSave = reader.ReadBool();
			MergeSettings.ConfigFromBytes(reader.ReadByteArray());
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		BGAddonMT bGAddonMT = new BGAddonMT
		{
			Repo = repo,
			mergeOnSave = mergeOnSave,
			multithreadedUpdates = multithreadedUpdates,
			MergeSettings = (BGMergeSettingsEntity)MergeSettings.Clone()
		};
		foreach (KeyValuePair<BGId, MetaSetting> item in id2MetaSetting)
		{
			bGAddonMT.id2MetaSetting.Add(item.Key, item.Value);
		}
		return bGAddonMT;
	}

	private void SettingsChanged()
	{
		FireChange();
	}

	public void Merge()
	{
		Merge(MergeSettings);
	}

	public void Merge(BGMergeSettingsEntity mergeSettings)
	{
		BGMTRepo fromRepo = Repo.MTService.RepoReadOnly;
		Repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGId id = meta.Id;
			bool flag = !HasMeta(id);
			bool flag2 = !mergeSettings.IsMetaIncluded(id);
			if (!(flag | flag2))
			{
				BGMTMeta fromMeta = fromRepo[meta.Id];
				if (fromMeta != null)
				{
					bool flag3 = mergeSettings.IsAddingMissing(id);
					bool flag4 = mergeSettings.IsRemovingOrphaned(id);
					HashSet<BGEntity> toRemoveList = (flag4 ? new HashSet<BGEntity>() : null);
					bool updatingMatching = mergeSettings.IsUpdatingMatching(id);
					List<BGField> fieldsList = new List<BGField>();
					List<int> fromFieldsIndexes = new List<int>();
					List<bool> updatingField = new List<bool>();
					if (updatingMatching | flag3)
					{
						meta.ForEachField((BGField field) =>
						{
							BGMTField field2 = fromMeta.GetField(field.Id, errorIfNotFound: false);
							if (field2 != null)
							{
								fieldsList.Add(field);
								fromFieldsIndexes.Add(field2.Index);
								updatingField.Add(mergeSettings.IsFieldIncluded(field));
							}
						});
					}
					meta.ForEachEntity((BGEntity entity) =>
					{
						BGMTEntity? bGMTEntity = fromMeta[entity.Id];
						if (!bGMTEntity.HasValue)
						{
							toRemoveList?.Add(entity);
						}
						else if (updatingMatching)
						{
							CopyFields(fieldsList, fromFieldsIndexes, entity, bGMTEntity.Value, updatingField);
						}
					});
					if (toRemoveList != null && toRemoveList.Count > 0)
					{
						meta.DeleteEntities(toRemoveList);
					}
					if (flag3)
					{
						fromMeta.ForEachEntity((BGMTEntity entity) =>
						{
							BGId id2 = entity.Id;
							if (!meta.HasEntity(id2))
							{
								CopyFields(fieldsList, fromFieldsIndexes, meta.NewEntity(id2), entity, null);
							}
						});
					}
				}
			}
		});
	}

	private void CopyFields(List<BGField> fieldsList, List<int> fromFieldsIndexes, BGEntity entity, BGMTEntity fromEntity, List<bool> updatingList)
	{
		if (fieldsList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < fieldsList.Count; i++)
		{
			BGField field = fieldsList[i];
			int fieldIndex = fromFieldsIndexes[i];
			if (updatingList == null || updatingList[i])
			{
				fromEntity.Meta.GetField(fieldIndex).CopyTo(field, entity, fromEntity);
			}
		}
	}
}
