using System.Collections;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the color of a target TMP over time.")]
[FeedbackPath("TextMesh Pro/TMP Color")]
public class MMFeedbackTMPColor : MMFeedback
{
	public enum ColorModes
	{
		Instant = 0,
		Gradient = 1,
		Interpolate = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Target")]
	[Tooltip(" TMP_Text component to control")]
	public TMP_Text TargetTMPText;

	[Header("Color")]
	[Tooltip("the selected color mode :None : nothing will happen,gradient : evaluates the color over time on that gradient, from left to right,interpolate : lerps from the current color to the destination one ")]
	public ColorModes ColorMode = ColorModes.Interpolate;

	[Tooltip("how long the color of the text should change over time")]
	[MMFEnumCondition("ColorMode", new int[] { 2, 1 })]
	public float Duration = 0.2f;

	[Tooltip("the color to apply")]
	[MMFEnumCondition("ColorMode", new int[] { 0 })]
	public Color InstantColor = Color.yellow;

	[Tooltip("the gradient to use to animate the color over time")]
	[MMFEnumCondition("ColorMode", new int[] { 1 })]
	[GradientUsage(true)]
	public Gradient ColorGradient;

	[Tooltip("the destination color when in interpolate mode")]
	[MMFEnumCondition("ColorMode", new int[] { 2 })]
	public Color DestinationColor = Color.yellow;

	[Tooltip("the curve to use when interpolating towards the destination color")]
	[MMFEnumCondition("ColorMode", new int[] { 2 })]
	public AnimationCurve ColorCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	protected Color _initialColor;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			if (ColorMode != ColorModes.Instant)
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
			_initialColor = TargetTMPText.color;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetTMPText == null)
		{
			return;
		}
		switch (ColorMode)
		{
		case ColorModes.Instant:
			TargetTMPText.color = InstantColor;
			break;
		case ColorModes.Gradient:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ChangeColor());
			}
			break;
		case ColorModes.Interpolate:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ChangeColor());
			}
			break;
		}
	}

	protected virtual IEnumerator ChangeColor()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float color = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetColor(color);
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		SetColor(FinalNormalizedTime);
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

	protected virtual void SetColor(float time)
	{
		if (ColorMode == ColorModes.Gradient)
		{
			TargetTMPText.color = ColorGradient.Evaluate(time);
		}
		else if (ColorMode == ColorModes.Interpolate)
		{
			float t = ColorCurve.Evaluate(time);
			TargetTMPText.color = Color.LerpUnclamped(_initialColor, DestinationColor, t);
		}
	}
}
