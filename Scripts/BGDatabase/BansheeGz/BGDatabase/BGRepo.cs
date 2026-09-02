using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepo
{
	public interface RepoReaderI
	{
		BGRepo Read(byte[] dataBytes);
	}

	public interface RepoWriterI
	{
		byte[] Write(BGRepo repo);
	}

	public const string Version = "1.8.9";

	public const string VersionBuild = "2024.02.09";

	private static readonly BGRepo instance = new BGRepo();

	private static readonly BGLoaderForRepo[] Loaders = new BGLoaderForRepo[3]
	{
		new BGLoaderForRepoCustom(),
		new BGLoaderForRepoStreamingAssets(),
		new BGLoaderForRepoResources()
	};

	public static RepoReaderI Reader = new BGRepoBinary();

	public static bool DefaultRepoLoaded;

	private static BGRepoCustomLoaderModel defaultRepoCustomLoaderModel;

	private static BGLoaderForRepo defaultRepoLoader;

	private static bool isLoading;

	private readonly BGRepoEvents repoEvents;

	public static bool DefaultRepoResetEventsOnLoad;

	private static BGEventsHolder defaultRepoEventsHolder;

	private readonly BGRepoAddons repoAddons;

	private BGMTService mtService;

	private readonly BGIdDictionary<BGMetaEntity> id2Meta = new BGIdDictionary<BGMetaEntity>();

	private readonly Dictionary<string, BGMetaEntity> name2Meta = new Dictionary<string, BGMetaEntity>();

	private readonly List<BGMetaEntity> metas = new List<BGMetaEntity>();

	private Dictionary<BGId, BGMetaView> id2View;

	private Dictionary<string, BGMetaView> name2View;

	private List<BGMetaView> views;

	public static BGRepo I
	{
		get
		{
			if (!DefaultRepoLoaded)
			{
				Load();
			}
			return instance;
		}
	}

	public static BGMTService M
	{
		get
		{
			if (!DefaultRepoLoaded)
			{
				Load();
			}
			return instance.MTService;
		}
	}

	public static string DefaultRepoErrorOnLoad { get; set; }

	public static int DefaultRepoAssetId { get; set; }

	public static string DefaultRepoAssetPath { get; set; }

	public static bool Ok => DefaultRepoErrorOnLoad == null;

	public static bool IsFallbackRepo
	{
		get
		{
			if (DefaultRepoAssetPath != null)
			{
				return DefaultRepoAssetPath.Contains("default");
			}
			return false;
		}
	}

	public static bool IsLoading => isLoading;

	public BGRepoEvents Events => repoEvents;

	public static BGRepoEvents DefaultEvents => instance.Events;

	public BGRepoAddons Addons => repoAddons;

	public int BinaryFormatVersion { get; set; }

	public BGLoaderForRepo RepoLoader { get; set; }

	public string RepoAssetPath { get; set; }

	public BGMTService MTService
	{
		get
		{
			if (mtService == null)
			{
				BGMainThreadRunner.EnsureMainThread("Multi-threading service should be created on main thread");
				BGAddonMT bGAddonMT = Addons.Get<BGAddonMT>();
				if (bGAddonMT != null)
				{
					mtService = bGAddonMT.CreateService();
				}
			}
			return mtService;
		}
	}

	public static BGRepoCustomLoaderModel DefaultRepoCustomLoaderModel => defaultRepoCustomLoaderModel;

	public static BGLoaderForRepo DefaultRepoLoader => defaultRepoLoader;

	public int CountMeta => metas.Count;

	internal BGId NewMetaId
	{
		get
		{
			BGId newId = BGId.NewId;
			while (id2Meta.ContainsKey(newId))
			{
				newId = BGId.NewId;
			}
			return newId;
		}
	}

	public int CountFields
	{
		get
		{
			int result = 0;
			ForEachMeta((BGMetaEntity meta) =>
			{
				result += meta.CountFields;
			});
			return result;
		}
	}

	public BGMetaEntity this[string metaName] => GetMeta(metaName);

	public BGMetaEntity this[BGId metaId] => GetMeta(metaId);

	public BGMetaEntity this[int index] => metas[index];

	[Obsolete]
	internal BGId NewEntityId => BGId.NewId;

	public int CountEntities
	{
		get
		{
			int num = 0;
			for (int i = 0; i < metas.Count; i++)
			{
				num += metas[i].CountEntities;
			}
			return num;
		}
	}

	public int CountViews => views?.Count ?? 0;

	internal BGId NewViewId
	{
		get
		{
			BGId newId = BGId.NewId;
			if (id2View != null)
			{
				while (id2View.ContainsKey(newId))
				{
					newId = BGId.NewId;
				}
			}
			return newId;
		}
	}

	private bool IsDefaultRepo => DefaultRepo(this);

	public static event Action OnBeforeLoad;

	public static event Action<bool> OnLoad;

	public BGRepo()
	{
		repoAddons = new BGRepoAddons(this);
		repoEvents = new BGRepoEvents(this);
	}

	public BGRepo(byte[] content)
		: this()
	{
		Load(content);
	}

	public BGRepo(BGRepo other, bool copyValues = false)
		: this(other, null, null, copyValues, null)
	{
	}

	public BGRepo(BGRepo other, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues)
		: this(other, metaFilter, fieldFilter, copyValues, null)
	{
	}

	public BGRepo(BGRepo other, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues, Predicate<BGEntity> entityFilter)
		: this()
	{
		other.CloneTo(this, metaFilter, fieldFilter, copyValues, entityFilter);
	}

	public static void Load()
	{
		Load((BGRepoLoadingContext)null);
	}

	public static void Load(BGRepoLoadingContext context)
	{
		if (isLoading)
		{
			return;
		}
		BGMainThreadRunner.EnsureMainThread("Database should be loaded on main thread");
		if (!DefaultRepoLoaded)
		{
			instance.Events.On = true;
		}
		DefaultRepoLoaded = true;
		DefaultRepoErrorOnLoad = null;
		isLoading = true;
		try
		{
			FireOnBeforeLoad();
			byte[] array = null;
			for (int i = 0; i < Loaders.Length; i++)
			{
				BGLoaderForRepo bGLoaderForRepo = Loaders[i];
				array = bGLoaderForRepo.Load((defaultRepoCustomLoaderModel == null) ? null : new BGLoaderForRepo.LoadRequest(defaultRepoCustomLoaderModel.MainDatabaseResource));
				if (array != null)
				{
					defaultRepoLoader = bGLoaderForRepo;
					break;
				}
			}
			if (array == null)
			{
				NoLuck("Can not load database from all possible locations. More info: http://www.bansheegz.com/BGDatabase/Setup/", includeLoadingInfo: false);
				return;
			}
			instance.Load(array);
			instance.RepoLoader = defaultRepoLoader;
			instance.RepoAssetPath = DefaultRepoAssetPath;
			List<BGAddon> addons = instance.Addons.Addons;
			addons.Sort((BGAddon a1, BGAddon a2) => a1.OnMainDatabaseLoadOrder.CompareTo(a2.OnMainDatabaseLoadOrder));
			foreach (BGAddon item in addons)
			{
				item.OnMainDatabaseLoad();
			}
		}
		catch (Exception ex)
		{
			NoLuck(ex.Message ?? ex.GetType().FullName, includeLoadingInfo: true);
			Debug.LogException(ex);
		}
		finally
		{
			context?.OnBeforeFiringOnLoad?.Invoke();
			try
			{
				FireOnLoad();
			}
			finally
			{
				isLoading = false;
			}
		}
	}

	private static void FireOnLoad()
	{
		if (OnLoad == null)
		{
			return;
		}
		try
		{
			OnLoad(DefaultRepoErrorOnLoad == null);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void FireOnBeforeLoad()
	{
		if (OnBeforeLoad == null)
		{
			return;
		}
		try
		{
			OnBeforeLoad();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void NoLuck(string message, bool includeLoadingInfo)
	{
		DefaultRepoErrorOnLoad = message ?? "unknown error!";
		if (includeLoadingInfo)
		{
			if (!string.IsNullOrEmpty(DefaultRepoAssetPath))
			{
				DefaultRepoErrorOnLoad = DefaultRepoErrorOnLoad + ", database path=" + DefaultRepoAssetPath;
			}
			if (defaultRepoLoader != null)
			{
				DefaultRepoErrorOnLoad = DefaultRepoErrorOnLoad + ", loader=" + defaultRepoLoader.Name;
			}
		}
		DefaultRepoLoaded = false;
		DefaultRepoAssetId = 0;
		DefaultRepoAssetPath = null;
	}

	public void Load(byte[] data)
	{
		repoAddons.Clear();
		BGRepo repo = Reader.Read(data);
		Events.WithEventsDisabled(() =>
		{
			repoAddons.AddFrom(repo.Addons);
		});
		MergeOnLoad(repo);
		if (Application.isPlaying && repoAddons.Has<BGAddonMT>())
		{
			mtService = null;
			BGMTService mTService = MTService;
		}
		if (Events.On)
		{
			Events.FireFullChange();
		}
	}

	public byte[] Save()
	{
		return new BGRepoBinary().Write(this);
	}

	private void MergeOnLoad(BGRepo repo)
	{
		Events.WithEventsDisabled(() =>
		{
			Merge(repo, new BGMergeSettingsEntity
			{
				Mode = BGMergeModeEnum.Transfer
			});
		});
		List<BGAddon> addons = repoAddons.Addons;
		for (int num = 0; num < addons.Count; num++)
		{
			addons[num].OnLoad();
		}
		if (!DefaultRepo(this) || defaultRepoEventsHolder == null)
		{
			return;
		}
		ForEachMeta((BGMetaEntity meta) =>
		{
			defaultRepoEventsHolder.TransferEventsTo(meta);
			if (!meta.LazyLoadingEnabledAndNotLoadedYet)
			{
				meta.ForEachField((BGField field) =>
				{
					defaultRepoEventsHolder.TransferEventsTo(field);
				});
			}
			else
			{
				BGEventsHolder holder = defaultRepoEventsHolder;
				meta.LazyLoader.AddAction(() =>
				{
					meta.ForEachField((BGField field) =>
					{
						holder.TransferEventsTo(field);
					});
				});
			}
		});
		defaultRepoEventsHolder = null;
	}

	public static void SetDefaultRepoContent(byte[] defaultRepoContent)
	{
		if (defaultRepoContent == null)
		{
			defaultRepoCustomLoaderModel = null;
		}
		else
		{
			defaultRepoCustomLoaderModel = new BGRepoCustomLoaderModel(new BGRepoCustomLoaderModel.DatabaseResource(defaultRepoContent));
		}
	}

	public static void SetDefaultRepoContentModel(BGRepoCustomLoaderModel defaultRepoContent)
	{
		defaultRepoCustomLoaderModel = defaultRepoContent;
	}

	public void Merge(BGRepo repo, BGMergeSettingsEntity settings = null)
	{
		if (repo == this)
		{
			throw new BGException("Can not merge with itself!");
		}
		new BGMergerEntity(null, repo, this, settings).Merge();
	}

	private void RebuildIndexes()
	{
		id2Meta.Clear();
		name2Meta.Clear();
		foreach (BGMetaEntity meta in metas)
		{
			id2Meta[meta.Id] = meta;
			name2Meta[meta.Name] = meta;
		}
	}

	public BGMetaEntity FindMeta(Predicate<BGMetaEntity> filter)
	{
		foreach (BGMetaEntity meta in metas)
		{
			if (filter(meta))
			{
				return meta;
			}
		}
		return null;
	}

	public bool HasMeta(BGId metaId)
	{
		return id2Meta.ContainsKey(metaId);
	}

	public bool HasMeta(string name)
	{
		return name2Meta.ContainsKey(name);
	}

	public BGId GetMetaId(string name)
	{
		return BGUtil.Get(name2Meta, name).Id;
	}

	public BGMetaEntity GetMeta(string name)
	{
		return BGUtil.Get(name2Meta, name);
	}

	public BGMetaEntity GetMeta(BGId id)
	{
		return BGUtil.Get(id2Meta, id);
	}

	public BGMetaEntity GetMeta(int index)
	{
		return this[index];
	}

	public T GetMeta<T>(BGId id) where T : BGMetaEntity
	{
		return (T)BGUtil.Get(id2Meta, id);
	}

	public T GetMeta<T>(string name) where T : BGMetaEntity
	{
		return (T)BGUtil.Get(name2Meta, name);
	}

	public void ForEachMeta(Action<BGMetaEntity> action)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			action(metas[i]);
		}
	}

	public void ForEachMeta(Action<BGMetaEntity> action, Predicate<BGMetaEntity> filter)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			BGMetaEntity obj = metas[i];
			if (filter == null || filter(obj))
			{
				action(obj);
			}
		}
	}

	public List<BGMetaEntity> FindMetas(List<BGMetaEntity> result = null, Predicate<BGMetaEntity> filter = null)
	{
		if (result == null)
		{
			result = new List<BGMetaEntity>();
		}
		else
		{
			result.Clear();
		}
		if (CountMeta == 0)
		{
			return result;
		}
		if (filter == null)
		{
			result.AddRange(metas);
		}
		else
		{
			ForEachMeta((BGMetaEntity meta) =>
			{
				result.Add(meta);
			}, filter);
		}
		return result;
	}

	internal void Register(BGMetaEntity meta)
	{
		ErrorIfMetaNameIsNotUnique(meta.Name);
		id2Meta[meta.Id] = meta;
		name2Meta[meta.Name] = meta;
		metas.Add(meta);
		repoEvents.MetaWasAdded(meta);
	}

	internal void Unregister(BGMetaEntity meta)
	{
		if (!id2Meta.ContainsKey(meta.Id))
		{
			throw new BGException("Meta with id ($) not found!", meta.Id);
		}
		id2Meta.Remove(meta.Id);
		name2Meta.Remove(meta.Name);
		metas.Remove(meta);
		repoEvents.MetaWasDeleted(meta);
	}

	internal void MetaNameWasChanged(string oldName, string newName)
	{
		BGMetaEntity bGMetaEntity = name2Meta[oldName];
		name2Meta[newName] = bGMetaEntity;
		name2Meta.Remove(oldName);
		repoEvents.MetaWasChanged(bGMetaEntity);
	}

	public void SwapMetas(int metaIndex1, int metaIndex2)
	{
		int countMeta = CountMeta;
		if (metaIndex1 < 0 || metaIndex2 < 0 || metaIndex1 >= countMeta || metaIndex2 >= countMeta)
		{
			throw new BGException("Invalid meta indexes for swap: $ and $ ", metaIndex1, metaIndex2);
		}
		List<BGMetaEntity> list = metas;
		List<BGMetaEntity> list2 = metas;
		BGMetaEntity value = metas[metaIndex2];
		BGMetaEntity value2 = metas[metaIndex1];
		list[metaIndex1] = value;
		list2[metaIndex2] = value2;
		if (Events.On)
		{
			Events.FireAnyChange();
		}
	}

	public int GetMetaIndex(BGId metaId)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			if (!(metas[i].Id != metaId))
			{
				return i;
			}
		}
		return -1;
	}

	public void ErrorIfMetaNameIsNotUnique(string metaName)
	{
		if (name2Meta.ContainsKey(metaName))
		{
			throw new BGException("Meta with name ($) already exists! name should be unique", metaName);
		}
		if (name2View != null && name2View.ContainsKey(metaName))
		{
			throw new BGException("View with name ($) already exists! name should be unique", metaName);
		}
	}

	public bool HasEntity(BGId entityId)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			if (metas[i].HasEntity(entityId))
			{
				return true;
			}
		}
		return false;
	}

	public BGEntity GetEntity(BGId entityId)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			BGEntity entity = metas[i].GetEntity(entityId);
			if (entity != null)
			{
				return entity;
			}
		}
		return null;
	}

	public void ForEachEntity(Action<BGEntity> action, Predicate<BGEntity> filter)
	{
		ForEachMeta((BGMetaEntity meta) =>
		{
			meta.ForEachEntity(action, filter);
		});
	}

	public void ForEachField(Action<BGField> action, Predicate<BGField> filter)
	{
		ForEachMeta((BGMetaEntity meta) =>
		{
			meta.ForEachField(action, filter);
		});
	}

	public BGField GetField(BGId fieldId)
	{
		for (int i = 0; i < metas.Count; i++)
		{
			BGMetaEntity bGMetaEntity = metas[i];
			int countFields = bGMetaEntity.CountFields;
			if (countFields == 0)
			{
				continue;
			}
			for (int j = 0; j < countFields; j++)
			{
				BGField field = bGMetaEntity.GetField(j);
				if (field.Id == fieldId)
				{
					return field;
				}
			}
		}
		return null;
	}

	[Obsolete("Use BGMetaEntity.NewFieldId instead")]
	internal BGId NewFieldId(BGMetaEntity meta)
	{
		return meta.NewFieldId;
	}

	public BGMetaView FindView(Predicate<BGMetaView> filter)
	{
		if (views == null)
		{
			return null;
		}
		foreach (BGMetaView view in views)
		{
			if (filter(view))
			{
				return view;
			}
		}
		return null;
	}

	public bool HasView(BGId viewId)
	{
		if (views == null)
		{
			return false;
		}
		return id2View.ContainsKey(viewId);
	}

	public bool HasView(string name)
	{
		if (views == null)
		{
			return false;
		}
		return name2View.ContainsKey(name);
	}

	public BGId GetViewId(string name)
	{
		if (views == null)
		{
			return BGId.Empty;
		}
		return BGUtil.Get(name2View, name).Id;
	}

	public BGMetaView GetView(string name)
	{
		if (views == null)
		{
			return null;
		}
		return BGUtil.Get(name2View, name);
	}

	public BGMetaView GetView(BGId id)
	{
		if (views == null)
		{
			return null;
		}
		return BGUtil.Get(id2View, id);
	}

	public BGMetaView GetView(int index)
	{
		if (views == null)
		{
			throw new Exception($"Can not get a view with index {index}- there are no views in the repository");
		}
		return views[index];
	}

	public void ForEachView(Action<BGMetaView> action)
	{
		ForEachView(action, null);
	}

	public void ForEachView(Action<BGMetaView> action, Predicate<BGMetaView> filter)
	{
		if (views == null)
		{
			return;
		}
		for (int i = 0; i < views.Count; i++)
		{
			BGMetaView obj = views[i];
			if (filter == null || filter(obj))
			{
				action(obj);
			}
		}
	}

	public List<BGMetaView> FindViews(List<BGMetaView> result = null, Predicate<BGMetaView> filter = null)
	{
		if (result == null)
		{
			result = new List<BGMetaView>();
		}
		else
		{
			result.Clear();
		}
		if (CountViews == 0)
		{
			return result;
		}
		if (filter == null)
		{
			result.AddRange(views);
		}
		else
		{
			ForEachView((BGMetaView view) =>
			{
				result.Add(view);
			}, filter);
		}
		return result;
	}

	internal void Register(BGMetaView view)
	{
		ErrorIfMetaNameIsNotUnique(view.Name);
		EnsureViewContainers();
		id2View[view.Id] = view;
		name2View[view.Name] = view;
		views.Add(view);
		repoEvents.ViewWasAdded(view);
	}

	internal void Unregister(BGMetaView view)
	{
		if (views != null)
		{
			if (!id2View.ContainsKey(view.Id))
			{
				throw new BGException("View with id ($) not found!", view.Id);
			}
			id2View.Remove(view.Id);
			name2View.Remove(view.Name);
			views.Remove(view);
			repoEvents.ViewWasDeleted(view);
		}
	}

	internal void ViewNameWasChanged(string oldName, string newName)
	{
		BGMetaView bGMetaView = name2View[oldName];
		name2View[newName] = bGMetaView;
		name2View.Remove(oldName);
		repoEvents.ViewWasChanged(bGMetaView);
	}

	public void SwapViews(int viewIndex1, int viewIndex2)
	{
		int countViews = CountViews;
		if (viewIndex1 < 0 || viewIndex2 < 0 || viewIndex1 >= countViews || viewIndex2 >= countViews)
		{
			throw new BGException("Invalid view indexes for swap: $ and $ ", viewIndex1, viewIndex2);
		}
		List<BGMetaView> list = views;
		List<BGMetaView> list2 = views;
		BGMetaView value = views[viewIndex2];
		BGMetaView value2 = views[viewIndex1];
		list[viewIndex1] = value;
		list2[viewIndex2] = value2;
		if (Events.On)
		{
			Events.FireAnyChange();
		}
	}

	public int GetViewIndex(BGId viewId)
	{
		if (views == null)
		{
			return -1;
		}
		for (int i = 0; i < views.Count; i++)
		{
			if (!(views[i].Id != viewId))
			{
				return i;
			}
		}
		return -1;
	}

	private void EnsureViewContainers()
	{
		if (views == null)
		{
			views = new List<BGMetaView>();
			id2View = new Dictionary<BGId, BGMetaView>();
			name2View = new Dictionary<string, BGMetaView>();
		}
	}

	public void Clear()
	{
		ClearInternal();
	}

	public void CloneTo(BGRepo repo, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues)
	{
		CloneTo(repo, metaFilter, fieldFilter, copyValues, null);
	}

	public void CloneTo(BGRepo repo, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues, Predicate<BGEntity> entityFilter)
	{
		repo.Addons.AddFrom(Addons);
		ForEachMeta((BGMetaEntity meta) =>
		{
			meta.CloneTo(repo, metaFilter, fieldFilter, copyValues, entityFilter);
		});
		ForEachView((BGMetaView view) =>
		{
			view.CloneTo(repo);
		});
	}

	public void Transaction(Action action)
	{
		BGRepo bGRepo = new BGRepo(this, copyValues: true);
		try
		{
			repoEvents.Batch(action);
		}
		catch (Exception exception)
		{
			Merge(bGRepo);
			Addons.Clear();
			Addons.AddFrom(bGRepo.Addons);
			Debug.LogException(exception);
			throw;
		}
	}

	private void ClearInternal()
	{
		ForEachMeta((BGMetaEntity meta) =>
		{
			if (!meta.LazyLoadingEnabledAndNotLoadedYet)
			{
				meta.ForEachField((BGField field) =>
				{
					field.OnDelete();
					field.Unload();
				});
				meta.ForEachKey((BGKey key) =>
				{
					key.OnDelete();
					key.Unload();
				});
				meta.ForEachIndex((BGIndex index) =>
				{
					index.OnDelete();
					index.Unload();
				});
				meta.ClearEntities();
			}
			meta.Unload();
		}, (BGMetaEntity meta) => meta.Repo == this);
		if (DefaultRepo(this))
		{
			if (DefaultRepoResetEventsOnLoad)
			{
				DefaultRepoResetEventsOnLoad = false;
				defaultRepoEventsHolder = null;
			}
			else if (defaultRepoEventsHolder == null)
			{
				defaultRepoEventsHolder = new BGEventsHolder();
				ForEachMeta((BGMetaEntity meta) =>
				{
					defaultRepoEventsHolder.TransferEventsFrom(meta);
					if (!meta.LazyLoadingEnabledAndNotLoadedYet)
					{
						meta.ForEachField((BGField field) =>
						{
							defaultRepoEventsHolder.TransferEventsFrom(field);
						});
					}
				}, (BGMetaEntity meta) => meta.Repo == this);
			}
		}
		ForEachView((BGMetaView view) =>
		{
			view.Unload();
		}, (BGMetaView view) => view.Repo == this);
		id2Meta.Clear();
		name2Meta.Clear();
		metas.Clear();
		if (views != null)
		{
			views.Clear();
			id2View.Clear();
			name2View.Clear();
		}
	}

	public static bool DefaultRepo(BGRepo repo)
	{
		return repo == instance;
	}
}
