namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Comparisons/Float greater")]
public class BGCalcUnitFloatGreater : BGCalcUnitFloatABBool
{
	public const int Code = 21;

	public override ushort TypeCode => 21;

	protected override string OutputLabel => "A > B";

	protected override bool Operation(float a, float b)
	{
		return a > b;
	}
}
