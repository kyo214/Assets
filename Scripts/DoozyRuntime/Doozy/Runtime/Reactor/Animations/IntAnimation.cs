using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations;

[Serializable]
public class IntAnimation : ReactorAnimation
{
	[SerializeField]
	private ReflectedIntReaction Animation;

	public ReflectedInt valueTarget { get; private set; }

	public override bool hasTarget
	{
		get
		{
			if (valueTarget != null)
			{
				return valueTarget.IsValid();
			}
			return false;
		}
	}

	public ReflectedIntReaction animation => Animation ?? (Animation = Reaction.Get<ReflectedIntReaction>());

	public int startValue
	{
		get
		{
			return animation.startValue;
		}
		set
		{
			animation.startValue = value;
		}
	}

	public override bool isEnabled => animation.enabled;

	public override bool isIdle => animation.isIdle;

	public override bool isActive => animation.isActive;

	public override bool isPaused => animation.isPaused;

	public override bool isPlaying => animation.isPlaying;

	public override bool inStartDelay => animation.inStartDelay;

	public override bool inLoopDelay => animation.inLoopDelay;

	public IntAnimation(ReflectedInt reflectedInt)
	{
		valueTarget = reflectedInt;
	}

	public void SetTarget(ReflectedInt target)
	{
		valueTarget = null;
		if (target == null)
		{
			throw new NullReferenceException("target");
		}
		valueTarget = target;
		Initialize();
	}

	public void Initialize()
	{
		animation?.Stop(silent: true);
		if (Animation == null)
		{
			Animation = Reaction.Get<ReflectedIntReaction>();
		}
		valueTarget.Initialize();
		animation?.SetTarget(valueTarget);
		UpdateValues();
	}

	public override void Recycle()
	{
		animation?.Recycle();
	}

	public override void UpdateValues()
	{
		animation.UpdateValues();
	}

	public override void StopAllReactionsOnTarget()
	{
		Reaction.StopAllReactionsByTargetObject(valueTarget.target, silent: true);
	}

	public override void SetProgressAt(float targetProgress)
	{
		base.SetProgressAt(targetProgress);
		if (animation.enabled)
		{
			animation.SetProgressAt(targetProgress);
		}
	}

	public override void PlayToProgress(float toProgress)
	{
		base.PlayToProgress(toProgress);
		if (animation.enabled)
		{
			animation.PlayToProgress(toProgress);
		}
	}

	public override void PlayFromProgress(float fromProgress)
	{
		base.PlayFromProgress(fromProgress);
		if (animation.enabled)
		{
			animation.PlayFromProgress(fromProgress);
		}
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		base.PlayFromToProgress(fromProgress, toProgress);
		if (animation.enabled)
		{
			animation.PlayFromToProgress(fromProgress, toProgress);
		}
	}

	public override void Play(bool inReverse = false)
	{
		if (valueTarget == null)
		{
			return;
		}
		valueTarget.Initialize();
		if (valueTarget.IsValid())
		{
			RegisterCallbacks();
			if (!isActive)
			{
				StopAllReactionsOnTarget();
			}
			if (animation.enabled)
			{
				animation.Play(inReverse);
			}
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (valueTarget != null && !(valueTarget.target == null) && (forced || animation.enabled))
		{
			animation.SetValue(startValue);
		}
	}

	public override void Stop()
	{
		if (animation.isActive || animation.enabled)
		{
			animation.Stop();
		}
		base.Stop();
	}

	public override void Finish()
	{
		if (animation.isActive || animation.enabled)
		{
			animation.Finish();
		}
		base.Finish();
	}

	public override void Reverse()
	{
		if (animation.isActive)
		{
			animation.Reverse();
		}
		else if (animation.enabled)
		{
			animation.Play(PlayDirection.Reverse);
		}
	}

	public override void Rewind()
	{
		if (animation.enabled)
		{
			animation.Rewind();
		}
	}

	public override void Pause()
	{
		animation.Pause();
	}

	public override void Resume()
	{
		animation.Resume();
	}

	protected override void RegisterCallbacks()
	{
		base.RegisterCallbacks();
		if (animation.enabled)
		{
			base.startedReactionsCount++;
			ReflectedIntReaction reflectedIntReaction = animation;
			reflectedIntReaction.OnPlayCallback = (ReactionCallback)Delegate.Combine(reflectedIntReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			ReflectedIntReaction reflectedIntReaction2 = animation;
			reflectedIntReaction2.OnStopCallback = (ReactionCallback)Delegate.Combine(reflectedIntReaction2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			ReflectedIntReaction reflectedIntReaction3 = animation;
			reflectedIntReaction3.OnFinishCallback = (ReactionCallback)Delegate.Combine(reflectedIntReaction3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}

	protected override void UnregisterOnPlayCallbacks()
	{
		if (animation.enabled)
		{
			ReflectedIntReaction reflectedIntReaction = animation;
			reflectedIntReaction.OnPlayCallback = (ReactionCallback)Delegate.Remove(reflectedIntReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
	}

	protected override void UnregisterOnStopCallbacks()
	{
		if (animation.enabled)
		{
			ReflectedIntReaction reflectedIntReaction = animation;
			reflectedIntReaction.OnStopCallback = (ReactionCallback)Delegate.Remove(reflectedIntReaction.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
	}

	protected override void UnregisterOnFinishCallbacks()
	{
		if (animation.enabled)
		{
			ReflectedIntReaction reflectedIntReaction = animation;
			reflectedIntReaction.OnFinishCallback = (ReactionCallback)Delegate.Remove(reflectedIntReaction.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}
}
