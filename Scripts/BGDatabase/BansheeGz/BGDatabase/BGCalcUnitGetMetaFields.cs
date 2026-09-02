using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/meta/Get meta fields")]
public class BGCalcUnitGetMetaFields : BGCalcUnit
{
	private BGCalcValueInput metaInput;

	public const int Code = 121;

	public override ushort TypeCode => 121;

	public override void Definition()
	{
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "a");
		ValueOutput(BGCalcTypeCodeRegistry.List, "fields", "b", GetFields);
	}

	private IList GetFields(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Can not get meta fields, cause the meta is not set!");
		}
		return value.FindFields();
	}
}
