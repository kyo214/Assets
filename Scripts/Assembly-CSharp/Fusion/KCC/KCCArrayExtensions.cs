using System;
using System.Runtime.CompilerServices;

namespace Fusion.KCC;

public static class KCCArrayExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Clear(this Array array)
	{
		Array.Clear(array, 0, array.Length);
	}
}
