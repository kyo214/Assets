using Unity.Collections;
using UnityEngine;

internal static class _0024BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		AllocatorManager.Initialize_0024StackAllocator_Try_00000980_0024BurstDirectCall();
		AllocatorManager.Initialize_0024SlabAllocator_Try_0000098E_0024BurstDirectCall();
		RewindableAllocator.Try_000006E8_0024BurstDirectCall.Initialize();
		xxHash3.Hash64Long_0000071F_0024BurstDirectCall.Initialize();
		xxHash3.Hash128Long_00000726_0024BurstDirectCall.Initialize();
	}
}
