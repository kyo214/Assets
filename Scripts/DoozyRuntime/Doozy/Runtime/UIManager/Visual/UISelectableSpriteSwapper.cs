using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Visual;

[AddComponentMenu("UI/Components/Addons/UISelectable SpriteSwapper")]
public class UISelectableSpriteSwapper : BaseUISelectableAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private Sprite NormalSprite;

	[SerializeField]
	private Sprite HighlightedSprite;

	[SerializeField]
	private Sprite PressedSprite;

	[SerializeField]
	private Sprite SelectedSprite;

	[SerializeField]
	private Sprite DisabledSprite;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public Sprite normalSprite => NormalSprite;

	public Sprite highlightedSprite => HighlightedSprite;

	public Sprite pressedSprite => PressedSprite;

	public Sprite selectedSprite => SelectedSprite;

	public Sprite disabledSprite => DisabledSprite;

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

	public override void UpdateSettings()
	{
	}

	public override void StopAllReactions()
	{
	}

	public override bool IsStateEnabled(UISelectionState state)
	{
		return true;
	}

	public override void Play(UISelectionState state)
	{
		if (!hasSpriteTarget)
		{
			return;
		}
		switch (state)
		{
		case UISelectionState.Normal:
			if (normalSprite != null)
			{
				SpriteTarget.SetSprite(normalSprite);
			}
			break;
		case UISelectionState.Highlighted:
			if (highlightedSprite != null)
			{
				SpriteTarget.SetSprite(highlightedSprite);
			}
			break;
		case UISelectionState.Pressed:
			if (pressedSprite != null)
			{
				SpriteTarget.SetSprite(pressedSprite);
			}
			break;
		case UISelectionState.Selected:
			if (selectedSprite != null)
			{
				SpriteTarget.SetSprite(selectedSprite);
			}
			break;
		case UISelectionState.Disabled:
			if (disabledSprite != null)
			{
				SpriteTarget.SetSprite(disabledSprite);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		return null;
	}
}
