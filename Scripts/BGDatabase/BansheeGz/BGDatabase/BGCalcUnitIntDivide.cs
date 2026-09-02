namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int divide")]
public class BGCalcUnitIntDivide : BGCalcUnitIntABInt
{
	public const int Code = 17;

	public override ushort TypeCode => 17;

	protected override string OutputLabel => "A / B";

	protected override int Operation(int a, int b)
	{
		return a / b;
	}
}
