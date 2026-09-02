using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/entity/Get entity")]
public class BGCalcUnitGetEntity : BGCalcUnitWithSource
{
	private BGCalcValueInput metaInput;

	public const int Code = 72;

	public override ushort TypeCode => 72;

	protected override BGCalcTypeCode ObjectTypeCode => BGCalcTypeCodeRegistry.Entity;

	public override void Definition()
	{
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "q");
		base.Definition();
	}

	protected override BGObject FetchObjectByName(BGCalcFlowI flow, string name)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetEntity(name);
	}

	protected override BGObject FetchObjectById(BGCalcFlowI flow, BGId id)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetEntity(id);
	}

	protected override BGObject FetchObjectByIndex(BGCalcFlowI flow, int index)
	{
		BGMetaEntity meta = GetMeta(flow);
		return meta.GetEntity(index);
	}

	private BGMetaEntity GetMeta(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Can not get an entity cause meta is not set!");
		}
		return value;
	}
}
