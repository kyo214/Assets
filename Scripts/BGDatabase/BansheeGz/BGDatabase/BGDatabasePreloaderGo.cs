using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDatabasePreloaderGo")]
public class BGDatabasePreloaderGo : MonoBehaviour
{
	[Serializable]
	public class PreloaderMetaSetting : BGMetaReference
	{
		public bool IdIndex;

		public bool NameIndex;
	}

	[Serializable]
	public class PreloaderKeySetting : BGKeyReference
	{
		public bool IncludePartialKeys;
	}

	[Serializable]
	public class PreloaderIndexSetting : BGIndexReference
	{
	}

	[Serializable]
	public class PreloaderReverseRelationSetting : BGFieldReference
	{
	}

	[SerializeField]
	private List<PreloaderMetaSetting> tableSettings = new List<PreloaderMetaSetting>();

	[SerializeField]
	private List<PreloaderKeySetting> keys = new List<PreloaderKeySetting>();

	[SerializeField]
	private List<PreloaderIndexSetting> indexes = new List<PreloaderIndexSetting>();

	[SerializeField]
	private List<PreloaderReverseRelationSetting> reverseRelations = new List<PreloaderReverseRelationSetting>();

	[SerializeField]
	private bool doNotInitializeKeys;

	[SerializeField]
	private bool printLoadingTime;

	private Dictionary<BGId, PreloaderMetaSetting> metaId2settings;

	private Dictionary<BGId, PreloaderKeySetting> keyId2settings;

	private Dictionary<BGId, PreloaderIndexSetting> indexId2settings;

	private Dictionary<BGId, PreloaderReverseRelationSetting> reverseRelationsId2settings;

	private HashSet<BGId> referencedMetas;

	public bool DoNotInitializeKeys
	{
		get
		{
			return doNotInitializeKeys;
		}
		set
		{
			doNotInitializeKeys = value;
		}
	}

	public bool PrintLoadingTime
	{
		get
		{
			return printLoadingTime;
		}
		set
		{
			printLoadingTime = value;
		}
	}

	private Dictionary<BGId, PreloaderMetaSetting> MetaId2settings
	{
		get
		{
			if (metaId2settings != null)
			{
				return metaId2settings;
			}
			metaId2settings = new Dictionary<BGId, PreloaderMetaSetting>();
			foreach (PreloaderMetaSetting tableSetting in TableSettings)
			{
				BGMetaEntity meta = tableSetting.Meta;
				if (meta != null)
				{
					metaId2settings[meta.Id] = tableSetting;
				}
			}
			return metaId2settings;
		}
	}

	private Dictionary<BGId, PreloaderKeySetting> KeyId2settings
	{
		get
		{
			if (keyId2settings != null)
			{
				return keyId2settings;
			}
			keyId2settings = new Dictionary<BGId, PreloaderKeySetting>();
			foreach (PreloaderKeySetting key2 in keys)
			{
				BGKey key = key2.Key;
				if (key != null)
				{
					keyId2settings[key.Id] = key2;
				}
			}
			return keyId2settings;
		}
	}

	private Dictionary<BGId, PreloaderIndexSetting> IndexId2settings
	{
		get
		{
			if (indexId2settings != null)
			{
				return indexId2settings;
			}
			indexId2settings = new Dictionary<BGId, PreloaderIndexSetting>();
			foreach (PreloaderIndexSetting index2 in indexes)
			{
				BGIndex index = index2.Index;
				if (index != null)
				{
					indexId2settings[index.Id] = index2;
				}
			}
			return indexId2settings;
		}
	}

	private Dictionary<BGId, PreloaderReverseRelationSetting> ReverseRelationsId2settings
	{
		get
		{
			if (reverseRelationsId2settings != null)
			{
				return reverseRelationsId2settings;
			}
			reverseRelationsId2settings = new Dictionary<BGId, PreloaderReverseRelationSetting>();
			foreach (PreloaderReverseRelationSetting reverseRelation in reverseRelations)
			{
				BGField field2 = reverseRelation.Field;
				if (field2 is BGAbstractRelationI)
				{
					reverseRelationsId2settings[field2.Id] = reverseRelation;
				}
			}
			return reverseRelationsId2settings;
		}
	}

	private HashSet<BGId> ReferencedMetas
	{
		get
		{
			if (referencedMetas != null)
			{
				return referencedMetas;
			}
			referencedMetas = new HashSet<BGId>();
			BGRepo.I.ForEachMeta((BGMetaEntity meta) =>
			{
				meta.ForEachField((BGField bGField) =>
				{
					if (bGField is BGAbstractRelationI bGAbstractRelationI)
					{
						if (!(bGAbstractRelationI is BGRelationI { ToId: var toId }))
						{
							if (bGAbstractRelationI is BGManyTablesRelationI { ToIds: { Count: not 0 } toIds })
							{
								foreach (BGId item in toIds)
								{
									referencedMetas.Add(item);
								}
							}
						}
						else if (!toId.IsEmpty)
						{
							referencedMetas.Add(toId);
						}
					}
				});
			});
			return referencedMetas;
		}
	}

	public List<PreloaderKeySetting> Keys => keys;

	public List<PreloaderIndexSetting> Indexes => indexes;

	public List<PreloaderReverseRelationSetting> ReverseRelations => reverseRelations;

	public List<PreloaderMetaSetting> TableSettings => tableSettings;

