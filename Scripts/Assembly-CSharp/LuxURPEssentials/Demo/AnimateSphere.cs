using UnityEngine;

namespace LuxURPEssentials.Demo;

public class AnimateSphere : MonoBehaviour
{
	private Transform trans;

	private float yPos;

	private void Start()
	{
		trans = GetComponent<Transform>();
		yPos = trans.position.y;
	}

	private void Update()
	{
		Vector3 position = trans.position;
		position.y = yPos + Mathf.Sin(Time.time) * 2f;
		trans.position = position;
	}
}
