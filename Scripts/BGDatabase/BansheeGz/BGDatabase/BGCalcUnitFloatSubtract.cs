namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float subtract")]
public class BGCalcUnitFloatSubtract : BGCalcUnitFloatABFloat
{
	public const int Code = 24;

	public override ushort TypeCode => 24;

	protected override string OutputLabel => "A - B";

	protected override float Operation(float a, float b)
	{
		return a - b;
	}
}
