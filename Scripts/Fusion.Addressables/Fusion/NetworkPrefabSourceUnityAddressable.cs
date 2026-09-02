using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fusion;

public class NetworkPrefabSourceUnityAddressable : NetworkPrefabSourceUnityBase
{
	public AssetReferenceGameObject Address;

	public override string EditorSummary => $"[Address: {Address}]";

	public override void Load(in NetworkPrefabLoadContext context)
	{
		AsyncOperationHandle<GameObject> asyncOperationHandle = Address.LoadAssetAsync();
		if (asyncOperationHandle.IsDone)
		{
			context.Loaded(asyncOperationHandle.Result);
		}
		else if (context.HasFlag(1))
		{
			NetworkPrefabLoadContext c = context;
			asyncOperationHandle.Completed += (AsyncOperationHandle<GameObject> _op) =>
			{
				c.Loaded(_op.Result);
			};
		}
		else
		{
			GameObject prefab = asyncOperationHandle.WaitForCompletion();
			context.Loaded(prefab);
		}
	}

	public override void Unload()
	{
		if (Address.IsValid())
		{
			Address.ReleaseAsset();
		}
	}
}
