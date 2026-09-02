using System.Diagnostics;

namespace System.Collections.Generic;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct Marker(int count, int index)
{
	public int Count { get; } = count;

	public int Index { get; } = index;

	private string DebuggerDisplay => string.Format("{0}: {1}, {2}: {3}", "Index", Index, "Count", Count);
}
