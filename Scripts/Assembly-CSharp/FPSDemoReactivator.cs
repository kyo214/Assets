using UnityEngine;

public class FPSDemoReactivator : MonoBehaviour
{
	public float StartDelay;

	public float TimeDelayToReactivate = 3f;

	private void Start()
	{
		InvokeRepeating("Reactivate", StartDelay, TimeDelayToReactivate);
	}

	private void Reactivate()
	{
		base.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
	}
}
