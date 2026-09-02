using System;

namespace Mono;

internal struct CFRange(int loc, int len)
{
	public IntPtr Location = (IntPtr)loc;

	public IntPtr Length = (IntPtr)len;
}
