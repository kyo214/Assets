using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGMetaEntity : BGMetaObject, IEquatable<BGMetaEntity>, IEnumerable<BGEntity>, IEnumerable
{
	public class MetaDescriptor : BGAttributeWithManager
	{
		public bool SkipInList;
	}

	private struct EntityEnumerator(BGStoreMeta store) : IEnumerator<BGEntity>, IDisposable, IEnumerator
	{
		private BGStoreMeta store = store;

		private int position = -1;

		object IEnumerator.Current => Current;

		public BGEntity Current
		{
			get
			{
				try
				{
					return store.Get(position);
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		public bool MoveNext()
		{
			position++;
			return position < store.Count;
		}

		public void Reset()
		{
			position = -1;
		}

		public void Dispose()
		{
			store = null;
		}
	}

	public class NewEntityContext
	{
		public readonly BGId? EntityId;

		public readonly Action<BGEntity> Callback;

		public NewEntityContext(Action<BGEntity> callback)
		{
			Callback = callback;
		}

		public NewEntityContext(BGId entityId, Action<BGEntity> callback)
		{
			EntityId = entityId;
			Callback = callback;
		}
	}

	private static readonly Dictionary<string, Func<BGRepo, BGId, string, BGMetaEntity>> MetaTypeName2Factory = new Dictionary<string, Func<BGRepo, BGId, string, BGMetaEntity>>();

	private static readonly List<Type> MetaTypesList = new List<Type>();

	private BGRepoEvents events;

	private bool uniqueName;

	private bool singleton;

	private bool emptyName;

	private bool userDefinedReadonly;

	internal BGLazyLoadMetaLoader LazyLoader;

	protected readonly BGStoreMeta Store = new BGStoreMeta();

	private BGEntity.EntityFactory factory;

	private bool factoryRetrieved;

	private BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>> entityId2DeleteEntityListener;

	private BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>> entityId2UpdateEntityListener;

	private readonly BGIdDictionary<BGField> id2Field = new BGIdDictionary<BGField>();

	private readonly Dictionary<string, BGField> name2Field = new Dictionary<string, BGField>();

	private readonly List<BGField> fields = new List<BGField>();

	private Dictionary<BGId, BGIndex> id2Index;

	private Dictionary<string, BGIndex> name2Index;

	private List<BGIndex> indexes;

	private Dictionary<BGId, BGKey> id2Key;

	private Dictionary<string, BGKey> name2Key;

	private List<BGKey> keys;

	public static List<Type> MetaTypes
	{
		get
		{
			if (MetaTypesList.Count != 0)
			{
				return MetaTypesList;
			}
			List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(BGMetaEntity));
			foreach (Type item in allSubTypes)
			{
				MetaTypesList.Add(item);
			}
			return MetaTypesList;
		}
	}

	public override string Name
	{
		set
		{
			if (!string.Equals(Name, value))
			{
				Repo.ErrorIfMetaNameIsNotUnique(value);
				string oldName = Name;
				base.Name = value;
				Repo.MetaNameWasChanged(oldName, Name);
			}
		}
	}

	public string DisplayName => BGAttribute.GetName(GetType());

	public BGRepo Repo { get; private set; }

	public virtual bool IsManagingItsOwnEntities => false;

	public virtual bool SupportPartitioningField => true;

	public virtual ushort TypeCode => 0;

	public virtual bool UniqueName
	{
		get
		{
			return uniqueName;
		}
		set
		{
			if (uniqueName != value)
			{
				uniqueName = value;
				if (events.On)
				{
					events.MetaWasChanged(this);
				}
			}
		}
	}

	public virtual bool Singleton
	{
		get
		{
			return singleton;
		}
		set
		{
			if (singleton != value)
			{
				singleton = value;
				if (events.On)
				{
					events.MetaWasChanged(this);
				}
			}
		}
	}

	public virtual bool EmptyName
	{
		get
		{
			return emptyName;
		}
		set
		{
			if (emptyName == value)
			{
				return;
			}
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			if (fields.Count > 0)
			{
				BGFieldEntityName nameField = NameField;
				if (nameField != null)
				{
					nameField.NameEmpty = value;
				}
			}
			emptyName = value;
			if (events.On)
			{
				events.MetaWasChanged(this);
			}
		}
	}

	public override int Index => Repo.GetMetaIndex(base.Id);

	public override string Comment
	{
		set
		{
			string text = base.Comment;
			if (string.Equals(value, text))
			{
				return;
			}
			bool flag = string.IsNullOrEmpty(value);
			bool flag2 = string.IsNullOrEmpty(text);
			if (!(flag & flag2))
			{
				SetComment(value);
				if (events.On)
				{
					events.MetaWasChanged(this);
				}
			}
		}
	}

	public override string ControllerType
	{
		set
		{
			string text = base.ControllerType;
			if (!string.Equals(value, text))
			{
				bool flag = string.IsNullOrEmpty(value);
				bool flag2 = string.IsNullOrEmpty(text);
				if (!(flag & flag2))
				{
					base.ControllerType = (string.IsNullOrEmpty(value) ? null : value);
					Repo.Events.MetaWasChanged(this);
				}
			}
		}
	}

	public bool UserDefinedReadonly
	{
		get
		{
			return userDefinedReadonly;
		}
		set
		{
			if (userDefinedReadonly != value)
			{
				userDefinedReadonly = value;
				Repo.Events.MetaWasChanged(this);
			}
		}
	}

	public bool LazyLoadingEnabledAndNotLoadedYet => LazyLoader != null;

	internal BGId NewEntityId
	{
		get
		{
			BGId newId = BGId.NewId;
			while (HasEntity(newId))
			{
				newId = BGId.NewId;
			}
			return newId;
		}
	}

	public BGEntity EntityFirst
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			if (Store.Count != 0)
			{
				return Store[0];
			}
			return null;
		}
	}

	public BGEntity this[BGId entityId]
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return Store[entityId];
		}
	}

	public BGEntity this[int index]
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return Store[index];
		}
	}

	public BGEntity this[string entityName]
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return Store[entityName];
		}
	}

	public int CountEntities
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return Store.Count;
		}
	}

	internal int EntitiesCapacity
	{
		set
		{
			Store.MinCapacity = value;
		}
	}

	internal BGId NewFieldId
	{
		get
		{
			BGId newId = BGId.NewId;
			while (HasField(newId))
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
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return fields.Count;
		}
	}

	public BGFieldEntityName NameField => GetField("name", errorIfNotFound: false) as BGFieldEntityName;

	public List<BGAbstractRelationI> RelationsInbound
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			List<BGAbstractRelationI> relationsInbound = new List<BGAbstractRelationI>();
			Repo.ForEachMeta((BGMetaEntity meta) =>
			{
				foreach (BGField field in meta.fields)
				{
					if (field is BGAbstractRelationI bGAbstractRelationI)
					{
						if (!(bGAbstractRelationI is BGRelationI { RelatedMeta: var relatedMeta }))
						{
							if (bGAbstractRelationI is BGManyTablesRelationI { RelatedMetas: { } relatedMetas } && relatedMetas.Contains(this))
							{
								relationsInbound.Add(bGAbstractRelationI);
							}
						}
						else if (relatedMeta != null && object.Equals(relatedMeta, this))
						{
							relationsInbound.Add(bGAbstractRelationI);
						}
					}
				}
			});
			return relationsInbound;
		}
	}

	public int CountIndexes
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return indexes?.Count ?? 0;
		}
	}

	public int CountKeys
	{
		get
		{
			if (LazyLoader != null)
			{
				LazyLoad();
			}
			return keys?.Count ?? 0;
		}
	}

	public event EventHandler<BGEventArgsAnyEntity> AnyEntityAdded;

	public event EventHandler<BGEventArgsAnyEntityBeforeAdded> AnyEntityBeforeAdded;

	public event EventHandler<BGEventArgsAnyEntity> AnyEntityDeleted;

	public event EventHandler<BGEventArgsAnyEntity> AnyEntityBeforeDeleted;

	public event EventHandler<BGEventArgsAnyEntityUpdated> AnyEntityUpdated;

	public event EventHandler<BGEventArgsAnyEntityUpdated> AnyEntityBeforeUpdated;

	public event EventHandler<BGEventArgsEntitiesOrder> EntitiesOrderChanged;

	protected abstract Func<BGRepo, BGId, string, BGMetaEntity> CreateMetaFactory();

	protected BGMetaEntity(BGRepo repo, string name)
		: this(repo, repo.NewMetaId, name)
	{
		new BGFieldEntityName(this, null).System = true;
		events = Repo.Events;
	}

	protected BGMetaEntity(BGRepo repo, BGId id, string name)
		: base(id, name)
	{
		Repo = repo ?? throw new BGException("Repo can not be null");
		Repo.Register(this);
		events = Repo.Events;
	}

	protected void Unregister()
	{
		Repo?.Unregister(this);
	}

	public override string ConfigToString()
	{
		return null;
	}

	public override void ConfigFromString(string config)
	{
	}

	public override byte[] ConfigToBytes()
	{
		return null;
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
	}

	public override void Delete()
	{
		if (base.IsDeleted)
		{
			return;
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		base.Delete();
		Repo.Events.Batch(() =>
		{
			List<BGField> list = new List<BGField>(fields);
			foreach (BGField item in list)
			{
				item.Delete();
			}
			if (CountEntities > 0)
			{
				DeleteEntities(new HashSet<BGEntity>(Store.ToList()));
			}
			List<BGAbstractRelationI> relationsInbound = RelationsInbound;
			if (!BGUtil.IsEmpty(relationsInbound))
			{
				foreach (BGAbstractRelationI item2 in relationsInbound)
				{
					if (!(item2 is BGRelationI))
					{
						if (item2 is BGManyTablesRelationI bGManyTablesRelationI)
						{
							bGManyTablesRelationI.RemoveRelatedMeta(this);
						}
					}
					else
					{
						((BGField)item2).Delete();
					}
				}
			}
			Repo.ForEachView((BGMetaView view) =>
			{
				if (view.Mappings.IsIncluded(base.Id))
				{
					view.Mappings.Remove(base.Id);
				}
			});
			Unregister();
		});
		Unload();
		Repo = null;
	}

	public override string ToString()
	{
		return "Meta [id:" + base.Id.ToString() + ", name:" + Name + ", type:" + GetType().FullName + "]";
	}

	private void SetComment(string comment)
	{
		base.Comment = comment;
	}

	public virtual BGMetaEntity CloneTo(BGRepo repo, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues)
	{
		return CloneTo(new BGCloneContextMeta(repo, metaFilter, fieldFilter, copyValues, null));
	}

	public virtual BGMetaEntity CloneTo(BGRepo repo, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues, Predicate<BGEntity> entityFilter)
	{
		return CloneTo(new BGCloneContextMeta(repo, metaFilter, fieldFilter, copyValues, entityFilter));
	}

	public virtual BGMetaEntity CloneTo(BGCloneContextMeta context)
	{
		if (context.metaFilter != null && !context.metaFilter(base.Id))
		{
			return null;
		}
		BGMetaEntity clone = CreateMetaFactory()(context.repo, base.Id, Name);
		clone.Addon = base.Addon;
		CopyAttributesTo(clone);
		if (context.copyValues)
		{
			if (context.entityFilter == null)
			{
				ForEachEntity((BGEntity entity) =>
				{
					clone.NewEntity(entity.Id);
				});
			}
			else
			{
				ForEachEntity((BGEntity entity) =>
				{
					clone.NewEntity(entity.Id);
				}, context.entityFilter);
			}
		}
		BGCloneContextField fieldCloneContext = new BGCloneContextField(clone, context.copyValues)
		{
			OnAfterFieldCreated = context.OnAfterFieldCreated
		};
		ForEachField((BGField field) =>
		{
			field.CloneTo(fieldCloneContext);
		}, context.fieldFilter);
		ForEachKey((BGKey key) =>
		{
			key.CloneTo(clone);
		});
		ForEachIndex((BGIndex index) =>
		{
			index.CloneTo(clone);
		});
		return clone;
	}

	public void CopyAttributesTo(BGMetaEntity clone)
	{
		clone.System = System;
		clone.uniqueName = UniqueName;
		clone.singleton = Singleton;
		clone.emptyName = EmptyName;
		clone.SetComment(Comment);
		clone.UserDefinedReadonly = UserDefinedReadonly;
		clone.ControllerType = ControllerType;
		byte[] array = ConfigToBytes();
		clone.ConfigFromBytes((array == null) ? new ArraySegment<byte>(Array.Empty<byte>()) : new ArraySegment<byte>(array));
	}

	internal void SwitchTo(BGRepo repo)
	{
		Repo = repo;
		Repo.Register(this);
		events = Repo.Events;
	}

	public bool Equals(BGMetaEntity other)
	{
		if (other != null)
		{
			return base.Id == other.Id;
		}
		return false;
	}

	public static BGMetaEntity Create(BGRepo repo, string type, BGId id, string name, string config, bool system, string addon, bool uniqueName, bool singleton, bool emptyName)
	{
		BGMetaEntity bGMetaEntity = Create(repo, type, id, name, system, addon, uniqueName, singleton, emptyName);
		bGMetaEntity.ConfigFromString(config);
		return bGMetaEntity;
	}

	public static BGMetaEntity Create(BGRepo repo, string type, BGId id, string name, ArraySegment<byte> config, bool system, string addon, bool uniqueName, bool singleton, bool emptyName)
	{
		BGMetaEntity bGMetaEntity = Create(repo, type, id, name, system, addon, uniqueName, singleton, emptyName);
		bGMetaEntity.ConfigFromBytes(config);
		return bGMetaEntity;
	}

	private static BGMetaEntity Create(BGRepo repo, string type, BGId id, string name, bool system, string addon, bool uniqueName, bool singleton, bool emptyName)
	{
		BGMetaEntity bGMetaEntity;
		if (MetaTypeName2Factory.TryGetValue(type, out var value))
		{
			bGMetaEntity = value(repo, id, name);
		}
		else
		{
			bGMetaEntity = BGUtil.Create<BGMetaEntity>(type, includePrivateConstructors: true, new object[3] { repo, id, name });
			MetaTypeName2Factory[type] = bGMetaEntity.CreateMetaFactory();
		}
		bGMetaEntity.System = system;
		bGMetaEntity.UniqueName = uniqueName;
		bGMetaEntity.Singleton = singleton;
		bGMetaEntity.EmptyName = emptyName;
		bGMetaEntity.Addon = addon;
		return bGMetaEntity;
	}

	internal void LazyLoad()
	{
		if (LazyLoader == null)
		{
			return;
		}
		BGLazyLoadMetaLoader lazyLoader = LazyLoader;
		LazyLoader = null;
		try
		{
			lazyLoader.Load();
		}
		catch (Exception)
		{
			LazyLoader = lazyLoader;
			throw;
		}
	}

	internal static BGMetaEntity FromBinary(BGBinaryReader binder, BGRepo repo)
	{
		int num = binder.ReadInt();
		switch (num)
		{
		case 1:
		{
			BGId bGId2 = binder.ReadId();
			string text3 = binder.ReadString();
			string type2 = binder.ReadString();
			ArraySegment<byte> config2 = binder.ReadByteArray();
			bool flag3 = binder.ReadBool();
			string text4 = binder.ReadString();
			bool flag4 = binder.ReadBool();
			bool flag5 = binder.ReadBool();
			bool flag6 = binder.ReadBool();
			BGMetaEntity bGMetaEntity2 = Create(repo, type2, bGId2, text3, config2, flag3, text4, flag4, flag5, flag6);
			bGMetaEntity2.SetComment(binder.ReadString());
			return bGMetaEntity2;
		}
		case 2:
		case 3:
		case 4:
		{
			ushort num2 = binder.ReadUShort();
			string type = null;
			if (num2 == 0)
			{
				type = binder.ReadString();
			}
			BGId bGId = binder.ReadId();
			string text = binder.ReadString();
			ArraySegment<byte> config = binder.ReadByteArray();
			bool flag = binder.ReadBool();
			string text2 = binder.ReadString();
			bool nameUnique = binder.ReadBool();
			bool flag2 = binder.ReadBool();
			bool nameEmpty = binder.ReadBool();
			BGMetaEntity bGMetaEntity = ((num2 == 0) ? Create(repo, type, bGId, text, config, flag, text2, nameUnique, flag2, nameEmpty) : BGMetaTypeCodeFactory.Instance.Create(repo, num2, bGId, text, config, flag, text2, nameUnique, flag2, nameEmpty));
			bGMetaEntity.SetComment(binder.ReadString());
			if (num >= 3)
			{
				bGMetaEntity.UserDefinedReadonly = binder.ReadBool();
			}
			if (num >= 4)
			{
				bGMetaEntity.ControllerType = binder.ReadString();
			}
			return bGMetaEntity;
		}
		default:
			throw new BGException("Can not read meta from binary array: unsupported version $", num);
		}
	}

	internal static void ToBinary(BGBinaryWriter builder, BGMetaEntity meta)
	{
		builder.AddInt(4);
		builder.AddUShort(meta.TypeCode);
		if (meta.TypeCode == 0)
		{
			builder.AddString(meta.GetType().AssemblyQualifiedName);
		}
		builder.AddId(meta.Id);
		builder.AddString(meta.Name);
		builder.AddByteArray(meta.ConfigToBytes());
		builder.AddBool(meta.System);
		builder.AddString(meta.Addon);
		builder.AddBool(meta.UniqueName);
		builder.AddBool(meta.Singleton);
		builder.AddBool(meta.EmptyName);
		builder.AddString(meta.Comment);
		builder.AddBool(meta.UserDefinedReadonly);
		builder.AddString(meta.ControllerType);
	}

	public void DeleteEntities(ICollection<BGEntity> entities)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (entities == null || entities.Count == 0)
		{
			return;
		}
		foreach (BGEntity entity2 in entities)
		{
			if (entity2 == null)
			{
				throw new BGException("One of the entities, submitted for removal, is null");
			}
			if (entity2.Meta != this)
			{
				throw new BGException("One of the entities, submitted for removal, does not belong to this meta");
			}
			if (entity2.IsDeleted)
			{
				throw new BGException("One of the entities, submitted for removal, already deleted");
			}
		}
		InvalidateNameCache();
		List<BGEntity> list = new List<BGEntity>(entities);
		list.Sort((BGEntity e1, BGEntity e2) => e2.Index.CompareTo(e1.Index));
		HashSet<BGId> allIds = new HashSet<BGId>();
		Exception exception = null;
		int count = entities.Count;
		for (int num = 0; num < count; num++)
		{
			BGEntity entity = list[num];
			if (allIds.Add(entity.Id))
			{
				Unregister(entity, clearRelations: false);
				entity.Unload();
				BGUtil.Catch(ref exception, () =>
				{
					FireEntityDeleted(entity);
				});
				entity.Index = -1;
			}
		}
		List<BGAbstractRelationI> relationsInbound = RelationsInbound;
		if (relationsInbound.Count > 0)
		{
			foreach (BGAbstractRelationI item in relationsInbound)
			{
				BGAbstractRelationI relation = item;
				BGUtil.Catch(ref exception, () =>
				{
					relation.ClearToValue(allIds);
				});
			}
		}
		if (exception == null)
		{
			return;
		}
		throw exception;
	}

	public BGEntity GetEntity(BGId entityId)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return Store[entityId];
	}

	public BGEntity GetEntity(string entityName)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return Store[entityName];
	}

	public BGEntity GetEntity(int index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return Store[index];
	}

	public void ForEachEntity(Action<BGEntity> action, Predicate<BGEntity> filter = null, Comparison<BGEntity> sort = null)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (sort == null)
		{
			int count = Store.Count;
			if (filter == null)
			{
				for (int i = 0; i < count; i++)
				{
					BGEntity obj = Store[i];
					action(obj);
				}
				return;
			}
			for (int j = 0; j < count; j++)
			{
				BGEntity obj2 = Store[j];
				if (filter(obj2))
				{
					action(obj2);
				}
			}
			return;
		}
		BGListPoolDefault<BGEntity> i2 = BGListPoolDefault<BGEntity>.I;
		List<BGEntity> list = i2.Get();
		try
		{
			if (filter == null)
			{
				Store.ToList(list);
			}
			else
			{
				int count2 = Store.Count;
				for (int k = 0; k < count2; k++)
				{
					BGEntity bGEntity = Store[k];
					if (filter(bGEntity))
					{
						list.Add(bGEntity);
					}
				}
			}
			list.Sort(sort);
			int count3 = list.Count;
			for (int l = 0; l < count3; l++)
			{
				action(list[l]);
			}
		}
		finally
		{
			i2.Return(list);
		}
	}

	public BGEntity FindEntity(Predicate<BGEntity> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (filter == null)
		{
			if (CountEntities != 0)
			{
				return GetEntity(0);
			}
			return null;
		}
		int count = Store.Count;
		for (int i = 0; i < count; i++)
		{
			BGEntity bGEntity = Store[i];
			if (filter(bGEntity))
			{
				return bGEntity;
			}
		}
		return null;
	}

	public List<BGEntity> FindEntities(Predicate<BGEntity> filter, List<BGEntity> result = null, Comparison<BGEntity> sort = null)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (result == null)
		{
			result = new List<BGEntity>();
		}
		else
		{
			result.Clear();
		}
		int count = Store.Count;
		if (filter == null)
		{
			Store.ToList(result);
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				BGEntity bGEntity = Store[i];
				if (filter(bGEntity))
				{
					result.Add(bGEntity);
				}
			}
		}
		if (sort != null)
		{
			result.Sort(sort);
		}
		return result;
	}

	public virtual void OnEntityCreate(BGEntity entity)
	{
		ForEachField((BGField field) =>
		{
			field.OnEntityCreate(entity);
		});
		try
		{
			if (base.Controller is BGControllerOnEntityAdd bGControllerOnEntityAdd)
			{
				bGControllerOnEntityAdd.OnEntityAdd(this, entity);
			}
		}
		catch (Exception exception)
		{
			Debug.Log("Controller BGControllerOnEntityAdd error, see details below...");
			Debug.LogException(exception);
		}
	}

	internal void OnEntityNameChange(int entityIndex, string oldName, string newName)
	{
		Store.OnEntityNameChange(entityIndex, oldName, newName);
	}

	internal void InvalidateNameCache()
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		Store.InvalidateNameCache();
	}

	public bool HasEntity(BGId entityId)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return Store.ContainsKey(entityId);
	}

	public List<BGEntity> EntitiesToList(List<BGEntity> result = null)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return Store.ToList(result);
	}

	public int FindEntityIndex(Predicate<BGEntity> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		for (int i = 0; i < Store.Count; i++)
		{
			BGEntity obj = Store[i];
			if (filter(obj))
			{
				return i;
			}
		}
		return -1;
	}

	public int FindEntityIndex(BGId id)
	{
		return GetEntity(id)?.Index ?? (-1);
	}

	public BGId FindEntityId(int index)
	{
		return GetEntity(index)?.Id ?? BGId.Empty;
	}

	public void ClearEntities()
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		int count = Store.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			Store[num].Unload();
		}
		ForEachField((BGField field) =>
		{
			field.ClearValues();
		});
		Store.Clear();
	}

	internal void Register(BGEntity entity)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		Store.Add(entity);
		for (int i = 0; i < fields.Count; i++)
		{
			fields[i].OnEntityAdd(entity);
		}
	}

	internal void Unregister(BGEntity entity, bool clearRelations = true)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		Store.Remove(entity);
		Exception exception = null;
		for (int i = 0; i < fields.Count; i++)
		{
			BGField field = fields[i];
			BGUtil.Catch(ref exception, () =>
			{
				field.OnEntityDelete(entity);
			});
		}
		if (clearRelations)
		{
			List<BGAbstractRelationI> relationsInbound = RelationsInbound;
			if (relationsInbound.Count > 0)
			{
				foreach (BGAbstractRelationI item in relationsInbound)
				{
					BGAbstractRelationI relation = item;
					BGUtil.Catch(ref exception, () =>
					{
						relation.ClearToValue(entity.Id);
					});
				}
			}
		}
		if (exception != null)
		{
			throw exception;
		}
	}

	public void SwapEntities(int entityIndex1, int entityIndex2)
	{
		if (entityIndex1 != entityIndex2)
		{
			int countEntities = CountEntities;
			if (entityIndex1 < 0 || entityIndex2 < 0 || entityIndex1 >= countEntities || entityIndex2 >= countEntities)
			{
				throw new BGException("Invalid entity indexes for swap: $ and $ ", entityIndex1, entityIndex2);
			}
			Store[entityIndex1].Index = entityIndex2;
			Store[entityIndex2].Index = entityIndex1;
			Store.Swap(entityIndex1, entityIndex2);
			for (int i = 0; i < fields.Count; i++)
			{
				fields[i].Swap(entityIndex1, entityIndex2);
			}
			FireEntitiesOrderChanged();
		}
	}

	public void MoveEntities(int fromIndex, int toIndex, int numberOfEntities)
	{
		if (fromIndex != toIndex)
		{
			int countEntities = CountEntities;
			if (numberOfEntities <= 0)
			{
				throw new BGException("Invalid numberOfEntities: $. It should be more than 0", numberOfEntities);
			}
			if (fromIndex < 0)
			{
				throw new BGException("Invalid fromIndex: $. It should be equal or more than 0", fromIndex);
			}
			if (fromIndex >= countEntities)
			{
				throw new BGException("Invalid fromIndex: $. It should be less than number of entities $", fromIndex, countEntities);
			}
			if (fromIndex + numberOfEntities > countEntities)
			{
				throw new BGException("Invalid fromIndex: $. fromIndex + numberOfEntities($) should not exceed the number of entities $", fromIndex, numberOfEntities, countEntities);
			}
			if (toIndex < 0)
			{
				throw new BGException("Invalid toIndex: $. It should be equal or more than 0", toIndex);
			}
			if (toIndex >= countEntities)
			{
				throw new BGException("Invalid toIndex: $. It should be less than number of entities $", toIndex, countEntities);
			}
			if (toIndex + numberOfEntities > countEntities)
			{
				throw new BGException("Invalid toIndex: $. toIndex + numberOfEntities($) should not exceed the number of entities $", toIndex, numberOfEntities, countEntities);
			}
			Store.MoveValues(fromIndex, toIndex, numberOfEntities);
			for (int i = Math.Min(fromIndex, toIndex); i < countEntities; i++)
			{
				Store[i].Index = i;
			}
			int count = fields.Count;
			for (int j = 0; j < count; j++)
			{
				fields[j].MoveEntitiesValues(fromIndex, toIndex, numberOfEntities);
			}
			Repo.Events.FireAnyChange();
		}
	}

	public BGEntity NewEntity()
	{
		FireEntityBeforeAdded();
		if (!factoryRetrieved)
		{
			InitFactory();
		}
		BGEntity bGEntity = ((factory == null) ? new BGEntity(this) : factory.NewEntity(this));
		FireEntityAdded(bGEntity);
		return bGEntity;
	}

	public BGEntity NewEntity(BGId entityId)
	{
		FireEntityBeforeAdded();
		if (!factoryRetrieved)
		{
			InitFactory();
		}
		BGEntity bGEntity = ((factory == null) ? new BGEntity(this, entityId) : factory.NewEntity(this, entityId));
		FireEntityAdded(bGEntity);
		return bGEntity;
	}

	public BGEntity NewEntity(NewEntityContext context)
	{
		FireEntityBeforeAdded();
		if (context == null)
		{
			return NewEntity();
		}
		if (!factoryRetrieved)
		{
			InitFactory();
		}
		BGEntity bGEntity = ((!context.EntityId.HasValue) ? ((factory == null) ? new BGEntity(this) : factory.NewEntity(this)) : ((factory == null) ? new BGEntity(this, context.EntityId.Value) : factory.NewEntity(this, context.EntityId.Value)));
		if (context.Callback != null)
		{
			bool flag = events.On;
			events.On = false;
			try
			{
				context.Callback(bGEntity);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				events.On = flag;
			}
		}
		FireEntityAdded(bGEntity);
		return bGEntity;
	}

	private void InitFactory()
	{
		factoryRetrieved = true;
		BGAddonCodeGen bGAddonCodeGen = Repo.Addons.Get<BGAddonCodeGen>();
		if (bGAddonCodeGen != null)
		{
			string entityFactoryTypeWithPackage = bGAddonCodeGen.GetEntityFactoryTypeWithPackage(Name);
			Type type = BGUtil.GetType(entityFactoryTypeWithPackage, publicOnly: true);
			if (!(type == null))
			{
				factory = Activator.CreateInstance(type) as BGEntity.EntityFactory;
			}
		}
	}

	public IEnumerator<BGEntity> GetEnumerator()
	{
		return new EntityEnumerator(Store);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	protected bool FieldChanged<T>(ref T oldValue, T newValue)
	{
		bool flag = oldValue == null;
		bool flag2 = newValue == null;
		if (flag & flag2)
		{
			return false;
		}
		if (flag == flag2 && oldValue.Equals(newValue))
		{
			return false;
		}
		oldValue = newValue;
		Repo.Events.MetaWasChanged(this);
		return true;
	}

	public void AddEntityUpdatedListener(BGId entityId, EventHandler<BGEventArgsEntityUpdated> handler)
	{
		entityId2UpdateEntityListener = entityId2UpdateEntityListener ?? new BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>>();
		if (!entityId2UpdateEntityListener.TryGetValue(entityId, out var value))
		{
			value = new BGEventsDelegatesHolder<BGEventArgsEntityUpdated>();
			entityId2UpdateEntityListener.Add(entityId, value);
		}
		BGEventsDelegatesHolder<BGEventArgsEntityUpdated> bGEventsDelegatesHolder = value;
		bGEventsDelegatesHolder.Handler = (EventHandler<BGEventArgsEntityUpdated>)Delegate.Combine(bGEventsDelegatesHolder.Handler, handler);
	}

	public void AddEntityDeletedListener(BGId entityId, EventHandler<BGEventArgsEntity> handler)
	{
		entityId2DeleteEntityListener = entityId2DeleteEntityListener ?? new BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>>();
		if (!entityId2DeleteEntityListener.TryGetValue(entityId, out var value))
		{
			value = new BGEventsDelegatesHolder<BGEventArgsEntity>();
			entityId2DeleteEntityListener.Add(entityId, value);
		}
		BGEventsDelegatesHolder<BGEventArgsEntity> bGEventsDelegatesHolder = value;
		bGEventsDelegatesHolder.Handler = (EventHandler<BGEventArgsEntity>)Delegate.Combine(bGEventsDelegatesHolder.Handler, handler);
	}

	public void RemoveEntityUpdatedListener(BGId entityId, EventHandler<BGEventArgsEntityUpdated> handler)
	{
		if (entityId2UpdateEntityListener != null && entityId2UpdateEntityListener.TryGetValue(entityId, out var value))
		{
			BGEventsDelegatesHolder<BGEventArgsEntityUpdated> bGEventsDelegatesHolder = value;
			bGEventsDelegatesHolder.Handler = (EventHandler<BGEventArgsEntityUpdated>)Delegate.Remove(bGEventsDelegatesHolder.Handler, handler);
		}
	}

	public void RemoveEntityDeletedListener(BGId entityId, EventHandler<BGEventArgsEntity> handler)
	{
		if (entityId2DeleteEntityListener != null && entityId2DeleteEntityListener.TryGetValue(entityId, out var value))
		{
			BGEventsDelegatesHolder<BGEventArgsEntity> bGEventsDelegatesHolder = value;
			bGEventsDelegatesHolder.Handler = (EventHandler<BGEventArgsEntity>)Delegate.Remove(bGEventsDelegatesHolder.Handler, handler);
		}
	}

	internal void FireValueChanged(BGField field, BGEntity entity, bool nested = false)
	{
		if (!nested && events.ConsumeOnChange(base.Id))
		{
			return;
		}
		if (entityId2UpdateEntityListener != null && entityId2UpdateEntityListener.TryGetValue(entity.Id, out var value) && value.Handler != null)
		{
			using BGEventArgsEntityUpdated e = BGEventArgsEntityUpdated.GetInstance(entity, field.Id);
			value.Handler(this, e);
		}
		if (AnyEntityUpdated != null)
		{
			using BGEventArgsAnyEntityUpdated e2 = BGEventArgsAnyEntityUpdated.GetInstance(entity, field.Id);
			AnyEntityUpdated(this, e2);
		}
		if (!nested)
		{
			events.FireAnyChange();
		}
	}

	internal void FireValueChanged<T>(BGField<T> field, BGEntity entity, T oldValue, T newValue)
	{
		if (events.ConsumeOnChange(base.Id))
		{
			return;
		}
		if (entityId2UpdateEntityListener != null && entityId2UpdateEntityListener.TryGetValue(entity.Id, out var value) && value.Handler != null)
		{
			using BGEventArgsEntityUpdatedWithValue<T> e = BGEventArgsEntityUpdatedWithValue<T>.GetInstance(entity, field, oldValue, newValue);
			value.Handler(this, e);
		}
		if (AnyEntityUpdated == null)
		{
			return;
		}
		using BGEventArgsAnyEntityUpdatedWithValue<T> e2 = BGEventArgsAnyEntityUpdatedWithValue<T>.GetInstance(entity, field, oldValue, newValue);
		AnyEntityUpdated(this, e2);
	}

	internal void FireBeforeValueChanged<T>(BGField<T> field, BGEntity entity, T oldValue, T newValue)
	{
		if (events.ConsumeOnChange(base.Id) || AnyEntityBeforeUpdated == null)
		{
			return;
		}
		using BGEventArgsAnyEntityUpdatedWithValue<T> e = BGEventArgsAnyEntityUpdatedWithValue<T>.GetInstance(entity, field, oldValue, newValue);
		AnyEntityBeforeUpdated(this, e);
	}

	internal void FireStoredValueChanged<T, TStoreType>(BGFieldCachedA<T, TStoreType> field, BGEntity entity, TStoreType oldValue, TStoreType newValue, bool nested = false)
	{
		if (!nested && events.ConsumeOnChange(base.Id))
		{
			return;
		}
		if (entityId2UpdateEntityListener != null && entityId2UpdateEntityListener.TryGetValue(entity.Id, out var value) && value.Handler != null)
		{
			using BGEventArgsEntityUpdatedWithValue<T, TStoreType> e = BGEventArgsEntityUpdatedWithValue<T, TStoreType>.GetInstance(entity, field, oldValue, newValue);
			value.Handler(this, e);
		}
		if (AnyEntityUpdated != null)
		{
			using BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType> e2 = BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType>.GetInstance(entity, field, oldValue, newValue);
			AnyEntityUpdated(this, e2);
		}
		if (!nested)
		{
			events.FireAnyChange();
		}
	}

	internal void FireEntityBeforeDelete(BGEntity entity)
	{
		if (events.ConsumeOnEntityDelete(base.Id) || AnyEntityBeforeDeleted == null)
		{
			return;
		}
		using BGEventArgsAnyEntity e = BGEventArgsAnyEntity.GetInstance(entity);
		AnyEntityBeforeDeleted(this, e);
	}

	internal void FireEntityDeleted(BGEntity entity)
	{
		if (events.ConsumeOnEntityDelete(base.Id))
		{
			return;
		}
		if (entityId2DeleteEntityListener != null && entityId2DeleteEntityListener.TryGetValue(entity.Id, out var value) && value.Handler != null)
		{
			using BGEventArgsEntity e = BGEventArgsEntity.GetInstance(entity);
			value.Handler(this, e);
		}
		if (AnyEntityDeleted != null)
		{
			using BGEventArgsAnyEntity e2 = BGEventArgsAnyEntity.GetInstance(entity);
			AnyEntityDeleted(this, e2);
		}
		events.FireAnyChange();
	}

	internal void FireEntityAdded(BGEntity entity)
	{
		if (events.ConsumeOnEntityAdded(base.Id))
		{
			return;
		}
		if (AnyEntityAdded != null)
		{
			using BGEventArgsAnyEntity e = BGEventArgsAnyEntity.GetInstance(entity);
			AnyEntityAdded(this, e);
		}
		events.FireAnyChange();
	}

	internal void FireEntityBeforeAdded()
	{
		if (events.ConsumeOnEntityAdded(base.Id) || AnyEntityBeforeAdded == null)
		{
			return;
		}
		using BGEventArgsAnyEntityBeforeAdded e = BGEventArgsAnyEntityBeforeAdded.GetInstance(this);
		AnyEntityBeforeAdded(this, e);
	}

	internal void FireEntitiesOrderChanged()
	{
		if (events.ConsumeOnEntitiesOrderChanged(base.Id))
		{
			return;
		}
		if (EntitiesOrderChanged != null)
		{
			using BGEventArgsEntitiesOrder e = BGEventArgsEntitiesOrder.GetInstance(this);
			EntitiesOrderChanged(this, e);
		}
		events.FireAnyChange();
	}

	internal void TransferEventsTo(BGEventsHolder eventsHolder)
	{
		if (AnyEntityAdded != null)
		{
			eventsHolder.AddOnAnyEntityAddedListeners(base.Id, AnyEntityAdded.GetInvocationList());
			AnyEntityAdded = null;
		}
		if (AnyEntityBeforeAdded != null)
		{
			eventsHolder.AddOnAnyEntityBeforeAddedListeners(base.Id, AnyEntityBeforeAdded.GetInvocationList());
			AnyEntityBeforeAdded = null;
		}
		if (AnyEntityUpdated != null)
		{
			eventsHolder.AddOnAnyEntityUpdatedListeners(base.Id, AnyEntityUpdated.GetInvocationList());
			AnyEntityUpdated = null;
		}
		if (AnyEntityBeforeUpdated != null)
		{
			eventsHolder.AddOnAnyEntityBeforeUpdatedListeners(base.Id, AnyEntityBeforeUpdated.GetInvocationList());
			AnyEntityBeforeUpdated = null;
		}
		if (AnyEntityDeleted != null)
		{
			eventsHolder.AddOnAnyEntityDeletedListeners(base.Id, AnyEntityDeleted.GetInvocationList());
			AnyEntityDeleted = null;
		}
		if (EntitiesOrderChanged != null)
		{
			eventsHolder.AddOnEntitiesOrderChangedListeners(base.Id, EntitiesOrderChanged.GetInvocationList());
			EntitiesOrderChanged = null;
		}
		BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>> bGIdDictionary = entityId2UpdateEntityListener;
		if (bGIdDictionary != null && bGIdDictionary.Count > 0)
		{
			eventsHolder.AddOnEntityUpdatedListeners(base.Id, entityId2UpdateEntityListener);
		}
		BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>> bGIdDictionary2 = entityId2DeleteEntityListener;
		if (bGIdDictionary2 != null && bGIdDictionary2.Count > 0)
		{
			eventsHolder.AddOnEntityDeletedListeners(base.Id, entityId2DeleteEntityListener);
		}
	}

	internal void TransferEventsFrom(BGEventsHolder eventsHolder)
	{
		Delegate[] onAnyEntityAddedListeners = eventsHolder.GetOnAnyEntityAddedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array = onAnyEntityAddedListeners;
			foreach (Delegate obj in array)
			{
				AnyEntityAdded += (EventHandler<BGEventArgsAnyEntity>)obj;
			}
		}
		onAnyEntityAddedListeners = eventsHolder.GetOnAnyEntityBeforeAddedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array2 = onAnyEntityAddedListeners;
			foreach (Delegate obj2 in array2)
			{
				AnyEntityBeforeAdded += (EventHandler<BGEventArgsAnyEntityBeforeAdded>)obj2;
			}
		}
		onAnyEntityAddedListeners = eventsHolder.GetOnAnyEntityUpdatedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array3 = onAnyEntityAddedListeners;
			foreach (Delegate obj3 in array3)
			{
				AnyEntityUpdated += (EventHandler<BGEventArgsAnyEntityUpdated>)obj3;
			}
		}
		onAnyEntityAddedListeners = eventsHolder.GetOnAnyEntityBeforeUpdatedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array4 = onAnyEntityAddedListeners;
			foreach (Delegate obj4 in array4)
			{
				AnyEntityBeforeUpdated += (EventHandler<BGEventArgsAnyEntityUpdated>)obj4;
			}
		}
		onAnyEntityAddedListeners = eventsHolder.GetOnAnyEntityDeletedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array5 = onAnyEntityAddedListeners;
			foreach (Delegate obj5 in array5)
			{
				AnyEntityDeleted += (EventHandler<BGEventArgsAnyEntity>)obj5;
			}
		}
		onAnyEntityAddedListeners = eventsHolder.GetOnEntitiesOrderChangedListeners(base.Id);
		if (onAnyEntityAddedListeners != null && onAnyEntityAddedListeners.Length != 0)
		{
			Delegate[] array6 = onAnyEntityAddedListeners;
			foreach (Delegate obj6 in array6)
			{
				EntitiesOrderChanged += (EventHandler<BGEventArgsEntitiesOrder>)obj6;
			}
		}
		entityId2UpdateEntityListener = eventsHolder.GetOnEntityUpdatedListeners(base.Id);
		entityId2DeleteEntityListener = eventsHolder.GetOnEntityDeletedListeners(base.Id);
	}

	private void RebuildIndexes()
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		id2Field.Clear();
		name2Field.Clear();
		foreach (BGField field in fields)
		{
			id2Field[field.Id] = field;
			name2Field[field.Name] = field;
		}
	}

	public void ForEachField(Action<BGField> action)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		int count = fields.Count;
		for (int i = 0; i < count; i++)
		{
			action(fields[i]);
		}
	}

	public void ForEachField(Action<BGField> action, Predicate<BGField> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (filter == null)
		{
			ForEachField(action);
			return;
		}
		int count = fields.Count;
		for (int i = 0; i < count; i++)
		{
			BGField obj = fields[i];
			if (filter(obj))
			{
				action(obj);
			}
		}
	}

	public BGField FindField(Predicate<BGField> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		int count = fields.Count;
		for (int i = 0; i < count; i++)
		{
			BGField bGField = fields[i];
			if (filter(bGField))
			{
				return bGField;
			}
		}
		return null;
	}

	public List<BGField> FindFields(List<BGField> result = null, Predicate<BGField> filter = null)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (result == null)
		{
			result = new List<BGField>();
		}
		else
		{
			result.Clear();
		}
		if (CountFields == 0)
		{
			return result;
		}
		if (filter == null)
		{
			result.AddRange(fields);
		}
		else
		{
			ForEachField((BGField field) =>
			{
				result.Add(field);
			}, filter);
		}
		return result;
	}

	[Obsolete("FieldsToList is deprecated, use FindFields instead.")]
	public List<BGField> FieldsToList(List<BGField> result = null, Predicate<BGField> filter = null)
	{
		return FindFields(result, filter);
	}

	public BGField GetField(BGId fieldId, bool errorIfNotFound = true)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (id2Field.TryGetValue(fieldId, out var value))
		{
			return value;
		}
		if (errorIfNotFound)
		{
			throw new BGException("No field with id ($) at meta ($)", fieldId, Name);
		}
		return null;
	}

	public BGField<T> GetField<T>(BGId fieldId, bool errorIfNotFound = true)
	{
		BGField<T> bGField = (BGField<T>)GetField(fieldId, errorIfNotFound);
		if ((bGField == null) & errorIfNotFound)
		{
			throw new BGException("There is no field with id ($) and value type ($)", fieldId, typeof(T));
		}
		return bGField;
	}

	public BGField GetField(string name, bool errorIfNotFound = true)
	{
		if (name == null)
		{
			if (errorIfNotFound)
			{
				throw new BGException("Field name can not be null");
			}
			return null;
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (name2Field.TryGetValue(name, out var value))
		{
			return value;
		}
		if (name.Length == 0)
		{
			throw new BGException("Field name can not be empty");
		}
		if (errorIfNotFound)
		{
			throw new BGException("No field with name ($) at meta ($)", name, Name);
		}
		return null;
	}

	public BGField<T> GetField<T>(string name, bool errorIfNotFound = true)
	{
		BGField<T> bGField = (BGField<T>)GetField(name, errorIfNotFound);
		if ((bGField == null) & errorIfNotFound)
		{
			throw new BGException("There is no field with name ($) and value type ($)", name, typeof(T));
		}
		return bGField;
	}

	public BGField GetField(int index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return fields[index];
	}

	public T GetFieldAs<T>(string name, bool errorIfNotFound = true) where T : BGField
	{
		T val = (T)GetField(name, errorIfNotFound);
		if ((val == null) & errorIfNotFound)
		{
			throw new BGException("There is no field with name ($) and type ($)", name, typeof(T));
		}
		return val;
	}

	public T GetFieldAs<T>(BGId id, bool errorIfNotFound = true) where T : BGField
	{
		T val = (T)GetField(id);
		if ((val == null) & errorIfNotFound)
		{
			throw new BGException("There is no field with id ($) and type ($)", id, typeof(T));
		}
		return val;
	}

	public BGId GetFieldId(string name)
	{
		return GetField(name).Id;
	}

	public int GetFieldIndex(BGId id)
	{
		int countFields = CountFields;
		for (int i = 0; i < countFields; i++)
		{
			BGField bGField = fields[i];
			if (!(bGField.Id != id))
			{
				return i;
			}
		}
		return -1;
	}

	public void SwapFields(int fieldIndex1, int fieldIndex2)
	{
		int countFields = CountFields;
		if (fieldIndex1 < 0 || fieldIndex2 < 0 || fieldIndex1 >= countFields || fieldIndex2 >= countFields)
		{
			throw new BGException("Invalid fields indexes for swap: $ and $ ", fieldIndex1, fieldIndex2);
		}
		List<BGField> list = fields;
		List<BGField> list2 = fields;
		BGField value = fields[fieldIndex2];
		BGField value2 = fields[fieldIndex1];
		list[fieldIndex1] = value;
		list2[fieldIndex2] = value2;
		if (events.On)
		{
			events.FireAnyChange();
		}
	}

	public bool HasField(string name)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return name2Field.ContainsKey(name);
	}

	public bool HasField(string name, Type fieldTypeType)
	{
		if (!HasField(name))
		{
			return false;
		}
		return GetField(name).GetType() == fieldTypeType;
	}

	public bool HasField(BGId id)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return id2Field.ContainsKey(id);
	}

	internal void FieldNameWasChanged(BGField field, string oldName)
	{
		name2Field.Remove(oldName);
		name2Field.Add(field.Name, field);
		Repo.Events.MetaWasChanged(this);
	}

	internal void Register(BGField field)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		CheckFieldName(field.Name);
		id2Field.Add(field.Id, field);
		name2Field.Add(field.Name, field);
		fields.Add(field);
		field.OnCreate();
		Repo.Events.MetaWasChanged(this);
	}

	internal void Unregister(BGField field)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		field.OnDelete();
		id2Field.Remove(field.Id);
		name2Field.Remove(field.Name);
		fields.Remove(field);
		if (CountKeys > 0)
		{
			List<BGKey> keysToRemove = new List<BGKey>();
			ForEachKey((BGKey key) =>
			{
				keysToRemove.Add(key);
			}, (BGKey key) => key.HasField(field));
			foreach (BGKey item in keysToRemove)
			{
				item.Delete();
			}
		}
		if (CountIndexes > 0)
		{
			List<BGIndex> indexesToRemove = new List<BGIndex>();
			ForEachIndex((BGIndex index) =>
			{
				indexesToRemove.Add(index);
			}, (BGIndex index) => object.Equals(index.Field, field));
			foreach (BGIndex item2 in indexesToRemove)
			{
				item2.Delete();
			}
		}
		Repo.Events.MetaWasChanged(this);
	}

	public void CheckFieldName(string name)
	{
		if (HasField(name))
		{
			throw new BGException("Name is not unique: field with name ($) already exists!", name);
		}
		if (HasKey(name))
		{
			throw new BGException("Name is not unique: key with name ($) already exists!", name);
		}
		if (HasIndex(name))
		{
			throw new BGException("Name is not unique: index with name ($) already exists!", name);
		}
	}

	public void ForEachIndex(Action<BGIndex> action)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (indexes != null)
		{
			int count = indexes.Count;
			for (int i = 0; i < count; i++)
			{
				BGIndex obj = indexes[i];
				action(obj);
			}
		}
	}

	public void ForEachIndex(Action<BGIndex> action, Predicate<BGIndex> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (filter == null)
		{
			ForEachIndex(action);
		}
		else
		{
			if (indexes == null)
			{
				return;
			}
			int count = indexes.Count;
			for (int i = 0; i < count; i++)
			{
				BGIndex obj = indexes[i];
				if (filter(obj))
				{
					action(obj);
				}
			}
		}
	}

	public BGIndex FindIndex(Predicate<BGIndex> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (indexes == null)
		{
			return null;
		}
		int count = indexes.Count;
		for (int i = 0; i < count; i++)
		{
			BGIndex bGIndex = indexes[i];
			if (filter(bGIndex))
			{
				return bGIndex;
			}
		}
		return null;
	}

	public List<BGIndex> FindIndexes(List<BGIndex> result = null, Predicate<BGIndex> filter = null)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (result == null)
		{
			result = new List<BGIndex>();
		}
		else
		{
			result.Clear();
		}
		if (indexes == null)
		{
			return result;
		}
		if (CountIndexes == 0)
		{
			return result;
		}
		if (filter == null)
		{
			result.AddRange(indexes);
		}
		else
		{
			ForEachIndex((BGIndex index) =>
			{
				result.Add(index);
			}, filter);
		}
		return result;
	}

	public BGIndex GetIndex(BGId indexId, bool errorIfNotFound = true)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (id2Index == null)
		{
			return null;
		}
		if (id2Index.TryGetValue(indexId, out var value))
		{
			return value;
		}
		if (errorIfNotFound)
		{
			throw new BGException("No index with id ($) at meta ($)", indexId, Name);
		}
		return null;
	}

	public BGIndex GetIndex(string name, bool errorIfNotFound = true)
	{
		if (name == null)
		{
			if (errorIfNotFound)
			{
				throw new BGException("Index name can not be null");
			}
			return null;
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (name2Index == null)
		{
			return null;
		}
		if (name2Index.TryGetValue(name, out var value))
		{
			return value;
		}
		if (name.Length == 0)
		{
			throw new BGException("Index name can not be empty");
		}
		if (errorIfNotFound)
		{
			throw new BGException("No index with name ($) at meta ($)", name, Name);
		}
		return null;
	}

	public BGIndex GetIndex(int index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (indexes == null)
		{
			return null;
		}
		return indexes[index];
	}

	public BGId GetIndexId(string name)
	{
		return GetIndex(name)?.Id ?? BGId.Empty;
	}

	public int GetIndexIndex(BGId id)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (indexes == null)
		{
			return -1;
		}
		int count = indexes.Count;
		for (int i = 0; i < count; i++)
		{
			BGIndex bGIndex = indexes[i];
			if (!(bGIndex.Id != id))
			{
				return i;
			}
		}
		return -1;
	}

	public void SwapIndexes(int indexIndex1, int indexIndex2)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (indexes != null)
		{
			int countIndexes = CountIndexes;
			if (indexIndex1 < 0 || indexIndex2 < 0 || indexIndex1 >= countIndexes || indexIndex2 >= countIndexes)
			{
				throw new BGException("Invalid indexes for swap: $ and $ ", indexIndex1, indexIndex2);
			}
			List<BGIndex> list = indexes;
			List<BGIndex> list2 = indexes;
			BGIndex value = indexes[indexIndex2];
			BGIndex value2 = indexes[indexIndex1];
			list[indexIndex1] = value;
			list2[indexIndex2] = value2;
			if (events.On)
			{
				events.FireAnyChange();
			}
		}
	}

	public bool HasIndex(string name)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return name2Index?.ContainsKey(name) ?? false;
	}

	public bool HasIndex(BGId id)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return id2Index?.ContainsKey(id) ?? false;
	}

	internal void IndexNameWasChanged(BGIndex index, string oldName)
	{
		if (name2Index != null)
		{
			name2Index.Remove(oldName);
			name2Index.Add(index.Name, index);
			Repo.Events.MetaWasChanged(this);
		}
	}

	internal void Register(BGIndex index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		CheckFieldName(index.Name);
		if (indexes == null)
		{
			indexes = new List<BGIndex>();
			id2Index = new Dictionary<BGId, BGIndex>();
			name2Index = new Dictionary<string, BGIndex>();
		}
		id2Index.Add(index.Id, index);
		name2Index.Add(index.Name, index);
		indexes.Add(index);
		index.OnCreate();
		Repo.Events.MetaWasChanged(this);
	}

	internal void Unregister(BGIndex index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (index == null)
		{
			throw new Exception("Can not unregister the index cause the index is null!");
		}
		if (index.Meta.Id != base.Id)
		{
			throw new Exception("Can not unregister the index cause the index metaId is not matching metaId!");
		}
		index.OnDelete();
		if (id2Index != null)
		{
			id2Index.Remove(index.Id);
			name2Index.Remove(index.Name);
			indexes.Remove(index);
		}
		Repo.Events.MetaWasChanged(this);
	}

	public void ForEachKey(Action<BGKey> action)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys != null)
		{
			int count = keys.Count;
			for (int i = 0; i < count; i++)
			{
				BGKey obj = keys[i];
				action(obj);
			}
		}
	}

	public void ForEachKey(Action<BGKey> action, Predicate<BGKey> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (filter == null)
		{
			ForEachKey(action);
		}
		else
		{
			if (keys == null)
			{
				return;
			}
			int count = keys.Count;
			for (int i = 0; i < count; i++)
			{
				BGKey obj = keys[i];
				if (filter(obj))
				{
					action(obj);
				}
			}
		}
	}

	public BGKey FindKey(Predicate<BGKey> filter)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys == null)
		{
			return null;
		}
		int count = keys.Count;
		for (int i = 0; i < count; i++)
		{
			BGKey bGKey = keys[i];
			if (filter(bGKey))
			{
				return bGKey;
			}
		}
		return null;
	}

	public List<BGKey> FindKeys(List<BGKey> result = null, Predicate<BGKey> filter = null)
	{
		if (result == null)
		{
			result = new List<BGKey>();
		}
		else
		{
			result.Clear();
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys == null)
		{
			return result;
		}
		if (CountKeys == 0)
		{
			return result;
		}
		if (filter == null)
		{
			result.AddRange(keys);
		}
		else
		{
			ForEachKey((BGKey key) =>
			{
				result.Add(key);
			}, filter);
		}
		return result;
	}

	public BGKey GetKey(BGId keyID, bool errorIfNotFound = true)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (id2Key == null)
		{
			return null;
		}
		if (id2Key.TryGetValue(keyID, out var value))
		{
			return value;
		}
		if (errorIfNotFound)
		{
			throw new BGException("No key with id ($) at meta ($)", keyID, Name);
		}
		return null;
	}

	public BGKey GetKey(string name, bool errorIfNotFound = true)
	{
		if (name == null)
		{
			if (errorIfNotFound)
			{
				throw new BGException("Key name can not be null");
			}
			return null;
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (name2Key == null)
		{
			return null;
		}
		if (name2Key.TryGetValue(name, out var value))
		{
			return value;
		}
		if (name.Length == 0)
		{
			throw new BGException("Key name can not be empty");
		}
		if (errorIfNotFound)
		{
			throw new BGException("No key with name ($) at meta ($)", name, Name);
		}
		return null;
	}

	public BGKey GetKey(int index)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys == null)
		{
			return null;
		}
		return keys[index];
	}

	public BGId GetKeyId(string name)
	{
		return GetKey(name)?.Id ?? BGId.Empty;
	}

	public int GetKeyIndex(BGId id)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys == null)
		{
			return -1;
		}
		int count = keys.Count;
		for (int i = 0; i < count; i++)
		{
			BGKey bGKey = keys[i];
			if (!(bGKey.Id != id))
			{
				return i;
			}
		}
		return -1;
	}

	public void SwapKeys(int keyIndex1, int keyIndex2)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (keys != null)
		{
			int countKeys = CountKeys;
			if (keyIndex1 < 0 || keyIndex2 < 0 || keyIndex1 >= countKeys || keyIndex2 >= countKeys)
			{
				throw new BGException("Invalid keys indexes for swap: $ and $ ", keyIndex1, keyIndex2);
			}
			List<BGKey> list = keys;
			List<BGKey> list2 = keys;
			BGKey value = keys[keyIndex2];
			BGKey value2 = keys[keyIndex1];
			list[keyIndex1] = value;
			list2[keyIndex2] = value2;
			if (events.On)
			{
				events.FireAnyChange();
			}
		}
	}

	public bool HasKey(string name)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return name2Key?.ContainsKey(name) ?? false;
	}

	public bool HasKey(BGId id)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		return id2Key?.ContainsKey(id) ?? false;
	}

	internal void KeyNameWasChanged(BGKey key, string oldName)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		if (name2Key != null)
		{
			name2Key.Remove(oldName);
			name2Key.Add(key.Name, key);
			Repo.Events.MetaWasChanged(this);
		}
	}

	internal void Register(BGKey key)
	{
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		CheckFieldName(key.Name);
		if (keys == null)
		{
			keys = new List<BGKey>();
			id2Key = new Dictionary<BGId, BGKey>();
			name2Key = new Dictionary<string, BGKey>();
		}
		id2Key.Add(key.Id, key);
		name2Key.Add(key.Name, key);
		keys.Add(key);
		key.OnCreate();
		Repo.Events.MetaWasChanged(this);
	}

	internal void Unregister(BGKey key)
	{
		if (key == null)
		{
			throw new Exception("Can not unregister the key cause the key is null!");
		}
		if (key.Meta.Id != base.Id)
		{
			throw new Exception("Can not unregister the key cause the key metaId is not matching metaId!");
		}
		if (LazyLoader != null)
		{
			LazyLoad();
		}
		key.OnDelete();
		if (id2Key != null)
		{
			id2Key.Remove(key.Id);
			name2Key.Remove(key.Name);
			keys.Remove(key);
		}
		Repo.Events.MetaWasChanged(this);
	}
}
