namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetServiceV4
{
	void Export(BGGoogleSheetServiceV4ExportTask task);

	void Import(BGGoogleSheetServiceV4ImportTask task);
}
