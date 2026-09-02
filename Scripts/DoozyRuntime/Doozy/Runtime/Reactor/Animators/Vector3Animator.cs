using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animators;

[AddComponentMenu("Reactor/Animators/Vector3 Animator")]
public class Vector3Animator : ReflectedValueAnimator
{
	public ReflectedVector3 ValueTarget;

	[SerializeField]
	private Vector3Animation Animation;

	public bool isValid => ValueTarget.IsValid();

	public Vector3Animation animation => Animation ?? (Animation = new Vector3Animation(ValueTarget));

	public override void Play(PlayDirection playDirection)
	{
		animation.Play(playDirection);
	}

	public override void Play(bool inReverse = false)
	{
		animation.Play(inReverse);
	}

	public override void SetTarget(object reflectedValue)
	{
		SetTarget(reflectedValue as ReflectedVector3);
	}

	public void SetTarget(ReflectedVector3 reflectedValue)
	{
		animation.SetTarget(reflectedValue);
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (animation.isActive)
		{
			Stop();
		}
		animation.ResetToStartValues(forced);
		if (ValueTarget != null && ValueTarget.IsValid())
		{
			ValueTarget.SetValue(animation.startValue);
		}
	}

	public override void UpdateSettings()
	{
		SetTarget(ValueTarget);
		if (animation.isPlaying)
		{
			UpdateValues();
		}
	}

	public override float GetStartDelay()
	{
		if (!animation.animation.isActive)
		{
			return animation.animation.settings.GetStartDelay();
		}
		return animation.animation.startDelay;
	}

	public override float GetDuration()
	{
		if (!animation.animation.isActive)
		{
			return animation.animation.settings.GetDuration();
		}
		return animation.animation.duration;
	}

	public override float GetTotalDuration()
	{
		return GetStartDelay() + GetDuration();
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>
		{
			new T()
		};
		animation.animation.SetHeartbeat(list[0]);
		return list;
	}

	public override void UpdateValues()
	{
		animation.UpdateValues();
	}

	public override void PlayToProgress(float toProgress)
	{
		animation.PlayToProgress(toProgress);
	}

	public override void PlayFromProgress(float fromProgress)
	{
		animation.PlayFromProgress(fromProgress);
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		animation.PlayFromToProgress(fromProgress, toProgress);
	}

	public override void Stop()
	{
		animation.Stop();
	}

	public override void Finish()
	{
		animation.Finish();
	}

	public override void Reverse()
	{
		animation.Reverse();
	}

	public override void Rewind()
	{
		animation.Rewind();
	}

	public override void Pause()
	{
		animation.Pause();
	}

	public override void Resume()
	{
		animation.Resume();
	}

	public override void SetProgressAtOne()
	{
		animation.SetProgressAtOne();
	}

	public override void SetProgressAtZero()
	{
		animation.SetProgressAtZero();
	}

	public override void SetProgressAt(float targetProgress)
	{
		animation.SetProgressAt(targetProgress);
	}

	protected override void Recycle()
	{
		animation?.Recycle();
	}

	private void ResetAnimation()
	{
		ReflectedVector3Reaction reflectedVector3Reaction = animation.animation;
		reflectedVector3Reaction.Reset();
		reflectedVector3Reaction.enabled = true;
		reflectedVector3Reaction.fromReferenceValue = ReferenceValue.CustomValue;
		reflectedVector3Reaction.fromCustomValue = Vector3.zero;
		reflectedVector3Reaction.toReferenceValue = ReferenceValue.CustomValue;
		reflectedVector3Reaction.toCustomValue = Vector3.one;
		reflectedVector3Reaction.settings.duration = 1f;
	}
}
