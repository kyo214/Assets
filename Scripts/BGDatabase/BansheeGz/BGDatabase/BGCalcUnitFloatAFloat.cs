namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatAFloat : BGCalcUnitFloatA<float>
{
	protected override BGCalcTypeCode<float> OutputCode => BGCalcTypeCodeRegistry.Float;
}
