using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGMTFieldFactory
{
	private static readonly Dictionary<Type, Func<BGField, BGMTField>> Type2Provider;

	static BGMTFieldFactory()
	{
		Type2Provider = new Dictionary<Type, Func<BGField, BGMTField>>();
		Type2Provider.Add(typeof(BGFieldBool), (BGField field) => new BGMTFieldCached<bool>(field));
		Type2Provider.Add(typeof(BGFieldDouble), (BGField field) => new BGMTFieldCached<double>(field));
		Type2Provider.Add(typeof(BGFieldDecimal), (BGField field) => new BGMTFieldCached<decimal>(field));
		Type2Provider.Add(typeof(BGFieldFloat), (BGField field) => new BGMTFieldCached<float>(field));
		Type2Provider.Add(typeof(BGFieldGuid), (BGField field) => new BGMTFieldCached<Guid>(field));
		Type2Provider.Add(typeof(BGFieldInt), (BGField field) => new BGMTFieldCached<int>(field));
		Type2Provider.Add(typeof(BGFieldLong), (BGField field) => new BGMTFieldCached<long>(field));
		Type2Provider.Add(typeof(BGFieldString), (BGField field) => new BGMTFieldCached<string>(field));
		Type2Provider.Add(typeof(BGFieldText), (BGField field) => new BGMTFieldCached<string>(field));
		Type2Provider.Add(typeof(BGFieldBoolNullable), (BGField field) => new BGMTFieldCached<bool?>(field));
		Type2Provider.Add(typeof(BGFieldDoubleNullable), (BGField field) => new BGMTFieldCached<double?>(field));
		Type2Provider.Add(typeof(BGFieldFloatNullable), (BGField field) => new BGMTFieldCached<float?>(field));
		Type2Provider.Add(typeof(BGFieldGuidNullable), (BGField field) => new BGMTFieldCached<Guid?>(field));
		Type2Provider.Add(typeof(BGFieldIntNullable), (BGField field) => new BGMTFieldCached<int?>(field));
		Type2Provider.Add(typeof(BGFieldLongNullable), (BGField field) => new BGMTFieldCached<long?>(field));
		Type2Provider.Add(typeof(BGFieldEntityName), (BGField field) => new BGMTFieldCached<string>(field));
		Type2Provider.Add(typeof(BGFieldId), (BGField field) => new BGMTFieldCached<BGId>(field));
		Type2Provider.Add(typeof(BGFieldBounds), (BGField field) => new BGMTFieldCached<Bounds>(field));
		Type2Provider.Add(typeof(BGFieldColor), (BGField field) => new BGMTFieldCached<Color>(field));
		Type2Provider.Add(typeof(BGFieldKeyCode), (BGField field) => new BGMTFieldCached<KeyCode>(field));
		Type2Provider.Add(typeof(BGFieldQuaternion), (BGField field) => new BGMTFieldCached<Quaternion>(field));
		Type2Provider.Add(typeof(BGFieldRay), (BGField field) => new BGMTFieldCached<Ray>(field));
		Type2Provider.Add(typeof(BGFieldRay2d), (BGField field) => new BGMTFieldCached<Ray2D>(field));
		Type2Provider.Add(typeof(BGFieldRect), (BGField field) => new BGMTFieldCached<Rect>(field));
		Type2Provider.Add(typeof(BGFieldVector2), (BGField field) => new BGMTFieldCached<Vector2>(field));
		Type2Provider.Add(typeof(BGFieldVector3), (BGField field) => new BGMTFieldCached<Vector3>(field));
		Type2Provider.Add(typeof(BGFieldVector4), (BGField field) => new BGMTFieldCached<Vector4>(field));
		Type2Provider.Add(typeof(BGFieldColorNullable), (BGField field) => new BGMTFieldCached<Color?>(field));
		Type2Provider.Add(typeof(BGFieldQuaternionNullable), (BGField field) => new BGMTFieldCached<Quaternion?>(field));
		Type2Provider.Add(typeof(BGFieldVector2Nullable), (BGField field) => new BGMTFieldCached<Vector2?>(field));
		Type2Provider.Add(typeof(BGFieldVector3Nullable), (BGField field) => new BGMTFieldCached<Vector3?>(field));
		Type2Provider.Add(typeof(BGFieldVector4Nullable), (BGField field) => new BGMTFieldCached<Vector4?>(field));
		Type2Provider.Add(typeof(BGFieldRelationSingle), (BGField field) => new BGMTFieldRelationSingle(field));
		Type2Provider.Add(typeof(BGFieldRelationMultiple), (BGField field) => new BGMTFieldRelationMultiple(field));
		Type2Provider.Add(typeof(BGFieldManyRelationsSingle), (BGField field) => new BGMTFieldManyTablesRelationSingle(field));
		Type2Provider.Add(typeof(BGFieldManyRelationsMultiple), (BGField field) => new BGMTFieldManyTablesRelationMultiple(field));
		Type2Provider.Add(typeof(BGFieldNested), (BGField field) => new BGMTFieldNested(field));
		Type2Provider.Add(typeof(BGFieldEnum), (BGField field) => new BGMTFieldEnum(field));
		Type2Provider.Add(typeof(BGFieldEnumShort), (BGField field) => new BGMTFieldEnumShort(field));
		Type2Provider.Add(typeof(BGFieldEnumByte), (BGField field) => new BGMTFieldEnumByte(field));
	}

	public static bool IsSupported(Type fieldType)
	{
		return Type2Provider.ContainsKey(fieldType);
	}

	public static List<Type> GetAllFieldTypes()
	{
		List<Type> list = new List<Type>();
		foreach (KeyValuePair<Type, Func<BGField, BGMTField>> item in Type2Provider)
		{
			list.Add(item.Key);
		}
		return list;
	}

	public static BGMTField Create(BGMTMeta meta, BGField field)
	{
		if (!Type2Provider.TryGetValue(field.GetType(), out var value))
		{
			return null;
		}
		BGMTField bGMTField = value(field);
		bGMTField.Meta = meta;
		return bGMTField;
	}
}
