namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatABBool : BGCalcUnitFloatAB<bool>
{
	protected override BGCalcTypeCode<bool> OutputCode => BGCalcTypeCodeRegistry.Bool;
}
