using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will animate the target object's position over time, for the specified duration, from the chosen initial position to the chosen destination. These can either be relative Vector3 offsets from the Feedback's position, or Transforms. If you specify transforms, the Vector3 values will be ignored.")]
[FeedbackPath("Transform/Position")]
public class MMFeedbackPosition : MMFeedback
{
	public enum Spaces
	{
		World = 0,
		Local = 1,
		RectTransform = 2
	}

	public enum Modes
	{
		AtoB = 0,
		AlongCurve = 1,
		ToDestination = 2
	}

	public enum TimeScales
	{
		Scaled = 0,
		Unscaled = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Position Target")]
	[Tooltip("the object this feedback will animate the position for")]
	public GameObject AnimatePositionTarget;

	[Header("Animation")]
	[Tooltip("the mode this animation should follow (either going from A to B, or moving along a curve)")]
	public Modes Mode;

	[Tooltip("whether this feedback should play in scaled or unscaled time")]
	public TimeScales TimeScale;

	[Tooltip("the space in which to move the position in")]
	public Spaces Space;

	[Tooltip("the duration of the animation on play")]
	public float AnimatePositionDuration = 0.2f;

	[Tooltip("the acceleration of the movement")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public AnimationCurve AnimatePositionCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[MMFEnumCondition("Mode", new int[] { 1 })]
	[Tooltip("the value to remap the curve's 0 value to")]
	public float RemapCurveZero;

	[Tooltip("the value to remap the curve's 1 value to")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	[FormerlySerializedAs("CurveMultiplier")]
	public float RemapCurveOne = 1f;

	[Tooltip("if this is true, the x position will be animated")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool AnimateX;

	[Tooltip("the acceleration of the movement")]
	[MMFCondition("AnimateX", true)]
	public AnimationCurve AnimatePositionCurveX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, the y position will be animated")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool AnimateY;

	[Tooltip("the acceleration of the movement")]
	[MMFCondition("AnimateY", true)]
	public AnimationCurve AnimatePositionCurveY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, the z position will be animated")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool AnimateZ;

	[Tooltip("the acceleration of the movement")]
	[MMFCondition("AnimateZ", true)]
	public AnimationCurve AnimatePositionCurveZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1f, 0f));

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Header("Positions")]
	[Tooltip("if this is true, the initial position won't be added to init and destination")]
	public bool RelativePosition = true;

	[Tooltip("if this is true, initial and destination positions will be recomputed on every play")]
	public bool DeterminePositionsOnPlay;

	[Tooltip("the initial position")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector3 InitialPosition = Vector3.zero;

	[Tooltip("the destination position")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public Vector3 DestinationPosition = Vector3.one;

	[Tooltip("the initial transform - if set, takes precedence over the Vector3 above")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Transform InitialPositionTransform;

	[Tooltip("the destination transform - if set, takes precedence over the Vector3 above")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public Transform DestinationPositionTransform;

	protected Vector3 _newPosition;

	protected RectTransform _rectTransform;

	protected Vector3 _initialPosition;

	protected Vector3 _destinationPosition;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(AnimatePositionDuration);
		}
		set
		{
			AnimatePositionDuration = value;
		}
	}

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (!Active)
		{
			return;
		}
		if (AnimatePositionTarget == null)
		{
			Debug.LogWarning("The animate position target for " + this?.ToString() + " is null, you have to define it in the inspector");
			return;
		}
		if (Space == Spaces.RectTransform)
		{
			_rectTransform = AnimatePositionTarget.GetComponent<RectTransform>();
		}
		if (!DeterminePositionsOnPlay)
		{
			DeterminePositions();
		}
	}

	protected virtual void DeterminePositions()
	{
		if ((!DeterminePositionsOnPlay || !RelativePosition || !(InitialPosition != Vector3.zero)) && Mode != Modes.ToDestination)
		{
			if (InitialPositionTransform != null)
			{
				InitialPosition = GetPosition(InitialPositionTransform);
			}
			else
			{
				InitialPosition = (RelativePosition ? (GetPosition(AnimatePositionTarget.transform) + InitialPosition) : GetPosition(AnimatePositionTarget.transform));
			}
			if (DestinationPositionTransform != null)
			{
				DestinationPosition = GetPosition(DestinationPositionTransform);
			}
			else
			{
				DestinationPosition = (RelativePosition ? (GetPosition(AnimatePositionTarget.transform) + DestinationPosition) : DestinationPosition);
			}
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || AnimatePositionTarget == null || (!base.isActiveAndEnabled && !_hostMMFeedbacks.AutoPlayOnEnable))
		{
			return;
		}
		if (DeterminePositionsOnPlay && NormalPlayDirection)
		{
			DeterminePositions();
		}
		switch (Mode)
		{
		case Modes.ToDestination:
			_initialPosition = GetPosition(AnimatePositionTarget.transform);
			_destinationPosition = (RelativePosition ? (_initialPosition + DestinationPosition) : DestinationPosition);
			if (DestinationPositionTransform != null)
			{
				_destinationPosition = GetPosition(DestinationPositionTransform);
			}
			_coroutine = StartCoroutine(MoveFromTo(AnimatePositionTarget, _initialPosition, _destinationPosition, FeedbackDuration, AnimatePositionCurve));
			break;
		case Modes.AtoB:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(MoveFromTo(AnimatePositionTarget, InitialPosition, DestinationPosition, FeedbackDuration, AnimatePositionCurve));
			}
			break;
		case Modes.AlongCurve:
			if (AllowAdditivePlays || _coroutine == null)
			{
				float intensityMultiplier = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
				_coroutine = StartCoroutine(MoveAlongCurve(AnimatePositionTarget, InitialPosition, FeedbackDuration, intensityMultiplier));
			}
			break;
		}
	}

