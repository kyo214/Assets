using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Transform/Scale")]
[FeedbackHelp("This feedback will animate the target's scale on the 3 specified animation curves, for the specified duration (in seconds). You can apply a multiplier, that will multiply each animation curve value.")]
public class MMFeedbackScale : MMFeedback
{
	public enum Modes
	{
		Absolute = 0,
		Additive = 1,
		ToDestination = 2
	}

	public enum TimeScales
	{
		Scaled = 0,
		Unscaled = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Scale")]
	[Tooltip("the mode this feedback should operate onAbsolute : follows the curveAdditive : adds to the current scale of the targetToDestination : sets the scale to the destination target, whatever the current scale is")]
	public Modes Mode;

	[Tooltip("whether this feedback should play in scaled or unscaled time")]
	public TimeScales TimeScale;

	[Tooltip("the object to animate")]
	public Transform AnimateScaleTarget;

	[Tooltip("the duration of the animation")]
	public float AnimateScaleDuration = 0.2f;

	[Tooltip("the value to remap the curve's 0 value to")]
	public float RemapCurveZero = 1f;

	[Tooltip("the value to remap the curve's 1 value to")]
	[FormerlySerializedAs("Multiplier")]
	public float RemapCurveOne = 2f;

	[Tooltip("how much should be added to the curve")]
	public float Offset;

	[Tooltip("if this is true, should animate the X scale value")]
	public bool AnimateX = true;

	[Tooltip("the x scale animation definition")]
	[MMFCondition("AnimateX", true)]
	public AnimationCurve AnimateScaleX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1.5f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, should animate the Y scale value")]
	public bool AnimateY = true;

	[Tooltip("the y scale animation definition")]
	[MMFCondition("AnimateY", true)]
	public AnimationCurve AnimateScaleY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1.5f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, should animate the z scale value")]
	public bool AnimateZ = true;

	[Tooltip("the z scale animation definition")]
	[MMFCondition("AnimateZ", true)]
	public AnimationCurve AnimateScaleZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1.5f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, initial and destination scales will be recomputed on every play")]
	public bool DetermineScaleOnPlay;

