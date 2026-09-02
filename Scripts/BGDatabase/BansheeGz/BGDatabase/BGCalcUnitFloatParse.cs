using System.Globalization;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float parse")]
public class BGCalcUnitFloatParse : BGCalcUnit
{
	public const int Code = 86;

	private BGCalcValueInput a;

	public override ushort TypeCode => 86;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Float, "Parse(A)", "r", (BGCalcFlowI flow) => float.Parse(flow.GetValue<string>(a), CultureInfo.InvariantCulture));
	}
}
