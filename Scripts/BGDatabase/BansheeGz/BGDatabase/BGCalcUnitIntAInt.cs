namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitIntAInt : BGCalcUnitIntA<int>
{
	protected override BGCalcTypeCode<int> OutputCode => BGCalcTypeCodeRegistry.Int;
}
