using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/relation/Get by multiple relation")]
public class BGCalcUnitGetByMultipleRelation : BGCalcUnit
{
	private BGCalcValueInput fieldInput;

	private BGCalcValueInput entityInput;

	public const int Code = 141;

	public override ushort TypeCode => 141;

	public override void Definition()
	{
		fieldInput = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "a");
		entityInput = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "b");
		ValueOutput(BGCalcTypeCodeRegistry.List, "result", "q", GetResult);
	}

	private List<BGEntity> GetResult(BGCalcFlowI flow)
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
		if (!(value is BGFieldRelationMultipleI bGFieldRelationMultipleI))
		{
			throw new Exception("Field " + value.FullName + " is not a multiple relation (it should be)!");
		}
		return bGFieldRelationMultipleI.GetRelatedEntity(value2.Index);
	}
}
