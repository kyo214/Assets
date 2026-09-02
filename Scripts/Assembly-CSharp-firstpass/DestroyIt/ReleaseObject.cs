using UnityEngine;

namespace DestroyIt;

public class ReleaseObject : MonoBehaviour
{
	public GameObject objectToRelease;

	public Vector3 angularVelocity;

	public Vector3 forceToAdd;

	private Vector3 _velocityLastUpdate;

	private void Start()
	{
		_velocityLastUpdate = GetComponent<Rigidbody>().velocity;
	}

	private void FixedUpdate()
	{
		if (((GetComponent<Rigidbody>().velocity - _velocityLastUpdate) / GetComponent<Rigidbody>().mass).magnitude > 0.3f && objectToRelease != null)
		{
			Release();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 2f && objectToRelease != null)
		{
			Release();
		}
	}

	private void Release()
	{
		objectToRelease.GetComponent<Rigidbody>().isKinematic = false;
		objectToRelease.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
		objectToRelease.GetComponent<Rigidbody>().WakeUp();
		if (forceToAdd != Vector3.zero)
		{
			objectToRelease.GetComponent<Rigidbody>().AddForce(forceToAdd);
		}
		Object.Destroy(this);
	}
}
