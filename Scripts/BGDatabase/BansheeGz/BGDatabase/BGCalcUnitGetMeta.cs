namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/meta/Get meta")]
public class BGCalcUnitGetMeta : BGCalcUnitWithSource
{
	public const int Code = 70;

	public override ushort TypeCode => 70;

	protected override BGCalcTypeCode ObjectTypeCode => BGCalcTypeCodeRegistry.Meta;

	protected override BGObject FetchObjectByName(BGCalcFlowI calcFlowI, string name)
	{
		return BGRepo.I.GetMeta(name);
	}

	protected override BGObject FetchObjectById(BGCalcFlowI calcFlowI, BGId id)
	{
		return BGRepo.I.GetMeta(id);
	}

	protected override BGObject FetchObjectByIndex(BGCalcFlowI calcFlowI, int index)
	{
		return BGRepo.I.GetMeta(index);
	}
}
