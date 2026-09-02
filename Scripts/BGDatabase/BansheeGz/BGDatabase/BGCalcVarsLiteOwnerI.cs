namespace BansheeGz.BGDatabase;

public interface BGCalcVarsLiteOwnerI : BGCalcVarsOwnerBaseI
{
	BGCalcVarLiteContainer GetVars(bool createIfMissing = false);
}
