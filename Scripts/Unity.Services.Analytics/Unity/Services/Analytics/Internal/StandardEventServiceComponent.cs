using System.Collections.Generic;
using Unity.Services.Core.Analytics.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Analytics.Internal;

internal class StandardEventServiceComponent : IAnalyticsStandardEventComponent, IServiceComponent
{
	private readonly IProjectConfiguration m_Configuration;

	private readonly IUnstructuredEventRecorder m_AnalyticsService;

	public StandardEventServiceComponent(IProjectConfiguration configuration, IUnstructuredEventRecorder analyticsService)
	{
		m_Configuration = configuration;
		m_AnalyticsService = analyticsService;
	}

	public void Record(string eventName, IDictionary<string, object> eventParameters, int eventVersion, string packageName)
	{
		string text = m_Configuration.GetString(packageName + ".version");
		string callingMethodIdentifier = packageName + "@" + text;
		m_AnalyticsService.CustomData(eventName, eventParameters, eventVersion, isStandardEvent: true, callingMethodIdentifier);
	}
}
