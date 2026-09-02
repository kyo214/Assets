using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UISelectable Int Animator")]
public class UISelectableIntAnimator : BaseUISelectableAnimator
{
	public ReflectedInt ValueTarget = new ReflectedInt();

	[SerializeField]
	private IntAnimation NormalAnimation;

	[SerializeField]
	private IntAnimation HighlightedAnimation;

	[SerializeField]
	private IntAnimation PressedAnimation;

	[SerializeField]
	private IntAnimation SelectedAnimation;

	[SerializeField]
	private IntAnimation DisabledAnimation;

	public bool isValid => ValueTarget.IsValid();

	public IntAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new IntAnimation(ValueTarget));

	public IntAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new IntAnimation(ValueTarget));

	public IntAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new IntAnimation(ValueTarget));

	public IntAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new IntAnimation(ValueTarget));

	public IntAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new IntAnimation(ValueTarget));

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

	public IntAnimation GetAnimation(UISelectionState state)
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
			IntAnimation animation = GetAnimation(uiSelectionState);
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
		IntAnimation animation = GetAnimation(state);
		animation.animation.enabled = true;
		animation.animation.fromReferenceValue = ReferenceValue.CurrentValue;
		animation.animation.toReferenceValue = ReferenceValue.StartValue;
		animation.animation.settings.duration = 0.2f;
	}
}
