namespace BansheeGz.BGDatabase;

public interface BGExcelServiceV2
{
	byte[] Import(BGExcelServiceV2ImportTask task);

	byte[] Export(BGExcelServiceV2ExportTask task);
}
