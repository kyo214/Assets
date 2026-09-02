using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeField : BGCalcTypeCode<BGField>
{
	[Serializable]
	private class JsonValue
	{
		public string MetaId;

		public string FieldId;
	}

	public const byte Code = 14;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 14;

	public override object DefaultValue => null;

	public override string Name => "field";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		BGField bGField = (BGField)value;
		if (bGField == null)
		{
			writer.AddId(BGId.Empty);
			writer.AddId(BGId.Empty);
		}
		else
		{
			writer.AddId(bGField.MetaId);
			writer.AddId(bGField.Id);
		}
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		BGId id = reader.ReadId();
		BGId fieldId = reader.ReadId();
		if (!id.IsEmpty && !fieldId.IsEmpty)
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			if (meta != null)
			{
				return meta.GetField(fieldId, errorIfNotFound: false);
			}
		}
		return null;
	}

	public override string ValueToString(object value)
	{
		JsonValue jsonValue = new JsonValue();
		BGField bGField = (BGField)value;
		if (bGField != null)
		{
			jsonValue.MetaId = bGField.MetaId.ToString();
			jsonValue.FieldId = bGField.Id.ToString();
		}
		return JsonUtility.ToJson(jsonValue);
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		JsonValue jsonValue = JsonUtility.FromJson<JsonValue>(value);
		if (BGId.TryParse(jsonValue.MetaId, out var id) && BGId.TryParse(jsonValue.FieldId, out var id2))
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			if (meta != null)
			{
				return meta.GetField(id2, errorIfNotFound: false);
			}
		}
		return null;
	}
}
