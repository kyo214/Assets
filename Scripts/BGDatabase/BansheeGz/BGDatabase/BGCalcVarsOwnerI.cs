namespace BansheeGz.BGDatabase;

public interface BGCalcVarsOwnerI : BGCalcVarsOwnerBaseI
{
	BGCalcVarContainer GetVars(bool createIfMissing = false);
}
