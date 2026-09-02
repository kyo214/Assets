namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int add")]
public class BGCalcUnitIntAdd : BGCalcUnitIntABInt
{
	public const int Code = 12;

	public override ushort TypeCode => 12;

	protected override string OutputLabel => "A + B";

	protected override int Operation(int a, int b)
	{
		return a + b;
	}
}
