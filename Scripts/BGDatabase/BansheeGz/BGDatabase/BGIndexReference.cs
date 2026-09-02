using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGIndexReference : BGMetaReference
{
	[SerializeField]
	private string indexId;

	private BGIndex index;

	public string IndexId => indexId;

	public BGIndex Index
	{
		get
		{
			return GetIndex();
		}
		set
		{
			SetIndex(value);
		}
	}

	public BGIndexReference()
	{
	}

	public BGIndexReference(BGIndex index)
	{
		SetIndex(index);
	}

	public BGIndex GetIndex()
	{
		if (index?.Meta != null && !index.Meta.IsDeleted && index.Id == BGId.Parse(indexId))
		{
			return index;
		}
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			return null;
		}
		index = meta.GetIndex(BGId.Parse(indexId), errorIfNotFound: false);
		return index;
	}

	public void SetIndex(BGIndex index)
	{
		if (index == null)
		{
			Reset();
			return;
		}
		BGId metaIdConstraint = MetaIdConstraint;
		if (!metaIdConstraint.IsEmpty && index.MetaId != metaIdConstraint)
		{
			string text = index.MetaId.ToString();
			BGId bGId = metaIdConstraint;
			throw new Exception("Can not assign an index, cause meta is wrong. IDs mismatch " + text + "!=" + bGId.ToString());
		}
		metaId = index.MetaId.ToString();
		indexId = index.Id.ToString();
		this.index = index;
	}

	public override void Reset()
	{
		base.Reset();
		indexId = null;
		index = null;
	}

	protected bool Equals(BGIndexReference other)
	{
		if (Equals((BGMetaReference)other))
		{
			return indexId == other.indexId;
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
		return Equals((BGIndexReference)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((indexId != null) ? indexId.GetHashCode() : 0);
	}
}
