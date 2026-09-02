using System.Runtime.CompilerServices;

namespace Fusion.KCC;

public static class KCCFloatExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(this float value)
	{
		return float.IsNaN(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAlmostZero(this float value, float tolerance = 0.01f)
	{
		if (value < tolerance)
		{
			return value > 0f - tolerance;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AlmostEquals(this float valueA, float valueB, float tolerance = 0.01f)
	{
		return (valueA - valueB).IsAlmostZero(tolerance);
	}
}
