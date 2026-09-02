using System;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitDbCount : BGCalcUnitDbMetaBasedA
{
	public const int Code = 105;

	public override ushort TypeCode => 105;

	public override string Title
	{
		get
		{
			BGMetaEntity meta = base.Meta;
			if (meta == null)
			{
				return "DB count [ERROR:meta not found]";
			}
			return "DB count [" + meta.Name + "]";
		}
	}

	public override void Definition()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		ValueOutput(BGCalcTypeCodeRegistry.Int, "count", "c", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		BGMetaEntity bGMetaEntity = base.MetaCached;
		flow.Context.Events?.AddOnCreate(bGMetaEntity);
		flow.Context.Events?.AddOnDelete(bGMetaEntity);
		return bGMetaEntity.CountEntities;
	}
}
