namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Comparisons/Float less")]
public class BGCalcUnitFloatLess : BGCalcUnitFloatABBool
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 19;

	public override ushort TypeCode => 19;

	protected override string OutputLabel => "A < B";

	protected override bool Operation(float a, float b)
	{
		return a < b;
	}
}
