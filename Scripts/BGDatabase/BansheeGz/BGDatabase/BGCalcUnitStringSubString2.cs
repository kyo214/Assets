namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/Substring2")]
public class BGCalcUnitStringSubString2 : BGCalcUnit
{
	private BGCalcValueInput startIndex;

	private BGCalcValueInput length;

	private BGCalcValueInput a;

	public const int Code = 55;

	public override ushort TypeCode => 55;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		startIndex = ValueInput(BGCalcTypeCodeRegistry.Int, "start", "s");
		length = ValueInput(BGCalcTypeCodeRegistry.Int, "length", "l");
		ValueOutput(BGCalcTypeCodeRegistry.String, "Substring(A)", "r", GetValue);
	}

	private string GetValue(BGCalcFlowI flow)
	{
		return flow.GetValue<string>(a).Substring(flow.GetValue<int>(startIndex), flow.GetValue<int>(length));
	}
}
