namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetServiceV2
{
	void Export(BGGoogleSheetServiceV2ExportTask task);

	void Import(BGGoogleSheetServiceV2ImportTask task);
}
