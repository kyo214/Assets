namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitIntABInt : BGCalcUnitIntAB<int>
{
	protected override BGCalcTypeCode<int> OutputCode => BGCalcTypeCodeRegistry.Int;
}
