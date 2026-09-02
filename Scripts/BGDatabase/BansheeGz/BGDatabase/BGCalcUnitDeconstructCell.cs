using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell deconstruct")]
public class BGCalcUnitDeconstructCell : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 78;

	public override ushort TypeCode => 78;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Cell, "cell", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Entity, "entity", "b", GetEntity);
		ValueOutput(BGCalcTypeCodeRegistry.Field, "field", "c", GetField);
	}

	private BGCalcCell GetCell(BGCalcFlowI flow)
	{
		BGCalcCell value = flow.GetValue<BGCalcCell>(a);
		if (value == null)
		{
			throw new Exception("Can not deconstruct a cell, cause the cell is not set!");
		}
		return value;
	}

	private BGField GetField(BGCalcFlowI flow)
	{
		return GetCell(flow).Field;
	}

	private BGEntity GetEntity(BGCalcFlowI flow)
	{
		return GetCell(flow).Entity;
	}
}
