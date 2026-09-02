using System;

namespace Fusion;

[Obsolete("Use INetworkSceneManager and optionally INetworkSceneManagerObjectResolver, if custom scene object resolving is needed")]
public interface INetworkSceneObjectProvider : INetworkSceneManager, INetworkSceneManagerObjectResolver
{
}
