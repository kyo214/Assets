using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List clear")]
public class BGCalcUnitListClear : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueOutput resultOutput;

	public const int Code = 66;

	public override ushort TypeCode => 66;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "b", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		if (value is Array)
		{
			flow.SetValue(resultOutput, Array.CreateInstance(value.GetType().GetElementType(), 0));
			return;
		}
		value.Clear();
		flow.SetValue(resultOutput, value);
	}
}