	private void Awake()
	{
		if (PrintLoadingTime)
		{
			BGUtil.Measure("[BGDatabase] database loaded in (mls)", Load);
		}
		else
		{
			Load();
		}
	}

	public void Load()
	{
		BGRepo i = BGRepo.I;
		if (doNotInitializeKeys)
		{
			return;
		}
		i.ForEachMeta((BGMetaEntity meta) =>
		{
			PreloaderMetaSetting metaSetting = GetMetaSetting(meta.Id);
			if (IsUsedByRelations(meta) || (metaSetting != null && metaSetting.IdIndex))
			{
				meta.GetEntity(BGId.Empty);
			}
			if (metaSetting != null && metaSetting.NameIndex)
			{
				meta.GetEntity("");
			}
			meta.ForEachField((BGField bGField) =>
			{
				if (bGField is BGAbstractRelationI bGAbstractRelationI2 && IsUsedByNestedMeta(bGField))
				{
					bGAbstractRelationI2.GetRelatedIn(BGId.Empty);
				}
			});
		});
		foreach (PreloaderKeySetting key2 in keys)
		{
			BGKey key = key2.Key;
			if (key != null)
			{
				if (key2.IncludePartialKeys)
				{
					key.BuildAll();
				}
				else
				{
					key.Build();
				}
			}
		}
		foreach (PreloaderIndexSetting index in indexes)
		{
			index.Index?.Build();
		}
		foreach (PreloaderReverseRelationSetting reverseRelation in reverseRelations)
		{
			BGField field = reverseRelation.Field;
			if (field is BGAbstractRelationI bGAbstractRelationI)
			{
				bGAbstractRelationI.GetRelatedIn(BGId.Empty);
			}
		}
	}

	public bool IsUsedByRelations(BGMetaEntity meta)
	{
		return ReferencedMetas.Contains(meta.Id);
	}

	public bool IsUsedByNestedMeta(BGField field)
	{
		if (field.Meta is BGMetaNested bGMetaNested)
		{
			return bGMetaNested.OwnerRelationId == field.Id;
		}
		return false;
	}

	public PreloaderMetaSetting GetMetaSetting(BGId metaId)
	{
		if (!MetaId2settings.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public PreloaderMetaSetting AddMetaSetting(BGMetaEntity meta)
	{
		PreloaderMetaSetting preloaderMetaSetting = new PreloaderMetaSetting
		{
			Meta = meta
		};
		tableSettings.Add(preloaderMetaSetting);
		MetaId2settings[meta.Id] = preloaderMetaSetting;
		return preloaderMetaSetting;
	}

	public void RemoveMetaSetting(BGId metaId)
	{
		string idStr = metaId.ToString();
		tableSettings.RemoveAll((PreloaderMetaSetting setting) => idStr == setting.MetaId.ToString());
		MetaId2settings.Remove(metaId);
	}

	public PreloaderKeySetting GetKeySetting(BGId keyId)
	{
		if (!KeyId2settings.TryGetValue(keyId, out var value))
		{
			return null;
		}
		return value;
	}

	public PreloaderKeySetting AddKeySetting(BGKey key)
	{
		PreloaderKeySetting preloaderKeySetting = new PreloaderKeySetting
		{
			Key = key
		};
		keys.Add(preloaderKeySetting);
		KeyId2settings[key.Id] = preloaderKeySetting;
		return preloaderKeySetting;
	}

	public void RemoveKeySetting(BGId keyId)
	{
		string idStr = keyId.ToString();
		keys.RemoveAll((PreloaderKeySetting setting) => idStr == setting.KeyId);
		KeyId2settings.Remove(keyId);
	}

	public PreloaderIndexSetting GetIndexSetting(BGId indexId)
	{
		if (!IndexId2settings.TryGetValue(indexId, out var value))
		{
			return null;
		}
		return value;
	}

	public PreloaderIndexSetting AddIndexSetting(BGIndex index)
	{
		PreloaderIndexSetting preloaderIndexSetting = new PreloaderIndexSetting
		{
			Index = index
		};
		indexes.Add(preloaderIndexSetting);
		IndexId2settings[index.Id] = preloaderIndexSetting;
		return preloaderIndexSetting;
	}

	public void RemoveIndexSetting(BGId indexId)
	{
		string idStr = indexId.ToString();
		indexes.RemoveAll((PreloaderIndexSetting setting) => idStr == setting.IndexId);
		IndexId2settings.Remove(indexId);
	}

	public PreloaderReverseRelationSetting GetReverseRelationSetting(BGId indexId)
	{
		if (!ReverseRelationsId2settings.TryGetValue(indexId, out var value))
		{
			return null;
		}
		return value;
	}

	public PreloaderReverseRelationSetting AddReverseRelationSetting(BGField field)
	{
		PreloaderReverseRelationSetting preloaderReverseRelationSetting = new PreloaderReverseRelationSetting
		{
			Field = field
		};
		reverseRelations.Add(preloaderReverseRelationSetting);
		ReverseRelationsId2settings[field.Id] = preloaderReverseRelationSetting;
		return preloaderReverseRelationSetting;
	}

	public void RemoveReverseRelationSetting(BGId relationId)
	{
		string idStr = relationId.ToString();
		reverseRelations.RemoveAll((PreloaderReverseRelationSetting setting) => idStr == setting.FieldId);
		ReverseRelationsId2settings.Remove(relationId);
	}
}
