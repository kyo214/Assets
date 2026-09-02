using System;

namespace BansheeGz.BGDatabase;

public class BGCalcVar : BGCalcVarA<BGCalcVarsOwnerI>, BGObjectI
{
	private readonly BGId id;

	private string name;

	private bool isPublic;

	public BGId Id => id;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(name == value))
			{
				string text = BGMetaObject.CheckName(value);
				if (text != null)
				{
					throw new Exception(text);
				}
				BGCalcVarContainer vars = owner.GetVars();
				if (vars != null && vars.Variables.Count > 0 && owner.GetVars().GetVar(value) != null)
				{
					throw new Exception("Variable with such name already exists");
				}
				name = value;
				FireOnChange();
			}
		}
	}

	public bool IsPublic
	{
		get
		{
			return isPublic;
		}
		set
		{
			if (isPublic != value)
			{
				isPublic = value;
				FireOnChange();
			}
		}
	}

	public override void FireOnChange()
	{
		base.FireOnChange();
		owner.OnVarsChange();
	}

	protected BGCalcVar(BGCalcVarsOwnerI owner, BGId id, string name, BGCalcTypeCode typeCode)
		: base(owner, typeCode)
	{
		this.id = id;
		string text = BGMetaObject.CheckName(name);
		if (text != null)
		{
			throw new Exception(text);
		}
		this.name = name;
		owner.GetVars(createIfMissing: true).AddVar(this);
	}

	public override string ToString()
	{
		return name + " [" + ((value == null) ? "null" : value.ToString()) + "]";
	}

	protected bool Equals(BGCalcVar other)
	{
		if (Equals((BGCalcVarA)other))
		{
			return id.Equals(other.id);
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
		return Equals((BGCalcVar)obj);
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}

	public BGCalcVar CloneTo(BGCalcVarsOwnerI owner, bool cloneId = false, bool cloneValue = false)
	{
		BGCalcVar bGCalcVar = Create(owner, cloneId ? id : BGId.NewId, name, typeCode);
		if (cloneValue)
		{
			bGCalcVar.value = value;
			bGCalcVar.isPublic = isPublic;
		}
		return bGCalcVar;
	}

	public static BGCalcVar Create(BGCalcVarsOwnerI owner, string name, BGCalcTypeCode code)
	{
		return Create(owner, BGId.NewId, name, code);
	}

	public static BGCalcVar Create(BGCalcVarsOwnerI owner, BGId id, string name, BGCalcTypeCode code)
	{
		return new BGCalcVar(owner, id, name, code);
	}
}
