using UnityEngine;

namespace DestroyIt;

public class ResetSkybox : MonoBehaviour
{
	private void Start()
	{
		if (RenderSettings.skybox.HasProperty("_Blend"))
		{
			RenderSettings.skybox.SetFloat("_Blend", 0f);
		}
		Object.Destroy(this);
	}
}
