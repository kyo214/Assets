using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Transform/Squash and Stretch")]
[FeedbackHelp("This feedback will let you modify the scale of an object on an axis while the other two axis (or only one) get automatically modified to conserve mass.")]
public class MMF_SquashAndStretch : MMF_Feedback
{
	public enum Modes
	{
		Absolute = 0,
		Additive = 1,
		ToDestination = 2
	}

	public enum PossibleAxis
	{
		XtoYZ = 0,
		XtoY = 1,
		XtoZ = 2,
		YtoXZ = 3,
		YtoX = 4,
		YtoZ = 5,
		ZtoXZ = 6,
		ZtoX = 7,
		ZtoY = 8
	}

	public enum TimeScales
	{
		Scaled = 0,
		Unscaled = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Squash & Stretch", true, 54, true, false)]
	[Tooltip("the object to animate")]
	public Transform SquashAndStretchTarget;

	[Tooltip("the mode this feedback should operate onAbsolute : follows the curveAdditive : adds to the current scale of the targetToDestination : sets the scale to the destination target, whatever the current scale is")]
	public Modes Mode;

	public PossibleAxis Axis = PossibleAxis.YtoXZ;

	[Tooltip("the duration of the animation")]
	public float AnimateScaleDuration = 0.2f;

	[Tooltip("the value to remap the curve's 0 value to")]
	public float RemapCurveZero = 1f;

	[Tooltip("the value to remap the curve's 1 value to")]
	[FormerlySerializedAs("Multiplier")]
	public float RemapCurveOne = 2f;

	[Tooltip("how much should be added to the curve")]
	public float Offset;

	[Tooltip("the curve along which to animate the scale")]
	public AnimationCurve AnimateCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1.5f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, initial and destination scales will be recomputed on every play")]
	public bool DetermineScaleOnPlay;

	[Tooltip("the scale to reach when in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float DestinationScale = 2f;

	protected Vector3 _initialScale;

	protected float _initialAxisScale;

	protected Vector3 _newScale;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(AnimateScaleDuration);
		}
		set
		{
			AnimateScaleDuration = value;
		}
	}

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (Active && SquashAndStretchTarget != null)
		{
			GetInitialScale();
		}
	}

	protected virtual void GetInitialScale()
	{
		_initialScale = SquashAndStretchTarget.localScale;
	}

	protected virtual void GetAxisScale()
	{
		switch (Axis)
		{
		case PossibleAxis.XtoYZ:
			_initialAxisScale = SquashAndStretchTarget.localScale.x;
			break;
		case PossibleAxis.XtoY:
			_initialAxisScale = SquashAndStretchTarget.localScale.x;
			break;
		case PossibleAxis.XtoZ:
			_initialAxisScale = SquashAndStretchTarget.localScale.x;
			break;
		case PossibleAxis.YtoXZ:
			_initialAxisScale = SquashAndStretchTarget.localScale.y;
			break;
		case PossibleAxis.YtoX:
			_initialAxisScale = SquashAndStretchTarget.localScale.y;
			break;
		case PossibleAxis.YtoZ:
			_initialAxisScale = SquashAndStretchTarget.localScale.y;
			break;
		case PossibleAxis.ZtoXZ:
			_initialAxisScale = SquashAndStretchTarget.localScale.z;
			break;
		case PossibleAxis.ZtoX:
			_initialAxisScale = SquashAndStretchTarget.localScale.z;
			break;
		case PossibleAxis.ZtoY:
			_initialAxisScale = SquashAndStretchTarget.localScale.z;
			break;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || SquashAndStretchTarget == null)
		{
			return;
		}
		if (DetermineScaleOnPlay && NormalPlayDirection)
		{
			GetInitialScale();
		}
		GetAxisScale();
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (!Active && !Owner.AutoPlayOnEnable)
		{
			return;
		}
		if (Mode == Modes.Absolute || Mode == Modes.Additive)
		{
			if (!AllowAdditivePlays && _coroutine != null)
			{
				return;
			}
			_coroutine = Owner.StartCoroutine(AnimateScale(SquashAndStretchTarget, FeedbackDuration, AnimateCurve, Axis, RemapCurveZero * num, RemapCurveOne * num));
		}
		if (Mode == Modes.ToDestination && (AllowAdditivePlays || _coroutine == null))
		{
			_coroutine = Owner.StartCoroutine(ScaleToDestination());
		}
	}

