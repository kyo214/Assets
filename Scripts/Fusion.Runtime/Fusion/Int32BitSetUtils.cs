using System.Runtime.CompilerServices;

namespace Fusion;

internal static class Int32BitSetUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool IsBitSet(int* bits, int bit)
	{
		return (bits[bit / 32] & (1 << bit % 32)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool IsBitSetOrNull(int* bits, int bit)
	{
		return bits == null || (bits[bit / 32] & (1 << bit % 32)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void SetBit(int* bits, int bit)
	{
		bits[bit / 32] |= 1 << bit % 32;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void ClearBit(int* bits, int bit)
	{
		bits[bit / 32] &= ~(1 << bit % 32);
	}
}
