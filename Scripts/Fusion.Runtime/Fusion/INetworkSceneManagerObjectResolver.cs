using System;

namespace Fusion;

public interface INetworkSceneManagerObjectResolver
{
	bool TryResolveSceneObject(NetworkRunner runner, Guid sceneObjectGuid, out NetworkObject instance);
}
