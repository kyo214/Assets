namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Comparisons/Float equal")]
public class BGCalcUnitFloatEqual : BGCalcUnitFloatABBool
{
	public const int Code = 90;

	public override ushort TypeCode => 90;

	protected override string OutputLabel => "A = B";

	protected override bool Operation(float a, float b)
	{
		return a.Equals(b);
	}
}