	protected virtual IEnumerator ScaleToDestination()
	{
		if (!(SquashAndStretchTarget == null) && FeedbackDuration != 0f)
		{
			float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
			_initialScale = SquashAndStretchTarget.localScale;
			_newScale = _initialScale;
			GetAxisScale();
			IsPlaying = true;
			while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
			{
				float time = Mathf.Clamp01(journey / FeedbackDuration);
				float x = Mathf.LerpUnclamped(_initialAxisScale, DestinationScale, AnimateCurve.Evaluate(time) + Offset);
				x = MMFeedbacksHelpers.Remap(x, 0f, 1f, RemapCurveZero, RemapCurveOne);
				ApplyScale(x);
				SquashAndStretchTarget.localScale = _newScale;
				journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
				yield return null;
			}
			ApplyScale(DestinationScale);
			SquashAndStretchTarget.localScale = (NormalPlayDirection ? _newScale : _initialScale);
			_coroutine = null;
			IsPlaying = false;
			yield return null;
		}
	}

	protected virtual IEnumerator AnimateScale(Transform targetTransform, float duration, AnimationCurve curve, PossibleAxis axis, float remapCurveZero = 0f, float remapCurveOne = 1f)
	{
		if (targetTransform == null || duration == 0f)
		{
			yield break;
		}
		float journey = (NormalPlayDirection ? 0f : duration);
		_initialScale = targetTransform.localScale;
		IsPlaying = true;
		while (journey >= 0f && journey <= duration && duration > 0f)
		{
			float time = Mathf.Clamp01(journey / duration);
			float x = curve.Evaluate(time) + Offset;
			x = MMFeedbacksHelpers.Remap(x, 0f, 1f, remapCurveZero, remapCurveOne);
			if (Mode == Modes.Additive)
			{
				x += _initialAxisScale;
			}
			ApplyScale(x);
			targetTransform.localScale = _newScale;
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		float x2 = curve.Evaluate(FinalNormalizedTime) + Offset;
		x2 = MMFeedbacksHelpers.Remap(x2, 0f, 1f, remapCurveZero, remapCurveOne);
		if (Mode == Modes.Additive)
		{
			x2 += _initialAxisScale;
		}
		ApplyScale(x2);
		targetTransform.localScale = _newScale;
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void ApplyScale(float newScale)
	{
		float num = 1f / Mathf.Sqrt(newScale);
		switch (Axis)
		{
		case PossibleAxis.XtoYZ:
			_newScale.x = newScale;
			_newScale.y = num;
			_newScale.z = num;
			break;
		case PossibleAxis.XtoY:
			_newScale.x = newScale;
			_newScale.y = num;
			_newScale.z = _initialScale.z;
			break;
		case PossibleAxis.XtoZ:
			_newScale.x = newScale;
			_newScale.y = _initialScale.y;
			_newScale.z = num;
			break;
		case PossibleAxis.YtoXZ:
			_newScale.x = num;
			_newScale.y = newScale;
			_newScale.z = num;
			break;
		case PossibleAxis.YtoX:
			_newScale.x = num;
			_newScale.y = newScale;
			_newScale.z = _initialScale.z;
			break;
		case PossibleAxis.YtoZ:
			_newScale.x = newScale;
			_newScale.y = _initialScale.y;
			_newScale.z = num;
			break;
		case PossibleAxis.ZtoXZ:
			_newScale.x = num;
			_newScale.y = num;
			_newScale.z = newScale;
			break;
		case PossibleAxis.ZtoX:
			_newScale.x = num;
			_newScale.y = _initialScale.y;
			_newScale.z = newScale;
			break;
		case PossibleAxis.ZtoY:
			_newScale.x = _initialScale.x;
			_newScale.y = num;
			_newScale.z = newScale;
			break;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && _coroutine != null)
		{
			Owner.StopCoroutine(_coroutine);
			_coroutine = null;
			IsPlaying = false;
		}
	}

	public override void OnDisable()
	{
		_coroutine = null;
	}
}
