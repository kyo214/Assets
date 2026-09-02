using System.Collections.Generic;
using Doozy.Runtime.Common.Layouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Animators;

[AddComponentMenu("UI/Components/Animators/UIToggle UIAnimator")]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIToggleUIAnimator : BaseUIToggleAnimator
{
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private UIAnimation OnAnimation;

	[SerializeField]
	private UIAnimation OffAnimation;

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

	public UIAnimation onAnimation => OnAnimation ?? (OnAnimation = new UIAnimation(base.rectTransform));

	public UIAnimation offAnimation => OffAnimation ?? (OffAnimation = new UIAnimation(base.rectTransform));

	public bool anyAnimationIsActive
	{
		get
		{
			if (!onAnimation.isActive)
			{
				return offAnimation.isActive;
			}
			return true;
		}
	}

	private bool isInLayoutGroup { get; set; }

	private Vector3 localPosition { get; set; }

	private UIBehaviourHandler uiBehaviourHandler { get; set; }

	private bool updateStartPositionInLateUpdate { get; set; }

	protected override bool onAnimationIsActive => onAnimation.isActive;

	protected override bool offAnimationIsActive => offAnimation.isActive;

	protected override UnityAction playOnAnimation => () =>
	{
		onAnimation.Play();
	};

	protected override UnityAction playOffAnimation => () =>
	{
		offAnimation.Play();
	};

	protected override UnityAction reverseOnAnimation => () =>
	{
		onAnimation.Reverse();
	};

	protected override UnityAction reverseOffAnimation => () =>
	{
		offAnimation.Reverse();
	};

	protected override UnityAction instantPlayOnAnimation => () =>
	{
		onAnimation.SetProgressAtOne();
	};

	protected override UnityAction instantPlayOffAnimation => () =>
	{
		offAnimation.SetProgressAtOne();
	};

	protected override UnityAction stopOnAnimation => () =>
	{
		onAnimation.Stop();
	};

	protected override UnityAction stopOffAnimation => () =>
	{
		offAnimation.Stop();
	};

	protected override UnityAction addResetToOnStateCallback => () =>
	{
		offAnimation.OnFinishCallback.AddListener(ResetToOnState);
	};

	protected override UnityAction removeResetToOnStateCallback => () =>
	{
		offAnimation.OnFinishCallback.RemoveListener(ResetToOnState);
	};

	protected override UnityAction addResetToOffStateCallback => () =>
	{
		onAnimation.OnFinishCallback.AddListener(ResetToOffState);
	};

	protected override UnityAction removeResetToOffStateCallback => () =>
	{
		onAnimation.OnFinishCallback.RemoveListener(ResetToOffState);
	};

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
			if (onAnimation.isPlaying)
			{
				onAnimation.SetProgressAtOne();
			}
			if (offAnimation.isPlaying)
			{
				offAnimation.SetProgressAtOne();
			}
			RefreshLayout();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnAnimation?.Recycle();
		OffAnimation?.Recycle();
	}

	private void LateUpdate()
	{
		if (base.animatorInitialized && isInLayoutGroup && base.isConnected && base.controller.isActiveAndEnabled && !anyAnimationIsActive && (updateStartPositionInLateUpdate || !(localPosition == base.rectTransform.localPosition)) && !CanvasUpdateRegistry.IsRebuildingLayout())
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
		onAnimation.startPosition = anchoredPosition3D;
		offAnimation.startPosition = anchoredPosition3D;
		if (onAnimation.Move.isPlaying)
		{
			onAnimation.Move.UpdateValues();
		}
		if (offAnimation.Move.isPlaying)
		{
			offAnimation.Move.UpdateValues();
		}
		localPosition = base.rectTransform.localPosition;
		updateStartPositionInLateUpdate = false;
	}

	private void RefreshStartPosition()
	{
		if (base.isConnected && !anyAnimationIsActive && base.controller.isActiveAndEnabled)
		{
			RefreshLayout();
			UpdateStartPosition();
		}
	}

	public override void UpdateSettings()
	{
		onAnimation.SetTarget(base.rectTransform, canvasGroup);
		offAnimation.SetTarget(base.rectTransform, canvasGroup);
	}

	public override void StopAllReactions()
	{
		onAnimation.Stop();
		offAnimation.Stop();
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (!(m_RectTransform == null))
		{
			if (onAnimation.isActive)
			{
				onAnimation.Stop();
			}
			if (offAnimation.isActive)
			{
				offAnimation.Stop();
			}
			onAnimation.ResetToStartValues(forced);
			offAnimation.ResetToStartValues(forced);
			base.rectTransform.anchoredPosition3D = onAnimation.startPosition;
			base.rectTransform.localEulerAngles = onAnimation.startRotation;
			base.rectTransform.localScale = onAnimation.startScale;
			canvasGroup.alpha = onAnimation.startAlpha;
		}
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 8; i++)
		{
			list.Add(new T());
		}
		onAnimation.Move.SetHeartbeat(list[0]);
		onAnimation.Rotate.SetHeartbeat(list[1]);
		onAnimation.Scale.SetHeartbeat(list[2]);
		onAnimation.Fade.SetHeartbeat(list[3]);
		offAnimation.Move.SetHeartbeat(list[4]);
		offAnimation.Rotate.SetHeartbeat(list[5]);
		offAnimation.Scale.SetHeartbeat(list[6]);
		offAnimation.Fade.SetHeartbeat(list[7]);
		return list;
	}

	public void SetStartPosition(Vector3 value)
	{
		onAnimation.startPosition = value;
		offAnimation.startPosition = value;
	}

	public void SetStartRotation(Vector3 value)
	{
		onAnimation.startRotation = value;
		offAnimation.startRotation = value;
	}

	public void SetStartScale(Vector3 value)
	{
		onAnimation.startScale = value;
		offAnimation.startScale = value;
	}

	public void SetStartAlpha(float value)
	{
		onAnimation.startAlpha = value;
		offAnimation.startAlpha = value;
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
