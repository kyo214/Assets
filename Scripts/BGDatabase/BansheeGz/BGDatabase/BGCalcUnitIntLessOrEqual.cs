namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/_Comparisons/Int less or equal")]
public class BGCalcUnitIntLessOrEqual : BGCalcUnitIntABBool
{
	public const int Code = 9;

	public override ushort TypeCode => 9;

	protected override string OutputLabel => "A <= B";

	protected override bool Operation(int a, int b)
	{
		return a <= b;
	}
}
