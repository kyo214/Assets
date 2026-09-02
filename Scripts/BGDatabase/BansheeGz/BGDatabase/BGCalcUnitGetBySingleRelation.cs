using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/relation/Get by single relation")]
public class BGCalcUnitGetBySingleRelation : BGCalcUnit
{
	private BGCalcValueInput fieldInput;

	private BGCalcValueInput entityInput;

	public const int Code = 140;

	public override ushort TypeCode => 140;

	public override void Definition()
	{
		fieldInput = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "a");
		entityInput = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Entity, "result", "q", GetResult);
	}

	private BGEntity GetResult(BGCalcFlowI flow)
	{
		BGField value = flow.GetValue<BGField>(fieldInput);
		if (value == null)
		{
			throw new Exception("Can not get a field cause the field is not set!");
		}
		BGEntity value2 = flow.GetValue<BGEntity>(entityInput);
		if (value2 == null)
		{
			throw new Exception("Can not get an entity cause the entity is not set!");
		}
		if (!value.Meta.Equals(value2.Meta))
		{
			throw new Exception("Field " + value.FullName + " and entity " + value2.FullName + " have different metas (should be the same)!");
		}
		if (!(value is BGFieldRelationSingleI bGFieldRelationSingleI))
		{
			throw new Exception("Field " + value.FullName + " is not a single relation (it should be)!");
		}
		return bGFieldRelationSingleI.GetRelatedEntity(value2.Index);
	}
}
