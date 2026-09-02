using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(AudioListener))]
public class MMAudioListener : MonoBehaviour
{
	protected AudioListener _audioListener;

	protected AudioListener[] _otherListeners;

	protected virtual void OnEnable()
	{
		_audioListener = base.gameObject.GetComponent<AudioListener>();
		_otherListeners = Object.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
		AudioListener[] otherListeners = _otherListeners;
		foreach (AudioListener audioListener in otherListeners)
		{
			if (audioListener != null && audioListener != _audioListener)
			{
				audioListener.enabled = false;
			}
		}
	}
}
