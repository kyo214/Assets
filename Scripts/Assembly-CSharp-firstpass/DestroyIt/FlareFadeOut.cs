using UnityEngine;

namespace DestroyIt;

public class FlareFadeOut : MonoBehaviour
{
	[Range(0f, 10f)]
	public float flareFadeSeconds = 5f;

	private float startBrightness;

	private LensFlare flare;

	private void Start()
	{
		flare = GetComponent<LensFlare>();
		startBrightness = flare.brightness;
	}

	private void Update()
	{
		flare.brightness -= Mathf.Clamp01(Time.deltaTime / (flareFadeSeconds / startBrightness));
		if (flare.brightness <= 0f)
		{
			Object.Destroy(flare);
			Object.Destroy(this);
		}
	}
}
