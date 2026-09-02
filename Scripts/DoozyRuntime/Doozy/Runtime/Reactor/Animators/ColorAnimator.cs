using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animators;

[AddComponentMenu("Reactor/Animators/Color Animator")]
public class ColorAnimator : ReactorAnimator
{
	[SerializeField]
	private ReactorColorTarget ColorTarget;

	[SerializeField]
	private ColorAnimation Animation;

	public ReactorColorTarget colorTarget => ColorTarget;

	public bool hasTarget => ColorTarget != null;

	public ColorAnimation animation => Animation ?? (Animation = new ColorAnimation(colorTarget));

	public void FindTarget()
	{
		if (ColorTarget != null)
		{
			if (animation.colorTarget != ColorTarget)
			{
				animation.SetTarget(ColorTarget);
			}
			return;
		}
		ColorTarget = ReactorColorTarget.FindTarget(base.gameObject);
		if (ColorTarget != null)
		{
			animation.SetTarget(ColorTarget);
		}
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.Awake();
			FindTarget();
		}
	}

	public override void Play(PlayDirection playDirection)
	{
		animation.Play(playDirection);
	}

	public override void Play(bool inReverse = false)
	{
		animation.Play(inReverse);
	}

	public override void SetTarget(object target)
	{
		SetTarget(target as ReactorColorTarget);
	}

	public void SetTarget(ReactorColorTarget target)
	{
		animation.SetTarget(target);
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (animation.isActive)
		{
			Stop();
		}
		animation.ResetToStartValues(forced);
		if (!(colorTarget == null))
		{
			colorTarget.color = animation.startColor;
		}
	}

	public override void UpdateSettings()
	{
		SetTarget(colorTarget);
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
		ColorTargetReaction colorTargetReaction = animation.animation;
		colorTargetReaction.Reset();
		colorTargetReaction.enabled = true;
		colorTargetReaction.fromReferenceValue = ReferenceValue.StartValue;
		colorTargetReaction.toReferenceValue = ReferenceValue.StartValue;
		colorTargetReaction.settings.duration = 0.5f;
	}
}
