using UnityEngine;

namespace DestroyIt;

public class WhenDestroyedPlaySound : MonoBehaviour
{
	public AudioClip clip;

	private Destructible _destObj;

	private void Start()
	{
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.DestroyedEvent += OnDestroyed;
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.DestroyedEvent -= OnDestroyed;
		}
	}

	private void OnDestroyed()
	{
		GameObject obj = new GameObject("Audio Source");
		obj.transform.position = _destObj.transform.position;
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		obj.AddComponent<DestroyAfter>().seconds = 5f;
		audioSource.PlayOneShot(clip);
	}
}
