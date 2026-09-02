using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Layouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Animators;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/Animators/UISelectable UIAnimator")]
public class UISelectableUIAnimator : BaseUISelectableAnimator
{
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private UIAnimation NormalAnimation;

	[SerializeField]
	private UIAnimation HighlightedAnimation;

	[SerializeField]
	private UIAnimation PressedAnimation;

	[SerializeField]
	private UIAnimation SelectedAnimation;

	[SerializeField]
	private UIAnimation DisabledAnimation;

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

	public UIAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new UIAnimation(base.rectTransform));

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

	private bool isInLayoutGroup { get; set; }

	private Vector3 localPosition { get; set; }

	private UIBehaviourHandler uiBehaviourHandler { get; set; }

	private bool updateStartPositionInLateUpdate { get; set; }

	private bool playStateAnimationFromLateUpdate { get; set; }

	public UIAnimation GetAnimation(UISelectionState state)
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
			playStateAnimationFromLateUpdate = true;
			base.OnEnable();
			UpdateLayout();
			updateStartPositionInLateUpdate = true;
		}
	}

	protected override void OnDisable()
	{
		if (Application.isPlaying)
		{
			base.OnDisable();
			RefreshLayout();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState)?.Recycle();
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		if (base.isConnected && isInLayoutGroup)
		{
			updateStartPositionInLateUpdate = true;
		}
	}

	private void LateUpdate()
	{
		if (base.animatorInitialized)
		{
			if (playStateAnimationFromLateUpdate && base.isConnected)
			{
				Play(base.controller.currentUISelectionState);
				playStateAnimationFromLateUpdate = false;
			}
			if (isInLayoutGroup && base.isConnected && !anyAnimationIsActive && (updateStartPositionInLateUpdate || !(localPosition == base.rectTransform.localPosition)) && base.controller.currentUISelectionState == UISelectionState.Normal && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				RefreshLayout();
				UpdateStartPosition();
			}
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
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			UIAnimation animation = GetAnimation(uiSelectionState);
			animation.startPosition = base.rectTransform.anchoredPosition3D;
			if (animation.Move.isPlaying)
			{
				animation.Move.UpdateValues();
			}
		}
		localPosition = base.rectTransform.localPosition;
		updateStartPositionInLateUpdate = false;
	}

	public override bool IsStateEnabled(UISelectionState state)
	{
		return GetAnimation(state).isEnabled;
	}

	public override void UpdateSettings()
	{
		foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
		{
			GetAnimation(uiSelectionState).SetTarget(base.rectTransform, canvasGroup);
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
		if (!(m_RectTransform == null))
		{
			base.rectTransform.anchoredPosition3D = normalAnimation.startPosition;
			base.rectTransform.localEulerAngles = normalAnimation.startRotation;
			base.rectTransform.localScale = normalAnimation.startScale;
			canvasGroup.alpha = normalAnimation.startAlpha;
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 20; i++)
		{
			list.Add(new T());
		}
		normalAnimation.Move.SetHeartbeat(list[0]);
		normalAnimation.Rotate.SetHeartbeat(list[1]);
		normalAnimation.Scale.SetHeartbeat(list[2]);
		normalAnimation.Fade.SetHeartbeat(list[3]);
		highlightedAnimation.Move.SetHeartbeat(list[4]);
		highlightedAnimation.Rotate.SetHeartbeat(list[5]);
		highlightedAnimation.Scale.SetHeartbeat(list[6]);
		highlightedAnimation.Fade.SetHeartbeat(list[7]);
		pressedAnimation.Move.SetHeartbeat(list[8]);
		pressedAnimation.Rotate.SetHeartbeat(list[9]);
		pressedAnimation.Scale.SetHeartbeat(list[10]);
		pressedAnimation.Fade.SetHeartbeat(list[11]);
		selectedAnimation.Move.SetHeartbeat(list[12]);
		selectedAnimation.Rotate.SetHeartbeat(list[13]);
		selectedAnimation.Scale.SetHeartbeat(list[14]);
		selectedAnimation.Fade.SetHeartbeat(list[15]);
		disabledAnimation.Move.SetHeartbeat(list[16]);
		disabledAnimation.Rotate.SetHeartbeat(list[17]);
		disabledAnimation.Scale.SetHeartbeat(list[18]);
		disabledAnimation.Fade.SetHeartbeat(list[19]);
		return list;
	}

	public override void Play(UISelectionState state)
	{
		if (playStateAnimationFromLateUpdate)
		{
			GetAnimation(state)?.SetProgressAtOne();
		}
		else
		{
			GetAnimation(state)?.Play();
		}
	}

	public void SetStartPosition(Vector3 value)
	{
		normalAnimation.startPosition = value;
		highlightedAnimation.startPosition = value;
		pressedAnimation.startPosition = value;
		selectedAnimation.startPosition = value;
		disabledAnimation.startPosition = value;
	}

	public void SetStartRotation(Vector3 value)
	{
		normalAnimation.startRotation = value;
		highlightedAnimation.startRotation = value;
		pressedAnimation.startRotation = value;
		selectedAnimation.startRotation = value;
		disabledAnimation.startRotation = value;
	}

	public void SetStartScale(Vector3 value)
	{
		normalAnimation.startScale = value;
		highlightedAnimation.startScale = value;
		pressedAnimation.startScale = value;
		selectedAnimation.startScale = value;
		disabledAnimation.startScale = value;
	}

	public void SetStartAlpha(float value)
	{
		normalAnimation.startAlpha = value;
		highlightedAnimation.startAlpha = value;
		pressedAnimation.startAlpha = value;
		selectedAnimation.startAlpha = value;
		disabledAnimation.startAlpha = value;
	}

	private static void ResetAnimation(UIAnimation target)
	{
		target.Move.Reset();
		target.Rotate.Reset();
		target.Scale.Reset();
		target.Fade.Reset();
		target.animationType = UIAnimationType.State;
		target.Move.fromReferenceValue = ReferenceValue.CurrentValue;
		target.Rotate.fromReferenceValue = ReferenceValue.CurrentValue;
		target.Scale.fromReferenceValue = ReferenceValue.CurrentValue;
		target.Fade.fromReferenceValue = ReferenceValue.CurrentValue;
		target.Move.settings.duration = 0.2f;
		target.Rotate.settings.duration = 0.2f;
		target.Scale.settings.duration = 0.2f;
		target.Fade.settings.duration = 0.2f;
	}
}
