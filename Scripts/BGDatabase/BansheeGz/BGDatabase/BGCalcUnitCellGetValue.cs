using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell get value")]
public class BGCalcUnitCellGetValue : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 73;

	public override ushort TypeCode => 73;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Cell, "cell", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Object, "result", "b", Operation);
	}

	private object Operation(BGCalcFlowI flow)
	{
		BGCalcCell cell = GetCell(flow);
		if (cell == null)
		{
			throw new Exception("Can not retrieve cell value, cause the cell is not set!");
		}
		AddListeners(flow, cell.Field, cell.Entity);
		return cell.Get();
	}

	public static void AddListeners(BGCalcFlowI flow, BGField field, BGEntity entity)
	{
		if (flow.Context.Events != null && field != null && entity != null)
		{
			if (field is BGFieldNested bGFieldNested)
			{
				flow.Context.Events.AddOnCreate(bGFieldNested.NestedMeta);
				flow.Context.Events.AddOnDelete(bGFieldNested.NestedMeta);
			}
			else
			{
				flow.Context.Events.AddOnEdit(field, entity);
			}
		}
	}

	private BGCalcCell GetCell(BGCalcFlowI flow)
	{
		return flow.GetValue<BGCalcCell>(a);
	}
}
