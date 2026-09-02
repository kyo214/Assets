using System;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitDbMetaBasedA : BGCalcUnit
{
	public static readonly byte MetaIdVarId = 1;

	private BGMetaEntity metaCached;

	protected BGMetaEntity MetaCached
	{
		get
		{
			if (metaCached == null || metaCached.IsDeleted)
			{
				metaCached = Meta;
			}
			if (metaCached == null)
			{
				throw new Exception($"Can not get a meta with Id={MetaId}!");
			}
			return metaCached;
		}
	}

	public BGCalcVarLite MetaVar => GetVar(MetaIdVarId);

	public BGMetaEntity Meta => BGRepo.I.GetMeta(MetaId);

	public BGId MetaId => (BGId)MetaVar.Value;

	public virtual void Init(BGId metaId)
	{
		GetVars()?.Variables.Clear();
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, MetaIdVarId, BGCalcTypeCodeRegistry.BGId);
		bGCalcVarLite.Value = metaId;
	}
}
