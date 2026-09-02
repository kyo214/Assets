#define DEBUG
using UnityEngine;

namespace Fusion;

public class NetworkPrefabSourceUnityResource : NetworkPrefabSourceUnityBase
{
	public string ResourcePath;

	private object _state;

	public override string EditorSummary => "[Resource: " + ResourcePath + "]";

	public override void Load(in NetworkPrefabLoadContext context)
	{
		Assert.Check(_state == null);
		if (context.HasFlag(1))
		{
			ResourceRequest resourceRequest = Resources.LoadAsync(ResourcePath, typeof(NetworkObject));
			if (resourceRequest.isDone)
			{
				Log.Debug(ResourcePath + " loaded immediately");
				context.Loaded((NetworkObject)(_state = (NetworkObject)resourceRequest.asset));
				return;
			}
			_state = resourceRequest;
			NetworkPrefabLoadContext cc = context;
			resourceRequest.completed += (AsyncOperation op) =>
			{
				NetworkObject networkObject = (NetworkObject)((ResourceRequest)op).asset;
				if (_state != op)
				{
					Assert.Check(_state == null);
					if (BehaviourUtils.IsAlive(networkObject))
					{
						UnloadPrefab(networkObject);
					}
				}
				else
				{
					_state = networkObject;
					cc.Loaded(networkObject);
				}
			};
		}
		else
		{
			context.Loaded((NetworkObject)(_state = Resources.Load<NetworkObject>(ResourcePath)));
		}
	}

	public override void Unload()
	{
		if (_state != null)
		{
			ResourceRequest resourceRequest = _state as ResourceRequest;
			if (resourceRequest == null && _state is NetworkObject asset)
			{
				UnloadPrefab(asset);
			}
			_state = null;
		}
	}

	private void UnloadPrefab(NetworkObject asset)
	{
	}
}
