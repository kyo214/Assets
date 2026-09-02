namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/Bool parse")]
public class BGCalcUnitBoolParse : BGCalcUnit
{
	public const int Code = 88;

	private BGCalcValueInput a;

	public override ushort TypeCode => 88;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "Parse(A)", "r", (BGCalcFlowI flow) => bool.Parse(flow.GetValue<string>(a)));
	}
}
