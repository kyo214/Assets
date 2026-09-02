using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UIToggle Sprite Animator")]
public class UIToggleSpriteAnimator : BaseUIToggleAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private SpriteAnimation OnAnimation;

	[SerializeField]
	private SpriteAnimation OffAnimation;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public SpriteAnimation onAnimation => OnAnimation ?? (OnAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation offAnimation => OffAnimation ?? (OffAnimation = new SpriteAnimation(spriteTarget));

	protected override bool onAnimationIsActive => onAnimation.isActive;

	protected override bool offAnimationIsActive => offAnimation.isActive;

	protected override UnityAction playOnAnimation => () =>
	{
		onAnimation.Play();
	};

	protected override UnityAction playOffAnimation => () =>
	{
		offAnimation.Play();
	};

	protected override UnityAction reverseOnAnimation => () =>
	{
		onAnimation.Reverse();
	};

	protected override UnityAction reverseOffAnimation => () =>
	{
		offAnimation.Reverse();
	};

	protected override UnityAction instantPlayOnAnimation => () =>
	{
		onAnimation.SetProgressAtOne();
	};

	protected override UnityAction instantPlayOffAnimation => () =>
	{
		offAnimation.SetProgressAtOne();
	};

	protected override UnityAction stopOnAnimation => () =>
	{
		onAnimation.Stop();
	};

	protected override UnityAction stopOffAnimation => () =>
	{
		offAnimation.Stop();
	};

	protected override UnityAction addResetToOnStateCallback => () =>
	{
		offAnimation.OnFinishCallback.AddListener(ResetToOnState);
	};

	protected override UnityAction removeResetToOnStateCallback => () =>
	{
		offAnimation.OnFinishCallback.RemoveListener(ResetToOnState);
	};

	protected override UnityAction addResetToOffStateCallback => () =>
	{
		onAnimation.OnFinishCallback.AddListener(ResetToOffState);
	};

	protected override UnityAction removeResetToOffStateCallback => () =>
	{
		onAnimation.OnFinishCallback.RemoveListener(ResetToOffState);
	};

	public void FindTarget()
	{
		if (!(SpriteTarget != null))
		{
			SpriteTarget = ReactorSpriteTarget.FindTarget(base.gameObject);
			UpdateSettings();
		}
	}

	protected override void Awake()
	{
		UpdateSettings();
		FindTarget();
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnAnimation?.Recycle();
		OffAnimation?.Recycle();
	}

	public override void UpdateSettings()
	{
		if (!(spriteTarget == null))
		{
			onAnimation.SetTarget(spriteTarget);
			offAnimation.SetTarget(spriteTarget);
		}
	}

	public override void StopAllReactions()
	{
		onAnimation.Stop();
		offAnimation.Stop();
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (onAnimation.isActive)
		{
			onAnimation.Stop();
		}
		if (offAnimation.isActive)
		{
			offAnimation.Stop();
		}
		onAnimation.ResetToStartValues(forced);
		offAnimation.ResetToStartValues(forced);
		if (!(spriteTarget == null))
		{
			spriteTarget.sprite = onAnimation.sprites[onAnimation.startFrame];
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 2; i++)
		{
			list.Add(new T());
		}
		onAnimation.animation.SetHeartbeat(list[0]);
		offAnimation.animation.SetHeartbeat(list[1]);
		return list;
	}

	private static void ResetAnimation(SpriteAnimation target)
	{
		target.animation.Reset();
		target.animation.enabled = false;
		target.animation.settings.duration = 1f;
	}
}
