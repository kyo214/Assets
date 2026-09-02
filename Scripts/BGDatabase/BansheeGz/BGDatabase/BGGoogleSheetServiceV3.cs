namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetServiceV3
{
	void Export(BGGoogleSheetServiceV3ExportTask task);

	void Import(BGGoogleSheetServiceV3ImportTask task);
}
