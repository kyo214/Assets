using UnityEngine;

public class Fan : MonoBehaviour
{
	public float _speed = 10f;

	private Transform trans;

	private void OnEnable()
	{
		trans = GetComponent<Transform>();
	}

	private void Update()
	{
		trans.Rotate(0f, 0f, _speed * Time.deltaTime, Space.Self);
	}
}
