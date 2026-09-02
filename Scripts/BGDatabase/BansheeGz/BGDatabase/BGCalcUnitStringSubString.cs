namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/Substring")]
public class BGCalcUnitStringSubString : BGCalcUnit
{
	private BGCalcValueInput startIndex;

	private BGCalcValueInput a;

	public const int Code = 54;

	public override ushort TypeCode => 54;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		startIndex = ValueInput(BGCalcTypeCodeRegistry.Int, "start", "s");
		ValueOutput(BGCalcTypeCodeRegistry.String, "Substring(A)", "r", GetValue);
	}

	private string GetValue(BGCalcFlowI flow)
	{
		return flow.GetValue<string>(a).Substring(flow.GetValue<int>(startIndex));
	}
}
