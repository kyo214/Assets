namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatABInt : BGCalcUnitFloatAB<int>
{
	protected override BGCalcTypeCode<int> OutputCode => BGCalcTypeCodeRegistry.Int;
}
