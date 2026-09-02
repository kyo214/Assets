using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Containers/Animators/UIContainer Vector3 Animator")]
public class UIContainerVector3Animator : BaseUIContainerAnimator
{
	public ReflectedVector3 ValueTarget = new ReflectedVector3();

	[SerializeField]
	private Vector3Animation ShowAnimation;

	[SerializeField]
	private Vector3Animation HideAnimation;

	public bool isValid => ValueTarget.IsValid();

	public Vector3Animation showAnimation => ShowAnimation ?? (ShowAnimation = new Vector3Animation(ValueTarget));

	public Vector3Animation hideAnimation => HideAnimation ?? (HideAnimation = new Vector3Animation(ValueTarget));

	protected override void Awake()
	{
		UpdateSettings();
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ShowAnimation.Recycle();
		HideAnimation.Recycle();
	}

	protected override void ConnectToController()
	{
		base.ConnectToController();
		if ((bool)base.controller)
		{
			base.controller.showReactions.Add(showAnimation.animation);
			base.controller.hideReactions.Add(hideAnimation.animation);
		}
	}

	protected override void DisconnectFromController()
	{
		base.DisconnectFromController();
		if ((bool)base.controller)
		{
			base.controller.showReactions.Remove(showAnimation.animation);
			base.controller.hideReactions.Remove(hideAnimation.animation);
		}
	}

	public override void Show()
	{
		if (base.reversingShow)
		{
			showAnimation.OnFinishCallback.RemoveListener(OnReverseShowComplete);
			base.reversingShow = false;
		}
		showAnimation.Play(PlayDirection.Forward);
	}

	public override void ReverseShow()
	{
		if (showAnimation.isPlaying)
		{
			showAnimation.OnFinishCallback.AddListener(OnReverseShowComplete);
			showAnimation.Reverse();
			base.reversingShow = true;
		}
		else
		{
			Hide();
		}
	}

	private void OnReverseShowComplete()
	{
		InstantHide();
		showAnimation.OnFinishCallback.RemoveListener(OnReverseShowComplete);
		base.reversingShow = false;
	}

	public override void Hide()
	{
		if (base.reversingHide)
		{
			hideAnimation.OnFinishCallback.RemoveListener(OnReverseHideComplete);
			base.reversingHide = false;
		}
		hideAnimation.Play(PlayDirection.Forward);
	}

	public override void ReverseHide()
	{
		if (hideAnimation.isPlaying)
		{
			hideAnimation.OnFinishCallback.AddListener(OnReverseHideComplete);
			hideAnimation.Reverse();
			base.reversingHide = true;
		}
		else
		{
			Show();
		}
	}

	private void OnReverseHideComplete()
	{
		InstantShow();
		hideAnimation.OnFinishCallback.RemoveListener(OnReverseHideComplete);
		base.reversingHide = false;
	}

	public override void InstantShow()
	{
		showAnimation.SetProgressAtOne();
	}

	public override void InstantHide()
	{
		hideAnimation.SetProgressAtOne();
	}

	private void SetTarget(object reflectedValue)
	{
		SetTarget(reflectedValue as ReflectedVector3);
	}

	private void SetTarget(ReflectedVector3 reflectedValue)
	{
		showAnimation.SetTarget(reflectedValue);
		hideAnimation.SetTarget(reflectedValue);
	}

	public override void UpdateSettings()
	{
		SetTarget(ValueTarget);
		if (showAnimation.isPlaying)
		{
			showAnimation.UpdateValues();
		}
		if (hideAnimation.isPlaying)
		{
			hideAnimation.UpdateValues();
		}
	}

	public override void StopAllReactions()
	{
		showAnimation.Stop();
		hideAnimation.Stop();
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (showAnimation.isActive)
		{
			showAnimation.Stop();
		}
		if (hideAnimation.isActive)
		{
			hideAnimation.Stop();
		}
		showAnimation.ResetToStartValues(forced);
		hideAnimation.ResetToStartValues(forced);
		if (ValueTarget != null && ValueTarget.IsValid())
		{
			ValueTarget.SetValue(showAnimation.startValue);
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 2; i++)
		{
			list.Add(new T());
		}
		showAnimation.animation.SetHeartbeat(list[0]);
		hideAnimation.animation.SetHeartbeat(list[1]);
		return list;
	}

	private static void ResetAnimation(Vector3Animation animation)
	{
		ReflectedVector3Reaction animation2 = animation.animation;
		animation2.Reset();
		animation2.enabled = true;
		animation2.fromReferenceValue = ReferenceValue.CustomValue;
		animation2.fromCustomValue = Vector3.zero;
		animation2.toReferenceValue = ReferenceValue.CustomValue;
		animation2.toCustomValue = Vector3.one;
		animation2.settings.duration = 0.5f;
	}
}
