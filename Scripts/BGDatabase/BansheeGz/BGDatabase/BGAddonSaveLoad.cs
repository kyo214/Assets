using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "SaveLoad", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerSaveLoad")]
public class BGAddonSaveLoad : BGAddon
{
	[Serializable]
	private class Settings
	{
		public BGMergeSettingsEntity MergeSettings = new BGMergeSettingsEntity();

		public BGHashtableForSerialization<string, BGMergeSettingsEntity> AdditionalSettings = new BGHashtableForSerialization<string, BGMergeSettingsEntity>();
	}

	public interface BeforeSaveReciever
	{
		void OnBeforeSave();
	}

	public interface AfterLoadReciever
	{
		void OnAfterLoad();
	}

	public const string DefaultSettingsName = "Default";

	private BGMergeSettingsEntity mergeSettings = new BGMergeSettingsEntity();

	private readonly Dictionary<string, BGMergeSettingsEntity> name2Setting = new Dictionary<string, BGMergeSettingsEntity>();

	private static readonly List<BeforeSaveReciever> saveReceivers = new List<BeforeSaveReciever>();

	private static readonly List<AfterLoadReciever> loadReceivers = new List<AfterLoadReciever>();

	private BGSaveLoadEventsManager events;

	public BGMergeSettingsEntity MergeSettings => mergeSettings;

	public Dictionary<string, BGMergeSettingsEntity> Name2Setting => name2Setting;

	public BGAddonSaveLoad()
	{
		mergeSettings.OnChange += SettingsChanged;
	}

	public override string ConfigToString()
	{
		Settings settings = new Settings
		{
			MergeSettings = mergeSettings
		};
		foreach (KeyValuePair<string, BGMergeSettingsEntity> item in name2Setting)
		{
			settings.AdditionalSettings[item.Key] = item.Value;
		}
		return JsonUtility.ToJson(settings);
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		mergeSettings = settings.MergeSettings;
		mergeSettings.OnChange += SettingsChanged;
		ClearAdditionalSettings();
		foreach (KeyValuePair<string, BGMergeSettingsEntity> additionalSetting in settings.AdditionalSettings)
		{
			BGMergeSettingsEntity value = additionalSetting.Value;
			value.OnChange += SettingsChanged;
			name2Setting[additionalSetting.Key] = value;
		}
	}

