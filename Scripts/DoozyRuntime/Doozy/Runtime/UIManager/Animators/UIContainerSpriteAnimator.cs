using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Containers/Animators/UIContainer Sprite Animator")]
public class UIContainerSpriteAnimator : BaseUIContainerAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private SpriteAnimation ShowAnimation;

	[SerializeField]
	private SpriteAnimation HideAnimation;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public SpriteAnimation showAnimation => ShowAnimation ?? (ShowAnimation = new SpriteAnimation(spriteTarget));

	public SpriteAnimation hideAnimation => HideAnimation ?? (HideAnimation = new SpriteAnimation(spriteTarget));

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
		ShowAnimation?.Recycle();
		HideAnimation?.Recycle();
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

	public override void UpdateSettings()
	{
		if (!(spriteTarget == null))
		{
			showAnimation.SetTarget(spriteTarget);
			hideAnimation.SetTarget(spriteTarget);
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
		if (!(spriteTarget == null))
		{
			spriteTarget.sprite = showAnimation.sprites[showAnimation.startFrame];
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

	private static void ResetAnimation(SpriteAnimation target)
	{
		target.animation.Reset();
		target.animation.enabled = false;
		target.animation.settings.duration = 1f;
	}
}
