using UnityEngine;

namespace Fusion;

public class NetworkPrefabSourceUnityStatic : NetworkPrefabSourceUnityBase
{
	public GameObject PrefabReference;

	public override string EditorSummary => $"[Static: {PrefabReference}]";

	public override void Load(in NetworkPrefabLoadContext context)
	{
		context.Loaded(PrefabReference);
	}
}
