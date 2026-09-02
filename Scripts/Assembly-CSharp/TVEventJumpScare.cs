using System.Collections;
using Toked;
using UnityEngine;

public class TVEventJumpScare : MonoBehaviour
{
	[SerializeField]
	private GameObject screenBurek;

	[SerializeField]
	private GameObject screenCewek;

	[SerializeField]
	private bool isCollided;

	private void OnTriggerEnter(Collider other)
	{
		if (!isCollided && other.CompareTag("Player"))
		{
			isCollided = true;
			StartCoroutine(StartEvent());
		}
	}

	private IEnumerator StartEvent()
	{
		AudioManager.PlaySFXTransform("TV-on", base.transform, isLocalPlayerTrigger: false);
		screenBurek.SetActive(value: true);
		yield return new WaitForSeconds(3f);
		screenBurek.SetActive(value: false);
		screenCewek.SetActive(value: true);
		AudioManager.PlaySFXTransform("female-screams", base.transform, isLocalPlayerTrigger: false);
		yield return new WaitForSeconds(4f);
		screenCewek.SetActive(value: false);
		screenBurek.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		screenBurek.SetActive(value: false);
	}
}
