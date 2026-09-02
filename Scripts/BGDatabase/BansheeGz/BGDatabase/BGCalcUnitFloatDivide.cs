namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float divide")]
public class BGCalcUnitFloatDivide : BGCalcUnitFloatABFloat
{
	public const int Code = 27;

	public override ushort TypeCode => 27;

	protected override string OutputLabel => "A / B";

	protected override float Operation(float a, float b)
	{
		return a / b;
	}
}
