using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverIdRT : BGExcelIdResolverART
{
	public BGExcelIdResolverIdRT(BGLogger logger)
		: base(logger)
	{
	}

	public override BGId ResolveId(BGExcelSheetReaderEntityRT reader, BGEntitySheetInfo info, IRow row)
	{
		BGId entityId = BGId.Empty;
		if (info.IndexId >= 0)
		{
			reader.ReadNotNull(row, info.IndexId, (string s) =>
			{
				entityId = new BGId(s);
			});
		}
		return entityId;
	}
}
