using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/Split")]
public class BGCalcUnitStringSplit : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	private BGCalcValueInput removeEmpty;

	public const int Code = 59;

	public override ushort TypeCode => 59;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.String, "separator", "b");
		removeEmpty = ValueInput(BGCalcTypeCodeRegistry.Bool, "remove empty", "c");
		ValueOutput(BGCalcTypeCodeRegistry.List, "Split(A,B)", "r", GetValue);
	}

	private IList GetValue(BGCalcFlowI flow)
	{
		string value = flow.GetValue<string>(a);
		string[] separator = new string[1] { flow.GetValue<string>(b) };
		bool value2 = flow.GetValue<bool>(removeEmpty);
		return value.Split(separator, value2 ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
	}
}
