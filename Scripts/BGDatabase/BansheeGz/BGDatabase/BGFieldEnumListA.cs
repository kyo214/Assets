using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldEnumListA<T> : BGFieldCachedClassListA<Enum>, BGFieldWithCustomConfigI where T : struct, IComparable, IConvertible, IFormattable
{
	[Serializable]
	private struct JsonConfig
	{
		public string EnumType;

		public BGFieldEnumListModeEnum Mode;
	}

	private Type enumType;

	private HashSet<Enum> constants;

	private BGFieldEnumListModeEnum mode;

	private bool allowDuplicates;

	protected new virtual char[] StringValueSeparator => BGField<List<Enum>>.AA;

	public Type UnderlyingType => typeof(T);

	public BGFieldEnumListModeEnum Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if (mode != value)
			{
				mode = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public Type EnumType
	{
		get
		{
			return enumType;
		}
		set
		{
			string errorForEnumType = BGFieldEnumA<int>.GetErrorForEnumType(value, UnderlyingType);
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
				List<Enum> storedValue = GetStoredValue(i);
				if (storedValue == null || storedValue.Count == 0)
				{
					continue;
				}
				foreach (Enum item in storedValue)
				{
					if (!Enum.IsDefined(value, item))
					{
						string text = "[enum name is not resolvable]";
						try
						{
							text = Enum.GetName(enumType, item);
						}
						catch
						{
						}
						throw new BGException("Can not resolve value with provided enum type $: entity index=$, entity name=$, not resolvable enum value=$(enum index=$)", value.FullName, i, meta[i].Name, text, item);
					}
				}
			}
			enumType = value;
			FillConstants();
			base.events.MetaWasChanged(base.Meta);
		}
	}

	public override List<Enum> this[int entityIndex]
	{
		set
		{
			if (value != null && value.Count > 0 && constants != null && constants.Count > 0)
			{
				for (int num = value.Count - 1; num >= 0; num--)
				{
					Enum item = value[num];
					if (!constants.Contains(item))
					{
						value.RemoveAt(num);
					}
				}
			}
			base[entityIndex] = value;
		}
	}

	protected BGFieldEnumListA(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name)
	{
		string errorForEnumType = BGFieldEnumA<int>.GetErrorForEnumType(enumType, UnderlyingType);
		if (!string.IsNullOrEmpty(errorForEnumType))
		{
			base.Meta.Unregister(this);
			throw new BGException(errorForEnumType);
		}
		this.enumType = enumType;
		FillConstants();
	}

	protected BGFieldEnumListA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			EnumType = enumType.FullName,
			Mode = mode
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		string typeName = jsonConfig.EnumType;
		mode = jsonConfig.Mode;
		enumType = BGFieldEnumA<int>.GetEnumType(this, UnderlyingType, typeName);
		FillConstants();
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(2);
		bGBinaryWriter.AddString(enumType.FullName);
		bGBinaryWriter.AddByte((byte)mode);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if ((uint)(num - 1) <= 1u)
		{
			string typeName = bGBinaryReader.ReadString();
			enumType = BGFieldEnumA<int>.GetEnumType(this, UnderlyingType, typeName);
			if (num == 2)
			{
				mode = (BGFieldEnumListModeEnum)bGBinaryReader.ReadByte();
			}
			FillConstants();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	private void FillConstants()
	{
		if (enumType == null)
		{
			return;
		}
		Array values = Enum.GetValues(enumType);
		if (values.Length <= 0)
		{
			return;
		}
		constants = new HashSet<Enum>();
		foreach (object item in values)
		{
			constants.Add((Enum)item);
		}
	}
}