	protected virtual IEnumerator MoveAlongCurve(GameObject movingObject, Vector3 initialPosition, float duration, float intensityMultiplier)
	{
		IsPlaying = true;
		float journey = (NormalPlayDirection ? 0f : duration);
		while (journey >= 0f && journey <= duration && duration > 0f)
		{
			float percent = Mathf.Clamp01(journey / duration);
			ComputeNewCurvePosition(movingObject, initialPosition, percent, intensityMultiplier);
			journey = ((TimeScale != TimeScales.Scaled) ? (journey + (NormalPlayDirection ? Time.unscaledDeltaTime : (0f - Time.unscaledDeltaTime))) : (journey + (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime))));
			yield return null;
		}
		ComputeNewCurvePosition(movingObject, initialPosition, FinalNormalizedTime, intensityMultiplier);
		_coroutine = null;
		IsPlaying = false;
	}

	protected virtual void ComputeNewCurvePosition(GameObject movingObject, Vector3 initialPosition, float percent, float intensityMultiplier)
	{
		float x = AnimatePositionCurveX.Evaluate(percent);
		float x2 = AnimatePositionCurveY.Evaluate(percent);
		float x3 = AnimatePositionCurveZ.Evaluate(percent);
		x = MMFeedbacksHelpers.Remap(x, 0f, 1f, RemapCurveZero * intensityMultiplier, RemapCurveOne * intensityMultiplier);
		x2 = MMFeedbacksHelpers.Remap(x2, 0f, 1f, RemapCurveZero * intensityMultiplier, RemapCurveOne * intensityMultiplier);
		x3 = MMFeedbacksHelpers.Remap(x3, 0f, 1f, RemapCurveZero * intensityMultiplier, RemapCurveOne * intensityMultiplier);
		_newPosition = initialPosition;
		if (RelativePosition)
		{
			_newPosition.x = (AnimateX ? (initialPosition.x + x) : initialPosition.x);
			_newPosition.y = (AnimateY ? (initialPosition.y + x2) : initialPosition.y);
			_newPosition.z = (AnimateZ ? (initialPosition.z + x3) : initialPosition.z);
		}
		else
		{
			_newPosition.x = (AnimateX ? x : initialPosition.x);
			_newPosition.y = (AnimateY ? x2 : initialPosition.y);
			_newPosition.z = (AnimateZ ? x3 : initialPosition.z);
		}
		SetPosition(movingObject.transform, _newPosition);
	}

	protected virtual IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float duration, AnimationCurve curve = null)
	{
		IsPlaying = true;
		float journey = (NormalPlayDirection ? 0f : duration);
		while (journey >= 0f && journey <= duration && duration > 0f)
		{
			float time = Mathf.Clamp01(journey / duration);
			_newPosition = Vector3.LerpUnclamped(pointA, pointB, curve.Evaluate(time));
			SetPosition(movingObject.transform, _newPosition);
			journey = ((TimeScale != TimeScales.Scaled) ? (journey + (NormalPlayDirection ? Time.unscaledDeltaTime : (0f - Time.unscaledDeltaTime))) : (journey + (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime))));
			yield return null;
		}
		if (NormalPlayDirection)
		{
			SetPosition(movingObject.transform, pointB);
		}
		else
		{
			SetPosition(movingObject.transform, pointA);
		}
		_coroutine = null;
		IsPlaying = false;
	}

	protected virtual Vector3 GetPosition(Transform target)
	{
		return Space switch
		{
			Spaces.World => target.position, 
			Spaces.Local => target.localPosition, 
			Spaces.RectTransform => target.gameObject.GetComponent<RectTransform>().anchoredPosition, 
			_ => Vector3.zero, 
		};
	}

	protected virtual void SetPosition(Transform target, Vector3 newPosition)
	{
		switch (Space)
		{
		case Spaces.World:
			target.position = newPosition;
			break;
		case Spaces.Local:
			target.localPosition = newPosition;
			break;
		case Spaces.RectTransform:
			_rectTransform.anchoredPosition = newPosition;
			break;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && _coroutine != null)
		{
			IsPlaying = false;
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}

	protected virtual void OnDisable()
	{
		_coroutine = null;
	}
}
