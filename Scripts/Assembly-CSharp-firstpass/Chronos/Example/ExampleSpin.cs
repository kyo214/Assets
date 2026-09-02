using UnityEngine;

namespace Chronos.Example;

public class ExampleSpin : ExampleBaseBehaviour
{
	public float speed = 20f;

	private void Update()
	{
		if (base.time.timeScale > 0f)
		{
			base.transform.Rotate(base.time.deltaTime * Vector3.one * speed);
		}
	}
}
