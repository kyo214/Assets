namespace BansheeGz.BGDatabase;

public class BGFieldPointer : BGMetaPointer
{
	private BGId fieldId;

	public BGId FieldId
	{
		get
		{
			return fieldId;
		}
		set
		{
			fieldId = value;
		}
	}

	public BGFieldPointer()
	{
	}

	public BGFieldPointer(BGId metaId, BGId fieldId)
		: base(metaId)
	{
		this.fieldId = fieldId;
	}

	public BGField GetField(BGRepo repo = null)
	{
		return GetMeta(repo)?.GetField(fieldId, errorIfNotFound: false);
	}

	public override void Reset()
	{
		base.Reset();
		fieldId = BGId.Empty;
	}

	protected bool Equals(BGFieldPointer other)
	{
		if (Equals((BGMetaPointer)other))
		{
			return fieldId.Equals(other.fieldId);
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
		return Equals((BGFieldPointer)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ fieldId.GetHashCode();
	}
}
