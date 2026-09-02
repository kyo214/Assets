using UnityEngine.Analytics;

namespace Unity.Services.Analytics;

internal class CoreStatsHelper : ICoreStatsHelper
{
	public void SetCoreStatsConsent(bool userProvidedConsent)
	{
		UGSAnalyticsInternalTools.SetPrivacyStatus(userProvidedConsent);
	}
}
