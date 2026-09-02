using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you animate the density, color, end and start distance of your scene's fog")]
[FeedbackPath("Renderer/Fog")]
public class MMF_Fog : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Fog", true, 24, false, false)]
	[Tooltip("whether the feedback should affect the sprite renderer instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the sprite renderer should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float Duration = 2f;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[MMFInspectorGroup("Fog Density", true, 25, false, false)]
	[Tooltip("whether or not to modify the fog's density")]
	public bool ModifyFogDensity = true;

	[Tooltip("a curve to use to animate the fog's density over time")]
	public MMTweenType DensityCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the fog's density curve zero value to")]
	public float DensityRemapZero = 0.01f;

	[Tooltip("the value to remap the fog's density curve one value to")]
	public float DensityRemapOne = 0.05f;

	[Tooltip("the value to change the fog's density to when in instant mode")]
	public float DensityInstantChange;

	[MMFInspectorGroup("Fog Start Distance", true, 26, false, false)]
	[Tooltip("whether or not to modify the fog's start distance")]
	public bool ModifyStartDistance = true;

	[Tooltip("a curve to use to animate the fog's start distance over time")]
	public MMTweenType StartDistanceCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the fog's start distance curve zero value to")]
	public float StartDistanceRemapZero;

	[Tooltip("the value to remap the fog's start distance curve one value to")]
	public float StartDistanceRemapOne;

	[Tooltip("the value to change the fog's start distance to when in instant mode")]
	public float StartDistanceInstantChange;

	[MMFInspectorGroup("Fog End Distance", true, 27, false, false)]
	[Tooltip("whether or not to modify the fog's end distance")]
	public bool ModifyEndDistance = true;

	[Tooltip("a curve to use to animate the fog's end distance over time")]
	public MMTweenType EndDistanceCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the fog's end distance curve zero value to")]
	public float EndDistanceRemapZero;

	[Tooltip("the value to remap the fog's end distance curve one value to")]
	public float EndDistanceRemapOne = 300f;

	[Tooltip("the value to change the fog's end distance to when in instant mode")]
	public float EndDistanceInstantChange;

	[MMFInspectorGroup("Fog Color", true, 28, false, false)]
	[Tooltip("whether or not to modify the fog's color")]
	public bool ModifyColor = true;

	[Tooltip("the colors to apply to the sprite renderer over time")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Gradient ColorOverTime;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public Color InstantColor;

	protected Coroutine _coroutine;

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
			if (Mode != Modes.Instant)
			{
				Duration = value;
			}
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		switch (Mode)
		{
		case Modes.Instant:
			if (ModifyColor)
			{
				RenderSettings.fogColor = InstantColor;
			}
			if (ModifyStartDistance)
			{
				RenderSettings.fogStartDistance = StartDistanceInstantChange;
			}
			if (ModifyEndDistance)
			{
				RenderSettings.fogEndDistance = EndDistanceInstantChange;
			}
			if (ModifyFogDensity)
			{
				RenderSettings.fogDensity = DensityInstantChange * num;
			}
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(FogSequence(num));
			}
			break;
		}
	}

	protected virtual IEnumerator FogSequence(float intensityMultiplier)
	{
		IsPlaying = true;
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetFogValues(time, intensityMultiplier);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetFogValues(FinalNormalizedTime, intensityMultiplier);
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void SetFogValues(float time, float intensityMultiplier)
	{
		if (ModifyColor)
		{
			RenderSettings.fogColor = ColorOverTime.Evaluate(time);
		}
		if (ModifyFogDensity)
		{
			RenderSettings.fogDensity = MMTween.Tween(time, 0f, 1f, DensityRemapZero, DensityRemapOne, DensityCurve) * intensityMultiplier;
		}
		if (ModifyStartDistance)
		{
			RenderSettings.fogStartDistance = MMTween.Tween(time, 0f, 1f, StartDistanceRemapZero, StartDistanceRemapOne, StartDistanceCurve);
		}
		if (ModifyEndDistance)
		{
			RenderSettings.fogEndDistance = MMTween.Tween(time, 0f, 1f, EndDistanceRemapZero, EndDistanceRemapOne, EndDistanceCurve);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && _coroutine != null)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			Owner.StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}
}
