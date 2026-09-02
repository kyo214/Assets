namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Comparisons/Float less or equal")]
public class BGCalcUnitFloatLessOrEqual : BGCalcUnitFloatABBool
{
	public const int Code = 20;

	public override ushort TypeCode => 20;

	protected override string OutputLabel => "A <= B";

	protected override bool Operation(float a, float b)
	{
		return a <= b;
	}
}
