using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverIndexRT : BGExcelIdResolverART
{
	private readonly BGMetaEntity mainMeta;

	public BGExcelIdResolverIndexRT(BGLogger logger, BGMetaEntity mainMeta)
		: base(logger)
	{
		this.mainMeta = mainMeta;
	}

	public override BGId ResolveId(BGExcelSheetReaderEntityRT reader, BGEntitySheetInfo info, IRow row)
	{
		if (mainMeta == null)
		{
			return BGId.Empty;
		}
		int num = row.RowNum - 1;
		if (num < 0 || num >= mainMeta.CountEntities)
		{
			return BGId.NewId;
		}
		return mainMeta.GetEntity(num).Id;
	}
}
