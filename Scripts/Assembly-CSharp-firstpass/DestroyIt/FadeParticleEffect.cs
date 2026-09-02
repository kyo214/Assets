using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class FadeParticleEffect : MonoBehaviour
{
	[Range(0f, 60f)]
	public float delaySeconds = 10f;

	[Range(0f, 60f)]
	public float fadeSeconds = 2f;

	[Range(1f, 30f)]
	public int updatesPerSecond = 15;

	private float fadeTiming;

	private int stepCounter;

	private float totalFadeSteps;

	private List<ParticleEffectPropertyBag> particleEffectProperties;

	private void Start()
	{
		particleEffectProperties = new List<ParticleEffectPropertyBag>();
		fadeTiming = 1f / (float)updatesPerSecond;
		totalFadeSteps = fadeSeconds / fadeTiming;
		Object.Destroy(base.transform.gameObject, delaySeconds + fadeSeconds);
		if (fadeSeconds > 0f)
		{
			InvokeRepeating("Fade", delaySeconds, fadeTiming);
		}
	}

	private void Fade()
	{
		stepCounter++;
		if (particleEffectProperties.Count == 0)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				Material material = particleSystem.GetComponent<Renderer>().material;
				if (material.HasProperty("_TintColor"))
				{
					Color color = material.GetColor("_TintColor");
					particleEffectProperties.Add(new ParticleEffectPropertyBag
					{
						ParticleSystem = particleSystem,
						TintColorStart = color
					});
				}
			}
		}
		foreach (ParticleEffectPropertyBag particleEffectProperty in particleEffectProperties)
		{
			Material material2 = particleEffectProperty.ParticleSystem.GetComponent<Renderer>().material;
			if (material2.HasProperty("_TintColor"))
			{
				Color tintColorStart = particleEffectProperty.TintColorStart;
				float num = (1f - particleEffectProperty.TintColorStart.a) / 1f / totalFadeSteps;
				float a = Mathf.Clamp01(particleEffectProperty.TintColorStart.a - num * (float)stepCounter);
				Color value = new Color(tintColorStart.r, tintColorStart.g, tintColorStart.b, a);
				material2.SetColor("_TintColor", value);
			}
		}
	}
}
