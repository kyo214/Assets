using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell set value2", true)]
public class BGCalcUnitCellSetValue2 : BGCalcUnit2ControlsA
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	private BGCalcValueInput c;

	private BGCalcValueOutput d;

	public const int Code = 76;

	public override ushort TypeCode => 76;

	public override void Definition()
	{
		base.Definition();
		a = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "b");
		c = ValueInput(BGCalcTypeCodeRegistry.Object, "value", "c");
		d = ValueOutput(BGCalcTypeCodeRegistry.Cell, "cell", "d", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGEntity entity = GetEntity(flow);
		if (entity == null)
		{
			throw new Exception("Can not retrieve field value, cause the entity is not set!");
		}
		BGField field = GetField(flow);
		if (field == null)
		{
			throw new Exception("Can not retrieve field value, cause the field is not set!");
		}
		if (field.ReadOnly)
		{
			throw new Exception("Can not set cell value, cause field " + field.FullName + " is readonly!");
		}
		if (field.MetaId != entity.MetaId)
		{
			throw new Exception("Can not retrieve field value, cause entity and field belong to different tables!");
		}
		field.SetValue(entity.Index, flow.GetValue(c));
		flow.SetValue(d, new BGCalcCell(field, entity));
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
