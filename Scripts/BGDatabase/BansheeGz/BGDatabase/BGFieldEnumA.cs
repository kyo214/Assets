using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldEnumA<T> : BGFieldCachedA<Enum, T>, BGFieldEnumI, BGFieldWithCustomConfigI where T : struct, IComparable, IConvertible, IFormattable
{
	[Serializable]
	private struct JsonConfig
	{
		public string EnumType;
	}

	private Type enumType;

	public override int ConstantSize => ValueSize;

	protected abstract int ValueSize { get; }

	public override bool CanBeUsedAsKey => true;

	protected Enum DefaultEnumValue => Activator.CreateInstance(EnumType) as Enum;

	public Type UnderlyingType => typeof(T);

	public Type EnumType
	{
		get
		{
			return enumType;
		}
		set
		{
			string errorForEnumType = GetErrorForEnumType(value, UnderlyingType);
			if (!string.IsNullOrEmpty(errorForEnumType))
			{
				throw new BGException("Can not change enum Type:" + errorForEnumType);
			}
			if (enumType == value)
			{
				return;
			}
			BGMetaEntity meta = base.Meta;
			for (int i = 0; i < meta.CountEntities; i++)
			{
				T storedValue = GetStoredValue(i);
				if (!Enum.IsDefined(value, storedValue))
				{
					string text = "[enum name is not resolvable]";
					try
					{
						text = Enum.GetName(enumType, storedValue);
					}
					catch
					{
					}
					throw new BGException("Can not resolve value with provided enum type $: entity index=$, entity name=$, not resolvable enum value=$(enum index=$)", value.FullName, i, meta[i].Name, text, storedValue);
				}
			}
			enumType = value;
			base.events.MetaWasChanged(base.Meta);
		}
	}

	public override Enum this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			T value = StoreItems[entityIndex];
			return StoredValueToEnum(value);
		}
		set
		{
			T value2 = EnumToStoredValue(value);
			if (base.events.On)
			{
				Enum obj = this[entityIndex];
				if (!object.Equals(obj, value))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, obj, value);
					StoreSet(entityIndex, value2);
					FireValueChanged(entity, obj, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value2);
			}
		}
	}

	private static BGEnumTypeNameMapper Mapper
	{
		get
		{
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.Editor.BGEnumTypeNameMapperDefault");
			if (type == null)
			{
				return null;
			}
			return Activator.CreateInstance(type) as BGEnumTypeNameMapper;
		}
	}

	protected BGFieldEnumA(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name)
	{
		string errorForEnumType = GetErrorForEnumType(enumType, UnderlyingType);
		if (!string.IsNullOrEmpty(errorForEnumType))
		{
			base.Meta.Unregister(this);
			throw new BGException(errorForEnumType);
		}
		this.enumType = enumType;
	}

	protected BGFieldEnumA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected abstract Enum StoredValueToEnum(T value);

	protected abstract T EnumToStoredValue(Enum value);

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			EnumType = enumType.FullName
		});
	}

	public override void ConfigFromString(string config)
	{
		string typeName = JsonUtility.FromJson<JsonConfig>(config).EnumType;
		enumType = GetEnumType(this, UnderlyingType, typeName);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(enumType.FullName);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			string typeName = bGBinaryReader.ReadString();
			enumType = GetEnumType(this, UnderlyingType, typeName);
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override string ToString(int entityIndex)
	{
		return Enum.GetName(EnumType, this[entityIndex]);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			this[entityIndex] = DefaultEnumValue;
			return;
		}
		if (!Enum.IsDefined(EnumType, value))
		{
			throw new BGException("Invalid enum value $ for enum $, entity index=$", value, EnumType.FullName, entityIndex);
		}
		this[entityIndex] = (Enum)Enum.Parse(EnumType, value);
	}

	public static Type GetEnumType(BGField field, Type underlyingType, string typeName)
	{
		string text = typeName;
		if (string.IsNullOrEmpty(text))
		{
			throw new BGException("Can not deserialize field $: enum type is not set!", field.FullName);
		}
		Type type = BGUtil.GetType(text);
		if (type == null)
		{
			string text2 = Mapper?.Map(text);
			if (text2 != null)
			{
				text = text2;
				type = BGUtil.GetType(text2);
				if (type == null)
				{
					throw new BGException("Can not deserialize field $: both enum type $ and mapped enum type $ are not found!", field.FullName, typeName, text2);
				}
			}
		}
		if (type == null)
		{
			throw new BGException("Can not deserialize field $: enum type $ is not found!", field.FullName, text);
		}
		if (!type.IsEnum)
		{
			throw new BGException("Can not deserialize field $: enum type $ is not enum!", field.FullName, text);
		}
		if (type.GetEnumUnderlyingType() != underlyingType)
		{
			throw new BGException("Can not deserialize field $: enum type $ has wrong underlying type, expected $ found $ !", field.FullName, text, underlyingType.FullName, type.GetEnumUnderlyingType().FullName);
		}
		return type;
	}

	public static string GetErrorForEnumType(Type enumType, Type targetUnderlyingType)
	{
		if (enumType == null)
		{
			return "enumType can not be null";
		}
		if (!enumType.IsEnum)
		{
			return BGUtil.Format("enumType $ is not enum", enumType.FullName);
		}
		Type underlyingType = Enum.GetUnderlyingType(enumType);
		if (underlyingType != targetUnderlyingType)
		{
			return BGUtil.Format("underlying type mismatch for enum $. Required type is $, but actual type is $", enumType.FullName, targetUnderlyingType.FullName, underlyingType.FullName);
		}
		return null;
	}
}
