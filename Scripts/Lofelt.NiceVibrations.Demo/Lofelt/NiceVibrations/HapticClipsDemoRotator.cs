using UnityEngine;

namespace Lofelt.NiceVibrations;

public class HapticClipsDemoRotator : MonoBehaviour
{
	public Vector3 RotationSpeed = new Vector3(0f, 0f, 100f);

	protected void Update()
	{
		base.transform.Rotate(RotationSpeed * Time.deltaTime, Space.Self);
	}
}
