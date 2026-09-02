using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "hashtable", Folder = "Dictionary", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerHashtable")]
public class BGFieldHashtable : BGFieldCachedClassA<Hashtable>, BGFieldWithCustomConfigI
{
	[Serializable]
	private struct JsonConfig
	{
		public string KeyType;

		public string KeyConfig;

		public string ValueType;

		public string ValueConfig;
	}

	public const ushort CodeType = 8;

	public const string DefaultDelegateMetaName = "m";

	public const string DefaultDelegateKeyFieldName = "k";

	public const string DefaultDelegateValueFieldName = "v";

	private const char EQ = '=';

	private static readonly List<byte> TempList = new List<byte>();

	private static readonly List<byte> KeysList = new List<byte>();

	private static readonly List<byte> ValuesList = new List<byte>();

	private string keyDelegateType;

	private byte[] keyDelegateConfig;

	private string valueDelegateType;

	private byte[] valueDelegateConfig;

	private BGRepo repoDelegate;

	private BGMetaRow metaDelegate;

	private BGField keyDelegate;

	private BGField valueDelegate;

	private static List<Type> allKeyFields;

	private static List<Type> allValueFields;

	private static readonly HashSet<Type> SupportedKeyFields = new HashSet<Type>
	{
		typeof(BGFieldBool),
		typeof(BGFieldByte),
		typeof(BGFieldGuid),
		typeof(BGFieldInt),
		typeof(BGFieldLong),
		typeof(BGFieldShort),
		typeof(BGFieldString),
		typeof(BGFieldText),
		typeof(BGFieldId),
		typeof(BGFieldEnum),
		typeof(BGFieldEnumByte),
		typeof(BGFieldEnumShort),
		typeof(BGFieldKeyCode)
	};

	public override ushort TypeCode => 8;

	private BGRepo RepoDelegate => repoDelegate ?? (repoDelegate = new BGRepo());

	private BGMetaRow MetaDelegate
	{
		get
		{
			if (metaDelegate != null)
			{
				return metaDelegate;
			}
			metaDelegate = new BGMetaRow(RepoDelegate, "m");
			metaDelegate.NewEntity();
			return metaDelegate;
		}
	}

	private BGField KeyDelegate
	{
		get
		{
			if (keyDelegate != null)
			{
				return keyDelegate;
			}
			if (MetaDelegate.CountFields > 1)
			{
				keyDelegate = MetaDelegate.GetField("k", errorIfNotFound: false);
			}
			if (keyDelegate == null)
			{
				keyDelegate = Create(MetaDelegate, keyDelegateType, "k", keyDelegateConfig);
			}
			return keyDelegate;
		}
	}

	private BGField ValueDelegate
	{
		get
		{
			if (valueDelegate != null)
			{
				return valueDelegate;
			}
			if (MetaDelegate.CountFields > 1)
			{
				valueDelegate = MetaDelegate.GetField("v", errorIfNotFound: false);
			}
			if (valueDelegate == null)
			{
				valueDelegate = Create(MetaDelegate, valueDelegateType, "v", valueDelegateConfig);
			}
			return valueDelegate;
		}
	}

	public string KeyDelegateType => keyDelegateType;

	public string ValueDelegateType => valueDelegateType;

	public override string Description => "Field [hashtable(key=" + KeyDelegate.ValueType.FullName + ",value=" + ValueDelegate.ValueType.FullName + ")]";

	public static List<Type> AllKeyFields
	{
		get
		{
			if (allKeyFields != null)
			{
				return allKeyFields;
			}
			allKeyFields = BGField.FieldTypes.FindAll((Type type) =>
			{
				if (!BGUtil.HasAttribute<FieldDescriptor>(type, inherit: false))
				{
					return false;
				}
				return !string.IsNullOrEmpty(BGUtil.GetAttribute<FieldDescriptor>(type).Name) && IsFieldSupportedAsKey(type);
			});
			return allKeyFields;
		}
	}

	public static List<Type> AllValueFields
	{
		get
		{
			if (allValueFields != null)
			{
				return allValueFields;
			}
			allValueFields = BGField.FieldTypes.FindAll((Type type) =>
			{
				if (!BGUtil.HasAttribute<FieldDescriptor>(type, inherit: false))
				{
					return false;
				}
				return !string.IsNullOrEmpty(BGUtil.GetAttribute<FieldDescriptor>(type).Name) && IsFieldSupportedAsValue(type);
			});
			return allValueFields;
		}
	}

