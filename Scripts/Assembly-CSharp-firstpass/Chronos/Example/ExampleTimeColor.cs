using UnityEngine;

namespace Chronos.Example;

public class ExampleTimeColor : MonoBehaviour
{
	private Color rewind = Color.magenta;

	private Color pause = Color.red;

	private Color slow = Color.yellow;

	private Color play = Color.green;

	private Color accelerate = Color.blue;

	private float slowTimeScale = 0.5f;

	private float rewindTimeScale = -1f;

	private float accelerateTimeScale = 2f;

	private Timeline time;

	private Renderer renderer;

	private ParticleSystem particleSystem;

	private void Awake()
	{
		time = GetComponentInParent<Timeline>();
		renderer = GetComponent<Renderer>();
		particleSystem = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		Color color = Color.white;
		if (time != null)
		{
			float timeScale = time.timeScale;
			color = ((timeScale < 0f) ? Color.Lerp(pause, rewind, Mathf.Max(rewindTimeScale, timeScale) / rewindTimeScale) : ((timeScale < slowTimeScale) ? Color.Lerp(pause, slow, timeScale / slowTimeScale) : ((!(timeScale < 1f)) ? Color.Lerp(play, accelerate, (timeScale - 1f) / (accelerateTimeScale - 1f)) : Color.Lerp(slow, play, (timeScale - slowTimeScale) / (1f - slowTimeScale)))));
		}
		if (renderer != null)
		{
			Material[] materials = GetComponent<Renderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].color = color;
			}
		}
		if (particleSystem != null)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.startColor = color;
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
			colorOverLifetime.color = new ParticleSystem.MinMaxGradient(color);
		}
	}
}
