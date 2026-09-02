namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/Negate")]
public class BGCalcUnitOrNegate : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 5;

	public override ushort TypeCode => 5;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Bool, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "!A", "r", Operation);
	}

	private bool Operation(BGCalcFlowI flow)
	{
		return !flow.GetValue<bool>(a);
	}
}
