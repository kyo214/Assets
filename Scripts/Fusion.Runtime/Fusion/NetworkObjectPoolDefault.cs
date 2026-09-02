#define DEBUG
using UnityEngine;

namespace Fusion;

internal class NetworkObjectPoolDefault : INetworkObjectPool
{
	public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
	{
		if (runner.Config.PrefabTable.TryGetPrefab(info.Prefab, out var obj))
		{
			return Object.Instantiate(obj);
		}
		return null;
	}

	public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
	{
		NetworkObjectFlags networkObjectFlags = instance.Flags & NetworkObjectFlags.MaskType;
		Assert.Check(networkObjectFlags == NetworkObjectFlags.TypeSceneObject || networkObjectFlags == NetworkObjectFlags.TypeSpawnedPrefab || networkObjectFlags == NetworkObjectFlags.TypeSpawnedPrefabChild, "Invalid type", networkObjectFlags);
		Object.Destroy(instance.gameObject);
	}
}
