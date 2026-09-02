namespace Fusion;

public class NetworkPrefabSourceStatic : INetworkPrefabSource
{
	public NetworkObject PrefabReference;

	string INetworkPrefabSource.EditorSummary => $"[StaticRaw: {PrefabReference}]";

	void INetworkPrefabSource.Load(in NetworkPrefabLoadContext context)
	{
		context.Loaded(PrefabReference);
	}

	void INetworkPrefabSource.Unload()
	{
	}
}
