namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float add")]
public class BGCalcUnitFloatAdd : BGCalcUnitFloatABFloat
{
	public const int Code = 23;

	public override ushort TypeCode => 23;

	protected override string OutputLabel => "A + B";

	protected override float Operation(float a, float b)
	{
		return a + b;
	}
}
