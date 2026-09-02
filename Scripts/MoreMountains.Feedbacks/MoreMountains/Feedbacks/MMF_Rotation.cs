using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will animate the target's rotation on the 3 specified animation curves (one per axis), for the specified duration (in seconds).")]
[FeedbackPath("Transform/Rotation")]
public class MMF_Rotation : MMF_Feedback
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

	[MMFInspectorGroup("Rotation Target", true, 61, true, false)]
	[Tooltip("the object whose rotation you want to animate")]
	public Transform AnimateRotationTarget;

	[MMFInspectorGroup("Transition", true, 63, false, false)]
	[Tooltip("whether this feedback should animate in absolute values or additive")]
	public Modes Mode;

	[Tooltip("whether this feedback should play on local or world rotation")]
	public Space RotationSpace;

	[Tooltip("the duration of the transition")]
	public float AnimateRotationDuration = 0.2f;

	[Tooltip("the value to remap the curve's 0 value to")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public float RemapCurveZero;

	[Tooltip("the value to remap the curve's 1 value to")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public float RemapCurveOne = 360f;

	[Tooltip("if this is true, should animate the X rotation")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public bool AnimateX = true;

	[Tooltip("how the x part of the rotation should animate over time, in degrees")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public AnimationCurve AnimateRotationX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, should animate the X rotation")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public bool AnimateY = true;

	[Tooltip("how the y part of the rotation should animate over time, in degrees")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public AnimationCurve AnimateRotationY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, should animate the X rotation")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public bool AnimateZ = true;

	[Tooltip("how the z part of the rotation should animate over time, in degrees")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public AnimationCurve AnimateRotationZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, initial and destination rotations will be recomputed on every play")]
	public bool DetermineRotationOnPlay;

	[Header("To Destination")]
	[Tooltip("the space in which the ToDestination mode should operate")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public Space ToDestinationSpace;

	[Tooltip("the angles to match when in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public Vector3 DestinationAngles = new Vector3(0f, 180f, 0f);

	[Tooltip("the animation curve to use when animating to destination (individual x,y,z curves above won't be used)")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	protected Quaternion _initialRotation;

	protected Vector3 _initialToDestinationAngles;

	protected Quaternion _destinationRotation;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(AnimateRotationDuration);
		}
		set
		{
			AnimateRotationDuration = value;
		}
	}

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (Active && AnimateRotationTarget != null)
		{
			GetInitialRotation();
		}
	}

	protected virtual void GetInitialRotation()
	{
		_initialRotation = ((RotationSpace == Space.World) ? AnimateRotationTarget.rotation : AnimateRotationTarget.localRotation);
		_initialToDestinationAngles = _initialRotation.eulerAngles;
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || AnimateRotationTarget == null)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (!Active && !Owner.AutoPlayOnEnable)
		{
			return;
		}
		if (Mode == Modes.Absolute || Mode == Modes.Additive)
		{
			if (AllowAdditivePlays || _coroutine == null)
			{
				if (DetermineRotationOnPlay && NormalPlayDirection)
				{
					GetInitialRotation();
				}
				ClearCoroutine();
				_coroutine = Owner.StartCoroutine(AnimateRotation(AnimateRotationTarget, Vector3.zero, FeedbackDuration, AnimateRotationX, AnimateRotationY, AnimateRotationZ, RemapCurveZero * num, RemapCurveOne * num));
			}
		}
		else if (Mode == Modes.ToDestination && (AllowAdditivePlays || _coroutine == null))
		{
			if (DetermineRotationOnPlay && NormalPlayDirection)
			{
				GetInitialRotation();
			}
			ClearCoroutine();
			_coroutine = Owner.StartCoroutine(RotateToDestination());
		}
	}

	protected virtual void ClearCoroutine()
	{
		if (_coroutine != null)
		{
			Owner.StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}

	protected virtual IEnumerator RotateToDestination()
	{
		if (!(AnimateRotationTarget == null) && AnimateRotationX != null && AnimateRotationY != null && AnimateRotationZ != null && FeedbackDuration != 0f)
		{
			Vector3 destinationAngles = (NormalPlayDirection ? DestinationAngles : _initialToDestinationAngles);
			float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
			_initialRotation = AnimateRotationTarget.transform.rotation;
			if (ToDestinationSpace == Space.Self)
			{
				AnimateRotationTarget.transform.localRotation = Quaternion.Euler(destinationAngles);
			}
			else
			{
				AnimateRotationTarget.transform.rotation = Quaternion.Euler(destinationAngles);
			}
			_destinationRotation = AnimateRotationTarget.transform.rotation;
			AnimateRotationTarget.transform.rotation = _initialRotation;
			IsPlaying = true;
			while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
			{
				float time = Mathf.Clamp01(journey / FeedbackDuration);
				time = ToDestinationCurve.Evaluate(time);
				Quaternion rotation = Quaternion.LerpUnclamped(_initialRotation, _destinationRotation, time);
				AnimateRotationTarget.transform.rotation = rotation;
				journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
				yield return null;
			}
			if (ToDestinationSpace == Space.Self)
			{
				AnimateRotationTarget.transform.localRotation = Quaternion.Euler(destinationAngles);
			}
			else
			{
				AnimateRotationTarget.transform.rotation = Quaternion.Euler(destinationAngles);
			}
			IsPlaying = false;
			_coroutine = null;
		}
	}

	protected virtual IEnumerator AnimateRotation(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float remapZero, float remapOne)
	{
		if (!(targetTransform == null) && curveX != null && curveY != null && curveZ != null && duration != 0f)
		{
			float journey = (NormalPlayDirection ? 0f : duration);
			if (Mode == Modes.Additive)
			{
				_initialRotation = ((RotationSpace == Space.World) ? targetTransform.rotation : targetTransform.localRotation);
			}
			IsPlaying = true;
			while (journey >= 0f && journey <= duration && duration > 0f)
			{
				float percent = Mathf.Clamp01(journey / duration);
				ApplyRotation(targetTransform, remapZero, remapOne, curveX, curveY, curveZ, percent);
				journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
				yield return null;
			}
			ApplyRotation(targetTransform, remapZero, remapOne, curveX, curveY, curveZ, FinalNormalizedTime);
			_coroutine = null;
			IsPlaying = false;
		}
	}

	protected virtual void ApplyRotation(Transform targetTransform, float remapZero, float remapOne, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float percent)
	{
		if (RotationSpace == Space.World)
		{
			targetTransform.transform.rotation = _initialRotation;
		}
		else
		{
			targetTransform.transform.localRotation = _initialRotation;
		}
		if (AnimateX)
		{
			float x = curveX.Evaluate(percent);
			x = MMFeedbacksHelpers.Remap(x, 0f, 1f, remapZero, remapOne);
			targetTransform.Rotate(Vector3.right, x, RotationSpace);
		}
		if (AnimateY)
		{
			float x2 = curveY.Evaluate(percent);
			x2 = MMFeedbacksHelpers.Remap(x2, 0f, 1f, remapZero, remapOne);
			targetTransform.Rotate(Vector3.up, x2, RotationSpace);
		}
		if (AnimateZ)
		{
			float x3 = curveZ.Evaluate(percent);
			x3 = MMFeedbacksHelpers.Remap(x3, 0f, 1f, remapZero, remapOne);
			targetTransform.Rotate(Vector3.forward, x3, RotationSpace);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && _coroutine != null)
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
