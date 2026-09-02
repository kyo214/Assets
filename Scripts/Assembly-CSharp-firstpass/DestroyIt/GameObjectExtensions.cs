using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DestroyIt;

public static class GameObjectExtensions
{
	public static void RemoveAllFromChildren<T>(this GameObject obj) where T : Component
	{
		if (!(obj == null))
		{
			T[] componentsInChildren = obj.GetComponentsInChildren<T>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
		}
	}

	public static void RemoveComponent<T>(this GameObject obj) where T : Component
	{
		if (!(obj == null))
		{
			T component = obj.GetComponent<T>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	public static List<T> GetComponentsInChildrenOnly<T>(this GameObject obj) where T : Component
	{
		return obj.GetComponentsInChildrenOnly<T>(includeInactive: false);
	}

	public static List<T> GetComponentsInChildrenOnly<T>(this GameObject obj, bool includeInactive) where T : Component
	{
		List<T> list = obj.GetComponentsInChildren<T>(includeInactive).ToList();
		list.Remove(obj.GetComponent<T>());
		return list;
	}

	public static void AddStiffJoint(this GameObject go, Rigidbody connectedBody, Vector3 anchorPosition, Vector3 axis, float breakForce, float breakTorque)
	{
		FixedJoint fixedJoint = go.AddComponent<FixedJoint>();
		fixedJoint.anchor = anchorPosition;
		fixedJoint.connectedBody = connectedBody;
		fixedJoint.breakForce = breakForce;
		fixedJoint.breakTorque = breakTorque;
	}

	public static Vector3 GetMeshCenterPoint(this GameObject go, MeshRenderer[] meshRenderers = null)
	{
		if (meshRenderers == null)
		{
			meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
		}
		if (meshRenderers.Length == 0)
		{
			return Vector3.zero;
		}
		if (go.IsAnyMeshPartOfStaticBatch(meshRenderers))
		{
			return Vector3.zero;
		}
		Bounds bounds = default;
		MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Mesh sharedMesh = componentsInChildren[i].sharedMesh;
			if (sharedMesh != null)
			{
				bounds.Encapsulate(sharedMesh.bounds);
			}
		}
		return bounds.center;
	}

	public static bool IsAnyMeshPartOfStaticBatch(this GameObject go, MeshRenderer[] meshRenderers = null)
	{
		if (meshRenderers == null)
		{
			meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
		}
		if (meshRenderers.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			if (meshRenderers[i].isPartOfStaticBatch)
			{
				return true;
			}
		}
		return false;
	}
}
