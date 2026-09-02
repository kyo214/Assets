using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List add")]
public class BGCalcUnitListAdd : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listIn;

	private BGCalcValueInput obj;

	private BGCalcValueOutput listOut;

	public const int Code = 65;

	public override ushort TypeCode => 65;

	public override void Definition()
	{
		base.Definition();
		listIn = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		obj = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "b");
		listOut = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "c", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listIn);
		object value2 = flow.GetValue(obj);
		if (value is Array array)
		{
			flow.SetValue(listOut, new ArrayList(array) { obj }.ToArray(array.GetType().GetElementType()));
		}
		else
		{
			value.Add(value2);
			flow.SetValue(listOut, value);
		}
	}
}
