using UnityEngine;

namespace DestroyIt;

public class TurnOnCamera : MonoBehaviour
{
	public Camera mainCamera;

	public void Awake()
	{
		mainCamera.gameObject.SetActive(value: true);
	}
}
