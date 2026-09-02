using System.Collections.Generic;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Visual;

[AddComponentMenu("UI/Containers/Addons/UIContainer SpriteSwapper")]
public class UIContainerSpriteSwapper : BaseUIContainerAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private Sprite ShowSprite;

	[SerializeField]
	private Sprite HideSprite;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public Sprite showSprite => ShowSprite;

	public Sprite hideSprite => HideSprite;

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

	public override void Show()
	{
		if (hasSpriteTarget && showSprite != null)
		{
			SpriteTarget.SetSprite(showSprite);
		}
	}

	public override void ReverseShow()
	{
		Hide();
	}

	public override void Hide()
	{
		if (hasSpriteTarget && hideSprite != null)
		{
			SpriteTarget.SetSprite(hideSprite);
		}
	}

	public override void ReverseHide()
	{
		Show();
	}

	public override void InstantShow()
	{
		Show();
	}

	public override void InstantHide()
	{
		Hide();
	}

	public override void ResetToStartValues(bool forced = false)
	{
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		return null;
	}
}
