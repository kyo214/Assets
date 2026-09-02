namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float modulo")]
public class BGCalcUnitFloatModulo : BGCalcUnitFloatABFloat
{
	public const int Code = 28;

	public override ushort TypeCode => 28;

	protected override string OutputLabel => "A % B";

	protected override float Operation(float a, float b)
	{
		return a % b;
	}
}
