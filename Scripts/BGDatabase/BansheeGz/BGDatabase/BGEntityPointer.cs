namespace BansheeGz.BGDatabase;

public class BGEntityPointer : BGMetaPointer
{
	private readonly BGId entityId;

	public BGId EntityId => entityId;

	public BGEntityPointer(BGId metaId, BGId entityId)
		: base(metaId)
	{
		this.entityId = entityId;
	}

	public BGEntity GetEntity(BGRepo repo = null)
	{
		return GetMeta(repo)?.GetEntity(entityId);
	}

	protected bool Equals(BGEntityPointer other)
	{
		if (Equals((BGMetaPointer)other))
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
		return Equals((BGEntityPointer)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ entityId.GetHashCode();
	}
}
