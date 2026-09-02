using System.Globalization;

namespace Unity.Services.Analytics.Internal;

internal static class Locale
{
	internal static string CurrentLanguageCode()
	{
		return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
	}

	internal static string AnalyticsRegionLanguageCode()
	{
		return CurrentLanguageCode() + "_ZZ";
	}
}
