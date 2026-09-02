using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/meta/Get meta entities")]
public class BGCalcUnitGetMetaEntities : BGCalcUnit
{
	private BGCalcValueInput metaInput;

	public const int Code = 122;

	public override ushort TypeCode => 122;

	public override void Definition()
	{
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "a");
		ValueOutput(BGCalcTypeCodeRegistry.List, "entities", "b", GetEntities);
	}

	private IList GetEntities(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Can not get meta fields, cause the meta is not set!");
		}
		return value.EntitiesToList();
	}
}
