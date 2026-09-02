using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UISelectable Float Animator")]
public class UISelectableFloatAnimator : BaseUISelectableAnimator
{
	public ReflectedFloat ValueTarget = new ReflectedFloat();

	[SerializeField]
	private FloatAnimation NormalAnimation;

	[SerializeField]
	private FloatAnimation HighlightedAnimation;

	[SerializeField]
	private FloatAnimation PressedAnimation;

	[SerializeField]
	private FloatAnimation SelectedAnimation;

	[SerializeField]
	private FloatAnimation DisabledAnimation;

	public bool isValid => ValueTarget.IsValid();

	public FloatAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new FloatAnimation(ValueTarget));

	public FloatAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new FloatAnimation(ValueTarget));

	public FloatAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new FloatAnimation(ValueTarget));

	public FloatAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new FloatAnimation(ValueTarget));

	public FloatAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new FloatAnimation(ValueTarget));

	public bool anyAnimationIsActive
	{
		get
		{
			if (!normalAnimation.isActive && !highlightedAnimation.isActive && !pressedAnimation.isActive && !selectedAnimation.isActive)
			{
				return disabledAnimation.isActive;
			}
			return true;
		}
	}

	public FloatAnimation GetAnimation(UISelectionState state)
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

	protected override void Awake()
	{
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
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			FloatAnimation animation = GetAnimation(uiSelectionState);
			if (animation != null)
			{
				animation.SetTarget(ValueTarget);
				if (animation.isPlaying)
				{
					animation.UpdateValues();
				}
			}
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
		if (ValueTarget != null && ValueTarget.IsValid())
		{
			ValueTarget.SetValue(normalAnimation.startValue);
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
		FloatAnimation animation = GetAnimation(state);
		animation.animation.enabled = true;
		animation.animation.fromReferenceValue = ReferenceValue.CurrentValue;
		animation.animation.toReferenceValue = ReferenceValue.StartValue;
		animation.animation.settings.duration = 0.2f;
	}
}
