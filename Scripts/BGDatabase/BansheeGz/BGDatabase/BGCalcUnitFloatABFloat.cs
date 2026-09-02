namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatABFloat : BGCalcUnitFloatAB<float>
{
	protected override BGCalcTypeCode<float> OutputCode => BGCalcTypeCodeRegistry.Float;
}
