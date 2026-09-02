using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public static class TransformExtensions
{
	public static void MMDestroyAllChildren(this Transform transform)
	{
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(transform.GetChild(num).gameObject);
			}
			else
			{
				Object.DestroyImmediate(transform.GetChild(num).gameObject);
			}
		}
	}

	public static Transform MMFindDeepChildBreadthFirst(this Transform parent, string transformName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(parent);
		while (queue.Count > 0)
		{
			Transform transform = queue.Dequeue();
			if (transform.name == transformName)
			{
				return transform;
			}
			foreach (Transform item in transform)
			{
				queue.Enqueue(item);
			}
		}
		return null;
	}

	public static Transform MMFindDeepChildDepthFirst(this Transform parent, string transformName)
	{
		foreach (Transform item in parent)
		{
			if (item.name == transformName)
			{
				return item;
			}
			Transform transform2 = item.MMFindDeepChildDepthFirst(transformName);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	public static void ChangeLayersRecursively(this Transform transform, string layerName)
	{
		transform.gameObject.layer = LayerMask.NameToLayer(layerName);
		foreach (Transform item in transform)
		{
			item.ChangeLayersRecursively(layerName);
		}
	}

	public static void ChangeLayersRecursively(this Transform transform, int layerIndex)
	{
		transform.gameObject.layer = layerIndex;
		foreach (Transform item in transform)
		{
			item.ChangeLayersRecursively(layerIndex);
		}
	}
}
