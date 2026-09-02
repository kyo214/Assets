using System;

namespace Unity.Services.Analytics.Internal;

internal interface IDispatcherDebug
{
	bool FlushInProgress { get; }

	event Action<byte[]> FlushStarted;

	event Action<int, bool, bool, bool, bool, byte[]> FlushFinished;
}
