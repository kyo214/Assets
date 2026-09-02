using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations;

[Serializable]
public class ColorAnimation : ReactorAnimation
{
	[SerializeField]
	private ColorTargetReaction Animation;

	[SerializeField]
	private bool UseCustomStartValue;

	[SerializeField]
	private Color CustomStartValue = Color.white;

	public ReactorColorTarget colorTarget { get; private set; }

	public override bool hasTarget => colorTarget != null;

	public ColorTargetReaction animation => Animation ?? (Animation = Reaction.Get<ColorTargetReaction>());

	public bool useCustomStartValue
	{
		get
		{
			return UseCustomStartValue;
		}
		set
		{
			UseCustomStartValue = value;
			if (value)
			{
				animation.startColor = customStartValue;
			}
			else if (colorTarget != null)
			{
				animation.startColor = colorTarget.color;
			}
		}
	}

	public Color customStartValue
	{
		get
		{
			return CustomStartValue;
		}
		set
		{
			useCustomStartValue = true;
			CustomStartValue = value;
			animation.startColor = value;
		}
	}

	public Color startColor
	{
		get
		{
			return animation.startColor;
		}
		set
		{
			animation.startColor = value;
		}
	}

	public override bool isEnabled => animation.enabled;

	public override bool isIdle => animation.isIdle;

	public override bool isActive => animation.isActive;

	public override bool isPaused => animation.isPaused;

	public override bool isPlaying => animation.isPlaying;

	public override bool inStartDelay => animation.inStartDelay;

	public override bool inLoopDelay => animation.inLoopDelay;

	public ColorAnimation(ReactorColorTarget target = null)
	{
		if (!(target == null))
		{
			SetTarget(target);
		}
	}

	public void SetTarget(ReactorColorTarget target)
	{
		colorTarget = null;
		if (!target)
		{
			throw new NullReferenceException("target");
		}
		colorTarget = target;
		Initialize();
	}

	public void Initialize()
	{
		animation?.Stop(silent: true);
		if (Animation == null)
		{
			Animation = Reaction.Get<ColorTargetReaction>();
		}
		animation?.SetTarget(colorTarget);
		if (Application.isPlaying)
		{
			ResetToStartValues();
		}
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
		Reaction.StopAllReactionsByTargetObject(colorTarget, silent: true);
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
		if (!(colorTarget == null))
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
		if (!(colorTarget == null) && (forced || animation.enabled))
		{
			animation.SetValue(useCustomStartValue ? customStartValue : startColor);
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
			ColorTargetReaction colorTargetReaction = animation;
			colorTargetReaction.OnPlayCallback = (ReactionCallback)Delegate.Combine(colorTargetReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			ColorTargetReaction colorTargetReaction2 = animation;
			colorTargetReaction2.OnStopCallback = (ReactionCallback)Delegate.Combine(colorTargetReaction2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			ColorTargetReaction colorTargetReaction3 = animation;
			colorTargetReaction3.OnFinishCallback = (ReactionCallback)Delegate.Combine(colorTargetReaction3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}

	protected override void UnregisterOnPlayCallbacks()
	{
		if (animation.enabled)
		{
			ColorTargetReaction colorTargetReaction = animation;
			colorTargetReaction.OnPlayCallback = (ReactionCallback)Delegate.Remove(colorTargetReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
	}

	protected override void UnregisterOnStopCallbacks()
	{
		if (animation.enabled)
		{
			ColorTargetReaction colorTargetReaction = animation;
			colorTargetReaction.OnStopCallback = (ReactionCallback)Delegate.Remove(colorTargetReaction.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
	}

	protected override void UnregisterOnFinishCallbacks()
	{
		if (animation.enabled)
		{
			ColorTargetReaction colorTargetReaction = animation;
			colorTargetReaction.OnFinishCallback = (ReactionCallback)Delegate.Remove(colorTargetReaction.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}
}
