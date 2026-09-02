using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitDbSet : BGCalcUnitDbRowBasedA
{
	private BGCalcControlInput enterPort;

	private BGCalcControlOutput exitPort;

	public const int Code = 109;

	public override ushort TypeCode => 109;

	protected override string Operation => "set";

	public override void Definition()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		enterPort = ControlInput("enter", "y", RunMe);
		exitPort = ControlOutput("exit", "z");
		base.Definition();
		int portsCount = base.PortsCount;
		List<BGField> list = meta.FindFields(null, (BGField f) => !f.ReadOnly);
		if (portsCount + list.Count > 255)
		{
			list.RemoveRange(255 - portsCount, portsCount + list.Count - 255);
		}
		foreach (BGField item in list)
		{
			if (!(item is BGFieldEnumI bGFieldEnumI))
			{
				if (!(item is BGListI) && !(item is BGFieldRelationMultiple) && !(item is BGFieldManyRelationsMultiple) && !(item is BGFieldNested))
				{
					if (!(item is BGFieldRelationSingle bGFieldRelationSingle))
					{
						if (item is BGFieldManyRelationsSingle)
						{
							ValueInput(BGCalcTypeCodeRegistry.Entity, item.Name, item.Id.ToString());
						}
						else
						{
							ValueInput(item.ValueType, item.Name, item.Id.ToString());
						}
					}
					else
					{
						ValueInput(new BGCalcTypeCodeEntityRuntime(bGFieldRelationSingle.RelatedMeta), item.Name, item.Id.ToString());
					}
				}
				else
				{
					ValueInput(BGCalcTypeCodeRegistry.List, item.Name, item.Id.ToString());
				}
			}
			else
			{
				ValueInput(new BGCalcTypeCodeEnum(bGFieldEnumI.EnumType), item.Name, item.Id.ToString());
			}
		}
	}

	private BGCalcControlOutputI RunMe(BGCalcFlowI flow)
	{
		BGMetaEntity bGMetaEntity = base.MetaCached;
		List<BGCalcPortI> list = FindPorts((BGCalcPortI port) => port is BGCalcValueInputI && port.Id.Length > 1 && port.IsConnected);
		BGEntity entity = GetEntity(flow);
		foreach (BGCalcPortI item in list)
		{
			BGField field = bGMetaEntity.GetField(BGId.Parse(item.Id));
			object value = flow.GetValue(item as BGCalcValueInputI);
			field.SetValue(entity.Index, value);
		}
		return exitPort;
	}
}
