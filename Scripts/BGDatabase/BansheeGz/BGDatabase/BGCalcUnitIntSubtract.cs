namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int subtract")]
public class BGCalcUnitIntSubtract : BGCalcUnitIntABInt
{
	public const int Code = 14;

	public override ushort TypeCode => 14;

	protected override string OutputLabel => "A - B";

	protected override int Operation(int a, int b)
	{
		return a - b;
	}
}
