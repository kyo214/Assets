using UnityEngine;

namespace Fusion;

public class NetworkPrefabAsset : AssetObject
{
	[UnityAssetGuid]
	public NetworkObjectGuid AssetGuid;

	[ContextMenu("Delete")]
	private void Destroy()
	{
		Object.DestroyImmediate(this, allowDestroyingAssets: true);
	}
}
