namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitIntABBool : BGCalcUnitIntAB<bool>
{
	protected override BGCalcTypeCode<bool> OutputCode => BGCalcTypeCodeRegistry.Bool;
}
