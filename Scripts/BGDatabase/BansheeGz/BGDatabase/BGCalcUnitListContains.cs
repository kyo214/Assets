using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List contains")]
public class BGCalcUnitListContains : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 83;

	public override ushort TypeCode => 83;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "result", "r", Contains);
	}

	private bool Contains(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(a);
		object value2 = flow.GetValue(b);
		return value.Contains(value2);
	}
}
