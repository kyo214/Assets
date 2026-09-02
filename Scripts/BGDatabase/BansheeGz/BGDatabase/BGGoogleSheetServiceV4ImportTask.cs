using BansheeGz.BGDatabase.Editor;

namespace BansheeGz.BGDatabase;

public class BGGoogleSheetServiceV4ImportTask : BGGoogleSheetServiceV3ImportTask
{
	public bool skipLocking;

	public BGDsGoogleSheets.ReadFormatEnum ReadFormat;

	public string FloatingFormatCountry;
}
