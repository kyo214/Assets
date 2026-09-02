using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List insert")]
public class BGCalcUnitListInsert : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueInput obj;

	private BGCalcValueInput index;

	private BGCalcValueOutput resultOutput;

	public const int Code = 67;

	public override ushort TypeCode => 67;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		obj = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "b");
		index = ValueInput(BGCalcTypeCodeRegistry.Int, "index", "c");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "d", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		object value2 = flow.GetValue(obj);
		int value3 = flow.GetValue<int>(index);
		if (value is Array)
		{
			ArrayList arrayList = new ArrayList(value);
			arrayList.Insert(value3, obj);
			flow.SetValue(resultOutput, arrayList.ToArray(value.GetType().GetElementType()));
		}
		else
		{
			value.Insert(value3, value2);
			flow.SetValue(resultOutput, value);
		}
	}
}
