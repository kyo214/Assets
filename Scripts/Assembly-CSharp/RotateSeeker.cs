using UnityEngine;

public class RotateSeeker : MonoBehaviour
{
	public float Speed = 20f;

	private Transform trans;

	private void Start()
	{
		trans = GetComponent<Transform>();
	}

	private void Update()
	{
		trans.Rotate(0f, Time.deltaTime * Speed, 0f, Space.World);
	}
}
