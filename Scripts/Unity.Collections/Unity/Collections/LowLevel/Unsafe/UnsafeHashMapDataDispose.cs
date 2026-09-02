namespace Unity.Collections.LowLevel.Unsafe;

[NativeContainer]
[BurstCompatible]
internal struct UnsafeHashMapDataDispose
{
	[NativeDisableUnsafePtrRestriction]
	internal unsafe UnsafeHashMapData* m_Buffer;

	internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

	public unsafe void Dispose()
	{
		UnsafeHashMapData.DeallocateHashMap(m_Buffer, m_AllocatorLabel);
	}
}
