using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List indexOf")]
public class BGCalcUnitListIndexOf : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 68;

	public override ushort TypeCode => 68;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "index", "r", IndexOf);
	}

	private int IndexOf(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(a);
		object value2 = flow.GetValue(b);
		return value.IndexOf(value2);
	}
}
