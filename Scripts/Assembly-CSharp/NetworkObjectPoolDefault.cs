using Fusion;
using UnityEngine;

public class NetworkObjectPoolDefault : INetworkObjectPool
{
	public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
	{
		Debug.Log("Created");
		if (runner.Config.PrefabTable.TryGetPrefab(info.Prefab, out var obj))
		{
			return Object.Instantiate(obj);
		}
		return null;
	}

	public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
	{
		Debug.Log("Destroyed");
		Object.Destroy(instance.gameObject);
	}
}
