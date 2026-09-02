using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
internal struct UnsafeHashMapDataDisposeJob : IJob
{
	internal UnsafeHashMapDataDispose Data;

	public void Execute()
	{
		Data.Dispose();
	}
}
