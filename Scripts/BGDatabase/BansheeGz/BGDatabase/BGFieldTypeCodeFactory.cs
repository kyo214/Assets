using System;

namespace BansheeGz.BGDatabase;

public class BGFieldTypeCodeFactory : BGFieldTypeCodeFactory.BGFieldTypeCodeFactoryI
{
	public interface BGFieldTypeCodeFactoryI
	{
		BGField Create(BGMetaEntity meta, ushort typeCode, BGId id, string name);
	}

	public static readonly BGFieldTypeCodeFactory Instance = new BGFieldTypeCodeFactory();

	private BGFieldTypeCodeFactory()
	{
	}

	public BGField Create(BGMetaEntity meta, ushort typeCode, BGId id, string name, ArraySegment<byte> config, bool system, string addon, string defaultValue, bool required)
	{
		BGField bGField = Create(meta, typeCode, id, name);
		if (bGField == null)
		{
			throw new Exception($"Can not create a field: unsupported field type code={typeCode}!");
		}
		bGField.DefaultValue = defaultValue;
		bGField.System = system;
		bGField.Addon = addon;
		bGField.Required = required;
		bGField.ConfigFromBytes(config);
		return bGField;
	}

	public BGField Create(BGMetaEntity meta, ushort typeCode, BGId id, string name)
	{
		return typeCode switch
		{
			1 => new BGFieldArrayByte(meta, id, name), 
			2 => new BGFieldCalcAction(meta, id, name), 
			3 => new BGFieldCalcBool(meta, id, name), 
			4 => new BGFieldCalcFloat(meta, id, name), 
			5 => new BGFieldCalcInt(meta, id, name), 
			6 => new BGFieldCalcObject(meta, id, name), 
			7 => new BGFieldCalcString(meta, id, name), 
			8 => new BGFieldHashtable(meta, id, name), 
			9 => new BGFieldEnum(meta, id, name), 
			10 => new BGFieldEnumByte(meta, id, name), 
			11 => new BGFieldEnumList(meta, id, name), 
			12 => new BGFieldEnumShort(meta, id, name), 
			13 => new BGFieldListBool(meta, id, name), 
			14 => new BGFieldListDouble(meta, id, name), 
			15 => new BGFieldListFloat(meta, id, name), 
			16 => new BGFieldListGuid(meta, id, name), 
			17 => new BGFieldListInt(meta, id, name), 
			18 => new BGFieldListLong(meta, id, name), 
			19 => new BGFieldListString(meta, id, name), 
			20 => new BGFieldListColor(meta, id, name), 
			21 => new BGFieldListQuaternion(meta, id, name), 
			22 => new BGFieldListVector2(meta, id, name), 
			23 => new BGFieldListVector3(meta, id, name), 
			24 => new BGFieldListVector4(meta, id, name), 
			25 => new BGFieldBool(meta, id, name), 
			26 => new BGFieldByte(meta, id, name), 
			27 => new BGFieldDecimal(meta, id, name), 
			28 => new BGFieldDouble(meta, id, name), 
			29 => new BGFieldFloat(meta, id, name), 
			30 => new BGFieldGuid(meta, id, name), 
			31 => new BGFieldInt(meta, id, name), 
			32 => new BGFieldLong(meta, id, name), 
			33 => new BGFieldShort(meta, id, name), 
			34 => new BGFieldString(meta, id, name), 
			35 => new BGFieldText(meta, id, name), 
			36 => new BGFieldBoolNullable(meta, id, name), 
			37 => new BGFieldDoubleNullable(meta, id, name), 
			38 => new BGFieldFloatNullable(meta, id, name), 
			39 => new BGFieldGuidNullable(meta, id, name), 
			40 => new BGFieldIntNullable(meta, id, name), 
			41 => new BGFieldLongNullable(meta, id, name), 
			42 => new BGFieldManyRelationsMultiple(meta, id, name), 
			43 => new BGFieldManyRelationsSingle(meta, id, name), 
			44 => new BGFieldNested(meta, id, name), 
			45 => new BGFieldRelationMultiple(meta, id, name), 
			46 => new BGFieldRelationSingle(meta, id, name), 
			47 => new BGFieldEntityName(meta, id, name), 
			48 => new BGFieldId(meta, id, name), 
			49 => new BGFieldUnityAudioClip(meta, id, name), 
			50 => new BGFieldUnityFont(meta, id, name), 
			51 => new BGFieldUnityMaterial(meta, id, name), 
			52 => new BGFieldUnityObject(meta, id, name), 
			53 => new BGFieldUnityPrefab(meta, id, name), 
			54 => new BGFieldUnityScriptableObject(meta, id, name), 
			55 => new BGFieldUnitySprite(meta, id, name), 
			56 => new BGFieldUnitySpriteMultiple(meta, id, name), 
			57 => new BGFieldUnityTexture(meta, id, name), 
			58 => new BGFieldUnityTexture2d(meta, id, name), 
			59 => new BGFieldAnimationCurve2017(meta, id, name), 
			60 => new BGFieldBounds(meta, id, name), 
			61 => new BGFieldColor(meta, id, name), 
			62 => new BGFieldGradient(meta, id, name), 
			63 => new BGFieldKeyCode(meta, id, name), 
			64 => new BGFieldQuaternion(meta, id, name), 
			65 => new BGFieldRay(meta, id, name), 
			66 => new BGFieldRay2d(meta, id, name), 
			67 => new BGFieldRect(meta, id, name), 
			68 => new BGFieldVector2(meta, id, name), 
			69 => new BGFieldVector3(meta, id, name), 
			70 => new BGFieldVector4(meta, id, name), 
			71 => new BGFieldColorNullable(meta, id, name), 
			72 => new BGFieldQuaternionNullable(meta, id, name), 
			73 => new BGFieldVector2Nullable(meta, id, name), 
			74 => new BGFieldVector3Nullable(meta, id, name), 
			75 => new BGFieldVector4Nullable(meta, id, name), 
			76 => new BGFieldReferenceToEntityGo(meta, id, name), 
			77 => new BGFieldReferenceToEntityGoList(meta, id, name), 
			78 => new BGFieldReferenceToUnityObject(meta, id, name), 
			79 => new BGFieldReferenceToUnityObjectList(meta, id, name), 
			96 => new BGFieldCalcList(meta, id, name), 
			97 => new BGFieldViewRelationSingle(meta, id, name), 
			98 => new BGFieldViewRelationMultiple(meta, id, name), 
			99 => new BGFieldReferenceListMV(meta, id, name), 
			100 => new BGFieldCodedBool(meta, id, name), 
			101 => new BGFieldCodedFloat(meta, id, name), 
			102 => new BGFieldCodedInt(meta, id, name), 
			103 => new BGFieldCodedString(meta, id, name), 
			104 => new BGFieldCodedObject(meta, id, name), 
			105 => new BGFieldByteNullable(meta, id, name), 
			106 => new BGFieldShortNullable(meta, id, name), 
			107 => new BGFieldAnimationCurve2020(meta, id, name), 
			108 => new BGFieldMetaReference(meta, id, name), 
			_ => BGLocalizationUglyHacks.LocalizationFieldFactory?.Create(meta, typeCode, id, name), 
		};
	}
}
