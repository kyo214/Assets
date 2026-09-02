using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGMetaReference
{
	[SerializeField]
	protected string metaId;

	public virtual BGId MetaIdConstraint => BGId.Empty;

	public BGMetaEntity Meta
	{
		get
		{
			BGId metaIdConstraint = MetaIdConstraint;
			return BGRepo.I.GetMeta(metaIdConstraint.IsEmpty ? BGId.Parse(metaId) : metaIdConstraint);
		}
		set
		{
			metaId = value?.Id.ToString();
		}
	}

	public string MetaId => metaId;

	public virtual void Reset()
	{
		metaId = null;
	}

	protected bool Equals(BGMetaReference other)
	{
		return metaId == other.metaId;
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
		return Equals((BGMetaReference)obj);
	}

	public override int GetHashCode()
	{
		if (metaId == null)
		{
			return 0;
		}
		return metaId.GetHashCode();
	}
}
