#define DEBUG
using System.Runtime.InteropServices;

namespace Fusion.Sockets;

[StructLayout(LayoutKind.Explicit)]
public struct ReliableHeader
{
	public const int SIZE = 32;

	[FieldOffset(0)]
	public unsafe ReliableHeader* Next;

	[FieldOffset(8)]
	public unsafe ReliableHeader* Prev;

	[FieldOffset(16)]
	public ulong Sequence;

	[FieldOffset(24)]
	public int Length;

	[FieldOffset(28)]
	public int Key;

	public unsafe static byte* GetData(ReliableHeader* header)
	{
		Assert.Check(sizeof(ReliableHeader) == 32);
		return (byte*)header + 32;
	}
}
