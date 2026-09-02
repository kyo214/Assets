namespace Fusion;

public interface INetworkObjectPool
{
	NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info);

	void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject);
}
