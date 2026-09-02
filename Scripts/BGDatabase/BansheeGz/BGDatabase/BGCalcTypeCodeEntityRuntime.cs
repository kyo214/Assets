using System;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeEntityRuntime : BGCalcTypeCode<BGEntity>, BGCalcTypeCodeStateful
{
	private BGId metaId;

	private BGMetaEntity meta;

	public const byte Code = 8;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 8;

	public override object DefaultValue => null;

	public override string TypeTitle => Name + ((Meta == null) ? "" : (" [" + Meta.Name + "]"));

	public BGMetaEntity Meta
	{
		get
		{
			if (meta != null && !meta.IsDeleted)
			{
				return meta;
			}
			meta = BGRepo.I.GetMeta(metaId);
			return meta;
		}
	}

	public override string Name => "row";

	internal BGCalcTypeCodeEntityRuntime()
	{
	}

	public BGCalcTypeCodeEntityRuntime(BGMetaEntity meta)
	{
		if (meta == null)
		{
			throw new Exception("meta can not be null!");
		}
		metaId = meta.Id;
		this.meta = meta;
	}

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddId(((BGEntity)value)?.Id ?? BGId.Empty);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		BGId entityId = reader.ReadId();
		if (entityId.IsEmpty)
		{
			return null;
		}
		return Meta?.GetEntity(entityId);
	}

	public override string ValueToString(object value)
	{
		return ((BGEntity)value)?.Id.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		BGMetaEntity bGMetaEntity = Meta;
		if (bGMetaEntity == null)
		{
			return null;
		}
		if (!BGId.TryParse(value, out var id))
		{
			return null;
		}
		return bGMetaEntity.GetEntity(id);
	}

	protected bool Equals(BGCalcTypeCodeEntityRuntime other)
	{
		if (Equals((BGCalcTypeCode)other))
		{
			return metaId.Equals(other.metaId);
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
		return Equals((BGCalcTypeCodeEntityRuntime)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ metaId.GetHashCode();
	}

	public void ReadState(BGBinaryReader reader)
	{
		metaId = reader.ReadId();
		meta = null;
	}

	public void WriteState(BGBinaryWriter writer)
	{
		writer.AddId(metaId);
	}

	public void ReadState(string state)
	{
		if (BGId.TryParse(state, out var id))
		{
			metaId = id;
		}
	}

	public string WriteState()
	{
		return metaId.ToString();
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		return otherCode is BGCalcTypeCodeEntity;
	}

	public override object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		if (otherCode == null)
		{
			return value;
		}
		if (value == null)
		{
			return null;
		}
		if (otherCode is BGCalcTypeCodeEntity)
		{
			BGEntity bGEntity = (BGEntity)value;
			if (metaId != bGEntity.MetaId)
			{
				throw new Exception($"Can not convert an entity, cause it seems to be from another table! metaId mismatch {metaId}!={bGEntity.MetaId}");
			}
			return value;
		}
		return value;
	}
}
