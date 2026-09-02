namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/Or exclusive")]
public class BGCalcUnitOrExclusive : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 4;

	public override ushort TypeCode => 4;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Bool, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Bool, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A ^ B", "r", Operation);
	}

	private bool Operation(BGCalcFlowI flow)
	{
		return flow.GetValue<bool>(a) ^ flow.GetValue<bool>(b);
	}
}