	public override byte[] ConfigToBytes()
	{
		byte[] value = mergeSettings.ConfigToBytes();
		BGBinaryWriter writer = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(value));
		writer.AddInt(2);
		writer.AddByteArray(value);
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<string, BGMergeSettingsEntity> item in name2Setting)
			{
				writer.AddString(item.Key);
				writer.AddByteArray(item.Value.ConfigToBytes());
			}
		}, name2Setting.Count);
		return writer.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		ClearAdditionalSettings();
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		switch (num)
		{
		case 1:
			mergeSettings.ConfigFromBytes(reader.ReadByteArray());
			break;
		case 2:
			mergeSettings.ConfigFromBytes(reader.ReadByteArray());
			reader.ReadArray(() =>
			{
				string key = reader.ReadString();
				BGMergeSettingsEntity bGMergeSettingsEntity = new BGMergeSettingsEntity();
				bGMergeSettingsEntity.ConfigFromBytes(reader.ReadByteArray());
				bGMergeSettingsEntity.OnChange += SettingsChanged;
				name2Setting[key] = bGMergeSettingsEntity;
			});
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		BGAddonSaveLoad bGAddonSaveLoad = new BGAddonSaveLoad
		{
			Repo = repo,
			mergeSettings = (BGMergeSettingsEntity)mergeSettings.Clone()
		};
		bGAddonSaveLoad.mergeSettings.OnChange += bGAddonSaveLoad.SettingsChanged;
		foreach (KeyValuePair<string, BGMergeSettingsEntity> item in name2Setting)
		{
			BGMergeSettingsEntity bGMergeSettingsEntity = (BGMergeSettingsEntity)item.Value.Clone();
			bGMergeSettingsEntity.OnChange += bGAddonSaveLoad.SettingsChanged;
			bGAddonSaveLoad.name2Setting[item.Key] = bGMergeSettingsEntity;
		}
		return bGAddonSaveLoad;
	}

	private void ClearAdditionalSettings()
	{
		foreach (KeyValuePair<string, BGMergeSettingsEntity> item in name2Setting)
		{
			item.Value.OnChange -= SettingsChanged;
		}
		name2Setting.Clear();
	}

	private void SettingsChanged()
	{
		FireChange();
	}

	private void CheckEncryption(BGRepo saveRepo)
	{
		BGAddonSettings bGAddonSettings = Repo.Addons.Get<BGAddonSettings>();
		if (bGAddonSettings != null && !string.IsNullOrEmpty(bGAddonSettings.EncryptorType))
		{
			BGEncryptor encryptor = bGAddonSettings.Encryptor;
			if (encryptor != null)
			{
				saveRepo.Addons.Add(new BGAddonSettings
				{
					EncryptorType = bGAddonSettings.EncryptorType,
					EncryptorConfig = bGAddonSettings.EncryptorConfig
				});
			}
		}
	}

	public byte[] Save()
	{
		return SaveInternal(new BGSaveLoadAddonSaveContext());
	}

	public byte[] Save(BGSaveLoadAddonSaveContext context)
	{
		return SaveInternal(context);
	}

	private byte[] SaveInternal(BGSaveLoadAddonSaveContext context)
	{
		BGMainThreadRunner.EnsureMainThread("SaveLoad add-on should be run on main thread");
		if (context == null)
		{
			throw new Exception("Can not save, cause saveContext is null");
		}
		if (string.IsNullOrEmpty(context.ConfigName))
		{
			throw new Exception("Can not save, cause saveContext.ConfigName is null or empty");
		}
		BGMergeSettingsEntity value;
		if (context.ConfigName == "Default")
		{
			value = mergeSettings;
		}
		else if (!Name2Setting.TryGetValue(context.ConfigName, out value))
		{
			throw new Exception("Can not save, cause config with name " + context.ConfigName + " can not be found");
		}
		if (context.FireBeforeSaveEvents)
		{
			foreach (BeforeSaveReciever saveReceiver in saveReceivers)
			{
				try
				{
					saveReceiver.OnBeforeSave();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			List<BeforeSaveReciever> list = BGInterfaceFinder.FindObjects<BeforeSaveReciever>(searchForInActive: true);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						list[i].OnBeforeSave();
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
					}
				}
			}
		}
		if (context.MergeDataFromMTAddon)
		{
			BGAddonMT bGAddonMT = Repo.Addons.Get<BGAddonMT>();
			if (bGAddonMT != null && bGAddonMT.MergeOnSave)
			{
				bGAddonMT.Merge();
			}
		}
		object obj = value.NewController(null);
		BGMergeSettingsEntity.ISaveLoadAddonSavedEntityFilter saveController = obj as BGMergeSettingsEntity.ISaveLoadAddonSavedEntityFilter;
		BGRepo saveRepo = ((saveController != null) ? value.NewRepo(Repo, copyValues: true, (BGEntity entity) => !saveController.OnSaveEntity(entity)) : value.NewRepo(Repo, copyValues: true));
		CheckEncryption(saveRepo);
		return new BGRepoBinary().Write(saveRepo);
	}

	public void Load(byte[] data)
	{
		LoadInternal(new BGSaveLoadAddonLoadContext(new BGSaveLoadAddonLoadContext.LoadRequest("Default", data)));
	}

	public void Load(BGSaveLoadAddonLoadContext loadContext)
	{
		LoadInternal(loadContext);
	}

	private void LoadInternal(BGSaveLoadAddonLoadContext context)
	{
		BGMainThreadRunner.EnsureMainThread("SaveLoad add-on should be run on main thread");
		if (context == null)
		{
			throw new Exception("Can not load, cause loadContext is null");
		}
		if (context.Requests.Count == 0)
		{
			throw new Exception("Can not load, cause load requests are empty");
		}
		List<Tuple<BGMergeSettingsEntity, byte[]>> list = new List<Tuple<BGMergeSettingsEntity, byte[]>>();
		foreach (BGSaveLoadAddonLoadContext.LoadRequest request in context.Requests)
		{
			if (string.IsNullOrEmpty(request.ConfigName))
			{
				throw new Exception("Can not load, cause one of the requests has empty config name");
			}
			Tuple<BGMergeSettingsEntity, byte[]> item;
			if (request.ConfigName == "Default")
			{
				item = new Tuple<BGMergeSettingsEntity, byte[]>(mergeSettings, request.data);
			}
			else
			{
				if (!Name2Setting.TryGetValue(request.ConfigName, out var value) || value == null)
				{
					throw new Exception("Can not load, cause config with name " + request.ConfigName + " can not be found");
				}
				item = new Tuple<BGMergeSettingsEntity, byte[]>(value, request.data);
			}
			list.Add(item);
		}
		if (context.PreserveRequests != null && context.PreserveRequests.Count > 0)
		{
			foreach (BGSaveLoadAddonLoadContext.PreserveRequest preserveRequest in context.PreserveRequests)
			{
				if (string.IsNullOrEmpty(preserveRequest.ConfigName))
				{
					throw new Exception("Can not load, cause one of the reload requests has empty config name");
				}
				BGMergeSettingsEntity item2;
				if (preserveRequest.ConfigName == "Default")
				{
					item2 = mergeSettings;
				}
				else
				{
					if (!Name2Setting.TryGetValue(preserveRequest.ConfigName, out var value2) || value2 == null)
					{
						throw new Exception("Can not load, cause config with name " + preserveRequest.ConfigName + " can not be found");
					}
					item2 = value2;
				}
				byte[] item3 = SaveInternal(new BGSaveLoadAddonSaveContext(preserveRequest.ConfigName));
				list.Add(new Tuple<BGMergeSettingsEntity, byte[]>(item2, item3));
			}
		}
		events?.BeforeLoad();
		if (context.ReloadDatabase)
		{
			BGRepo.Load();
		}
		foreach (var (settings, dataBytes) in list)
		{
			BGRepo bGRepo = new BGRepoBinary().Read(dataBytes);
			new BGMergerEntity(null, bGRepo, Repo, settings).Merge();
		}
		if (context.FireAfterLoadEvents)
		{
			foreach (AfterLoadReciever loadReceiver in loadReceivers)
			{
				try
				{
					loadReceiver.OnAfterLoad();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			List<AfterLoadReciever> list2 = BGInterfaceFinder.FindObjects<AfterLoadReciever>();
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					try
					{
						list2[i].OnAfterLoad();
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
					}
				}
			}
		}
		if (events != null && context.ReloadDatabase)
		{
			BGAddonSaveLoad bGAddonSaveLoad = Repo.Addons.Get<BGAddonSaveLoad>();
			bGAddonSaveLoad.events = events;
			events.Addon = bGAddonSaveLoad;
			events.AfterLoad();
			events = null;
		}
	}

	public void AddEntityListener(BGEntityPointer pointer, EventHandler<BGSaveLoadEventArgsEntityChanged> handler)
	{
		if (pointer == null)
		{
			throw new Exception("entity pointer can not be null");
		}
		events = events ?? new BGSaveLoadEventsManager(this);
		events.Add(pointer, handler);
	}

	public void RemoveEntityListener(BGEntityPointer pointer, EventHandler<BGSaveLoadEventArgsEntityChanged> handler)
	{
		if (pointer == null)
		{
			throw new Exception("entity pointer can not be null");
		}
		if (events != null)
		{
			events.Remove(pointer, handler);
		}
	}

	public void AddCellListener(BGCellPointer pointer, EventHandler<BGSaveLoadEventArgsCellChanged> handler)
	{
		if (pointer == null)
		{
			throw new Exception("cell pointer can not be null");
		}
		events = events ?? new BGSaveLoadEventsManager(this);
		events.Add(pointer, handler);
	}

	public void RemoveCellListener(BGCellPointer pointer, EventHandler<BGSaveLoadEventArgsCellChanged> handler)
	{
		if (pointer == null)
		{
			throw new Exception("cell pointer can not be null");
		}
		if (events != null)
		{
			events.Remove(pointer, handler);
		}
	}

	public static void AddSaveReceiver(BeforeSaveReciever receiver)
	{
		saveReceivers.Add(receiver);
	}

	public static void RemoveSaveReceiver(BeforeSaveReciever receiver)
	{
		saveReceivers.Remove(receiver);
	}

	public static void AddLoadReceiver(AfterLoadReciever receiver)
	{
		loadReceivers.Add(receiver);
	}

	public static void RemoveLoadReceiver(AfterLoadReciever receiver)
	{
		loadReceivers.Remove(receiver);
	}
}