	[Header("To Destination")]
	[Tooltip("the scale to reach when in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public Vector3 DestinationScale = new Vector3(0.5f, 0.5f, 0.5f);

	protected Vector3 _initialScale;

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

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (Active && AnimateScaleTarget != null)
		{
			GetInitialScale();
		}
	}

	protected virtual void GetInitialScale()
	{
		_initialScale = AnimateScaleTarget.localScale;
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || AnimateScaleTarget == null)
		{
			return;
		}
		if (DetermineScaleOnPlay && NormalPlayDirection)
		{
			GetInitialScale();
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (!base.isActiveAndEnabled && !_hostMMFeedbacks.AutoPlayOnEnable)
		{
			return;
		}
		if (Mode == Modes.Absolute || Mode == Modes.Additive)
		{
			if (!AllowAdditivePlays && _coroutine != null)
			{
				return;
			}
			_coroutine = StartCoroutine(AnimateScale(AnimateScaleTarget, Vector3.zero, FeedbackDuration, AnimateScaleX, AnimateScaleY, AnimateScaleZ, RemapCurveZero * num, RemapCurveOne * num));
		}
		if (Mode == Modes.ToDestination && (AllowAdditivePlays || _coroutine == null))
		{
			_coroutine = StartCoroutine(ScaleToDestination());
		}
	}

	protected virtual IEnumerator ScaleToDestination()
	{
		if (AnimateScaleTarget == null || AnimateScaleX == null || AnimateScaleY == null || AnimateScaleZ == null || FeedbackDuration == 0f)
		{
			yield break;
		}
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		_initialScale = AnimateScaleTarget.localScale;
		_newScale = _initialScale;
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = Mathf.Clamp01(journey / FeedbackDuration);
			if (AnimateX)
			{
				_newScale.x = Mathf.LerpUnclamped(_initialScale.x, DestinationScale.x, AnimateScaleX.Evaluate(time) + Offset);
				_newScale.x = MMFeedbacksHelpers.Remap(_newScale.x, 0f, 1f, RemapCurveZero, RemapCurveOne);
			}
			if (AnimateY)
			{
				_newScale.y = Mathf.LerpUnclamped(_initialScale.y, DestinationScale.y, AnimateScaleY.Evaluate(time) + Offset);
				_newScale.y = MMFeedbacksHelpers.Remap(_newScale.y, 0f, 1f, RemapCurveZero, RemapCurveOne);
			}
			if (AnimateZ)
			{
				_newScale.z = Mathf.LerpUnclamped(_initialScale.z, DestinationScale.z, AnimateScaleZ.Evaluate(time) + Offset);
				_newScale.z = MMFeedbacksHelpers.Remap(_newScale.z, 0f, 1f, RemapCurveZero, RemapCurveOne);
			}
			AnimateScaleTarget.localScale = _newScale;
			journey = ((TimeScale != TimeScales.Scaled) ? (journey + (NormalPlayDirection ? Time.unscaledDeltaTime : (0f - Time.unscaledDeltaTime))) : (journey + (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime))));
			yield return null;
		}
		AnimateScaleTarget.localScale = (NormalPlayDirection ? DestinationScale : _initialScale);
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual IEnumerator AnimateScale(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float remapCurveZero = 0f, float remapCurveOne = 1f)
	{
		if (targetTransform == null || curveX == null || curveY == null || curveZ == null || duration == 0f)
		{
			yield break;
		}
		float journey = (NormalPlayDirection ? 0f : duration);
		_initialScale = targetTransform.localScale;
		IsPlaying = true;
		while (journey >= 0f && journey <= duration && duration > 0f)
		{
			vector = Vector3.zero;
			float time = Mathf.Clamp01(journey / duration);
			if (AnimateX)
			{
				vector.x = (AnimateX ? (curveX.Evaluate(time) + Offset) : targetTransform.localScale.x);
				vector.x = MMFeedbacksHelpers.Remap(vector.x, 0f, 1f, remapCurveZero, remapCurveOne);
				if (Mode == Modes.Additive)
				{
					vector.x += _initialScale.x;
				}
			}
			else
			{
				vector.x = targetTransform.localScale.x;
			}
			if (AnimateY)
			{
				vector.y = (AnimateY ? (curveY.Evaluate(time) + Offset) : targetTransform.localScale.y);
				vector.y = MMFeedbacksHelpers.Remap(vector.y, 0f, 1f, remapCurveZero, remapCurveOne);
				if (Mode == Modes.Additive)
				{
					vector.y += _initialScale.y;
				}
			}
			else
			{
				vector.y = targetTransform.localScale.y;
			}
			if (AnimateZ)
			{
				vector.z = (AnimateZ ? (curveZ.Evaluate(time) + Offset) : targetTransform.localScale.z);
				vector.z = MMFeedbacksHelpers.Remap(vector.z, 0f, 1f, remapCurveZero, remapCurveOne);
				if (Mode == Modes.Additive)
				{
					vector.z += _initialScale.z;
				}
			}
			else
			{
				vector.z = targetTransform.localScale.z;
			}
			targetTransform.localScale = vector;
			journey = ((TimeScale != TimeScales.Scaled) ? (journey + (NormalPlayDirection ? Time.unscaledDeltaTime : (0f - Time.unscaledDeltaTime))) : (journey + (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime))));
			yield return null;
		}
		vector = Vector3.zero;
		if (AnimateX)
		{
			vector.x = (AnimateX ? (curveX.Evaluate(FinalNormalizedTime) + Offset) : targetTransform.localScale.x);
			vector.x = MMFeedbacksHelpers.Remap(vector.x, 0f, 1f, remapCurveZero, remapCurveOne);
			if (Mode == Modes.Additive)
			{
				vector.x += _initialScale.x;
			}
		}
		else
		{
			vector.x = targetTransform.localScale.x;
		}
		if (AnimateY)
		{
			vector.y = (AnimateY ? (curveY.Evaluate(FinalNormalizedTime) + Offset) : targetTransform.localScale.y);
			vector.y = MMFeedbacksHelpers.Remap(vector.y, 0f, 1f, remapCurveZero, remapCurveOne);
			if (Mode == Modes.Additive)
			{
				vector.y += _initialScale.y;
			}
		}
		else
		{
			vector.y = targetTransform.localScale.y;
		}
		if (AnimateZ)
		{
			vector.z = (AnimateZ ? (curveZ.Evaluate(FinalNormalizedTime) + Offset) : targetTransform.localScale.z);
			vector.z = MMFeedbacksHelpers.Remap(vector.z, 0f, 1f, remapCurveZero, remapCurveOne);
			if (Mode == Modes.Additive)
			{
				vector.z += _initialScale.z;
			}
		}
		else
		{
			vector.z = targetTransform.localScale.z;
		}
		targetTransform.localScale = vector;
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && _coroutine != null)
		{
			StopCoroutine(_coroutine);
			IsPlaying = false;
			_coroutine = null;
		}
	}

	protected virtual void OnDisable()
	{
		_coroutine = null;
	}
}
