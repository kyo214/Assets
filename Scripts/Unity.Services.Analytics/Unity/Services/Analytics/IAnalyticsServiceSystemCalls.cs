using System;

namespace Unity.Services.Analytics;

internal interface IAnalyticsServiceSystemCalls
{
	DateTime UtcNow { get; }
}
