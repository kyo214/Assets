using System.Collections;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the alpha of a target Image over time.")]
[FeedbackPath("UI/Image Alpha")]
public class MMFeedbackImageAlpha : MMFeedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1,
		ToDestination = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Sprite Renderer")]
	[Tooltip("the Image to affect when playing the feedback")]
	public Image BoundImage;

	[Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the Image should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float Duration = 0.2f;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Header("Alpha")]
	[Tooltip("the alpha to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantAlpha = 1f;

	[Tooltip("the curve to use when interpolating towards the destination alpha")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	[Tooltip("the value to which the curve's 0 should be remapped")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float CurveRemapZero;

	[Tooltip("the value to which the curve's 1 should be remapped")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float CurveRemapOne = 1f;

	[Tooltip("the alpha to aim towards when in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float DestinationAlpha = 1f;

	protected Coroutine _coroutine;

	protected Color _imageColor;

	protected float _initialAlpha;

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

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
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
			_imageColor = BoundImage.color;
			_imageColor.a = InstantAlpha;
			BoundImage.color = _imageColor;
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ImageSequence());
			}
			break;
		case Modes.ToDestination:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ImageSequence());
			}
			break;
		}
	}

	protected virtual IEnumerator ImageSequence()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		_imageColor = BoundImage.color;
		_initialAlpha = BoundImage.color.a;
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float alpha = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetAlpha(alpha);
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		SetAlpha(FinalNormalizedTime);
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void SetAlpha(float time)
	{
		float a = 0f;
		if (Mode == Modes.OverTime)
		{
			a = MMTween.Tween(time, 0f, 1f, CurveRemapZero, CurveRemapOne, Curve);
		}
		else if (Mode == Modes.ToDestination)
		{
			a = MMTween.Tween(time, 0f, 1f, _initialAlpha, DestinationAlpha, Curve);
		}
		_imageColor.a = a;
		BoundImage.color = _imageColor;
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			IsPlaying = false;
			base.CustomStopFeedback(position, feedbacksIntensity);
			Turn(status: false);
			_coroutine = null;
		}
	}

	protected virtual void Turn(bool status)
	{
		BoundImage.gameObject.SetActive(status);
		BoundImage.enabled = status;
	}
}
