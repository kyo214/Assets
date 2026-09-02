using System.Runtime.CompilerServices;

namespace Fusion.KCC;

public static class KCCTypesExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Has(this EKCCStages stages, EKCCStage stage)
	{
		return ((uint)stages & (uint)(1 << (int)stage)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Has(this EKCCFeatures features, EKCCFeature feature)
	{
		return ((uint)features & (uint)(1 << (int)feature)) != 0;
	}
}
