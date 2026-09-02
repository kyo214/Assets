using System.Collections;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you modify the fill value of a target Image over time.")]
[FeedbackPath("UI/Image Fill")]
public class MMF_ImageFill : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1,
		ToDestination = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Target Image", true, 12, true, false)]
	[Tooltip("the Image to affect when playing the feedback")]
	public Image BoundImage;

	[MMFInspectorGroup("Image Fill Animation", true, 24, false, false)]
	[Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the Image should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float Duration = 0.2f;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("the fill to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantFill = 1f;

	[Tooltip("the curve to use when interpolating towards the destination fill")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	[Tooltip("the value to which the curve's 0 should be remapped")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float CurveRemapZero;

	[Tooltip("the value to which the curve's 1 should be remapped")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float CurveRemapOne = 1f;

	[Tooltip("the fill to aim towards when in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float DestinationFill = 1f;

	[Tooltip("if this is true, the target will be disabled when this feedbacks is stopped")]
	public bool DisableOnStop = true;

	protected Coroutine _coroutine;

	protected float _initialFill;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != Modes.Instant)
			{
				return ApplyTimeMultiplier(Duration);
			}
			return 0f;
		}
		set
		{
			Duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		Turn(status: true);
		switch (Mode)
		{
		case Modes.Instant:
			BoundImage.fillAmount = InstantFill;
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(ImageSequence());
			}
			break;
		case Modes.ToDestination:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(ImageSequence());
			}
			break;
		}
	}

	protected virtual IEnumerator ImageSequence()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		_initialFill = BoundImage.fillAmount;
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float fill = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetFill(fill);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetFill(FinalNormalizedTime);
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void SetFill(float time)
	{
		float fillAmount = 0f;
		if (Mode == Modes.OverTime)
		{
			fillAmount = MMTween.Tween(time, 0f, 1f, CurveRemapZero, CurveRemapOne, Curve);
		}
		else if (Mode == Modes.ToDestination)
		{
			fillAmount = MMTween.Tween(time, 0f, 1f, _initialFill, DestinationFill, Curve);
		}
		BoundImage.fillAmount = fillAmount;
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			IsPlaying = false;
			base.CustomStopFeedback(position, feedbacksIntensity);
			if (Active && DisableOnStop)
			{
				Turn(status: false);
			}
			_coroutine = null;
		}
	}

	protected virtual void Turn(bool status)
	{
		BoundImage.gameObject.SetActive(status);
		BoundImage.enabled = status;
	}
}
