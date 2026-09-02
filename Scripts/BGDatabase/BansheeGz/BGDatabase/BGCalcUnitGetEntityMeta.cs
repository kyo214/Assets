using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/entity/Get entity meta")]
public class BGCalcUnitGetEntityMeta : BGCalcUnit
{
	private BGCalcValueInput entityInput;

	public const int Code = 120;

	public override ushort TypeCode => 120;

	public override void Definition()
	{
		entityInput = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Meta, "meta", "b", GetMeta);
	}

	private BGMetaEntity GetMeta(BGCalcFlowI flow)
	{
		BGEntity value = flow.GetValue<BGEntity>(entityInput);
		if (value == null)
		{
			throw new Exception("Can not get a meta, cause the entity is not set!");
		}
		return value.Meta;
	}
}
