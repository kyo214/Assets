namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Rad2Deg")]
public class BGCalcUnitFloatRad2Deg : BGCalcUnitFloatAFloat
{
	public const int Code = 42;

	public override ushort TypeCode => 42;

	protected override string OutputLabel => "Rad2Deg(A)";

	protected override float Operation(float a)
	{
		return 57.29578f * a;
	}
}
