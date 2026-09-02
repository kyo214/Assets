namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatAInt : BGCalcUnitFloatA<int>
{
	protected override BGCalcTypeCode<int> OutputCode => BGCalcTypeCodeRegistry.Int;
}
