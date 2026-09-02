using UnityEngine;

namespace DestroyIt;

public class DelayedRigidbody : MonoBehaviour
{
	public float mass = 1f;

	public float drag;

	public float angularDrag = 0.05f;

	public float delaySeconds;

	public bool reenableColliders = true;

	public void Initialize()
	{
		Invoke("AddRigidbody", delaySeconds);
	}

	public void AddRigidbody()
	{
		if (base.gameObject.GetComponent<Rigidbody>() == null)
		{
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = mass;
			rigidbody.drag = drag;
			rigidbody.angularDrag = angularDrag;
		}
		if (reenableColliders)
		{
			Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
		Object.Destroy(this);
	}
}
