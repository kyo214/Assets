using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UISelectable Color Animator")]
public class UISelectableColorAnimator : BaseUISelectableAnimator
{
	[SerializeField]
	private ReactorColorTarget ColorTarget;

	[SerializeField]
	private ColorAnimation NormalAnimation;

	[SerializeField]
	private ColorAnimation HighlightedAnimation;

	[SerializeField]
	private ColorAnimation PressedAnimation;

	[SerializeField]
	private ColorAnimation SelectedAnimation;

	[SerializeField]
	private ColorAnimation DisabledAnimation;

	public ReactorColorTarget colorTarget => ColorTarget;

	public bool hasColorTarget => ColorTarget != null;

	public ColorAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new ColorAnimation(colorTarget));

	public ColorAnimation GetAnimation(UISelectionState state)
	{
		return state switch
		{
			UISelectionState.Normal => normalAnimation, 
			UISelectionState.Highlighted => highlightedAnimation, 
			UISelectionState.Pressed => pressedAnimation, 
			UISelectionState.Selected => selectedAnimation, 
			UISelectionState.Disabled => disabledAnimation, 
			_ => throw new ArgumentOutOfRangeException("state", state, null), 
		};
	}

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
		FindTarget();
		UpdateSettings();
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState)?.Recycle();
		}
	}

	public override bool IsStateEnabled(UISelectionState state)
	{
		return GetAnimation(state).isEnabled;
	}

	public override void UpdateSettings()
	{
		if (colorTarget == null)
		{
			return;
		}
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState)?.SetTarget(colorTarget);
		}
	}

	public override void StopAllReactions()
	{
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState)?.Stop();
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (normalAnimation.isActive)
		{
			normalAnimation.Stop();
		}
		if (highlightedAnimation.isActive)
		{
			highlightedAnimation.Stop();
		}
		if (pressedAnimation.isActive)
		{
			pressedAnimation.Stop();
		}
		if (selectedAnimation.isActive)
		{
			selectedAnimation.Stop();
		}
		if (disabledAnimation.isActive)
		{
			disabledAnimation.Stop();
		}
		normalAnimation.ResetToStartValues();
		highlightedAnimation.ResetToStartValues();
		pressedAnimation.ResetToStartValues();
		selectedAnimation.ResetToStartValues();
		disabledAnimation.ResetToStartValues();
		if (!(colorTarget == null))
		{
			colorTarget.color = normalAnimation.startColor;
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 5; i++)
		{
			list.Add(new T());
		}
		normalAnimation.animation.SetHeartbeat(list[0]);
		highlightedAnimation.animation.SetHeartbeat(list[1]);
		pressedAnimation.animation.SetHeartbeat(list[2]);
		selectedAnimation.animation.SetHeartbeat(list[3]);
		disabledAnimation.animation.SetHeartbeat(list[4]);
		return list;
	}

	public override void Play(UISelectionState state)
	{
		GetAnimation(state)?.Play();
	}

	private void ResetAnimation(UISelectionState state)
	{
		ColorAnimation animation = GetAnimation(state);
		animation.animation.Reset();
		animation.animation.enabled = true;
		animation.animation.fromReferenceValue = ReferenceValue.CurrentValue;
		animation.animation.settings.duration = 0.2f;
		switch (state)
		{
		case UISelectionState.Normal:
			animation.animation.settings.duration = 0.1f;
			break;
		case UISelectionState.Highlighted:
			animation.animation.toLightnessOffset = 0.1f;
			break;
		case UISelectionState.Pressed:
			animation.animation.toLightnessOffset = -0.1f;
			animation.animation.settings.duration = 0.05f;
			break;
		case UISelectionState.Selected:
			animation.animation.toHueOffset = -1f / 36f;
			animation.animation.toLightnessOffset = 0.2f;
			break;
		case UISelectionState.Disabled:
			animation.animation.toSaturationOffset = -0.5f;
			animation.animation.toAlphaOffset = -0.5f;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	public void SetStartColor(Color color)
	{
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			ColorAnimation animation = GetAnimation(uiSelectionState);
			if (animation != null)
			{
				animation.customStartValue = color;
			}
		}
		if (!(base.controller == null))
		{
			base.controller.RefreshState();
		}
	}

	public void SetStartColor(Color color, UISelectionState selectionState)
	{
		ColorAnimation animation = GetAnimation(selectionState);
		if (animation != null)
		{
			animation.customStartValue = color;
			if (!(base.controller == null) && base.controller.currentUISelectionState == selectionState)
			{
				base.controller.RefreshState();
			}
		}
	}
}
