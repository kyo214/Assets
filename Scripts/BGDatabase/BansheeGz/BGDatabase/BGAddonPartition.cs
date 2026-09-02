using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "Partition", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerPartition")]
public class BGAddonPartition : BGAddon
{
	[Serializable]
	private class Settings
	{
		public bool DisableTemporarily;

		public bool DisableHTemporarily;

		public bool DisableVTemporarily;
	}

	public class BGPartitionLoadRequest
	{
		public bool fireEvents;
	}

	public class BGPartitionUnLoadRequest
	{
		public bool fireEvents;
	}

	public interface OnLoadHandler
	{
		void OnLoad(BGEntity partitionEntity);

		void OnUnload(BGEntity partitionEntity);

		void UpdateMergeSettings(BGMergeSettingsEntity settings);
	}

	private readonly HashSet<BGId> loaded = new HashSet<BGId>();

	private bool disableTemporarily;

	public const string PartitionMetaName = "DbPartition";

	public const string PartitionFieldName = "dbPartition";

	public const string PartitionFilePathKey = "p";

	private bool disableHorizontalTemporarily;

	private static List<OnLoadHandler> loadHandlers;

	public const string PartitionVerticalMetaName = "DbPartitionVertical";

	public const string PartitionVerticalNestedField = "DbPartitionVerticalMetas";

	public const string PartitionVerticalMetaRefField = "metaRef";

	public const string PartitionVerticalFilePathKey = "v";

	private bool disableVerticalTemporarily;

	public bool DisableTemporarily
	{
		get
		{
			return disableTemporarily;
		}
		set
		{
			if (disableTemporarily != value)
			{
				disableTemporarily = value;
				FireChange();
			}
		}
	}

	public override int OnMainDatabaseLoadOrder => 16;

	public bool Enabled
	{
		get
		{
			if (!EnabledHorizontal)
			{
				return EnabledVertical;
			}
			return true;
		}
	}

