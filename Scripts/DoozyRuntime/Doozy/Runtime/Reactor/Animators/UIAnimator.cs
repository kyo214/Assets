using System.Collections.Generic;
using Doozy.Runtime.Common.Layouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Animators;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Reactor/Animators/UI Animator")]
public class UIAnimator : ReactorAnimator
{
	private CanvasGroup m_CanvasGroup;

	private RectTransform m_RectTransform;

	[SerializeField]
	private UIAnimation Animation;

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

	public RectTransform rectTransform
	{
		get
		{
			if (!m_RectTransform)
			{
				return m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public UIAnimation animation => Animation ?? (Animation = new UIAnimation(rectTransform, canvasGroup));

	private bool isInLayoutGroup { get; set; }

	private Vector3 localPosition { get; set; }

	private UIBehaviourHandler uiBehaviourHandler { get; set; }

	private bool updateStartPositionInLateUpdate { get; set; }

	private float lastMoveAnimationProgress { get; set; }

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.animatorInitialized = false;
			m_CanvasGroup = GetComponent<CanvasGroup>();
			m_RectTransform = GetComponent<RectTransform>();
			UpdateLayout();
		}
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.OnEnable();
			UpdateLayout();
			updateStartPositionInLateUpdate = true;
		}
	}

	private void OnDisable()
	{
		RefreshLayout();
	}

	private void OnRectTransformDimensionsChange()
	{
		if (base.animatorInitialized && isInLayoutGroup)
		{
			updateStartPositionInLateUpdate = true;
		}
	}

	private void LateUpdate()
	{
		if (!base.animatorInitialized || !isInLayoutGroup)
		{
			return;
		}
		if (animation.isActive)
		{
			lastMoveAnimationProgress = animation.Move.progress;
			return;
		}
		if (localPosition != rectTransform.localPosition)
		{
			updateStartPositionInLateUpdate = true;
		}
		if (updateStartPositionInLateUpdate && !CanvasUpdateRegistry.IsRebuildingLayout())
		{
			UpdateStartPosition();
			RefreshLayout();
		}
	}

	private void UpdateLayout()
	{
		isInLayoutGroup = rectTransform.IsInLayoutGroup();
		uiBehaviourHandler = null;
		if (isInLayoutGroup)
		{
			LayoutGroup layoutGroupInParent = rectTransform.GetLayoutGroupInParent();
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
		animation.startPosition = rectTransform.anchoredPosition3D;
		if (animation.isPlaying)
		{
			animation.UpdateValues();
		}
		localPosition = rectTransform.localPosition;
		updateStartPositionInLateUpdate = false;
	}

	public override void Play(PlayDirection playDirection)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Play(playDirection);
			});
		}
		else
		{
			animation.Play(playDirection);
		}
	}

	public override void Play(bool inReverse = false)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Play(inReverse);
			});
		}
		else
		{
			animation.Play(inReverse);
		}
	}

	public override void SetTarget(object target)
	{
		SetTarget(target as RectTransform);
	}

	public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null)
	{
		animation.SetTarget(targetRectTransform, targetCanvasGroup);
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (animation.isActive)
		{
			Stop();
		}
		animation.ResetToStartValues(forced);
		if (!(this == null))
		{
			rectTransform.anchoredPosition3D = animation.startPosition;
			rectTransform.localEulerAngles = animation.startRotation;
			rectTransform.localScale = animation.startScale;
			canvasGroup.alpha = animation.startAlpha;
		}
	}

	public override void UpdateSettings()
	{
		SetTarget(rectTransform, canvasGroup);
		if (animation.isPlaying)
		{
			UpdateValues();
		}
	}

	public override float GetStartDelay()
	{
		return animation.GetStartDelay();
	}

	public override float GetDuration()
	{
		return animation.GetDuration();
	}

	public override float GetTotalDuration()
	{
		return GetStartDelay() + GetDuration();
	}

	public override List<Heartbeat> SetHeartbeat<T>()
	{
		List<Heartbeat> list = new List<Heartbeat>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(new T());
		}
		animation.Move.SetHeartbeat(list[0]);
		animation.Rotate.SetHeartbeat(list[1]);
		animation.Scale.SetHeartbeat(list[2]);
		animation.Fade.SetHeartbeat(list[3]);
		return list;
	}

	public override void UpdateValues()
	{
		animation.UpdateValues();
	}

	public override void PlayToProgress(float toProgress)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.PlayToProgress(toProgress);
			});
		}
		else
		{
			animation.PlayToProgress(toProgress);
		}
	}

	public override void PlayFromProgress(float fromProgress)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.PlayFromProgress(fromProgress);
			});
		}
		else
		{
			animation.PlayFromProgress(fromProgress);
		}
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.PlayFromToProgress(fromProgress, toProgress);
			});
		}
		else
		{
			animation.PlayFromToProgress(fromProgress, toProgress);
		}
	}

	public override void Stop()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Stop();
			});
		}
		else
		{
			animation.Stop();
		}
	}

	public override void Finish()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Finish();
			});
		}
		else
		{
			animation.Finish();
		}
	}

	public override void Reverse()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Reverse();
			});
		}
		else
		{
			animation.Reverse();
		}
	}

	public override void Rewind()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Rewind();
			});
		}
		else
		{
			animation.Rewind();
		}
	}

	public override void Pause()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Pause();
			});
		}
		else
		{
			animation.Pause();
		}
	}

	public override void Resume()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.Resume();
			});
		}
		else
		{
			animation.Resume();
		}
	}

	public override void SetProgressAtOne()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.SetProgressAtOne();
			});
		}
		else
		{
			animation.SetProgressAtOne();
		}
	}

	public override void SetProgressAtZero()
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				animation.SetProgressAtZero();
			});
		}
		else
		{
			animation.SetProgressAtZero();
		}
	}

	public override void SetProgressAt(float targetProgress)
	{
		if (!base.animatorInitialized)
		{
			DelayExecution(() =>
			{
				SetProgressAt(targetProgress);
			});
		}
		else
		{
			animation.SetProgressAt(targetProgress);
		}
	}

	protected override void Recycle()
	{
		animation?.Recycle();
	}

	public void SetStartPosition(Vector3 value)
	{
		animation.startPosition = value;
	}

	public void SetStartRotation(Vector3 value)
	{
		animation.startRotation = value;
	}

	public void SetStartScale(Vector3 value)
	{
		animation.startScale = value;
	}

	public void SetStartAlpha(float value)
	{
		animation.startAlpha = value;
	}

	private void ResetAnimation()
	{
		animation.animationType = UIAnimationType.Show;
		animation.Move.Reset();
		animation.Move.enabled = true;
		animation.Rotate.Reset();
		animation.Scale.Reset();
		animation.Fade.Reset();
	}
}
