using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell get value2")]
public class BGCalcUnitCellGetValue2 : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 75;

	public override ushort TypeCode => 75;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Object, "result", "c", Operation);
		ValueOutput(BGCalcTypeCodeRegistry.Cell, "cell", "d", GetCell);
	}

	private object Operation(BGCalcFlowI flow)
	{
		BGEntity entity = GetEntity(flow);
		if (entity == null)
		{
			throw new Exception("Can not retrieve cell value, cause the entity is not set!");
		}
		BGField field = GetField(flow);
		if (field != null)
		{
			if (field is BGFieldCalcI)
			{
				throw new Exception("Can not get a value cause field is calculated field. To get calculated field value from graph, use 'Call calculated cell' unit!");
			}
			if (field.MetaId != entity.MetaId)
			{
				throw new Exception("Can not retrieve cell value, cause entity and field belong to different tables!");
			}
			BGCalcUnitCellGetValue.AddListeners(flow, field, entity);
			return field.GetValue(entity.Index);
		}
		throw new Exception("Can not retrieve cell value, cause the field is not set!");
	}

	private BGCalcCell GetCell(BGCalcFlowI flow)
	{
		return new BGCalcCell(GetField(flow), GetEntity(flow));
	}

	private BGField GetField(BGCalcFlowI flow)
	{
		return flow.GetValue<BGField>(b);
	}

	private BGEntity GetEntity(BGCalcFlowI flow)
	{
		return flow.GetValue<BGEntity>(a);
	}
}
