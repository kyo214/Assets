using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverNoIdRT : BGExcelIdResolverART
{
	public BGExcelIdResolverNoIdRT(BGLogger logger)
		: base(logger)
	{
	}

	public override BGId ResolveId(BGExcelSheetReaderEntityRT reader, BGEntitySheetInfo info, IRow row)
	{
		return BGId.Empty;
	}
}
