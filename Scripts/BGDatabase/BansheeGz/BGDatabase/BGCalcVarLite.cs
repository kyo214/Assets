namespace BansheeGz.BGDatabase;

public class BGCalcVarLite : BGCalcVarA<BGCalcVarsLiteOwnerI>
{
	private readonly byte id;

	public byte Id => id;

	protected BGCalcVarLite(BGCalcVarsLiteOwnerI owner, BGCalcTypeCode typeCode, byte id)
		: base(owner, typeCode)
	{
		this.id = id;
		owner.GetVars(createIfMissing: true).AddVar(this);
	}

	protected bool Equals(BGCalcVarLite other)
	{
		if (Equals((BGCalcVarA)other))
		{
			return id == other.id;
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
		return Equals((BGCalcVarLite)obj);
	}

	public override int GetHashCode()
	{
		int num = base.GetHashCode() * 397;
		byte b = id;
		return num ^ b.GetHashCode();
	}

	public BGCalcVarLite CloneTo(BGCalcVarsLiteOwnerI owner, bool cloneValue = false)
	{
		BGCalcVarLite bGCalcVarLite = Create(owner, id, typeCode);
		if (cloneValue)
		{
			bGCalcVarLite.value = value;
		}
		return bGCalcVarLite;
	}

	public override void FireOnChange()
	{
		base.FireOnChange();
		owner.OnVarsChange();
	}

	public static BGCalcVarLite Create(BGCalcVarsLiteOwnerI owner, byte id, BGCalcTypeCode code)
	{
		return new BGCalcVarLite(owner, code, id);
	}
}
