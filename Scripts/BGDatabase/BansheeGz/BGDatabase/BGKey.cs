using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGKey : BGMetaObject
{
	[Serializable]
	private class JsonConfig
	{
		public bool IsUnique;
	}

	public const int MaxFieldsCount = 10;

	private readonly List<BGField> fields = new List<BGField>();

	private bool isUnique;

	private BGKeyStorage store;

	private BGKeyStorage[] stores;

	public BGMetaEntity Meta => fields[0].Meta;

	public int CountFields => fields.Count;

	public override int Index => Meta.GetKeyIndex(base.Id);

	public bool IsUnique
	{
		get
		{
			return isUnique;
		}
		set
		{
			isUnique = value;
		}
	}

	public override string Name
	{
		set
		{
			if (!string.Equals(Name, value))
			{
				Meta.CheckFieldName(value);
				string oldName = Name;
				base.Name = value;
				Meta.KeyNameWasChanged(this, oldName);
			}
		}
	}

	public string FullName => Meta.Name + "." + Name;

	public BGId MetaId => Meta.Id;

	public BGRepo Repo => Meta.Repo;

	public BGKey(string name, BGField[] fields)
		: this(BGId.NewId, name, fields)
	{
	}

	private BGKey(BGId id, string name, BGField[] fields)
		: base(id, name)
	{
		if (fields == null || fields.Length == 0)
		{
			throw new BGException("Fields can not be empty");
		}
		if (fields.Length > 10)
		{
			throw new BGException("Fields count for a key can not exceed max=$", 10);
		}
		BGMetaEntity meta = fields[0].Meta;
		HashSet<BGId> hashSet = new HashSet<BGId>();
		for (int i = 0; i < fields.Length; i++)
		{
			BGField bGField = fields[i];
			if (bGField == null)
			{
				throw new BGException("Field can not be null! index=$", i);
			}
			if (!bGField.CanBeUsedAsKey)
			{
				throw new BGException("Field $ can not be used as a key!", bGField.Name);
			}
			if (meta.Id != bGField.MetaId)
			{
				throw new BGException("Fields with different metas was submitted, expected $, found $", meta.Name, bGField.MetaName);
			}
			if (hashSet.Contains(bGField.Id))
			{
				throw new BGException("Duplicate field was submitted: $", bGField.MetaName);
			}
			hashSet.Add(bGField.Id);
		}
		this.fields.AddRange(fields);
		meta.Register(this);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			IsUnique = isUnique
		});
	}

	public override void ConfigFromString(string config)
	{
		isUnique = JsonUtility.FromJson<JsonConfig>(config).IsUnique;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(5);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddBool(isUnique);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			isUnique = bGBinaryReader.ReadBool();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public void OnCreate()
	{
	}

	public void OnDelete()
	{
	}

	public List<BGField> FindFields(List<BGField> result = null, Predicate<BGField> filter = null)
	{
		if (result == null)
		{
			result = new List<BGField>();
		}
		if (filter == null)
		{
			result.AddRange(fields);
		}
		else
		{
			foreach (BGField field in fields)
			{
				if (filter(field))
				{
					result.Add(field);
				}
			}
		}
		return result;
	}

	public void ForEachField(Action<BGField> action, Predicate<BGField> filter = null)
	{
		int count = fields.Count;
		for (int i = 0; i < count; i++)
		{
			BGField obj = fields[i];
			if (filter == null || filter(obj))
			{
				action(obj);
			}
		}
	}

	public BGField GetField(int index)
	{
		return fields[index];
	}

	public int GetFieldIndex(BGField field)
	{
		for (int i = 0; i < fields.Count; i++)
		{
			BGField bGField = fields[i];
			if (bGField.Id == field.Id)
			{
				return i;
			}
		}
		return -1;
	}

	public bool HasField(BGField field)
	{
		return GetFieldIndex(field) != -1;
	}

	public void AddField(int position, BGField field)
	{
		if (Meta.Id != field.MetaId)
		{
			throw new BGException("Can not add field $ to key $: wrong meta, expected: $, found $", field.Name, Name, Meta.Name, field.MetaName);
		}
		if (fields.Count >= 10)
		{
			throw new BGException("Fields count for a key can not exceed max=$", 10);
		}
		fields.Insert(position, field);
		Meta.Repo.Events.MetaWasChanged(Meta);
	}

	public void RemoveField(BGField field)
	{
		int fieldIndex = GetFieldIndex(field);
		if (fieldIndex == -1)
		{
			throw new BGException("Unable to remove a field: field $ is not contained in key $", field.Name, Name);
		}
		if (fields.Count == 1)
		{
			throw new BGException("Unable to remove the last field $ from the key $", field.Name, Name);
		}
		fields.Remove(field);
		Meta.Repo.Events.MetaWasChanged(Meta);
	}

	public void SetFields(List<BGField> fields)
	{
		this.fields.Clear();
		this.fields.AddRange(fields);
	}

	internal static BGKey FromBinary(BGBinaryReader binder, BGMetaEntity meta)
	{
		int num = binder.ReadInt();
		if ((uint)(num - 1) <= 1u)
		{
			BGId bGId = binder.ReadId();
			string text = binder.ReadString();
			bool unique = binder.ReadBool();
			List<BGId> fieldIds = new List<BGId>();
			binder.ReadArray(() =>
			{
				fieldIds.Add(binder.ReadId());
			});
			if (fieldIds.Count == 0)
			{
				return null;
			}
			BGField[] array = new BGField[fieldIds.Count];
			for (int num2 = 0; num2 < fieldIds.Count; num2++)
			{
				BGId fieldId = fieldIds[num2];
				BGField field = meta.GetField(fieldId, errorIfNotFound: false);
				if (field == null)
				{
					return null;
				}
				array[num2] = field;
			}
			BGKey bGKey = Create(bGId, text, unique, array);
			if (num >= 2)
			{
				bGKey.Comment = binder.ReadString();
				bGKey.ControllerType = binder.ReadString();
			}
			return bGKey;
		}
		throw new BGException("Can not read key from binary array: unsupported version $", num);
	}

	internal static void ToBinary(BGBinaryWriter builder, BGKey key)
	{
		builder.AddInt(2);
		builder.AddId(key.Id);
		builder.AddString(key.Name);
		builder.AddBool(key.isUnique);
		builder.AddArray(() =>
		{
			key.ForEachField((BGField field) =>
			{
				builder.AddId(field.Id);
			});
		}, key.CountFields);
		builder.AddString(key.Comment);
		builder.AddString(key.ControllerType);
	}

	public BGEntity GetEntityByKey(params object[] keys)
	{
		CheckKeys(keys);
		if (keys.Length == fields.Count)
		{
			Build();
			return store.GetEntity(keys);
		}
		int num = keys.Length - 1;
		EnsureStore(num);
		return stores[num].GetEntity(keys);
	}

	public BGEntity GetEntityByKey<T0>(T0 t0)
	{
		CheckKeysCount(1);
		if (1 == fields.Count)
		{
			Build();
			return store.GetEntity(t0);
		}
		EnsureStore(0);
		return stores[0].GetEntity(t0);
	}

	public BGEntity GetEntityByKey<T0, T1>(T0 t0, T1 t1)
	{
		CheckKeysCount(2);
		if (2 == fields.Count)
		{
			Build();
			return store.GetEntity(t0, t1);
		}
		EnsureStore(1);
		return stores[1].GetEntity(t0, t1);
	}

	public BGEntity GetEntityByKey<T0, T1, T2>(T0 t0, T1 t1, T2 t2)
	{
		CheckKeysCount(3);
		if (3 == fields.Count)
		{
			Build();
			return store.GetEntity(t0, t1, t2);
		}
		EnsureStore(2);
		return stores[2].GetEntity(t0, t1, t2);
	}

	public BGEntity GetEntityByKey<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3)
	{
		CheckKeysCount(4);
		if (4 == fields.Count)
		{
			Build();
			return store.GetEntity(t0, t1, t2, t3);
		}
		EnsureStore(3);
		return stores[3].GetEntity(t0, t1, t2, t3);
	}

	public List<BGEntity> GetEntitiesByKey(params object[] keys)
	{
		return GetEntitiesByKey<BGEntity>(null, keys);
	}

	public List<T> GetEntitiesByKey<T>(List<T> result, params object[] keys) where T : BGEntity
	{
		CheckKeys(keys);
		if (keys.Length == fields.Count)
		{
			Build();
			return store.GetEntities(result, keys);
		}
		EnsureStore(keys.Length - 1);
		return stores[keys.Length - 1].GetEntities(result, keys);
	}

	public List<T> GetEntitiesByKey<T, T0>(List<T> result, T0 t0) where T : BGEntity
	{
		CheckKeysCount(1);
		if (1 == fields.Count)
		{
			Build();
			return store.GetEntities(result, t0);
		}
		EnsureStore(0);
		return stores[0].GetEntities(result, t0);
	}

	public List<T> GetEntitiesByKey<T, T0, T1>(List<T> result, T0 t0, T1 t1) where T : BGEntity
	{
		CheckKeysCount(2);
		if (2 == fields.Count)
		{
			Build();
			return store.GetEntities(result, t0, t1);
		}
		EnsureStore(1);
		return stores[1].GetEntities(result, t0, t1);
	}

	public List<T> GetEntitiesByKey<T, T0, T1, T2>(List<T> result, T0 t0, T1 t1, T2 t2) where T : BGEntity
	{
		CheckKeysCount(3);
		if (3 == fields.Count)
		{
			Build();
			return store.GetEntities(result, t0, t1, t2);
		}
		EnsureStore(2);
		return stores[2].GetEntities(result, t0, t1, t2);
	}

	public List<T> GetEntitiesByKey<T, T0, T1, T2, T3>(List<T> result, T0 t0, T1 t1, T2 t2, T3 t3) where T : BGEntity
	{
		CheckKeysCount(4);
		if (4 == fields.Count)
		{
			Build();
			return store.GetEntities(result, t0, t1, t2, t3);
		}
		EnsureStore(3);
		return stores[3].GetEntities(result, t0, t1, t2, t3);
	}

	public void MarkDirty()
	{
		store?.MarkDirty();
		if (stores != null)
		{
			BGKeyStorage[] array = stores;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]?.MarkDirty();
			}
		}
	}

	public void Build()
	{
		if (store == null)
		{
			store = new BGKeyStorage(this, fields.ToArray());
		}
	}

	public void BuildAll()
	{
		Build();
		for (int i = 0; i < fields.Count - 1; i++)
		{
			EnsureStore(i);
		}
	}

	private void EnsureStore(int index)
	{
		if (stores == null)
		{
			stores = new BGKeyStorage[index + 1];
		}
		else if (stores.Length <= index)
		{
			BGKeyStorage[] array = stores;
			stores = new BGKeyStorage[index + 1];
			Array.Copy(array, stores, array.Length);
		}
		if (stores[index] == null)
		{
			BGField[] array2 = new BGField[index + 1];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = fields[i];
			}
			stores[index] = new BGKeyStorage(this, array2);
		}
	}

	private void CheckKeys(object[] keys)
	{
		CheckKeysCount((keys != null) ? keys.Length : 0);
		for (int i = 0; i < keys.Length; i++)
		{
			CheckKey(i, keys[i]);
		}
	}

	private void CheckKeysCount(int keysCount)
	{
		if (keysCount == 0)
		{
			throw new BGException("Keys are null or empty!");
		}
		if (keysCount > fields.Count)
		{
			throw new BGException("Keys count more than fields count! $ > $", keysCount, fields.Count);
		}
	}

	private void CheckKey<T>(int index, T key)
	{
		BGField bGField = fields[index];
		if (key == null)
		{
			if (bGField.ConstantSize > 0)
			{
				throw new BGException("Key $ is null, but field $ can not have null values!", index, bGField.Name);
			}
			return;
		}
		BGField bGField2 = bGField;
		if (!(bGField2 is BGFieldEnumI { EnumType: var enumType }))
		{
			if (bGField2 is BGFieldRelationSingle)
			{
				if (!(key is BGEntity))
				{
					throw new BGException("Key $ has incompatible type: required [$], actual [$] !", index, typeof(BGEntity).FullName, GetValueType(key.GetType()));
				}
				BGFieldRelationSingle bGFieldRelationSingle = (BGFieldRelationSingle)bGField2;
				BGEntity bGEntity = key as BGEntity;
				if (bGEntity.MetaId != bGFieldRelationSingle.RelatedMeta.Id)
				{
					throw new BGException("Key $ has incompatible type: required entity of [$] meta, actual entity of [$] meta !", index, bGFieldRelationSingle.RelatedMeta.Name, bGEntity.Meta.Name);
				}
			}
			else
			{
				Type valueType = bGField.ValueType;
				if (key.GetType() != valueType)
				{
					throw new BGException("Key $ has incompatible type: required [$], actual [$] !", index, GetValueType(valueType), GetValueType(key.GetType()));
				}
			}
		}
		else if (key.GetType() != enumType)
		{
			throw new BGException("Key $ has incompatible type: required [$], actual [$] !", index, GetValueType(enumType), GetValueType(key.GetType()));
		}
	}

	public override void Delete()
	{
		if (!base.IsDeleted)
		{
			base.Delete();
			Meta.Unregister(this);
			Unload();
		}
	}

	public static BGKey Create(BGId id, string name, bool unique, BGField[] fields)
	{
		return new BGKey(id, name, fields)
		{
			isUnique = unique
		};
	}

	public override string ToString()
	{
		return "Key [id:" + base.Id.ToString() + ", name:" + Name + ", fields count:" + CountFields + "]";
	}

	private static string GetValueType(Type valueType)
	{
		if (valueType.IsGenericType)
		{
			string text = valueType.GetGenericTypeDefinition().FullName;
			int num = text.IndexOf('`');
			if (num > 0)
			{
				text = text.Remove(num);
			}
			text += "<";
			Type[] genericArguments = valueType.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type type = genericArguments[i];
				if (i != 0)
				{
					text += ",";
				}
				text += type.FullName;
			}
			return text + ">";
		}
		return valueType.FullName;
	}

	public BGKey CloneTo(BGMetaEntity meta)
	{
		BGField[] array = new BGField[CountFields];
		for (int i = 0; i < array.Length; i++)
		{
			BGField field = meta.GetField(fields[i].Id, errorIfNotFound: false);
			if (field == null)
			{
				return null;
			}
			array[i] = field;
		}
		return new BGKey(base.Id, Name, array)
		{
			isUnique = isUnique,
			Comment = Comment,
			ControllerType = ControllerType
		};
	}

	public bool DeepEqual(BGKey t2)
	{
		if (!string.Equals(ConfigToString(), t2.ConfigToString()))
		{
			return false;
		}
		if (!string.Equals(Name, t2.Name))
		{
			return false;
		}
		if (!string.Equals(Comment, t2.Comment))
		{
			return false;
		}
		if (!string.Equals(ControllerType, t2.ControllerType))
		{
			return false;
		}
		if (isUnique != t2.isUnique)
		{
			return false;
		}
		if (CountFields != t2.CountFields)
		{
			return false;
		}
		for (int i = 0; i < CountFields; i++)
		{
			if (!object.Equals(fields[i], t2.fields[i]))
			{
				return false;
			}
		}
		return true;
	}
}
