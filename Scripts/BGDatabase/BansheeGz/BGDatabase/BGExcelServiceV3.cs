namespace BansheeGz.BGDatabase;

public interface BGExcelServiceV3
{
	byte[] Import(BGExcelServiceV3ImportTask task);

	byte[] Export(BGExcelServiceV3ExportTask task);
}
