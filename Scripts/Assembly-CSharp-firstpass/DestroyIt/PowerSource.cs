using UnityEngine;

namespace DestroyIt;

public class PowerSource : MonoBehaviour
{
	public bool hasPower = true;

	public bool cutPowerOnRapidTilt = true;

	public float tiltThreshold = 1.5f;

	private void Update()
	{
		if (cutPowerOnRapidTilt && hasPower && GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().angularVelocity.magnitude > tiltThreshold)
		{
			Object.Destroy(this);
		}
	}
}
