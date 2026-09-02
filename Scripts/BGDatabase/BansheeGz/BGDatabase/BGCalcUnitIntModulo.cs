namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int modulo")]
public class BGCalcUnitIntModulo : BGCalcUnitIntABInt
{
	public const int Code = 18;

	public override ushort TypeCode => 18;

	protected override string OutputLabel => "A % B";

	protected override int Operation(int a, int b)
	{
		return a % b;
	}
}
