namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/_Comparisons/Int less")]
public class BGCalcUnitIntLess : BGCalcUnitIntABBool
{
	public const int Code = 8;

	public override ushort TypeCode => 8;

	protected override string OutputLabel => "A < B";

	protected override bool Operation(int a, int b)
	{
		return a < b;
	}
}
