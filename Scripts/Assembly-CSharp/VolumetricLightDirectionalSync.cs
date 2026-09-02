using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class VolumetricLightDirectionalSync : MonoBehaviour
{
	public Light directionalLight;

	private Light fakeLight;

	private void OnEnable()
	{
		fakeLight = GetComponent<Light>();
	}

	private void LateUpdate()
	{
		if (directionalLight != null)
		{
			base.transform.forward = directionalLight.transform.forward;
			fakeLight.color = directionalLight.color;
			fakeLight.intensity = directionalLight.intensity;
		}
	}
}
