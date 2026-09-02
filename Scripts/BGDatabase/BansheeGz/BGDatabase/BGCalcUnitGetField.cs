using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/field/Get field")]
public class BGCalcUnitGetField : BGCalcUnitWithSource
{
	private BGCalcValueInput metaInput;

	public const int Code = 71;

	public override ushort TypeCode => 71;

	protected override BGCalcTypeCode ObjectTypeCode => BGCalcTypeCodeRegistry.Field;

	public override void Definition()
	{
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "q");
		base.Definition();
	}

	protected override BGObject FetchObjectByName(BGCalcFlowI flow, string name)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetField(name, errorIfNotFound: false);
	}

	protected override BGObject FetchObjectById(BGCalcFlowI flow, BGId id)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetField(id, errorIfNotFound: false);
	}

	protected override BGObject FetchObjectByIndex(BGCalcFlowI flow, int index)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetField(index);
	}

	private BGMetaEntity GetMeta(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Can not get a field cause meta is not set!");
		}
		return value;
	}
}
