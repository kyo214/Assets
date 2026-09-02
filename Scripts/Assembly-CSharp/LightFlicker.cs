using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	public bool flicker = true;

	public float flickerIntensity = 0.5f;

	private float baseIntensity;

	private Light lightComp;

	private void Awake()
	{
		lightComp = base.gameObject.GetComponent<Light>();
		baseIntensity = lightComp.intensity;
	}

	private void Update()
	{
		if (flicker)
		{
			float t = Mathf.PerlinNoise(Random.Range(0f, 1000f), Time.time);
			lightComp.intensity = Mathf.Lerp(baseIntensity - flickerIntensity, baseIntensity, t);
		}
	}
}
