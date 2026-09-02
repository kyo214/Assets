using System.Collections.Generic;
using Doozy.Runtime.Common.Layouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Containers/Animators/UIContainer UIAnimator")]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIContainerUIAnimator : BaseUIContainerAnimator
{
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private UIAnimation ShowAnimation;

	[SerializeField]
	private UIAnimation HideAnimation;

	public CanvasGroup canvasGroup
	{
		get
		{
			if (!m_CanvasGroup)
			{
				return m_CanvasGroup = GetComponent<CanvasGroup>();
			}
			return m_CanvasGroup;
		}
	}

	public UIAnimation showAnimation => ShowAnimation ?? (ShowAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation hideAnimation => HideAnimation ?? (HideAnimation = new UIAnimation(base.rectTransform));

	public bool anyAnimationIsActive
	{
		get
		{
			if (!showAnimation.isActive)
			{
				return hideAnimation.isActive;
			}
			return true;
		}
	}

	private bool isInLayoutGroup { get; set; }

	private Vector3 localPosition { get; set; }

	private UIBehaviourHandler uiBehaviourHandler { get; set; }

	private bool updateStartPositionInLateUpdate { get; set; }

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.animatorInitialized = false;
			m_RectTransform = GetComponent<RectTransform>();
			m_CanvasGroup = GetComponent<CanvasGroup>();
			UpdateLayout();
		}
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.OnEnable();
			updateStartPositionInLateUpdate = true;
		}
	}

	protected override void OnDisable()
	{
		if (Application.isPlaying)
		{
			base.OnDisable();
			if (showAnimation.isPlaying)
			{
				showAnimation.SetProgressAtOne();
			}
			if (hideAnimation.isPlaying)
			{
				hideAnimation.SetProgressAtOne();
			}
			RefreshLayout();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ShowAnimation?.Recycle();
		HideAnimation?.Recycle();
	}

	private void LateUpdate()
	{
		if (base.animatorInitialized && isInLayoutGroup && base.isConnected && base.controller.visibilityState == VisibilityState.Visible && !anyAnimationIsActive && (updateStartPositionInLateUpdate || !(localPosition == base.rectTransform.localPosition)) && !CanvasUpdateRegistry.IsRebuildingLayout())
		{
			RefreshLayout();
			UpdateStartPosition();
		}
	}

	private void UpdateLayout()
	{
		isInLayoutGroup = base.rectTransform.IsInLayoutGroup();
		uiBehaviourHandler = null;
		if (isInLayoutGroup)
		{
			LayoutGroup layoutGroupInParent = base.rectTransform.GetLayoutGroupInParent();
			if (!(layoutGroupInParent == null))
			{
				uiBehaviourHandler = layoutGroupInParent.GetUIBehaviourHandler();
				uiBehaviourHandler.SetDirty();
			}
		}
	}

	private void RefreshLayout()
	{
		if (!(uiBehaviourHandler == null))
		{
			uiBehaviourHandler.RefreshLayout();
		}
	}

	public void UpdateStartPosition()
	{
		Vector3 anchoredPosition3D = base.rectTransform.anchoredPosition3D;
		showAnimation.startPosition = anchoredPosition3D;
		hideAnimation.startPosition = anchoredPosition3D;
		if (showAnimation.Move.isPlaying)
		{
			showAnimation.Move.UpdateValues();
		}
		if (hideAnimation.Move.isPlaying)
		{
			hideAnimation.Move.UpdateValues();
		}
		localPosition = base.rectTransform.localPosition;
		updateStartPositionInLateUpdate = false;
	}

	private void RefreshStartPosition()
	{
		if (base.isConnected && !anyAnimationIsActive && base.controller.visibilityState == VisibilityState.Visible)
		{
			RefreshLayout();
			UpdateStartPosition();
		}
	}

	protected override void ConnectToController()
	{
		base.ConnectToController();
		if ((bool)base.controller)
		{
			base.controller.showReactions.Add(showAnimation.Move);
			base.controller.showReactions.Add(showAnimation.Rotate);
			base.controller.showReactions.Add(showAnimation.Scale);
			base.controller.showReactions.Add(showAnimation.Fade);
			base.controller.hideReactions.Add(hideAnimation.Move);
			base.controller.hideReactions.Add(hideAnimation.Rotate);
			base.controller.hideReactions.Add(hideAnimation.Scale);
			base.controller.hideReactions.Add(hideAnimation.Fade);
		}
	}

	protected override void DisconnectFromController()
	{
		base.DisconnectFromController();
		if ((bool)base.controller)
		{
			base.controller.showReactions.Remove(showAnimation.Move);
			base.controller.showReactions.Remove(showAnimation.Rotate);
			base.controller.showReactions.Remove(showAnimation.Scale);
			base.controller.showReactions.Remove(showAnimation.Fade);
			base.controller.hideReactions.Remove(hideAnimation.Move);
			base.controller.hideReactions.Remove(hideAnimation.Rotate);
			base.controller.hideReactions.Remove(hideAnimation.Scale);
			base.controller.hideReactions.Remove(hideAnimation.Fade);
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
		if (base.animatorInitialized && isInLayoutGroup)
		{
			updateStartPositionInLateUpdate = true;
		}
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
		if (base.animatorInitialized && isInLayoutGroup)
		{
			RefreshStartPosition();
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
		updateStartPositionInLateUpdate = true;
	}

	public override void InstantShow()
	{
		showAnimation.SetProgressAtOne();
		if (base.animatorInitialized && isInLayoutGroup)
		{
			updateStartPositionInLateUpdate = true;
		}
	}

	public override void InstantHide()
	{
		if (base.animatorInitialized && isInLayoutGroup)
		{
			RefreshStartPosition();
		}
		hideAnimation.SetProgressAtOne();
	}

	public override void UpdateSettings()
	{
		showAnimation.SetTarget(base.rectTransform, canvasGroup);
		hideAnimation.SetTarget(base.rectTransform, canvasGroup);
	}

	public override void StopAllReactions()
	{
		showAnimation.Stop();
		hideAnimation.Stop();
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (!(m_RectTransform == null))
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
			base.rectTransform.anchoredPosition3D = showAnimation.startPosition;
			base.rectTransform.localEulerAngles = showAnimation.startRotation;
			base.rectTransform.localScale = showAnimation.startScale;
			canvasGroup.alpha = showAnimation.startAlpha;
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 8; i++)
		{
			list.Add(new T());
		}
		showAnimation.Move.SetHeartbeat(list[0]);
		showAnimation.Rotate.SetHeartbeat(list[1]);
		showAnimation.Scale.SetHeartbeat(list[2]);
		showAnimation.Fade.SetHeartbeat(list[3]);
		hideAnimation.Move.SetHeartbeat(list[4]);
		hideAnimation.Rotate.SetHeartbeat(list[5]);
		hideAnimation.Scale.SetHeartbeat(list[6]);
		hideAnimation.Fade.SetHeartbeat(list[7]);
		return list;
	}

	public void SetStartPosition(Vector3 value)
	{
		showAnimation.startPosition = value;
		hideAnimation.startPosition = value;
	}

	public void SetStartRotation(Vector3 value)
	{
		showAnimation.startRotation = value;
		hideAnimation.startRotation = value;
	}

	public void SetStartScale(Vector3 value)
	{
		showAnimation.startScale = value;
		hideAnimation.startScale = value;
	}

	public void SetStartAlpha(float value)
	{
		showAnimation.startAlpha = value;
		hideAnimation.startAlpha = value;
	}

	private static void ResetAnimation(UIAnimation target)
	{
		target.Move.Reset();
		target.Rotate.Reset();
		target.Scale.Reset();
		target.Fade.Reset();
		target.Move.fromReferenceValue = ReferenceValue.StartValue;
		target.Rotate.fromReferenceValue = ReferenceValue.StartValue;
		target.Scale.fromReferenceValue = ReferenceValue.StartValue;
		target.Fade.fromReferenceValue = ReferenceValue.StartValue;
		target.Move.settings.duration = 0.3f;
		target.Rotate.settings.duration = 0.3f;
		target.Scale.settings.duration = 0.3f;
		target.Fade.settings.duration = 0.3f;
	}
}
