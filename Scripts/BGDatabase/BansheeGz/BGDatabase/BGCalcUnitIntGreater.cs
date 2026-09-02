namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/_Comparisons/Int greater")]
public class BGCalcUnitIntGreater : BGCalcUnitIntABBool
{
	public const int Code = 10;

	public override ushort TypeCode => 10;

	protected override string OutputLabel => "A > B";

	protected override bool Operation(int a, int b)
	{
		return a > b;
	}
}
