using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UIToggle Int Animator")]
public class UIToggleIntAnimator : BaseUIToggleAnimator
{
	public ReflectedInt ValueTarget = new ReflectedInt();

	[SerializeField]
	private IntAnimation OnAnimation;

	[SerializeField]
	private IntAnimation OffAnimation;

	public bool isValid => ValueTarget.IsValid();

	public IntAnimation onAnimation => OnAnimation ?? (OnAnimation = new IntAnimation(ValueTarget));

	public IntAnimation offAnimation => OffAnimation ?? (OffAnimation = new IntAnimation(ValueTarget));

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

	protected override void Awake()
	{
		UpdateSettings();
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnAnimation?.Recycle();
		OffAnimation?.Recycle();
	}

	private void SetTarget(object reflectedValue)
	{
		SetTarget(reflectedValue as ReflectedInt);
	}

	private void SetTarget(ReflectedInt reflectedValue)
	{
		onAnimation.SetTarget(reflectedValue);
		offAnimation.SetTarget(reflectedValue);
	}

	public override void UpdateSettings()
	{
		SetTarget(ValueTarget);
		if (onAnimation.isPlaying)
		{
			onAnimation.UpdateValues();
		}
		if (offAnimation.isPlaying)
		{
			offAnimation.UpdateValues();
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
		if (ValueTarget != null && ValueTarget.IsValid())
		{
			ValueTarget.SetValue(onAnimation.startValue);
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

	private static void ResetAnimation(IntAnimation animation)
	{
		ReflectedIntReaction animation2 = animation.animation;
		animation2.Reset();
		animation2.enabled = true;
		animation2.fromReferenceValue = ReferenceValue.CustomValue;
		animation2.fromCustomValue = 0;
		animation2.toReferenceValue = ReferenceValue.CustomValue;
		animation2.toCustomValue = 100;
		animation2.settings.duration = 0.5f;
	}
}
