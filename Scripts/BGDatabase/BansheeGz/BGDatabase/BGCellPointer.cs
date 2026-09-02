namespace BansheeGz.BGDatabase;

public class BGCellPointer : BGFieldPointer
{
	private BGId entityId;

	public BGId EntityId
	{
		get
		{
			return entityId;
		}
		set
		{
			entityId = value;
		}
	}

	public BGCellPointer()
	{
	}

	public BGCellPointer(BGId metaId, BGId fieldId, BGId entityId)
		: base(metaId, fieldId)
	{
		this.entityId = entityId;
	}

	public BGEntity GetEntity(BGRepo repo = null)
	{
		return GetMeta(repo)?.GetEntity(entityId);
	}

	public override void Reset()
	{
		base.Reset();
		entityId = BGId.Empty;
	}

	public void Reset(BGField field, BGEntity entity)
	{
		base.MetaId = field.MetaId;
		base.FieldId = field.Id;
		EntityId = entity.Id;
	}

	protected bool Equals(BGCellPointer other)
	{
		if (Equals((BGFieldPointer)other))
		{
			return entityId.Equals(other.entityId);
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
		return Equals((BGCellPointer)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ entityId.GetHashCode();
	}
}
