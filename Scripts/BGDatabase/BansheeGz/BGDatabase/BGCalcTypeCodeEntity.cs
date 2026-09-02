using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeEntity : BGCalcTypeCode<BGEntity>
{
	[Serializable]
	private class JsonValue
	{
		public string MetaId;

		public string EntityId;
	}

	public const byte Code = 15;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 15;

	public override object DefaultValue => null;

	public override string Name => "entity";

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		return otherCode is BGCalcTypeCodeEntityRuntime;
	}

	public override object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		if (otherCode == null)
		{
			return value;
		}
		BGCalcTypeCodeEntityRuntime bGCalcTypeCodeEntityRuntime = otherCode as BGCalcTypeCodeEntityRuntime;
		return value;
	}

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		BGEntity bGEntity = (BGEntity)value;
		if (bGEntity == null)
		{
			writer.AddId(BGId.Empty);
			writer.AddId(BGId.Empty);
		}
		else
		{
			writer.AddId(bGEntity.MetaId);
			writer.AddId(bGEntity.Id);
		}
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		BGId id = reader.ReadId();
		BGId entityId = reader.ReadId();
		if (!id.IsEmpty && !entityId.IsEmpty)
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			if (meta != null)
			{
				return meta.GetEntity(entityId);
			}
		}
		return null;
	}

	public override string ValueToString(object value)
	{
		JsonValue jsonValue = new JsonValue();
		BGEntity bGEntity = (BGEntity)value;
		if (bGEntity != null)
		{
			jsonValue.MetaId = bGEntity.MetaId.ToString();
			jsonValue.EntityId = bGEntity.Id.ToString();
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
		if (BGId.TryParse(jsonValue.MetaId, out var id) && BGId.TryParse(jsonValue.EntityId, out var id2))
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			if (meta != null)
			{
				return meta.GetEntity(id2);
			}
		}
		return null;
	}
}
