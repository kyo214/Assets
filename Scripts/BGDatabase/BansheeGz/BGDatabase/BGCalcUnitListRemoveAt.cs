using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List removeAt")]
public class BGCalcUnitListRemoveAt : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueInput index;

	private BGCalcValueOutput resultOutput;

	public const int Code = 64;

	public override ushort TypeCode => 64;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		index = ValueInput(BGCalcTypeCodeRegistry.Int, "index", "b");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "d", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		int value2 = flow.GetValue<int>(index);
		if (value is Array)
		{
			ArrayList arrayList = new ArrayList(value);
			arrayList.RemoveAt(value2);
			flow.SetValue(resultOutput, arrayList.ToArray(value.GetType().GetElementType()));
		}
		else
		{
			value.RemoveAt(value2);
			flow.SetValue(resultOutput, value);
		}
	}
}
