using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the alpha of a target TMP over time.")]
[FeedbackPath("TextMesh Pro/TMP Alpha")]
public class MMFeedbackTMPAlpha : MMFeedback
{
	public enum AlphaModes
	{
		Instant = 0,
		Interpolate = 1,
		ToDestination = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Target")]
	[Tooltip(" TMP_Text component to control")]
	public TMP_Text TargetTMPText;

	[Header("Alpha")]
	[Tooltip("the selected color mode :Instant : the alpha will change instantly to the target one,Curve : the alpha will be interpolated along the curve,interpolate : lerps from the current color to the destination one ")]
	public AlphaModes AlphaMode = AlphaModes.Interpolate;

	[Tooltip("how long the color of the text should change over time")]
	[MMFEnumCondition("AlphaMode", new int[] { 1, 2 })]
	public float Duration = 0.2f;

	[Tooltip("the alpha to apply when in instant mode")]
	[MMFEnumCondition("AlphaMode", new int[] { 0 })]
	public float InstantAlpha = 1f;

	[Tooltip("the curve to use when interpolating towards the destination alpha")]
	[MMFEnumCondition("AlphaMode", new int[] { 1, 2 })]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	[Tooltip("the value to which the curve's 0 should be remapped")]
	[MMFEnumCondition("AlphaMode", new int[] { 1 })]
	public float CurveRemapZero;

	[Tooltip("the value to which the curve's 1 should be remapped")]
	[MMFEnumCondition("AlphaMode", new int[] { 1 })]
	public float CurveRemapOne = 1f;

	[Tooltip("the alpha to aim towards when in ToDestination mode")]
	[MMFEnumCondition("AlphaMode", new int[] { 2 })]
	public float DestinationAlpha = 1f;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	protected float _initialAlpha;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			if (AlphaMode != AlphaModes.Instant)
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
		if (!(TargetTMPText == null))
		{
			_initialAlpha = TargetTMPText.alpha;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetTMPText == null)
		{
			return;
		}
		switch (AlphaMode)
		{
		case AlphaModes.Instant:
			TargetTMPText.alpha = InstantAlpha;
			break;
		case AlphaModes.Interpolate:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ChangeAlpha());
			}
			break;
		case AlphaModes.ToDestination:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_initialAlpha = TargetTMPText.alpha;
				_coroutine = StartCoroutine(ChangeAlpha());
			}
			break;
		}
	}

	protected virtual IEnumerator ChangeAlpha()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
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
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}
	}

	protected virtual void SetAlpha(float time)
	{
		float alpha = 0f;
		if (AlphaMode == AlphaModes.Interpolate)
		{
			alpha = MMTween.Tween(time, 0f, 1f, CurveRemapZero, CurveRemapOne, Curve);
		}
		else if (AlphaMode == AlphaModes.ToDestination)
		{
			alpha = MMTween.Tween(time, 0f, 1f, _initialAlpha, DestinationAlpha, Curve);
		}
		TargetTMPText.alpha = alpha;
	}
}
