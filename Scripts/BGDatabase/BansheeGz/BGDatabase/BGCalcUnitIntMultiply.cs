namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int multiply")]
public class BGCalcUnitIntMultiply : BGCalcUnitIntABInt
{
	public const int Code = 16;

	public override ushort TypeCode => 16;

	protected override string OutputLabel => "A * B";

	protected override int Operation(int a, int b)
	{
		return a * b;
	}
}
