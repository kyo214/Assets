using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/IndexOf")]
public class BGCalcUnitStringIndexOf : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 56;

	public override ushort TypeCode => 56;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.String, "value", "v");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "IndexOf(value)", "r", GetValue);
	}

	private int GetValue(BGCalcFlowI flow)
	{
		return flow.GetValue<string>(a).IndexOf(flow.GetValue<string>(b), StringComparison.Ordinal);
	}
}
