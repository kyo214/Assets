using UnityEngine;

namespace MoreMountains.Feel;

public struct StrikePin
{
	public Rigidbody Rb;

	public Vector3 InitialPosition;

	public Quaternion InitialRotation;

	public void ResetPin()
	{
		Rb.transform.position = InitialPosition;
		Rb.transform.rotation = InitialRotation;
		Rb.velocity = Vector3.zero;
		Rb.angularVelocity = Vector3.zero;
	}
}
