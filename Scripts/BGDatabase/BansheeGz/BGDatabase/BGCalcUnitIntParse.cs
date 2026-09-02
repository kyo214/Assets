using System.Globalization;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int parse")]
public class BGCalcUnitIntParse : BGCalcUnit
{
	public const int Code = 85;

	private BGCalcValueInput a;

	public override ushort TypeCode => 85;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "Parse(A)", "r", (BGCalcFlowI flow) => int.Parse(flow.GetValue<string>(a), CultureInfo.InvariantCulture));
	}
}
