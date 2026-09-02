using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations;

[Serializable]
public class SpriteAnimation : ReactorAnimation
{
	[SerializeField]
	private List<Sprite> Sprites = new List<Sprite>();

	[SerializeField]
	private SpriteTargetReaction Animation;

	public ReactorSpriteTarget spriteTarget { get; private set; }

	public List<Sprite> sprites => Sprites;

	public override bool hasTarget => spriteTarget != null;

	public SpriteTargetReaction animation => Animation ?? (Animation = Reaction.Get<SpriteTargetReaction>());

	public int startFrame
	{
		get
		{
			return animation.startFrame;
		}
		set
		{
			animation.startFrame = value;
		}
	}

	public override bool isEnabled => animation.enabled;

	public override bool isIdle => animation.isIdle;

	public override bool isActive => animation.isActive;

	public override bool isPaused => animation.isPaused;

	public override bool isPlaying => animation.isPlaying;

	public override bool inStartDelay => animation.inStartDelay;

	public override bool inLoopDelay => animation.inLoopDelay;

	public SpriteAnimation(ReactorSpriteTarget target = null)
	{
		if (!(target == null))
		{
			SetTarget(target);
		}
	}

	public SpriteAnimation SetSprites(IEnumerable<Sprite> spriteEnumerable)
	{
		if (spriteEnumerable == null)
		{
			return this;
		}
		Sprites.Clear();
		Sprites.AddRange(spriteEnumerable);
		return UpdateAnimationSprites();
	}

	public SpriteAnimation UpdateAnimationSprites()
	{
		animation.SetSprites(Sprites, setFirstFrame: false);
		return this;
	}

	public SpriteAnimation SortSpritesAz()
	{
		Sprites = Sprites.OrderBy((Sprite item) => item.name).ToList();
		return UpdateAnimationSprites();
	}

	public SpriteAnimation SortSpritesZa()
	{
		Sprites = Sprites.OrderByDescending((Sprite item) => item.name).ToList();
		return UpdateAnimationSprites();
	}

	public void SetTarget(ReactorSpriteTarget target)
	{
		spriteTarget = null;
		if (!target)
		{
			throw new NullReferenceException("target");
		}
		spriteTarget = target;
		Initialize();
	}

	public void Initialize()
	{
		animation?.Stop(silent: true);
		if (Animation == null)
		{
			Animation = Reaction.Get<SpriteTargetReaction>();
		}
		animation?.SetTarget(spriteTarget);
		UpdateValues();
	}

	public override void Recycle()
	{
		animation?.Recycle();
	}

	public override void UpdateValues()
	{
		UpdateAnimationSprites();
		animation.UpdateValues();
	}

	public override void StopAllReactionsOnTarget()
	{
		Reaction.StopAllReactionsByTargetObject(spriteTarget, silent: true);
	}

	public override void SetProgressAt(float targetProgress)
	{
		base.SetProgressAt(targetProgress);
		if (animation.enabled)
		{
			UpdateAnimationSprites();
			animation.SetProgressAt(targetProgress);
		}
	}

	public override void PlayToProgress(float toProgress)
	{
		base.PlayToProgress(toProgress);
		if (animation.enabled)
		{
			UpdateAnimationSprites();
			animation.PlayToProgress(toProgress);
		}
	}

	public override void PlayFromProgress(float fromProgress)
	{
		base.PlayFromProgress(fromProgress);
		if (animation.enabled)
		{
			UpdateAnimationSprites();
			animation.PlayFromProgress(fromProgress);
		}
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		base.PlayFromToProgress(fromProgress, toProgress);
		if (animation.enabled)
		{
			UpdateAnimationSprites();
			animation.PlayFromToProgress(fromProgress, toProgress);
		}
	}

	public override void Play(bool inReverse = false)
	{
		if (!(spriteTarget == null))
		{
			RegisterCallbacks();
			if (!isActive)
			{
				StopAllReactionsOnTarget();
			}
			if (animation.enabled)
			{
				UpdateAnimationSprites();
				animation.Play(inReverse);
			}
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (!(spriteTarget == null) && (forced || animation.enabled))
		{
			UpdateAnimationSprites();
			animation.SetValue(startFrame);
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
			UpdateAnimationSprites();
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
			SpriteTargetReaction spriteTargetReaction = animation;
			spriteTargetReaction.OnPlayCallback = (ReactionCallback)Delegate.Combine(spriteTargetReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			SpriteTargetReaction spriteTargetReaction2 = animation;
			spriteTargetReaction2.OnStopCallback = (ReactionCallback)Delegate.Combine(spriteTargetReaction2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			SpriteTargetReaction spriteTargetReaction3 = animation;
			spriteTargetReaction3.OnFinishCallback = (ReactionCallback)Delegate.Combine(spriteTargetReaction3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}

	protected override void UnregisterOnPlayCallbacks()
	{
		if (animation.enabled)
		{
			SpriteTargetReaction spriteTargetReaction = animation;
			spriteTargetReaction.OnPlayCallback = (ReactionCallback)Delegate.Remove(spriteTargetReaction.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
	}

	protected override void UnregisterOnStopCallbacks()
	{
		if (animation.enabled)
		{
			SpriteTargetReaction spriteTargetReaction = animation;
			spriteTargetReaction.OnStopCallback = (ReactionCallback)Delegate.Remove(spriteTargetReaction.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
	}

	protected override void UnregisterOnFinishCallbacks()
	{
		if (animation.enabled)
		{
			SpriteTargetReaction spriteTargetReaction = animation;
			spriteTargetReaction.OnFinishCallback = (ReactionCallback)Delegate.Remove(spriteTargetReaction.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}
}
