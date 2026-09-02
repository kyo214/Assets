using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGKeyReference : BGMetaReference
{
	[SerializeField]
	private string keyId;

	private BGKey key;

	public BGKey Key
	{
		get
		{
			return GetKey();
		}
		set
		{
			SetKey(value);
		}
	}

	public string KeyId => keyId;

	public BGKeyReference()
	{
	}

	public BGKeyReference(BGKey key)
	{
		SetKey(key);
	}

	public BGKey GetKey()
	{
		if (key?.Meta != null && !key.Meta.IsDeleted && key.Id == BGId.Parse(keyId))
		{
			return key;
		}
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			return null;
		}
		key = meta.GetKey(BGId.Parse(keyId), errorIfNotFound: false);
		return key;
	}

	public void SetKey(BGKey key)
	{
		if (key == null)
		{
			Reset();
			return;
		}
		BGId metaIdConstraint = MetaIdConstraint;
		if (!metaIdConstraint.IsEmpty && key.MetaId != metaIdConstraint)
		{
			string text = key.MetaId.ToString();
			BGId bGId = metaIdConstraint;
			throw new Exception("Can not assign a key, cause meta is wrong. IDs mismatch " + text + "!=" + bGId.ToString());
		}
		metaId = key.MetaId.ToString();
		keyId = key.Id.ToString();
		this.key = key;
	}

	public override void Reset()
	{
		base.Reset();
		keyId = null;
		key = null;
	}

	protected bool Equals(BGKeyReference other)
	{
		if (Equals((BGMetaReference)other))
		{
			return keyId == other.keyId;
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
		return Equals((BGKeyReference)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((keyId != null) ? keyId.GetHashCode() : 0);
	}
}
