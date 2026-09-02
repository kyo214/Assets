using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private float fps;

	public TextMeshProUGUI counter;

	private void Start()
	{
		counter.text = "";
		timeleft = updateInterval;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			fps = accum / (float)frames;
			counter.text = fps.ToString();
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
