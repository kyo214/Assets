using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List addRange")]
public class BGCalcUnitListAddRange : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueInput listToAdd;

	private BGCalcValueOutput resultOutput;

	public const int Code = 82;

	public override ushort TypeCode => 82;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		listToAdd = ValueInput(BGCalcTypeCodeRegistry.Object, "list2", "b");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "c", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		IList value2 = flow.GetValue<IList>(listToAdd);
		if (value is Array array)
		{
			ArrayList arrayList = new ArrayList(value);
			arrayList.AddRange(value2);
			flow.SetValue(resultOutput, arrayList.ToArray(array.GetType().GetElementType()));
			return;
		}
		for (int i = 0; i < value2.Count; i++)
		{
			value.Add(value2[i]);
		}
		flow.SetValue(resultOutput, value);
	}
}
