namespace Fusion;

public abstract class NetworkPrefabSourceUnityBase : AssetObject, INetworkPrefabSource
{
	[UnityAssetGuid]
	public NetworkObjectGuid AssetGuid;

	public abstract string EditorSummary { get; }

	public abstract void Load(in NetworkPrefabLoadContext context);

	public virtual void Unload()
	{
	}
}
