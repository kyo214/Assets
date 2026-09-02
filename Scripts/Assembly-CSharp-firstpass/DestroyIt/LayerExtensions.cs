using UnityEngine;

namespace DestroyIt;

public static class LayerExtensions
{
	public static void SetLayerRecursively(this GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform item in obj.transform)
		{
			item.gameObject.SetLayerRecursively(layer);
		}
	}
}
