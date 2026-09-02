using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeCell : BGCalcTypeCode<BGCalcCell>
{
	[Serializable]
	private class JsonCell
	{
		public string MetaId;

		public string FieldId;

		public string EntityId;
	}

	public const byte Code = 16;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 16;

	public override object DefaultValue => null;

	public override string Name => "cell";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		BGCalcCell bGCalcCell = (BGCalcCell)value;
		if (bGCalcCell?.Field == null || bGCalcCell.Entity == null)
		{
			writer.AddId(BGId.Empty);
			writer.AddId(BGId.Empty);
			writer.AddId(BGId.Empty);
		}
		else
		{
			writer.AddId(bGCalcCell.Field.MetaId);
			writer.AddId(bGCalcCell.Field.Id);
			writer.AddId(bGCalcCell.Entity.Id);
		}
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		BGId id = reader.ReadId();
		BGId fieldId = reader.ReadId();
		BGId entityId = reader.ReadId();
		if (!id.IsEmpty && !fieldId.IsEmpty && !entityId.IsEmpty)
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			BGField bGField = meta?.GetField(fieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				BGEntity entity = meta.GetEntity(entityId);
				if (entity != null)
				{
					return new BGCalcCell(bGField, entity);
				}
			}
		}
		return null;
	}

	public override string ValueToString(object value)
	{
		BGCalcCell bGCalcCell = (BGCalcCell)value;
		if (bGCalcCell?.Field != null && bGCalcCell.Entity != null)
		{
			JsonCell obj = new JsonCell
			{
				MetaId = bGCalcCell.Field.MetaId.ToString(),
				FieldId = bGCalcCell.Field.Id.ToString(),
				EntityId = bGCalcCell.Entity.Id.ToString()
			};
			return JsonUtility.ToJson(obj);
		}
		return null;
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		JsonCell jsonCell = JsonUtility.FromJson<JsonCell>(value);
		if (jsonCell != null && BGId.TryParse(jsonCell.MetaId, out var id) && BGId.TryParse(jsonCell.FieldId, out var id2) && BGId.TryParse(jsonCell.EntityId, out var id3))
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(id);
			BGField bGField = meta?.GetField(id2, errorIfNotFound: false);
			if (bGField != null)
			{
				BGEntity entity = meta.GetEntity(id3);
				if (entity != null)
				{
					return new BGCalcCell(bGField, entity);
				}
			}
		}
		return null;
	}
}
