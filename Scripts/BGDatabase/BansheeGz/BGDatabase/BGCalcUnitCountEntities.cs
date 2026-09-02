using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/meta/Count entities")]
public class BGCalcUnitCountEntities : BGCalcUnit
{
	private BGCalcValueInput metaInput;

	public const int Code = 124;

	public override ushort TypeCode => 124;

	public override void Definition()
	{
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "count", "b", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Meta is not set!");
		}
		flow.Context.Events?.AddOnCreate(value);
		flow.Context.Events?.AddOnDelete(value);
		return value.CountEntities;
	}
}
