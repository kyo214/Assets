using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell set value", true)]
public class BGCalcUnitCellSetValue : BGCalcUnit2ControlsA
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	private BGCalcValueOutput c;

	public const int Code = 74;

	public override ushort TypeCode => 74;

	public override void Definition()
	{
		base.Definition();
		a = ValueInput(BGCalcTypeCodeRegistry.Cell, "cell", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Object, "value", "b");
		c = ValueOutput(BGCalcTypeCodeRegistry.Cell, "result", "c", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGCalcCell cell = GetCell(flow);
		if (cell == null)
		{
			throw new Exception("Can not retrieve field value, cause the cell is not set!");
		}
		object value = flow.GetValue(b);
		cell.Set(value);
		flow.SetValue(c, cell);
	}

	private BGCalcCell GetCell(BGCalcFlowI flow)
	{
		return flow.GetValue<BGCalcCell>(a);
	}
}
