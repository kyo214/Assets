using System.Collections.Generic;

namespace Unity.Services.Analytics;

internal interface IUnstructuredEventRecorder
{
	void CustomData(string eventName, IDictionary<string, object> eventParams, int? eventVersion, bool isStandardEvent, string callingMethodIdentifier);
}
