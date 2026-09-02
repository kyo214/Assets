using System;
using System.Globalization;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeEnum : BGCalcTypeCode<Enum>, BGCalcTypeCodeStateful
{
	[Serializable]
	private class JsonValue
	{
		public byte code;

		public string value;
	}

	private string enumTypeAsString;

	private Type enumType;

	public const byte Code = 7;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 7;

	public override object DefaultValue
	{
		get
		{
			Type type = EnumType;
			if (type == null)
			{
				return null;
			}
			Array values = Enum.GetValues(type);
			if (values.Length == 0)
			{
				return null;
			}
			return values.GetValue(0);
		}
	}

	public string EnumTypeAsString => enumTypeAsString;

	public Type EnumType
	{
		get
		{
			if (enumType != null)
			{
				return enumType;
			}
			Type type = BGUtil.GetType(enumTypeAsString);
			if (type != null && type.IsEnum)
			{
				enumType = type;
			}
			return enumType;
		}
	}

	public override string TypeTitle => Name + " [" + enumTypeAsString + "]";

	public override string Name => "enum";

	internal BGCalcTypeCodeEnum()
	{
	}

	public BGCalcTypeCodeEnum(Type enumType)
	{
		if (enumType == null)
		{
			throw new Exception("type can not be null!");
		}
		if (!enumType.IsEnum)
		{
			throw new Exception(enumType.FullName + " type is not enum!");
		}
		enumTypeAsString = enumType.FullName;
		this.enumType = enumType;
	}

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		Type type = EnumType;
		if (type == null)
		{
			throw new Exception("Can not serialize enum field, cause enum type with name " + enumTypeAsString + " can not be found!");
		}
		TypeCode typeCode = Type.GetTypeCode(type.GetEnumUnderlyingType());
		writer.AddByte((byte)typeCode);
		switch (typeCode)
		{
		case System.TypeCode.Byte:
			writer.AddByte((byte)value);
			break;
		case System.TypeCode.Int16:
			writer.AddShort((short)value);
			break;
		case System.TypeCode.Int32:
			writer.AddInt((int)value);
			break;
		case System.TypeCode.Int64:
			writer.AddLong((long)value);
			break;
		case System.TypeCode.SByte:
			writer.AddSByte((sbyte)value);
			break;
		case System.TypeCode.UInt16:
			writer.AddUShort((ushort)value);
			break;
		case System.TypeCode.UInt32:
			writer.AddUInt((uint)value);
			break;
		case System.TypeCode.UInt64:
			writer.AddULong((ulong)value);
			break;
		default:
			throw new ArgumentOutOfRangeException("code", "Unsupported enum underlying enum type code=" + typeCode);
		}
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		Type type = EnumType;
		if (type == null)
		{
			throw new Exception("Can not deserialize enum field, cause enum type with name " + enumTypeAsString + " can not be found!");
		}
		byte b = reader.ReadByte();
		return Enum.ToObject(type, (TypeCode)b switch
		{
			System.TypeCode.Byte => reader.ReadByte(), 
			System.TypeCode.Int16 => reader.ReadShort(), 
			System.TypeCode.Int32 => reader.ReadInt(), 
			System.TypeCode.Int64 => reader.ReadLong(), 
			System.TypeCode.SByte => reader.ReadSByte(), 
			System.TypeCode.UInt16 => reader.ReadUShort(), 
			System.TypeCode.UInt32 => reader.ReadUInt(), 
			System.TypeCode.UInt64 => reader.ReadULong(), 
			_ => throw new ArgumentOutOfRangeException("code", "Unsupported enum underlying enum type code=" + b), 
		});
	}

	public override string ValueToString(object value)
	{
		Type type = EnumType;
		if (type == null)
		{
			throw new Exception("Can not serialize enum field, cause enum type with name " + enumTypeAsString + " can not be found!");
		}
		JsonValue jsonValue = new JsonValue
		{
			code = (byte)Type.GetTypeCode(type.GetEnumUnderlyingType())
		};
		switch ((TypeCode)jsonValue.code)
		{
		case System.TypeCode.Byte:
			jsonValue.value = ((byte)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.Int16:
			jsonValue.value = ((short)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.Int32:
			jsonValue.value = ((int)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.Int64:
			jsonValue.value = ((long)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.SByte:
			jsonValue.value = ((sbyte)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.UInt16:
			jsonValue.value = ((ushort)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.UInt32:
			jsonValue.value = ((uint)value).ToString(CultureInfo.InvariantCulture);
			break;
		case System.TypeCode.UInt64:
			jsonValue.value = ((ulong)value).ToString(CultureInfo.InvariantCulture);
			break;
		default:
			throw new ArgumentOutOfRangeException("code", "Unsupported enum underlying enum type code=" + jsonValue.code);
		}
		return JsonUtility.ToJson(jsonValue);
	}

	public override object ValueFromString(string valueString)
	{
		if (string.IsNullOrEmpty(valueString))
		{
			return null;
		}
		JsonValue jsonValue = JsonUtility.FromJson<JsonValue>(valueString);
		return Enum.ToObject(value: (TypeCode)jsonValue.code switch
		{
			System.TypeCode.Byte => byte.Parse(jsonValue.value), 
			System.TypeCode.Int16 => short.Parse(jsonValue.value), 
			System.TypeCode.Int32 => int.Parse(jsonValue.value), 
			System.TypeCode.Int64 => long.Parse(jsonValue.value), 
			System.TypeCode.SByte => sbyte.Parse(jsonValue.value), 
			System.TypeCode.UInt16 => ushort.Parse(jsonValue.value), 
			System.TypeCode.UInt32 => uint.Parse(jsonValue.value), 
			System.TypeCode.UInt64 => ulong.Parse(jsonValue.value), 
			_ => throw new ArgumentOutOfRangeException("code", "Unsupported enum underlying enum type code=" + jsonValue.code), 
		}, enumType: enumType);
	}

	protected bool Equals(BGCalcTypeCodeEnum other)
	{
		if (Equals((BGCalcTypeCode)other))
		{
			return enumTypeAsString == other.enumTypeAsString;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGCalcTypeCodeEnum)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((enumTypeAsString != null) ? enumTypeAsString.GetHashCode() : 0);
	}

	public void ReadState(BGBinaryReader reader)
	{
		enumTypeAsString = reader.ReadString();
		enumType = null;
	}

	public void WriteState(BGBinaryWriter writer)
	{
		writer.AddString(enumTypeAsString ?? "");
	}

	public void ReadState(string state)
	{
		enumTypeAsString = state;
		enumType = null;
	}

	public string WriteState()
	{
		return enumTypeAsString;
	}
}
