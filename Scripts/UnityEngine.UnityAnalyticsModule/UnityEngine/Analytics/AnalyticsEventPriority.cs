using System;

namespace UnityEngine.Analytics;

[Flags]
public enum AnalyticsEventPriority
{
	FlushQueueFlag = 1,
	CacheImmediatelyFlag = 2,
	AllowInStopModeFlag = 4,
	SendImmediateFlag = 8,
	NoCachingFlag = 0x10,
	NoRetryFlag = 0x20,
	NormalPriorityEvent = 0,
	NormalPriorityEvent_WithCaching = CacheImmediatelyFlag,
	NormalPriorityEvent_NoRetryNoCaching = NoCachingFlag | NoRetryFlag,
	HighPriorityEvent = FlushQueueFlag,
	HighPriorityEvent_InStopMode = FlushQueueFlag | AllowInStopModeFlag,
	HighestPriorityEvent = FlushQueueFlag | SendImmediateFlag,
	HighestPriorityEvent_NoRetryNoCaching = NormalPriorityEvent_NoRetryNoCaching | FlushQueueFlag
}
