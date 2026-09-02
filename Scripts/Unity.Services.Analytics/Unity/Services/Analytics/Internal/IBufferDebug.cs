using System;
using System.Collections.Generic;

namespace Unity.Services.Analytics.Internal;

internal interface IBufferDebug
{
	event Action<string, string, DateTime, byte[]> EventRecorded;

	event Action<HashSet<string>> EventsClearing;

	event Action<HashSet<string>> EventsCleared;
}
