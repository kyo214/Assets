using UnityEngine;

namespace DissolveExample;

public class Follow : MonoBehaviour
{
	[Range(0f, 5f)]
	public float speed;

	public float height;

	private Vector3 pos;

	private void Start()
	{
		pos = base.transform.position;
	}

	private void Update()
	{
		float num = Mathf.PingPong(Time.time * speed, height);
		base.transform.position = pos + num * Vector3.up;
	}
}
