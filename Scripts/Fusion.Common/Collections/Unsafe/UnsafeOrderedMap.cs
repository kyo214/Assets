using System.Runtime.InteropServices;

namespace Collections.Unsafe;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UnsafeOrderedMap
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Iterator<K, V> where K : unmanaged where V : unmanaged
	{
	}
}
