using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chronos.Example;

public class ExamplePhysics : ExampleBaseBehaviour
{
	public float delay = 5f;

	public float amount = 10f;

	private List<GameObject> spawned = new List<GameObject>();

	private void Start()
	{
		StartCoroutine(SpawnCoroutine());
	}

	private IEnumerator SpawnCoroutine()
	{
		while (true)
		{
			Spawn();
			yield return base.time.WaitForSeconds(delay);
		}
	}

	private void Spawn()
	{
		foreach (GameObject item in spawned)
		{
			Object.Destroy(item);
		}
		spawned.Clear();
		for (int i = 0; (float)i < amount; i++)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.position += new Vector3
			{
				x = Random.Range(-1f, 1f),
				y = 2 * i,
				z = Random.Range(-1f, 1f)
			};
			gameObject.AddComponent<Rigidbody>();
			Timeline timeline = gameObject.AddComponent<Timeline>();
			timeline.mode = TimelineMode.Global;
			timeline.globalClockKey = base.time.globalClockKey;
			timeline.rewindable = base.time.rewindable;
			gameObject.AddComponent<ExampleTimeColor>();
			spawned.Add(gameObject);
		}
	}
}