	public bool EnabledHorizontal
	{
		get
		{
			if (disableTemporarily || disableHorizontalTemporarily)
			{
				return false;
			}
			BGMetaEntity partitionMeta = PartitionMeta;
			if (partitionMeta == null)
			{
				return false;
			}
			if (partitionMeta.CountEntities == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool DisableHorizontalTemporarily
	{
		get
		{
			return disableHorizontalTemporarily;
		}
		set
		{
			if (disableHorizontalTemporarily != value)
			{
				disableHorizontalTemporarily = value;
				FireChange();
			}
		}
	}

	public BGMetaEntity PartitionMeta => Repo.GetMeta("DbPartition");

	private BGMetaEntity PartitionMetaWithCheck
	{
		get
		{
			BGMetaEntity partitionMeta = PartitionMeta;
			if (partitionMeta == null)
			{
				throw new BGException("Can not find $ meta!", "DbPartition");
			}
			if (partitionMeta.CountEntities == 0)
			{
				throw new BGException("$ meta does not have any entity!", "DbPartition");
			}
			return partitionMeta;
		}
	}

	private static List<OnLoadHandler> LoadHandlers
	{
		get
		{
			if (loadHandlers != null)
			{
				return loadHandlers;
			}
			loadHandlers = new List<OnLoadHandler>();
			List<Type> allImplementations = BGUtil.GetAllImplementations(typeof(OnLoadHandler));
			if (allImplementations != null && allImplementations.Count > 0)
			{
				foreach (Type item in allImplementations)
				{
					loadHandlers.Add((OnLoadHandler)Activator.CreateInstance(item));
				}
			}
			return loadHandlers;
		}
	}

	public bool DisableVerticalTemporarily
	{
		get
		{
			return disableVerticalTemporarily;
		}
		set
		{
			if (disableVerticalTemporarily != value)
			{
				disableVerticalTemporarily = value;
				FireChange();
			}
		}
	}

	public bool EnabledVertical
	{
		get
		{
			if (disableTemporarily || disableVerticalTemporarily)
			{
				return false;
			}
			BGMetaEntity partitionVerticalMeta = PartitionVerticalMeta;
			if (partitionVerticalMeta == null)
			{
				return false;
			}
			if (partitionVerticalMeta.CountEntities == 0)
			{
				return false;
			}
			return true;
		}
	}

	public BGMetaEntity PartitionVerticalMeta => Repo.GetMeta("DbPartitionVertical");

	public override void OnDelete(BGRepo repo)
	{
		repo.GetMeta("DbPartition")?.Delete();
		repo.GetMeta("DbPartitionVertical")?.Delete();
		loaded.Clear();
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		return new BGAddonPartition
		{
			Repo = repo,
			disableTemporarily = disableTemporarily,
			disableHorizontalTemporarily = disableHorizontalTemporarily,
			disableVerticalTemporarily = disableVerticalTemporarily
		};
	}

	public override void OnMainDatabaseLoad()
	{
		LoadVertical();
		if (!Application.isPlaying && Application.isEditor && !BGPrivate.GetProperty<bool>(BGUtil.GetType("UnityEditor.EditorApplication"), "isPlayingOrWillChangePlaymode"))
		{
			LoadAll();
		}
	}

	public BGPartitionSaveModel Save(string basicPath)
	{
		return new BGPartitionSaveProcessor().Save(new BGPartitionSaveContext(basicPath, Repo, this, EnabledHorizontal ? new BGPartitionsModel(Repo) : null, new BGMetaPartitionModelProvider(), verticalDisabled: false));
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter();
		bGBinaryWriter.AddInt(2);
		bGBinaryWriter.AddBool(disableTemporarily);
		bGBinaryWriter.AddBool(disableHorizontalTemporarily);
		bGBinaryWriter.AddBool(disableVerticalTemporarily);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			disableTemporarily = bGBinaryReader.ReadBool();
			break;
		case 2:
			disableTemporarily = bGBinaryReader.ReadBool();
			disableHorizontalTemporarily = bGBinaryReader.ReadBool();
			disableVerticalTemporarily = bGBinaryReader.ReadBool();
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			DisableTemporarily = disableTemporarily,
			DisableHTemporarily = disableHorizontalTemporarily,
			DisableVTemporarily = disableVerticalTemporarily
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		disableTemporarily = settings.DisableTemporarily;
		disableHorizontalTemporarily = settings.DisableHTemporarily;
		disableVerticalTemporarily = settings.DisableVTemporarily;
	}

	internal void CheckConfig()
	{
		if (!EnabledVertical)
		{
			return;
		}
		BGPartitionVerticalStructure structure = new BGPartitionVerticalStructure(Repo);
		Dictionary<BGId, BGEntity> meta2Partition = new Dictionary<BGId, BGEntity>();
		structure.ForEachPartition((BGEntity partition) =>
		{
			structure.ForEachMeta(partition, (BGMetaEntity meta) =>
			{
				if (meta2Partition.ContainsKey(meta.Id))
				{
					BGEntity bGEntity = BGUtil.Get(meta2Partition, meta.Id);
					if (bGEntity.Equals(partition))
					{
						throw new Exception("Invalid vertical partitioning config: meta " + meta.Name + " was included twice to partition: " + bGEntity.Name);
					}
					throw new Exception("Invalid vertical partitioning config: meta " + meta.Name + " was included to two different partitions: " + bGEntity.Name + " and " + partition.Name);
				}
				meta2Partition.Add(meta.Id, partition);
			});
		});
	}

	public void LoadAll()
	{
		if (disableTemporarily)
		{
			return;
		}
		BGMetaEntity meta = Repo.GetMeta("DbPartition");
		if (meta != null && meta.CountEntities != 0)
		{
			meta.ForEachEntity((BGEntity entity) =>
			{
				LoadNoEvent(entity, null);
			});
		}
	}

	public void Load(string partitionName, BGPartitionLoadRequest request = null)
	{
		BGMetaEntity partitionMetaWithCheck = PartitionMetaWithCheck;
		BGEntity entity = partitionMetaWithCheck.GetEntity(partitionName);
		if (entity == null)
		{
			throw new BGException("Can not get a partition with name $", partitionName);
		}
		Load(entity.Index, request);
	}

	public void Load(BGId partitionId, BGPartitionLoadRequest request = null)
	{
		BGMetaEntity partitionMetaWithCheck = PartitionMetaWithCheck;
		BGEntity entity = partitionMetaWithCheck.GetEntity(partitionId);
		if (entity == null)
		{
			throw new BGException("Can not get a partition with Id $", partitionId);
		}
		Load(entity.Index, request);
	}

	public void Load(int partitionIndex, BGPartitionLoadRequest request = null)
	{
		if (!disableTemporarily)
		{
			if (!Application.isPlaying && !BGUtil.TestIsRunning)
			{
				Debug.Log("Trying to load partition while application is not playing. Operation is cancelled.");
				return;
			}
			BGEntity partitionEntity = GetPartitionEntity(partitionIndex);
			LoadNoEvent(partitionEntity, request);
			OnAfterLoad(partitionEntity);
		}
	}

	private static void OnAfterLoad(BGEntity partitionEntity)
	{
		foreach (OnLoadHandler loadHandler in LoadHandlers)
		{
			loadHandler.OnLoad(partitionEntity);
		}
	}

	private void LoadNoEvent(BGEntity partitionEntity, BGPartitionLoadRequest request)
	{
		BGLoaderForRepo bGLoaderForRepo = Repo.RepoLoader ?? BGRepo.DefaultRepoLoader;
		byte[] array = null;
		BGLoaderForRepo.LoadRequest loadRequest = null;
		if (bGLoaderForRepo != null)
		{
			loadRequest = new BGLoaderForRepo.LoadRequest(Repo.RepoAssetPath ?? BGRepo.DefaultRepoAssetPath, GetPartitionPaths(partitionEntity.Id));
			array = bGLoaderForRepo.Load(loadRequest);
		}
		if (array == null)
		{
			string text = ((loadRequest == null) ? ("Partition ID is " + partitionEntity.Id.ToString()) : ("File path is " + loadRequest.ToPath(bGLoaderForRepo)));
			throw new BGException("Can not load partition file for $ partition! $", partitionEntity.Name, text);
		}
		BGRepo repo = new BGRepo(array);
		loaded.Add(partitionEntity.Id);
		if (request != null && request.fireEvents)
		{
			Merge(repo, Repo);
			return;
		}
		BGPartitionSnapshot bGPartitionSnapshot = new BGPartitionSnapshot(Repo);
		Repo.Events.WithEventsDisabled(() =>
		{
			Merge(repo, Repo);
		});
		bGPartitionSnapshot.MarkKeysAndIndexesDirty();
		Repo.Events.FireAnyChange();
	}

	public void Unload(string partitionName, BGPartitionUnLoadRequest request = null)
	{
		BGMetaEntity partitionMetaWithCheck = PartitionMetaWithCheck;
		BGEntity entity = partitionMetaWithCheck.GetEntity(partitionName);
		if (entity == null)
		{
			throw new BGException("Can not get a partition with name $", partitionName);
		}
		Unload(entity.Index, request);
	}

	public void Unload(BGId partitionId, BGPartitionUnLoadRequest request = null)
	{
		BGMetaEntity partitionMetaWithCheck = PartitionMetaWithCheck;
		BGEntity entity = partitionMetaWithCheck.GetEntity(partitionId);
		if (entity == null)
		{
			throw new BGException("Can not get a partition with Id $", partitionId);
		}
		Unload(entity.Index, request);
	}

	public void Unload(int partitionIndex, BGPartitionUnLoadRequest request = null)
	{
		if (disableTemporarily)
		{
			return;
		}
		if (!Application.isPlaying && !BGUtil.TestIsRunning)
		{
			Debug.Log("Trying to unload partition while application is not playing. Operation is cancelled.");
			return;
		}
		BGEntity partitionEntity = GetPartitionEntity(partitionIndex);
		if (request != null && request.fireEvents)
		{
			Unload(partitionEntity);
			return;
		}
		BGPartitionSnapshot bGPartitionSnapshot = new BGPartitionSnapshot(Repo);
		Repo.Events.WithEventsDisabled(() =>
		{
			Unload(partitionEntity);
		});
		bGPartitionSnapshot.MarkKeysAndIndexesDirty();
		Repo.Events.FireAnyChange();
	}

	private void Unload(BGEntity partitionEntity)
	{
		List<BGEntity> toDelete = new List<BGEntity>();
		BGMetaPartitionModelProvider bGMetaPartitionModelProvider = new BGMetaPartitionModelProvider();
		bGMetaPartitionModelProvider.ForEachRootModel(Repo, (BGMetaPartitionModelI model) =>
		{
			BGMetaEntity meta = model.Meta;
			toDelete.Clear();
			meta.ForEachEntity((BGEntity entity) =>
			{
				int? partitionIndex = model.GetPartitionIndex(entity);
				if (partitionIndex.HasValue && partitionIndex.Value == partitionEntity.Index)
				{
					toDelete.Add(entity);
				}
			});
			meta.DeleteEntities(toDelete);
			toDelete.Clear();
		});
		foreach (OnLoadHandler loadHandler in LoadHandlers)
		{
			loadHandler.OnUnload(partitionEntity);
		}
		loaded.Remove(partitionEntity.Id);
	}

	private BGEntity GetPartitionEntity(int partitionIndex)
	{
		BGMetaEntity partitionMetaWithCheck = PartitionMetaWithCheck;
		if (partitionIndex < 0 || partitionIndex >= partitionMetaWithCheck.CountEntities)
		{
			throw new BGException("Can not get partition entity with index $. Valid range is $ - $", partitionIndex, 0, partitionMetaWithCheck.CountEntities - 1);
		}
		return partitionMetaWithCheck.GetEntity(partitionIndex);
	}

	public bool IsLoaded(BGId entityId)
	{
		return loaded.Contains(entityId);
	}

	public void ForEachLoaded(Action<BGEntity> action)
	{
		BGMetaEntity partitionMeta = PartitionMeta;
		if (partitionMeta == null)
		{
			return;
		}
		foreach (BGId item in loaded)
		{
			BGEntity entity = partitionMeta.GetEntity(item);
			if (entity != null)
			{
				action(entity);
			}
		}
	}

	public static string[] GetPartitionPaths(BGId id)
	{
		return new string[2]
		{
			"p",
			ToFilePath(id)
		};
	}

	public static BGMetaEntity GetPartitionMeta(BGRepo repo)
	{
		return repo.GetMeta("DbPartition");
	}

	public static void CreatePartitionMeta(BGRepo repo)
	{
		new BGMetaRow(repo, "DbPartition").Addon = "Partition";
	}

	public static BGPartitionVerticalStructure CreatePartitionVerticalMetas(BGRepo repo)
	{
		return BGPartitionVerticalStructure.Create(repo);
	}

	public static void DeletePartitionMeta(BGRepo repo)
	{
		repo.GetMeta("DbPartition")?.Delete();
	}

	public static void DeletePartitionVerticalMetas(BGRepo repo)
	{
		BGPartitionVerticalStructure.Delete(repo);
	}

	public static BGMetaEntity GetPartitionVerticalMeta(BGRepo repo)
	{
		return repo.GetMeta("DbPartitionVertical");
	}

	public static bool SupportPartitioningField(BGMetaEntity meta)
	{
		if (string.Equals(meta.Name, "DbPartition"))
		{
			return false;
		}
		if (string.Equals(meta.Name, "DbPartitionVertical"))
		{
			return false;
		}
		if (!meta.SupportPartitioningField)
		{
			return false;
		}
		if (!BGLocalizationUglyHacks.SupportPartitioning(meta))
		{
			return false;
		}
		return true;
	}

	private static void Merge(BGRepo source, BGRepo target)
	{
		BGMergeSettingsEntity settings = new BGMergeSettingsEntity
		{
			AddMissing = true
		};
		foreach (OnLoadHandler loadHandler in LoadHandlers)
		{
			loadHandler.UpdateMergeSettings(settings);
		}
		Merge(source, target, settings);
	}

	public static void Merge(BGRepo source, BGRepo target, BGMergeSettingsEntity settings)
	{
		if (settings == null)
		{
			throw new Exception("Settings can not be null!");
		}
		new BGMergerEntity(null, source, target, settings).Merge();
	}

	public static string ToFilePath(BGId id)
	{
		return id.ToString().Replace('/', '!');
	}

	public static bool IsEnabled(BGRepo repo = null)
	{
		if (repo == null)
		{
			repo = BGRepo.I;
		}
		return repo.Addons.Get<BGAddonPartition>()?.Enabled ?? false;
	}

	private void LoadVertical()
	{
		if (!EnabledVertical)
		{
			return;
		}
		BGPartitionVerticalStructure structure = new BGPartitionVerticalStructure(Repo);
		BGLoaderForRepo loader = Repo.RepoLoader ?? BGRepo.DefaultRepoLoader;
		structure.ForEachPartition((BGEntity partition) =>
		{
			List<BGMetaEntity> metas = structure.GetMetas(partition);
			if (metas != null && metas.Count != 0)
			{
				byte[] array = null;
				BGLoaderForRepo.LoadRequest loadRequest = null;
				if (loader != null)
				{
					loadRequest = new BGLoaderForRepo.LoadRequest(Repo.RepoAssetPath ?? BGRepo.DefaultRepoAssetPath, "v", ToFilePath(partition.Id));
					array = loader.Load(loadRequest);
				}
				if (array == null)
				{
					string text = ((loadRequest == null) ? ("Partition name is " + partition.Name) : ("File path is " + loadRequest.ToPath(loader)));
					throw new BGException("Can not load vertical partition file for $ partition! $", partition.Name, text);
				}
				Merge(new BGRepo(array), Repo, new BGMergeSettingsEntity
				{
					AddMissing = true
				});
			}
		});
	}

	public static bool IsSupportedForVerticalPartitioning(BGMetaEntity meta)
	{
		if (!(meta is BGMetaRow))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(meta.Addon))
		{
			return false;
		}
		return true;
	}
}
