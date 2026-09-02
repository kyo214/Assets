using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animators;

[AddComponentMenu("Reactor/Animators/Sprite Animator")]
public class SpriteAnimator : ReactorAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private SpriteAnimation Animation;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasTarget => SpriteTarget != null;

	public SpriteAnimation animation => Animation ?? (Animation = new SpriteAnimation(spriteTarget));

	public void FindTarget()
	{
		if (SpriteTarget != null)
		{
			if (animation.spriteTarget != SpriteTarget)
			{
				animation.SetTarget(SpriteTarget);
			}
			return;
		}
		SpriteTarget = ReactorSpriteTarget.FindTarget(base.gameObject);
		if (SpriteTarget != null)
		{
			animation.SetTarget(SpriteTarget);
		}
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.Awake();
			animation.UpdateAnimationSprites();
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
		SetTarget(target as ReactorSpriteTarget);
	}

	public void SetTarget(ReactorSpriteTarget target)
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
		if (!(spriteTarget == null))
		{
			spriteTarget.sprite = ((animation.sprites != null && animation.sprites.Count > 0 && animation.startFrame >= 0 && animation.startFrame < animation.sprites.Count - 1) ? animation.sprites[animation.startFrame] : null);
		}
	}

	public override void UpdateSettings()
	{
		SetTarget(spriteTarget);
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
		SpriteTargetReaction spriteTargetReaction = animation.animation;
		spriteTargetReaction.Reset();
		spriteTargetReaction.enabled = true;
		spriteTargetReaction.fromReferenceValue = FrameReferenceValue.FirstFrame;
		spriteTargetReaction.toReferenceValue = FrameReferenceValue.LastFrame;
		spriteTargetReaction.settings.duration = 1f;
	}
}
