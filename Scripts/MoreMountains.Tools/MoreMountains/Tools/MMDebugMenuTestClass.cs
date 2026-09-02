using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuTestClass : MonoBehaviour
{
	public string Label;

	private float multiplier;

	private void Start()
	{
		multiplier = Random.Range(0f, 50000f);
	}

	private void Update()
	{
		float num = (Mathf.Sin(Time.time) + 2f) * multiplier;
		MMDebug.DebugOnScreen(Label, num);
	}
}
