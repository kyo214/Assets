using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGFieldReference : BGMetaReference
{
	[SerializeField]
	protected string fieldId;

	private BGField field;

	public string FieldId => fieldId;

	public BGField Field
	{
		get
		{
			return GetField();
		}
		set
		{
			SetField(value);
		}
	}

	public BGFieldReference()
	{
	}

	public BGFieldReference(BGField field)
	{
		SetField(field);
	}

	public BGField GetField()
	{
		if (field?.Meta != null && !field.Meta.IsDeleted && field.Id == BGId.Parse(fieldId))
		{
			return field;
		}
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			return null;
		}
		field = meta.GetField(BGId.Parse(fieldId), errorIfNotFound: false);
		return field;
	}

	public void SetField(BGField field)
	{
		if (field == null)
		{
			Reset();
			return;
		}
		BGId metaIdConstraint = MetaIdConstraint;
		if (!metaIdConstraint.IsEmpty && field.MetaId != metaIdConstraint)
		{
			string text = field.MetaId.ToString();
			BGId bGId = metaIdConstraint;
			throw new Exception("Can not assign a field, cause meta is wrong. IDs mismatch " + text + "!=" + bGId.ToString());
		}
		metaId = field.MetaId.ToString();
		fieldId = field.Id.ToString();
		this.field = field;
	}

	public override void Reset()
	{
		base.Reset();
		fieldId = null;
		field = null;
	}

	protected bool Equals(BGFieldReference other)
	{
		if (Equals((BGMetaReference)other))
		{
			return fieldId == other.fieldId;
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
		return Equals((BGFieldReference)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((fieldId != null) ? fieldId.GetHashCode() : 0);
	}
}
