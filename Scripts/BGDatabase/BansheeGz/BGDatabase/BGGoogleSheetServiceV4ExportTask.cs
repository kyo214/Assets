using BansheeGz.BGDatabase.Editor;

namespace BansheeGz.BGDatabase;

public class BGGoogleSheetServiceV4ExportTask : BGGoogleSheetServiceV3ExportTask
{
	public bool skipLocking;

	public BGDsGoogleSheets.ReadFormatEnum ReadFormat;

	public string ReadFormatCountry;

	public BGDsGoogleSheets.WriteFormatEnum WriteFormat;

	public string WriteFormatCountry;
}
