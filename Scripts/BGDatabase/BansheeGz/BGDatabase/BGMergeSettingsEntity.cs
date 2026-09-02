using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGMergeSettingsEntity : BGMergeSettingsA, BGConfigurableBinaryI, ICloneable
{
	[Serializable]
	public class MetaSettings : ICloneable, BGConfigurableBinaryI
	{
		[SerializeField]
		private bool useIncludedFields;

		[SerializeField]
		private bool addMissing;

		[SerializeField]
		private bool updateMatching;

		[SerializeField]
		private bool removeOrphaned;

		[SerializeField]
		private BGIdList fields = new BGIdList();

		public bool UseIncludedFields
		{
			get
			{
				return useIncludedFields;
			}
			set
			{
				if (useIncludedFields != value)
				{
					useIncludedFields = value;
					FireOnChange();
				}
			}
		}

		public bool AddMissing
		{
			get
			{
				return addMissing;
			}
			set
			{
				if (addMissing != value)
				{
					addMissing = value;
					FireOnChange();
				}
			}
		}

		public bool UpdateMatching
		{
			get
			{
				return updateMatching;
			}
			set
			{
				if (updateMatching != value)
				{
					updateMatching = value;
					FireOnChange();
				}
			}
		}

		public bool RemoveOrphaned
		{
			get
			{
				return removeOrphaned;
			}
			set
			{
				if (removeOrphaned != value)
				{
					removeOrphaned = value;
					FireOnChange();
				}
			}
		}

		public bool Included
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

		public int CountFields => fields?.Count ?? 0;

		public event Action OnChange;

		public void AddField(BGId fieldId)
		{
			if (fields == null)
			{
				fields = new BGIdList();
			}
			if (!fields.Contains(fieldId))
			{
				fields.Add(fieldId);
				FireOnChange();
			}
		}

		public void RemoveField(BGId fieldId)
		{
			if (fields != null && fields.Contains(fieldId))
			{
				fields.Remove(fieldId);
				FireOnChange();
			}
		}

		public BGId GetField(int index)
		{
			if (fields == null)
			{
				throw new BGException("Can not get field with index $ : fields are null", index);
			}
			return fields[index];
		}

		public object Clone()
		{
			return new MetaSettings
			{
				addMissing = addMissing,
				updateMatching = updateMatching,
				removeOrphaned = removeOrphaned,
				useIncludedFields = useIncludedFields,
				fields = new BGIdList(fields)
			};
		}

		public void CopyFrom(MetaSettings source)
		{
			if (source != null)
			{
				addMissing = source.addMissing;
				updateMatching = source.updateMatching;
				removeOrphaned = source.removeOrphaned;
				useIncludedFields = source.useIncludedFields;
				fields = ((source.fields == null) ? new BGIdList() : new BGIdList(source.fields));
			}
		}

		public bool HasField(BGId fieldId)
		{
			if (fields != null)
			{
				return fields.Contains(fieldId);
			}
			return false;
		}

		public void Exclude()
		{
			if (Included)
			{
				bool flag = (UpdateMatching = (removeOrphaned = false));
				AddMissing = flag;
				FireOnChange();
			}
		}

		public void ForEachField(Action<BGId> action)
		{
			if (fields != null)
			{
				for (int num = fields.Count - 1; num >= 0; num--)
				{
					action(fields[num]);
				}
			}
		}

		private void FireOnChange()
		{
			OnChange?.Invoke();
		}

		public byte[] ConfigToBytes()
		{
			int fieldsCount = fields?.Count ?? 0;
			BGBinaryWriter writer = new BGBinaryWriter(8 + fieldsCount * 16);
			writer.AddInt(1);
			writer.AddBool(addMissing);
			writer.AddBool(updateMatching);
			writer.AddBool(removeOrphaned);
			writer.AddBool(useIncludedFields);
			writer.AddArray(() =>
			{
				for (int i = 0; i < fieldsCount; i++)
				{
					writer.AddId(fields[i]);
				}
			}, fieldsCount);
			return writer.ToArray();
		}

		public void ConfigFromBytes(ArraySegment<byte> config)
		{
			BGBinaryReader reader = new BGBinaryReader(config);
			int num = reader.ReadInt();
			if (num == 1)
			{
				addMissing = reader.ReadBool();
				updateMatching = reader.ReadBool();
				removeOrphaned = reader.ReadBool();
				useIncludedFields = reader.ReadBool();
				if (fields == null)
				{
					fields = new BGIdList();
				}
				fields.Clear();
				reader.ReadArray(() =>
				{
					fields.Add(reader.ReadId());
				});
				return;
			}
			throw new BGException("Unknown version: $", num);
		}
	}

	[Serializable]
	public class HashtableId2MetaSettings : BGHashtableIdKey<MetaSettings>
	{
	}

	public interface IUpdateMatchingReceiver
	{
		bool OnBeforeUpdate(BGEntity from, BGEntity to);
	}

	public interface IUpdateMatchingFieldReceiver
	{
		bool OnBeforeFieldUpdate(BGField fromField, BGField toField, BGEntity from, BGEntity to);
	}

	public interface IAddMissingReceiver
	{
		bool OnBeforeAdd(BGEntity fromEntity);
	}

	public interface IRemoveOrphanedReceiver
	{
		bool OnBeforeDelete(BGEntity toEntity);
	}

	public interface IMergeReceiver
	{
		bool OnBeforeMerge(BGRepo from, BGRepo to);

		void OnAfterMerge(BGRepo from, BGRepo to);
	}

	public interface ISaveLoadAddonSavedEntityFilter
	{
		bool OnSaveEntity(BGEntity entity);
	}

	[SerializeField]
	private HashtableId2MetaSettings id2Meta = new HashtableId2MetaSettings();

	[SerializeField]
	private string controllerType;

	public string ControllerType
	{
		get
		{
			return controllerType;
		}
		set
		{
			if (!string.Equals(controllerType, value, StringComparison.Ordinal))
			{
				controllerType = value;
				FireOnChange();
			}
		}
	}

	public Predicate<BGField> AddMissingFieldFilter { get; set; }

	public object NewController(BGLogger logger)
	{
		if (string.IsNullOrEmpty(controllerType))
		{
			return null;
		}
		try
		{
			return BGUtil.Create<object>(controllerType, includePrivateConstructors: false, Array.Empty<object>());
		}
		catch (Exception ex)
		{
			Debug.Log("[WARNING!] BGDatabase: Controller object can not be created using " + controllerType + " type! See the next line for error details!");
			Debug.LogException(ex);
			logger?.AppendLine("Controller Type is set up, however the object can not be created (the error is $). Skipping..", ex.Message);
		}
		return null;
	}

	public MetaSettings GetSettings(BGId metaId)
	{
		return BGUtil.Get(id2Meta, metaId);
	}

	public bool Has(BGId metaId)
	{
		return id2Meta.ContainsKey(metaId);
	}

	public void Remove(BGId metaId)
	{
		if (Has(metaId))
		{
			id2Meta.Remove(metaId);
			FireOnChange();
		}
	}

	public bool HasAny(BGRepo repo)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
		}
		if (base.IncludedByDefault && repo.CountMeta > 0)
		{
			return true;
		}
		if (id2Meta == null)
		{
			return false;
		}
		foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
		{
			if (repo.HasMeta(id2Metum.Key))
			{
				return true;
			}
		}
		return false;
	}

	public MetaSettings Ensure(BGId metaId, bool copyFlags = false)
	{
		if (id2Meta.ContainsKey(metaId))
		{
			return id2Meta[metaId];
		}
		MetaSettings metaSettings = new MetaSettings();
		if (copyFlags)
		{
			metaSettings.AddMissing = addMissing;
			metaSettings.UpdateMatching = updateMatching;
			metaSettings.RemoveOrphaned = removeOrphaned;
		}
		metaSettings.OnChange += base.FireOnChange;
		id2Meta[metaId] = metaSettings;
		FireOnChange();
		return metaSettings;
	}

	public bool IsMetaIncluded(BGId metaId)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
		}
		if (!Has(metaId))
		{
			return base.IncludedByDefault;
		}
		return GetSettings(metaId).Included;
	}

	public bool IsFieldIncluded(BGField field)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
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
		if (settings.UseIncludedFields)
		{
			return settings.HasField(field.Id);
		}
		return true;
	}

	public object Clone()
	{
		BGMergeSettingsEntity bGMergeSettingsEntity = new BGMergeSettingsEntity
		{
			Mode = base.Mode,
			addMissing = addMissing,
			updateMatching = updateMatching,
			removeOrphaned = removeOrphaned,
			controllerType = controllerType
		};
		foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
		{
			bGMergeSettingsEntity.id2Meta[id2Metum.Key] = (MetaSettings)id2Metum.Value.Clone();
		}
		return bGMergeSettingsEntity;
	}

	public void RemoveNotExistent(BGRepo repo, BGMergerEntity.ParseResultI parseResult)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (IsMetaIncluded(meta.Id))
			{
				if (!parseResult.HasEntitySheet(meta.Id))
				{
					ExcludeMeta(meta.Id);
				}
				else
				{
					meta.ForEachField((BGField field) =>
					{
						if (IsFieldIncluded(field) && !parseResult.HasFieldInEntitySheet(meta.Id, field.Id))
						{
							ExcludeField(field);
						}
					});
				}
			}
		});
	}

	public void ExcludeField(BGField field)
	{
		BGId metaId = field.MetaId;
		if (!IsMetaIncluded(metaId))
		{
			return;
		}
		bool flag = Has(metaId);
		MetaSettings settings = null;
		if (updateMatching && !flag)
		{
			settings = Ensure(metaId, copyFlags: true);
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (settings == null)
		{
			settings = GetSettings(metaId);
		}
		if (!settings.UpdateMatching)
		{
			return;
		}
		if (settings.UseIncludedFields)
		{
			settings.RemoveField(field.Id);
		}
		else
		{
			settings.UseIncludedFields = true;
			field.Meta.ForEachField((BGField f) =>
			{
				settings.AddField(f.Id);
			});
			settings.RemoveField(field.Id);
		}
		FireOnChange();
	}

	public void ExcludeMeta(BGId metaId)
	{
		if (base.IncludedByDefault)
		{
			Ensure(metaId).Exclude();
		}
		else
		{
			Remove(metaId);
		}
		FireOnChange();
	}

	public bool IsAddingMissing(BGId metaId)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
		}
		if (!Has(metaId))
		{
			return addMissing;
		}
		return GetSettings(metaId).AddMissing;
	}

	public bool IsRemovingOrphaned(BGId metaId)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
		}
		if (!Has(metaId))
		{
			return removeOrphaned;
		}
		return GetSettings(metaId).RemoveOrphaned;
	}

	public bool IsUpdatingMatching(BGId metaId)
	{
		if (mode == BGMergeModeEnum.Transfer)
		{
			return true;
		}
		if (!Has(metaId))
		{
			return updateMatching;
		}
		return GetSettings(metaId).UpdateMatching;
	}

	public void ComplyTo(BGRepo repo)
	{
		List<BGId> list = new List<BGId>();
		foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
		{
			BGId key = id2Metum.Key;
			if (!repo.HasMeta(key))
			{
				list.Add(key);
				continue;
			}
			BGMetaEntity meta = repo.GetMeta(key);
			KeyValuePair<BGId, MetaSettings> pair1 = id2Metum;
			id2Metum.Value.ForEachField((BGId fieldId) =>
			{
				if (!meta.HasField(fieldId))
				{
					pair1.Value.RemoveField(fieldId);
				}
			});
		}
		foreach (BGId item in list)
		{
			Remove(item);
		}
		FireOnChange();
	}

	public void ForEachSetting(Action<MetaSettings> action)
	{
		foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
		{
			action(id2Metum.Value);
		}
	}

	public BGRepo NewRepo(BGRepo repo, bool copyValues)
	{
		return new BGRepo(repo, IsMetaIncluded, IsFieldIncluded, copyValues);
	}

	public BGRepo NewRepo(BGRepo repo, bool copyValues, Predicate<BGEntity> entityFilter)
	{
		return new BGRepo(repo, IsMetaIncluded, IsFieldIncluded, copyValues, entityFilter);
	}

	public int CountIncluded(BGRepo repo)
	{
		int count = 0;
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (IsMetaIncluded(meta.Id))
			{
				count++;
			}
		});
		return count;
	}

	public byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter(1024);
		writer.AddInt(2);
		writer.AddString(controllerType);
		writer.AddByte((byte)base.Mode);
		writer.AddBool(addMissing);
		writer.AddBool(updateMatching);
		writer.AddBool(removeOrphaned);
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, MetaSettings> id2Metum in id2Meta)
			{
				writer.AddId(id2Metum.Key);
				writer.AddByteArray(id2Metum.Value.ConfigToBytes());
			}
		}, id2Meta.Count);
		return writer.ToArray();
	}

	public void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			ReadFromBytes(bGBinaryReader);
			break;
		case 2:
			controllerType = bGBinaryReader.ReadString();
			ReadFromBytes(bGBinaryReader);
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	private void ReadFromBytes(BGBinaryReader reader)
	{
		mode = (BGMergeModeEnum)reader.ReadByte();
		addMissing = reader.ReadBool();
		updateMatching = reader.ReadBool();
		removeOrphaned = reader.ReadBool();
		ForEachSetting((MetaSettings settings) =>
		{
			settings.OnChange -= base.FireOnChange;
		});
		id2Meta.Clear();
		reader.ReadArray(() =>
		{
			MetaSettings metaSettings = new MetaSettings();
			BGId key = reader.ReadId();
			metaSettings.ConfigFromBytes(reader.ReadByteArray());
			id2Meta[key] = metaSettings;
			metaSettings.OnChange += base.FireOnChange;
		});
	}
}
