using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
internal struct UnsafeHashMapDisposeJob : IJob
{
	[NativeDisableUnsafePtrRestriction]
	public unsafe UnsafeHashMapData* Data;

	public AllocatorManager.AllocatorHandle Allocator;

	public unsafe void Execute()
	{
		UnsafeHashMapData.DeallocateHashMap(Data, Allocator);
	}
}
