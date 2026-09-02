namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Comparisons/Float greater or equal")]
public class BGCalcUnitFloatGreaterOrEqual : BGCalcUnitFloatABBool
{
	public const int Code = 22;

	public override ushort TypeCode => 22;

	protected override string OutputLabel => "A >= B";

	protected override bool Operation(float a, float b)
	{
		return a >= b;
	}
}
