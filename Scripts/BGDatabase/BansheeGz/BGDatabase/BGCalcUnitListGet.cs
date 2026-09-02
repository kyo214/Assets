using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List get")]
public class BGCalcUnitListGet : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput index;

	public const int Code = 62;

	public override ushort TypeCode => 62;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		index = ValueInput(BGCalcTypeCodeRegistry.Int, "index", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Object, "value", "r", Get);
	}

	private object Get(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(a);
		int value2 = flow.GetValue<int>(index);
		return value[value2];
	}
}
