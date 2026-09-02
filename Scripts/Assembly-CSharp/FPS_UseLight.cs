using UnityEngine;

public class FPS_UseLight : MonoBehaviour
{
	private void OnEnable()
	{
		_ = Camera.main == null;
	}
}
