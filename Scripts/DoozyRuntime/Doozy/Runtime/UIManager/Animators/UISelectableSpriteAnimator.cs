using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UISelectable Sprite Animator")]
public class UISelectableSpriteAnimator : BaseUISelectableAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private SpriteAnimation NormalAnimation;

	[SerializeField]
	private SpriteAnimation HighlightedAnimation;

	[SerializeField]
	private SpriteAnimation PressedAnimation;

	[SerializeField]
	private SpriteAnimation SelectedAnimation;

	[SerializeField]
	private SpriteAnimation DisabledAnimation;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public SpriteAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation GetAnimation(UISelectionState state)
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
		if (!(SpriteTarget != null))
		{
			SpriteTarget = ReactorSpriteTarget.FindTarget(base.gameObject);
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
		if (spriteTarget == null)
		{
			return;
		}
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState)?.SetTarget(spriteTarget);
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
		if (!(spriteTarget == null))
		{
			spriteTarget.sprite = normalAnimation.sprites[normalAnimation.startFrame];
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
		SpriteAnimation animation = GetAnimation(state);
		animation.animation.Reset();
		animation.animation.enabled = false;
		animation.animation.settings.duration = 0.5f;
	}
}