	public BGFieldHashtable(BGMetaEntity meta, string name, Type keyFieldType, Type valueFieldType)
		: base(meta, name)
	{
		BGException ex = null;
		if (keyFieldType == null)
		{
			ex = new BGException("keyFieldType can not be null");
		}
		else if (valueFieldType == null)
		{
			ex = new BGException("valueFieldType can not be null");
		}
		else if (!IsFieldSupportedAsKey(keyFieldType))
		{
			ex = new BGException("$ field is not supported as key field", keyFieldType.FullName);
		}
		else if (!IsFieldSupportedAsValue(valueFieldType))
		{
			ex = new BGException("$ field is not supported as value field", valueFieldType.FullName);
		}
		else
		{
			Type[] types = new Type[2]
			{
				typeof(BGMetaEntity),
				typeof(string)
			};
			ConstructorInfo constructor = keyFieldType.GetConstructor(types);
			if (constructor == null)
			{
				ex = new BGException("$ field is not supported by this constructor, cause it requires custom configuration.Use BGFieldHashtable(BGMetaEntity meta, string name, BGField keyField, BGField valueField)", keyFieldType.FullName);
			}
			else
			{
				ConstructorInfo constructor2 = valueFieldType.GetConstructor(types);
				if (constructor2 == null)
				{
					ex = new BGException("$ field is not supported by this constructor, cause it requires custom configuration.Use BGFieldHashtable(BGMetaEntity meta, string name, BGField keyField, BGField valueField)", valueFieldType.FullName);
				}
				else
				{
					keyDelegateType = keyFieldType.AssemblyQualifiedName;
					valueDelegateType = valueFieldType.AssemblyQualifiedName;
				}
			}
		}
		if (ex != null)
		{
			Unregister();
			throw ex;
		}
	}

	public BGFieldHashtable(BGMetaEntity meta, string name, BGField keyField, BGField valueField)
		: base(meta, name)
	{
		BGException ex = null;
		if (keyField == null)
		{
			ex = new BGException("keyField can not be null");
		}
		else if (valueField == null)
		{
			ex = new BGException("valueField can not be null");
		}
		else if (!IsFieldSupportedAsKey(keyField.GetType()))
		{
			ex = new BGException("$ field is not supported as key field", keyField.GetType().FullName);
		}
		else if (!IsFieldSupportedAsValue(valueField.GetType()))
		{
			ex = new BGException("$ field is not supported as value field", valueField.GetType().FullName);
		}
		else
		{
			keyDelegateType = keyField.GetType().AssemblyQualifiedName;
			keyDelegateConfig = keyField.ConfigToBytes();
			valueDelegateType = valueField.GetType().AssemblyQualifiedName;
			valueDelegateConfig = valueField.ConfigToBytes();
		}
		if (ex != null)
		{
			Unregister();
			throw ex;
		}
	}

