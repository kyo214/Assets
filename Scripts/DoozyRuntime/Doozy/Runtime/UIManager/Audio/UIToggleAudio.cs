using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Audio;

[AddComponentMenu("UI/Components/Addons/UIToggle Audio")]
public class UIToggleAudio : BaseUIToggleAnimator
{
	[SerializeField]
	private AudioSource AudioSource;

	[SerializeField]
	private AudioClip OnAudioClip;

	[SerializeField]
	private AudioClip OffAudioClip;

	public AudioSource audioSource => AudioSource;

	public bool hasAudioSource => AudioSource != null;

	public AudioClip onAudioClip => OnAudioClip;

	public AudioClip offAudioClip => OffAudioClip;

	protected override bool onAnimationIsActive
	{
		get
		{
			if (base.hasController && hasAudioSource && base.controller.isOn && onAudioClip != null)
			{
				return audioSource.isPlaying;
			}
			return false;
		}
	}

	protected override bool offAnimationIsActive
	{
		get
		{
			if (base.hasController && hasAudioSource && !base.controller.isOn && offAudioClip != null)
			{
				return audioSource.isPlaying;
			}
			return false;
		}
	}

	protected override UnityAction playOnAnimation => () =>
	{
		if (hasAudioSource && !(onAudioClip == null))
		{
			audioSource.Stop();
			audioSource.clip = onAudioClip;
			audioSource.Play();
		}
	};

	protected override UnityAction playOffAnimation => () =>
	{
		if (hasAudioSource && !(offAudioClip == null))
		{
			audioSource.Stop();
			audioSource.clip = offAudioClip;
			audioSource.Play();
		}
	};

	protected override UnityAction reverseOnAnimation => () =>
	{
		playOffAnimation();
	};

	protected override UnityAction reverseOffAnimation => () =>
	{
		playOnAnimation();
	};

	protected override UnityAction instantPlayOnAnimation => () =>
	{
	};

	protected override UnityAction instantPlayOffAnimation => () =>
	{
	};

	protected override UnityAction stopOnAnimation => () =>
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
		}
	};

	protected override UnityAction stopOffAnimation => () =>
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
		}
	};

	protected override UnityAction addResetToOnStateCallback => () =>
	{
	};

	protected override UnityAction removeResetToOnStateCallback => () =>
	{
	};

	protected override UnityAction addResetToOffStateCallback => () =>
	{
	};

	protected override UnityAction removeResetToOffStateCallback => () =>
	{
	};

	public override void UpdateSettings()
	{
		if (hasAudioSource && base.hasController)
		{
			audioSource.playOnAwake = false;
			audioSource.loop = false;
			audioSource.spatialBlend = 0f;
			audioSource.dopplerLevel = 0f;
			audioSource.clip = (base.controller.isOn ? onAudioClip : offAudioClip);
		}
	}

	public override void StopAllReactions()
	{
		if (hasAudioSource)
		{
			audioSource.Stop();
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
	}

	public override List<Heartbeat> SetHeartbeat<Theartbeat>()
	{
		return null;
	}
}
