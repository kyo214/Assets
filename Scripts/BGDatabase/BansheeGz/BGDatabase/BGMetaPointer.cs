namespace BansheeGz.BGDatabase;

public class BGMetaPointer
{
	private BGId metaId;

	public BGId MetaId
	{
		get
		{
			return metaId;
		}
		set
		{
			metaId = value;
		}
	}

	public BGMetaPointer()
	{
	}

	public BGMetaPointer(BGId metaId)
	{
		this.metaId = metaId;
	}

	public BGMetaEntity GetMeta(BGRepo repo = null)
	{
		repo = repo ?? BGRepo.I;
		return repo.GetMeta(metaId);
	}

	public virtual void Reset()
	{
		metaId = BGId.Empty;
	}

	protected bool Equals(BGMetaPointer other)
	{
		return metaId.Equals(other.metaId);
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
		return Equals((BGMetaPointer)obj);
	}

	public override int GetHashCode()
	{
		return metaId.GetHashCode();
	}
}
