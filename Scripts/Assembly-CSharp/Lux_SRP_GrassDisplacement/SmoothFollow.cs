using UnityEngine;

namespace Lux_SRP_GrassDisplacement;

public class SmoothFollow : MonoBehaviour
{
	public Transform targetTransform;

	public float smoothTime = 0.15f;

	private Vector3 velocity = Vector3.zero;

	private void Update()
	{
		Vector3 position = targetTransform.position;
		base.transform.position = Vector3.SmoothDamp(base.transform.position, position, ref velocity, smoothTime);
	}
}
