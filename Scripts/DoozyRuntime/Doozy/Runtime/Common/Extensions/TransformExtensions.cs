using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class TransformExtensions
{
	public static void DestroyChildren(this Transform target)
	{
		for (int num = target.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(target.GetChild(num).gameObject);
		}
	}

	public static void ResetTransformation(this Transform target)
	{
		target.position = Vector3.zero;
		target.localRotation = Quaternion.identity;
		target.localScale = Vector3.one;
	}

	public static Transform GetChildByName(this Transform target, string childName)
	{
		foreach (Transform item in target)
		{
			if (item.name == childName)
			{
				return item;
			}
		}
		throw new KeyNotFoundException();
	}

	public static Transform GetFromPath(this Transform target, string path)
	{
		return path.Split('/').Aggregate(target, (Transform current1, string childName) => current1.GetChildByName(childName));
	}

	public static IEnumerable<Transform> GetChildren(this Transform target)
	{
		foreach (Transform item in target)
		{
			yield return item;
		}
	}

	public static IEnumerable<Transform> Traverse(this Transform target)
	{
		yield return target;
		foreach (Transform item in target)
		{
			foreach (Transform item2 in item.Traverse())
			{
				yield return item2;
			}
		}
	}

	public static IEnumerable<Transform> Ancestors(this Transform target)
	{
		yield return target;
		if (target.parent == null)
		{
			yield break;
		}
		foreach (Transform item in target.parent.Ancestors())
		{
			yield return item;
		}
	}
}
