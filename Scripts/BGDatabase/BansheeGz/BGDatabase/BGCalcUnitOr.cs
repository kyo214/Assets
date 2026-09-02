namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/Or")]
public class BGCalcUnitOr : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 3;

	public override ushort TypeCode => 3;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Bool, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Bool, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A | B", "r", Operation);
	}

	private bool Operation(BGCalcFlowI flow)
	{
		if (!flow.GetValue<bool>(a))
		{
			return flow.GetValue<bool>(b);
		}
		return true;
	}
}
