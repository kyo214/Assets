namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/And")]
public class BGCalcUnitAnd : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 2;

	public override ushort TypeCode => 2;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Bool, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Bool, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A & B", "r", Operation);
	}

	private bool Operation(BGCalcFlowI flow)
	{
		if (flow.GetValue<bool>(a))
		{
			return flow.GetValue<bool>(b);
		}
		return false;
	}
}
