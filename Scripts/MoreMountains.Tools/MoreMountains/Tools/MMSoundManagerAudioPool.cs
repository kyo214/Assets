using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[Serializable]
public class MMSoundManagerAudioPool
{
	protected List<AudioSource> _pool;

	public virtual void FillAudioSourcePool(int poolSize, Transform parent)
	{
		if (_pool == null)
		{
			_pool = new List<AudioSource>();
		}
		if (poolSize <= 0 || _pool.Count >= poolSize)
		{
			return;
		}
		foreach (AudioSource item2 in _pool)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		for (int i = 0; i < poolSize; i++)
		{
			GameObject gameObject = new GameObject("MMAudioSourcePool_" + i);
			SceneManager.MoveGameObjectToScene(gameObject.gameObject, parent.gameObject.scene);
			AudioSource item = gameObject.AddComponent<AudioSource>();
			gameObject.transform.SetParent(parent);
			gameObject.SetActive(value: false);
			_pool.Add(item);
		}
	}

	public virtual IEnumerator AutoDisableAudioSource(float duration, AudioSource source, AudioClip clip, bool doNotAutoRecycleIfNotDonePlaying)
	{
		yield return MMCoroutine.WaitFor(duration);
		if (source.clip != clip)
		{
			yield break;
		}
		if (doNotAutoRecycleIfNotDonePlaying)
		{
			while (source.time < source.clip.length)
			{
				yield return null;
			}
		}
		source.gameObject.SetActive(value: false);
	}

	public virtual AudioSource GetAvailableAudioSource(bool poolCanExpand, Transform parent)
	{
		foreach (AudioSource item in _pool)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				item.gameObject.SetActive(value: true);
				return item;
			}
		}
		if (poolCanExpand)
		{
			GameObject gameObject = new GameObject("MMAudioSourcePool_" + _pool.Count);
			SceneManager.MoveGameObjectToScene(gameObject.gameObject, parent.gameObject.scene);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			gameObject.transform.SetParent(parent);
			gameObject.SetActive(value: true);
			_pool.Add(audioSource);
			return audioSource;
		}
		return null;
	}

	public virtual bool FreeSound(AudioSource sourceToStop)
	{
		foreach (AudioSource item in _pool)
		{
			if (item == sourceToStop)
			{
				item.Stop();
				item.gameObject.SetActive(value: false);
				return true;
			}
		}
		return false;
	}
}
