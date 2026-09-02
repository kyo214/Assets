using UnityEngine;

namespace DestroyIt;

public class FloatUp : MonoBehaviour
{
	[Range(0f, 10f)]
	public float floatSpeed = 5f;

	private float checkFrequency = 0.05f;

	private float nextUpdateCheck;

	private void Start()
	{
		nextUpdateCheck = Time.time + checkFrequency;
	}

	private void Update()
	{
		if (Time.time > nextUpdateCheck)
		{
			base.gameObject.transform.position = base.gameObject.transform.position + Vector3.up * floatSpeed;
			nextUpdateCheck = Time.time + checkFrequency;
		}
	}
}
