using Unity.Services.Core.Analytics.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Analytics.Internal;

internal class AnalyticsUserIdServiceComponent : IAnalyticsUserId, IServiceComponent
{
	private readonly IAnalyticsService m_AnalyticsService;

	public AnalyticsUserIdServiceComponent(IAnalyticsService analyticsService)
	{
		m_AnalyticsService = analyticsService;
	}

	public string GetAnalyticsUserId()
	{
		return m_AnalyticsService.GetAnalyticsUserID();
	}
}
