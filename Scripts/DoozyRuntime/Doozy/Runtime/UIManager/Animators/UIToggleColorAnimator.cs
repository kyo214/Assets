using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UIToggle Color Animator")]
public class UIToggleColorAnimator : BaseUIToggleAnimator
{
	[SerializeField]
	private ReactorColorTarget ColorTarget;

	[SerializeField]
	private ColorAnimation OnAnimation;

	[SerializeField]
	private ColorAnimation OffAnimation;

	public ReactorColorTarget colorTarget => ColorTarget;

	public bool hasColorTarget => ColorTarget != null;

	public ColorAnimation onAnimation => OnAnimation ?? (OnAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation offAnimation => OffAnimation ?? (OffAnimation = new ColorAnimation(colorTarget));

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
		offAnimation.OnStopCallback.AddListener(ResetToOnState);
	};

	protected override UnityAction removeResetToOnStateCallback => () =>
	{
		offAnimation.OnStopCallback.RemoveListener(ResetToOnState);
	};

	protected override UnityAction addResetToOffStateCallback => () =>
	{
		onAnimation.OnStopCallback.AddListener(ResetToOffState);
	};

	protected override UnityAction removeResetToOffStateCallback => () =>
	{
		onAnimation.OnStopCallback.RemoveListener(ResetToOffState);
	};

	public void FindTarget()
	{
		if (!(ColorTarget != null))
		{
			ColorTarget = ReactorColorTarget.FindTarget(base.gameObject);
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
		if (!(colorTarget == null))
		{
			onAnimation.SetTarget(colorTarget);
			offAnimation.SetTarget(colorTarget);
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
		if (!(colorTarget == null))
		{
			colorTarget.color = onAnimation.startColor;
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

	private static void ResetAnimation(ColorAnimation target)
	{
		target.animation.Reset();
		target.animation.enabled = true;
		target.animation.fromReferenceValue = ReferenceValue.CurrentValue;
		target.animation.settings.duration = 0.2f;
	}

	public void SetStartColor(Color color)
	{
		SetStartColorForOn(color);
		SetStartColorForOff(color);
	}

	public void SetStartColorForOn(Color color)
	{
		onAnimation.customStartValue = color;
		if (!(base.controller == null))
		{
			if (base.controller.isOn)
			{
				onAnimation.SetProgressAtOne();
			}
			else
			{
				onAnimation.Play(PlayDirection.Forward);
			}
		}
	}

	public void SetStartColorForOff(Color color)
	{
		offAnimation.customStartValue = color;
		if (!(base.controller == null))
		{
			if (base.controller.isOn)
			{
				offAnimation.Play(PlayDirection.Forward);
			}
			else
			{
				offAnimation.SetProgressAtOne();
			}
		}
	}
}
