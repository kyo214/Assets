using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public static class BGCalcTypeCodeRegistry
{
	public static readonly BGCalcTypeCodeControl Control = new BGCalcTypeCodeControl();

	public static readonly BGCalcTypeCodeBool Bool = new BGCalcTypeCodeBool();

	public static readonly BGCalcTypeCodeString String = new BGCalcTypeCodeString();

	public static readonly BGCalcTypeCodeInt Int = new BGCalcTypeCodeInt();

	public static readonly BGCalcTypeCodeFloat Float = new BGCalcTypeCodeFloat();

	public static readonly BGCalcTypeCodeBGId BGId = new BGCalcTypeCodeBGId();

	public static readonly BGCalcTypeCodeSource EntitySource = new BGCalcTypeCodeSource();

	public static readonly BGCalcTypeCodeObject Object = new BGCalcTypeCodeObject();

	public static readonly BGCalcTypeCodeList List = new BGCalcTypeCodeList();

	public static readonly BGCalcTypeCodeMeta Meta = new BGCalcTypeCodeMeta();

	public static readonly BGCalcTypeCodeField Field = new BGCalcTypeCodeField();

	public static readonly BGCalcTypeCodeEntity Entity = new BGCalcTypeCodeEntity();

	public static readonly BGCalcTypeCodeCell Cell = new BGCalcTypeCodeCell();

	public static readonly BGCalcTypeCodeByte Byte = new BGCalcTypeCodeByte();

	public static readonly BGCalcTypeCodeShort Short = new BGCalcTypeCodeShort();

	public static readonly BGCalcTypeCodeSByte SByte = new BGCalcTypeCodeSByte();

	public static readonly BGCalcTypeCodeUShort UShort = new BGCalcTypeCodeUShort();

	public static readonly BGCalcTypeCodeVector2 Vector2 = new BGCalcTypeCodeVector2();

	public static readonly BGCalcTypeCodeVector3 Vector3 = new BGCalcTypeCodeVector3();

	public static readonly BGCalcTypeCodeVector4 Vector4 = new BGCalcTypeCodeVector4();

	public static readonly BGCalcTypeCodeGameObject GameObject = new BGCalcTypeCodeGameObject();

	public static readonly BGCalcTypeCodeComponent Component = new BGCalcTypeCodeComponent();

	public static readonly BGCalcTypeCodeCalcAction CalcAction = new BGCalcTypeCodeCalcAction();

	public static BGCalcTypeCode[] TypeCodes => new BGCalcTypeCode[25]
	{
		Control,
		Bool,
		String,
		Int,
		Float,
		BGId,
		new BGCalcTypeCodeEnum(),
		new BGCalcTypeCodeEntityRuntime(),
		EntitySource,
		Object,
		List,
		Meta,
		Field,
		Entity,
		Cell,
		Byte,
		Short,
		SByte,
		UShort,
		Vector2,
		Vector3,
		Vector4,
		GameObject,
		Component,
		CalcAction
	};

	public static BGCalcTypeCode Get(byte code)
	{
		return code switch
		{
			1 => Control, 
			2 => Bool, 
			3 => String, 
			4 => Int, 
			5 => Float, 
			6 => BGId, 
			7 => new BGCalcTypeCodeEnum(), 
			8 => new BGCalcTypeCodeEntityRuntime(), 
			9 => new BGCalcTypeCodeSource(), 
			10 => Object, 
			11 => List, 
			12 => Meta, 
			14 => Field, 
			15 => Entity, 
			16 => Cell, 
			17 => Byte, 
			18 => Byte, 
			19 => SByte, 
			20 => UShort, 
			21 => Vector2, 
			22 => Vector3, 
			23 => Vector4, 
			24 => GameObject, 
			25 => Component, 
			26 => CalcAction, 
			_ => throw new Exception($"unknown type code {code}"), 
		};
	}

	public static BGCalcTypeCode Get(Type type)
	{
		BGCalcTypeCode result = null;
		switch (type.FullName)
		{
		case "BansheeGz.BGDatabase.BGCalcControl":
			result = Control;
			break;
		case "System.Boolean":
			result = Bool;
			break;
		case "System.String":
			result = String;
			break;
		case "System.Int32":
			result = Int;
			break;
		case "System.Single":
			result = Float;
			break;
		case "BansheeGz.Database.BGId":
			result = BGId;
			break;
		case "BansheeGz.Database.BGCalcVarTypeCodeEnum":
			result = EntitySource;
			break;
		case "System.Object":
			result = Object;
			break;
		case "System.Collections.IList":
			result = List;
			break;
		case "BansheeGz.Database.BGMetaEntity":
			result = Meta;
			break;
		case "BansheeGz.Database.BGField":
			result = Field;
			break;
		case "BansheeGz.Database.BGEntity":
			result = Entity;
			break;
		case "BansheeGz.Database.BGCalcCell":
			result = Cell;
			break;
		case "System.Byte":
			result = Byte;
			break;
		case "System.Int16":
			result = Short;
			break;
		case "System.SByte":
			result = SByte;
			break;
		case "System.UInt16":
			result = UShort;
			break;
		case "UnityEngine.Vector2":
			result = Vector2;
			break;
		case "UnityEngine.Vector3":
			result = Vector3;
			break;
		case "UnityEngine.Vector4":
			result = Vector4;
			break;
		case "UnityEngine.GameObject":
			result = GameObject;
			break;
		case "UnityEngine.Component":
			result = Component;
			break;
		case "BansheeGz.BGDatabase.BGFieldCalcActionValue":
			result = CalcAction;
			break;
		}
		return result;
	}

	public static List<BGCalcTypeCode> Find(Predicate<BGCalcTypeCode> filter)
	{
		List<BGCalcTypeCode> list = new List<BGCalcTypeCode>();
		BGCalcTypeCode[] typeCodes = TypeCodes;
		foreach (BGCalcTypeCode bGCalcTypeCode in typeCodes)
		{
			if (filter == null || filter(bGCalcTypeCode))
			{
				list.Add(bGCalcTypeCode);
			}
		}
		return list;
	}
}
