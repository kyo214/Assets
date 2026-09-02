using System.Collections.Generic;
using System.IO;

namespace Unity.Services.Analytics.Internal;

internal interface IDiskCache
{
	void Clear();

	void Write(List<EventSummary> eventSummaries, Stream payload);

	bool Read(List<EventSummary> eventSummaries, Stream buffer);
}
