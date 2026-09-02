namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/_Comparisons/Int equal")]
public class BGCalcUnitIntEqual : BGCalcUnitIntABBool
{
	public const int Code = 89;

	public override ushort TypeCode => 89;

	protected override string OutputLabel => "A = B";

	protected override bool Operation(int a, int b)
	{
		return a == b;
	}
}
