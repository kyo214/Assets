using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List remove")]
public class BGCalcUnitListRemove : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueInput allInput;

	private BGCalcValueInput objInput;

	private BGCalcValueOutput resultOutput;

	public const int Code = 92;

	public override ushort TypeCode => 92;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		objInput = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "b");
		allInput = ValueInput(BGCalcTypeCodeRegistry.Bool, "all?", "c");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "d", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		object value2 = flow.GetValue<object>(objInput);
		bool value3 = flow.GetValue<bool>(allInput);
		if (value is Array)
		{
			ArrayList arrayList = new ArrayList(value);
			if (value3)
			{
				for (int num = arrayList.IndexOf(value2); num != -1; num = arrayList.IndexOf(value2))
				{
					arrayList.RemoveAt(num);
				}
			}
			else
			{
				arrayList.Remove(value2);
			}
			flow.SetValue(resultOutput, arrayList.ToArray(value.GetType().GetElementType()));
			return;
		}
		if (value3)
		{
			for (int num2 = value.IndexOf(value2); num2 != -1; num2 = value.IndexOf(value2))
			{
				value.RemoveAt(num2);
			}
		}
		else
		{
			value.Remove(value2);
		}
		flow.SetValue(resultOutput, value);
	}
}
