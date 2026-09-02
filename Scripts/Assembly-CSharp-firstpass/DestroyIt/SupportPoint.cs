using UnityEngine;

namespace DestroyIt;

public class SupportPoint : MonoBehaviour
{
	public int breakForce = 750;

	public int breakTorque = 750;

	private bool _canSupport = true;

	private void Start()
	{
		if (base.transform.parent == null)
		{
			Debug.Log("[" + base.name + "] has no parent. Support points are designed to be children of objects that have attached colliders.");
			_canSupport = false;
		}
		else if (base.transform.parent.GetComponent<Collider>() == null || !base.transform.parent.GetComponent<Collider>().enabled)
		{
			Debug.Log("[" + base.transform.parent.name + "] has a support point but no enabled collider. Support points only work on objects with colliders.");
			_canSupport = false;
		}
		else if (base.transform.parent.GetComponent<Collider>().attachedRigidbody == null)
		{
			Debug.Log("[" + base.transform.parent.name + "] has a support point but no attached rigidbody. Support points only work on objects with rigidbodies.");
			_canSupport = false;
		}
		if (_canSupport)
		{
			RaycastHit[] array = Physics.RaycastAll(new Ray(base.transform.position - base.transform.forward * 0.025f, base.transform.forward), 0.075f);
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].collider.attachedRigidbody == null) && !array[i].collider.isTrigger)
				{
					Vector3 axis = base.transform.parent.transform.InverseTransformDirection(base.transform.TransformDirection(Vector3.forward));
					base.transform.parent.GetComponent<Collider>().attachedRigidbody.gameObject.AddStiffJoint(array[i].collider.attachedRigidbody, base.transform.localPosition, axis, breakForce, breakTorque);
					break;
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position - base.transform.forward * 0.025f, 0.01f);
		Gizmos.DrawRay(base.transform.position - base.transform.forward * 0.025f, base.transform.forward * 0.075f);
	}
}
