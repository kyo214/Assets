using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
	public Vector3 endAngles;

	public float speed = 0.5f;

	private Vector3 startAngles;

	private void Start()
	{
		startAngles = base.transform.eulerAngles;
	}

	private void Update()
	{
		float t = Mathf.PingPong(Time.time * speed, 1f);
		t = Mathf.SmoothStep(0f, 1f, t);
		Vector3 eulerAngles = Vector3.Slerp(startAngles, endAngles, t);
		base.transform.eulerAngles = eulerAngles;
	}
}
