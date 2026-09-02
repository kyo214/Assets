namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Constants/NegativeInfinity constant")]
public class BGCalcUnitFloatConstantInfinityNegative : BGCalcUnitFloatConstant
{
	public const int Code = 45;

	public override ushort TypeCode => 45;

	protected override string OutputLabel => "NegativeInfinity";

	protected override float Operation()
	{
		return float.NegativeInfinity;
	}
}
