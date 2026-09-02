using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/field/Get field meta")]
public class BGCalcUnitGetFieldMeta : BGCalcUnit
{
	private BGCalcValueInput fieldInput;

	public const int Code = 119;

	public override ushort TypeCode => 119;

	public override void Definition()
	{
		fieldInput = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Meta, "meta", "b", GetMeta);
	}

	private BGMetaEntity GetMeta(BGCalcFlowI flow)
	{
		BGField value = flow.GetValue<BGField>(fieldInput);
		if (value == null)
		{
			throw new Exception("Can not get a meta, cause the field is not set!");
		}
		return value.Meta;
	}
}
