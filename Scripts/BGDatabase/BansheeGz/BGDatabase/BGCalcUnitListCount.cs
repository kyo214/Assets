using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List count")]
public class BGCalcUnitListCount : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 61;

	public override ushort TypeCode => 61;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "count", "r", Count);
	}

	private int Count(BGCalcFlowI flow)
	{
		return flow.GetValue<IList>(a).Count;
	}
}
