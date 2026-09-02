using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Audio;

[AddComponentMenu("UI/Containers/Addons/UIContainer Audio")]
public class UIContainerAudio : BaseUIContainerAnimator
{
	[SerializeField]
	private AudioSource AudioSource;

	[SerializeField]
	private AudioClip ShowAudioClip;

	[SerializeField]
	private AudioClip HideAudioClip;

	public AudioSource audioSource => AudioSource;

	public bool hasAudioSource => AudioSource != null;

	public AudioClip showAudioClip => ShowAudioClip;

	public AudioClip hideAudioClip => HideAudioClip;

	public override void StopAllReactions()
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
		}
	}

	public override void Show()
	{
		if (hasAudioSource && !(showAudioClip == null))
		{
			audioSource.Stop();
			audioSource.clip = showAudioClip;
			audioSource.Play();
		}
	}

	public override void ReverseShow()
	{
		Hide();
	}

	public override void Hide()
	{
		if (hasAudioSource && !(hideAudioClip == null))
		{
			audioSource.Stop();
			audioSource.clip = hideAudioClip;
			audioSource.Play();
		}
	}

	public override void ReverseHide()
	{
		Show();
	}

	public override void UpdateSettings()
	{
	}

	public override void InstantShow()
	{
	}

	public override void InstantHide()
	{
	}

	public override void ResetToStartValues(bool forced = false)
	{
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		return null;
	}
}
