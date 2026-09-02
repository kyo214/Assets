using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List set")]
public class BGCalcUnitListSet : BGCalcUnit2ControlsA
{
	private BGCalcValueInput listInput;

	private BGCalcValueInput indexInput;

	private BGCalcValueInput objInput;

	private BGCalcValueOutput resultOutput;

	public const int Code = 91;

	public override ushort TypeCode => 91;

	public override void Definition()
	{
		base.Definition();
		listInput = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		indexInput = ValueInput(BGCalcTypeCodeRegistry.Int, "index", "b");
		objInput = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "c");
		resultOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "result", "d", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(listInput);
		int value2 = flow.GetValue<int>(indexInput);
		object value3 = flow.GetValue<object>(objInput);
		value[value2] = value3;
		flow.SetValue(resultOutput, value);
	}
}
