namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Constants/NaN constant")]
public class BGCalcUnitFloatConstantNan : BGCalcUnitFloatConstant
{
	public const int Code = 87;

	public override ushort TypeCode => 87;

	protected override string OutputLabel => "Nan";

	protected override float Operation()
	{
		return float.NaN;
	}
}
