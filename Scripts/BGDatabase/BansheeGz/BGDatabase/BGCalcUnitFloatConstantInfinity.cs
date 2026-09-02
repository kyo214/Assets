namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Constants/Infinity constant")]
public class BGCalcUnitFloatConstantInfinity : BGCalcUnitFloatConstant
{
	public const int Code = 44;

	public override ushort TypeCode => 44;

	protected override string OutputLabel => "Infinity";

	protected override float Operation()
	{
		return float.PositiveInfinity;
	}
}
