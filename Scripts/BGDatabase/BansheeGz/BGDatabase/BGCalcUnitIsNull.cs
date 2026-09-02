namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/IsNull")]
public class BGCalcUnitIsNull : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 50;

	public override ushort TypeCode => 50;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A is null", "r", IsNull);
	}

	private bool IsNull(BGCalcFlowI flow)
	{
		return flow.GetValue(a) == null;
	}
}
