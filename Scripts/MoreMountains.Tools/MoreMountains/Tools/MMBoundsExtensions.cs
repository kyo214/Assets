using UnityEngine;

namespace MoreMountains.Tools;

public class MMBoundsExtensions : MonoBehaviour
{
	public static Vector3 MMRandomPointInBounds(Bounds bounds)
	{
		return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
	}

	public static Bounds GetColliderBounds(GameObject theObject)
	{
		if (theObject.GetComponent<Collider>() != null)
		{
			return theObject.GetComponent<Collider>().bounds;
		}
		if (theObject.GetComponent<Collider2D>() != null)
		{
			return theObject.GetComponent<Collider2D>().bounds;
		}
		if (theObject.GetComponentInChildren<Collider>() != null)
		{
			Bounds bounds = theObject.GetComponentInChildren<Collider>().bounds;
			Collider[] componentsInChildren = theObject.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				bounds.Encapsulate(collider.bounds);
			}
			return bounds;
		}
		if (theObject.GetComponentInChildren<Collider2D>() != null)
		{
			Bounds bounds2 = theObject.GetComponentInChildren<Collider2D>().bounds;
			Collider2D[] componentsInChildren2 = theObject.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D in componentsInChildren2)
			{
				bounds2.Encapsulate(collider2D.bounds);
			}
			return bounds2;
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	public static Bounds GetRendererBounds(GameObject theObject)
	{
		if (theObject.GetComponent<Renderer>() != null)
		{
			return theObject.GetComponent<Renderer>().bounds;
		}
		if (theObject.GetComponentInChildren<Renderer>() != null)
		{
			Bounds bounds = theObject.GetComponentInChildren<Renderer>().bounds;
			Renderer[] componentsInChildren = theObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				bounds.Encapsulate(renderer.bounds);
			}
			return bounds;
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}
}
