using UnityEngine;

namespace Lux_SRP_GrassDisplacement;

public class RotateAndMove : MonoBehaviour
{
	public bool Rotate = true;

	public bool MoveUpDown;

	private float posy;

	private Transform trans;

	private void OnEnable()
	{
		trans = GetComponent<Transform>();
		posy = trans.position.y;
	}

	private void Update()
	{
		if (Rotate)
		{
			trans.Rotate(0f, 10f * Time.deltaTime, 0f, Space.World);
		}
		if (MoveUpDown)
		{
			Vector3 position = trans.position;
			position.y = posy + 1f + Mathf.Sin(Time.time);
			trans.position = position;
		}
	}
}
