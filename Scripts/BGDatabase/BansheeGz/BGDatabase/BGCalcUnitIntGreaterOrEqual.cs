namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/_Comparisons/Int greater or equal")]
public class BGCalcUnitIntGreaterOrEqual : BGCalcUnitIntABBool
{
	public const int Code = 11;

	public override ushort TypeCode => 11;

	protected override string OutputLabel => "A >= B";

	protected override bool Operation(int a, int b)
	{
		return a >= b;
	}
}
