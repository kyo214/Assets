using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitDbGet : BGCalcUnitDbRowBasedA
{
	public const int Code = 104;

	public override ushort TypeCode => 104;

	protected override string Operation => "get";

	public override void Definition()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		base.Definition();
		int portsCount = base.PortsCount;
		List<BGField> list = meta.FindFields(null, (BGField f) => !(f is BGFieldCalcAction));
		if (portsCount + list.Count > 255)
		{
			list.RemoveRange(255 - portsCount, portsCount + list.Count - 255);
		}
		foreach (BGField field in list)
		{
			if (!(field is BGFieldEnumI bGFieldEnumI))
			{
				if (!(field is BGListI) && !(field is BGFieldRelationMultiple) && !(field is BGFieldManyRelationsMultiple) && !(field is BGFieldNested))
				{
					if (!(field is BGFieldRelationSingle bGFieldRelationSingle))
					{
						if (field is BGFieldManyRelationsSingle)
						{
							ValueOutput(BGCalcTypeCodeRegistry.Entity, field.Name, field.Id.ToString(), (BGCalcFlowI flow) => GetValue(flow, field.MetaId, field.Id));
						}
						else
						{
							ValueOutput(field.ValueType, field.Name, field.Id.ToString(), (BGCalcFlowI flow) => GetValue(flow, field.MetaId, field.Id));
						}
					}
					else
					{
						ValueOutput(new BGCalcTypeCodeEntityRuntime(bGFieldRelationSingle.RelatedMeta), field.Name, field.Id.ToString(), (BGCalcFlowI flow) => GetValue(flow, field.MetaId, field.Id));
					}
				}
				else
				{
					ValueOutput(BGCalcTypeCodeRegistry.List, field.Name, field.Id.ToString(), (BGCalcFlowI flow) => GetValue(flow, field.MetaId, field.Id));
				}
			}
			else
			{
				ValueOutput(new BGCalcTypeCodeEnum(bGFieldEnumI.EnumType), field.Name, field.Id.ToString(), (BGCalcFlowI flow) => GetValue(flow, field.MetaId, field.Id));
			}
		}
	}

	private object GetValue(BGCalcFlowI flow, BGId metaId, BGId fieldId)
	{
		BGField field = base.MetaCached.GetField(fieldId, errorIfNotFound: false);
		if (field == null)
		{
			throw new Exception($"Can not get a field with Id={fieldId}!");
		}
		BGEntity entity = GetEntity(flow);
		if (entity.MetaId != field.MetaId)
		{
			throw new Exception("Can not get a value, cause entity is from different table! Expected=" + field.MetaName + " actual=" + entity.MetaName);
		}
		if (field is BGFieldCalcI)
		{
			return CallCalculated(flow, field, entity);
		}
		BGCalcUnitCellGetValue.AddListeners(flow, field, entity);
		return field.GetValue(entity.Index);
	}

	public static object CallCalculated(BGCalcFlowI flow, BGField field, BGEntity entity)
	{
		BGStorable<BGFieldCalcValue> bGStorable = (BGStorable<BGFieldCalcValue>)field;
		BGFieldCalcI bGFieldCalcI = (BGFieldCalcI)field;
		BGCalcGraph bGCalcGraph = bGStorable.GetStoredValue(entity.Index)?.Graph ?? bGFieldCalcI.Graph;
		if (bGCalcGraph == null)
		{
			return null;
		}
		BGCalcFlowContext bGCalcFlowContext = BGCalcFlowContext.Get();
		try
		{
			bGCalcFlowContext.CopyCellsFrom(flow.Context);
			return BGFieldCalcA<object>.Run(bGCalcFlowContext, bGCalcGraph, field, entity);
		}
		finally
		{
			BGCalcFlowContext.Return(bGCalcFlowContext);
		}
	}
}
