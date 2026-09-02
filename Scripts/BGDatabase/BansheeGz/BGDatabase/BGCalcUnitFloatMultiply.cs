namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float multiply")]
public class BGCalcUnitFloatMultiply : BGCalcUnitFloatABFloat
{
	public const int Code = 26;

	public override ushort TypeCode => 26;

	protected override string OutputLabel => "A * B";

	protected override float Operation(float a, float b)
	{
		return a * b;
	}
}