	protected internal BGFieldHashtable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldHashtable(meta, id, name);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		Hashtable hashtable = this[entityIndex];
		if (hashtable == null || hashtable.Count == 0)
		{
			return null;
		}
		BGField bGField = KeyDelegate;
		BGField bGField2 = ValueDelegate;
		KeysList.Clear();
		ValuesList.Clear();
		TempList.Clear();
		int num = 0;
		foreach (DictionaryEntry item in hashtable)
		{
			object key = item.Key;
			object value = item.Value;
			if (IsSupported(bGField, bGField2, key, value))
			{
				byte[] array;
				byte[] array2;
				try
				{
					array = ToBytes(bGField, key);
					array2 = ToBytes(bGField2, value);
				}
				catch
				{
					continue;
				}
				if (array != null && array2 != null)
				{
					num++;
					KeysList.AddRange(array);
					ValuesList.AddRange(array2);
				}
			}
		}
		if (num == 0)
		{
			return null;
		}
		TempList.AddRange(BGFieldInt.ValueToBytes(num));
		TempList.AddRange(KeysList);
		TempList.AddRange(ValuesList);
		KeysList.Clear();
		ValuesList.Clear();
		byte[] result = TempList.ToArray();
		TempList.Clear();
		return result;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count < 4)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset, 4));
		if (num == 0)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		Hashtable hashtable = this[entityIndex];
		if (hashtable == null)
		{
			hashtable = (this[entityIndex] = new Hashtable());
		}
		else
		{
			hashtable.Clear();
		}
		BGField delegateField = KeyDelegate;
		BGField delegateField2 = ValueDelegate;
		object[] array = new object[num];
		object[] array2 = new object[num];
		int cursor = segment.Offset + 4;
		for (int i = 0; i < num; i++)
		{
			array[i] = FromBytes(ref cursor, delegateField, segment.Array);
		}
		for (int j = 0; j < num; j++)
		{
			array2[j] = FromBytes(ref cursor, delegateField2, segment.Array);
		}
		for (int k = 0; k < num; k++)
		{
			object obj = array[k];
			object obj2 = array2[k];
			if (obj != null && obj2 != null)
			{
				hashtable[obj] = obj2;
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		Hashtable hashtable = this[entityIndex];
		if (hashtable == null || hashtable.Count == 0)
		{
			return null;
		}
		BGField bGField = KeyDelegate;
		BGField bGField2 = ValueDelegate;
		string text = "";
		foreach (DictionaryEntry item in hashtable)
		{
			object key = item.Key;
			object value = item.Value;
			if (!IsSupported(bGField, bGField2, key, value))
			{
				continue;
			}
			string text2;
			string text3;
			try
			{
				text2 = ToString(bGField, key);
				text3 = ToString(bGField2, value);
			}
			catch
			{
				continue;
			}
			if (text2 != null && text3 != null)
			{
				if (text.Length != 0)
				{
					text += "|";
				}
				text = text + text2 + "=" + text3;
			}
		}
		return text;
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		List<string> list = new List<string>();
		BGFieldListString.Split(list, value, '|', '\\', keepEscape: true);
		if (list.Count == 0)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		BGField delegateField = KeyDelegate;
		BGField delegateField2 = ValueDelegate;
		Hashtable hashtable = new Hashtable();
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			list2.Clear();
			BGFieldListString.Split(list2, text, '=', '\\');
			if (list2.Count == 2)
			{
				string value2 = list2[0];
				string value3 = list2[1];
				object obj;
				object obj2;
				try
				{
					obj = FromString(delegateField, value2);
					obj2 = FromString(delegateField2, value3);
				}
				catch
				{
					continue;
				}
				if (obj != null && obj2 != null)
				{
					hashtable[obj] = obj2;
				}
			}
		}
		if (hashtable.Count == 0)
		{
			ClearValueNoEvent(entityIndex);
		}
		else
		{
			this[entityIndex] = hashtable;
		}
	}

	public static string ToString(BGField delegateField, object value)
	{
		delegateField.ClearValue(0);
		delegateField.SetValue(0, value);
		return EscapeString(delegateField.ToString(0));
	}

	public static object FromString(BGField delegateField, string value)
	{
		delegateField.ClearValue(0);
		delegateField.FromString(0, value);
		return delegateField.GetValue(0);
	}

	private static string EscapeString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		return value.Replace("\\", "\\\\").Replace("=", "\\=").Replace("|", "\\|");
	}

	public static byte[] ToBytes(BGField delegateField, object value)
	{
		delegateField.ClearValue(0);
		delegateField.SetValue(0, value);
		if (delegateField.ConstantSize > 0)
		{
			return delegateField.ToBytes(0);
		}
		byte[] array = delegateField.ToBytes(0);
		if (array == null || array.Length == 0)
		{
			return BGFieldInt.ValueToBytes(0);
		}
		byte[] array2 = new byte[array.Length + 4];
		byte[] array3 = BGFieldInt.ValueToBytes(array.Length);
		array3.CopyTo(array2, 0);
		array.CopyTo(array2, 4);
		return array2;
	}

	public static object FromBytes(ref int cursor, BGField delegateField, byte[] array)
	{
		delegateField.ClearValue(0);
		int constantSize = delegateField.ConstantSize;
		if (constantSize > 0)
		{
			delegateField.FromBytes(0, new ArraySegment<byte>(array, cursor, constantSize));
			cursor += constantSize;
		}
		else
		{
			int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, cursor, 4));
			cursor += 4;
			delegateField.FromBytes(0, new ArraySegment<byte>(array, cursor, num));
			cursor += num;
		}
		return delegateField.GetValue(0);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			KeyType = keyDelegateType,
			KeyConfig = KeyDelegate.ConfigToString(),
			ValueType = valueDelegateType,
			ValueConfig = ValueDelegate.ConfigToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		keyDelegateType = jsonConfig.KeyType;
		valueDelegateType = jsonConfig.ValueType;
		KeyDelegate.ConfigFromString(jsonConfig.KeyConfig);
		ValueDelegate.ConfigFromString(jsonConfig.ValueConfig);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(64);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(keyDelegateType);
		bGBinaryWriter.AddString(valueDelegateType);
		bGBinaryWriter.AddByteArray(keyDelegateConfig);
		bGBinaryWriter.AddByteArray(valueDelegateConfig);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			keyDelegateType = bGBinaryReader.ReadString();
			valueDelegateType = bGBinaryReader.ReadString();
			keyDelegateConfig = BGUtil.ToArray(bGBinaryReader.ReadByteArray());
			valueDelegateConfig = BGUtil.ToArray(bGBinaryReader.ReadByteArray());
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public static bool IsFieldSupportedAsKey(Type fieldType)
	{
		return SupportedKeyFields.Contains(fieldType);
	}

	public static bool IsFieldSupportedAsValue(Type fieldType)
	{
		if (!typeof(BGField).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGAssetLoaderA.WithLoaderI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGFieldCalcI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGFieldCodedI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGAbstractRelationI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGSceneObjectReferenceI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGStructNullableI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (typeof(BGFieldHashtable) == fieldType)
		{
			return false;
		}
		if (typeof(BGFieldEnumList) == fieldType)
		{
			return false;
		}
		if (typeof(BGArrayI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		if (fieldType.FullName.StartsWith("BansheeGz.BGDatabase.BGFieldAnimationCurve"))
		{
			return false;
		}
		if (typeof(BGFieldGradient) == fieldType)
		{
			return false;
		}
		if (typeof(BGFieldMetaReference) == fieldType)
		{
			return false;
		}
		return true;
	}

	public static bool IsSupported(BGField keyDelegateField, BGField valueDelegateField, object key, object value)
	{
		if (IsSupported(keyDelegateField, key))
		{
			return IsSupported(valueDelegateField, value);
		}
		return false;
	}

	public static bool IsSupported(BGField delegateField, object value)
	{
		if (value == null)
		{
			return false;
		}
		if (delegateField.ValueType == value.GetType())
		{
			return true;
		}
		if (!(delegateField is BGFieldEnumI))
		{
			if (delegateField is BGStructNullableI)
			{
				Type[] genericArguments = delegateField.ValueType.GetGenericArguments();
				Type type = ((genericArguments.Length != 0) ? genericArguments[0] : null);
				if (type == value.GetType())
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldHashtable bGFieldHashtable))
		{
			return false;
		}
		Hashtable hashtable = this[myEntityIndex];
		Hashtable hashtable2 = bGFieldHashtable[otherEntityIndex];
		bool flag = IsEmpty(hashtable);
		bool flag2 = IsEmpty(hashtable2);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		if (hashtable.Count != hashtable2.Count)
		{
			return false;
		}
		BGField bGField = ValueDelegate;
		BGField bGField2 = bGFieldHashtable.ValueDelegate;
		if (bGField.GetType() != bGField2.GetType())
		{
			return false;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			object obj = hashtable2[item.Key];
			if (item.Value != null || obj != null)
			{
				if (item.Value == null || obj == null)
				{
					return false;
				}
				bGField.SetValue(0, item.Value);
				bGField2.SetValue(0, obj);
				if (!bGField.AreStoredValuesEqual(bGField2, 0, 0))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsEmpty(Hashtable list)
	{
		if (list != null)
		{
			return list.Count == 0;
		}
		return true;
	}

	private static BGField Create(BGMetaEntity meta, string type, string name, byte[] config)
	{
		return BGField.Create(meta, type, BGId.NewId, name, (config == null) ? new ArraySegment<byte>(Array.Empty<byte>()) : new ArraySegment<byte>(config), system: false, null, null, required: false);
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || fromField.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num == -1)
		{
			return;
		}
		BGFieldHashtable bGFieldHashtable = (BGFieldHashtable)fromField;
		Hashtable hashtable = bGFieldHashtable[fromEntityIndex];
		if (hashtable == null || hashtable.Count == 0)
		{
			ClearValueNoEvent(num);
			return;
		}
		BGField bGField = KeyDelegate;
		BGField bGField2 = ValueDelegate;
		bool isValueType = bGField.ValueType.IsValueType;
		bool isCloneable = !isValueType && typeof(ICloneable).IsAssignableFrom(bGField.ValueType);
		bool isValueType2 = bGField2.ValueType.IsValueType;
		bool isCloneable2 = !isValueType2 && typeof(ICloneable).IsAssignableFrom(bGField2.ValueType);
		Hashtable hashtable2 = new Hashtable(hashtable.Count);
		foreach (DictionaryEntry item in hashtable)
		{
			hashtable2[Clone(isValueType, isCloneable, item.Key)] = Clone(isValueType2, isCloneable2, item.Value);
		}
		StoreSet(num, hashtable2);
	}

	private static object Clone(bool isValue, bool isCloneable, object obj)
	{
		if (isValue)
		{
			return obj;
		}
		if (isCloneable)
		{
			return ((ICloneable)obj).Clone();
		}
		return BGUtil.Clone(obj);
	}

	public void CreateKeyValueDelegates(out BGField key, out BGField value)
	{
		BGRepo repo = new BGRepo();
		BGMetaRow bGMetaRow = new BGMetaRow(repo, "m");
		bGMetaRow.NewEntity();
		key = Create(bGMetaRow, keyDelegateType, "k", keyDelegateConfig);
		value = Create(bGMetaRow, valueDelegateType, "v", valueDelegateConfig);
	}
}
