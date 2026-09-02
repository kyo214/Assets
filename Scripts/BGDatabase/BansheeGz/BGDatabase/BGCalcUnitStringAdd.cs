namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/String add")]
public class BGCalcUnitStringAdd : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 57;

	public override ushort TypeCode => 57;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.String, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.String, "A + B", "r", GetValue);
	}

	private string GetValue(BGCalcFlowI flow)
	{
		return flow.GetValue<string>(a) + flow.GetValue<string>(b);
	}
}
