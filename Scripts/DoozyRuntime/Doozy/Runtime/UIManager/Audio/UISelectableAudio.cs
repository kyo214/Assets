using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Audio;

[AddComponentMenu("UI/Components/Addons/UISelectable Audio")]
public class UISelectableAudio : BaseUISelectableAnimator
{
	[SerializeField]
	private AudioSource AudioSource;

	[SerializeField]
	private AudioClip NormalAudioClip;

	[SerializeField]
	private AudioClip HighlightedAudioClip;

	[SerializeField]
	private AudioClip PressedAudioClip;

	[SerializeField]
	private AudioClip SelectedAudioClip;

	[SerializeField]
	private AudioClip DisabledAudioClip;

	public AudioSource audioSource => AudioSource;

	public bool hasAudioSource => AudioSource != null;

	public AudioClip normalAudioClip => NormalAudioClip;

	public AudioClip highlightedAudioClip => HighlightedAudioClip;

	public AudioClip pressedAudioClip => PressedAudioClip;

	public AudioClip selectedAudioClip => SelectedAudioClip;

	public AudioClip disabledAudioClip => DisabledAudioClip;

	private bool initialized { get; set; }

	protected override void OnEnable()
	{
		initialized = false;
		base.OnEnable();
	}

	public override void StopAllReactions()
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
		}
	}

	public override bool IsStateEnabled(UISelectionState state)
	{
		return true;
	}

	public override void Play(UISelectionState state)
	{
		if (!initialized)
		{
			initialized = true;
			if (state == UISelectionState.Normal)
			{
				return;
			}
		}
		if (!hasAudioSource)
		{
			return;
		}
		switch (state)
		{
		default:
			return;
		case UISelectionState.Normal:
			if (normalAudioClip == null)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = normalAudioClip;
			break;
		case UISelectionState.Highlighted:
			if (highlightedAudioClip == null)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = highlightedAudioClip;
			break;
		case UISelectionState.Pressed:
			if (pressedAudioClip == null)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = pressedAudioClip;
			break;
		case UISelectionState.Selected:
			if (selectedAudioClip == null)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = selectedAudioClip;
			break;
		case UISelectionState.Disabled:
			if (disabledAudioClip == null)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = disabledAudioClip;
			break;
		}
		if (!(audioSource.clip == null))
		{
			audioSource.Play();
		}
	}

	public override void UpdateSettings()
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
