using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMPaniniProjectionShaker_URP")]
public class MMPaniniProjectionShaker_URP : MMShaker
{
	[Header("Distance")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeDistance;

	[Tooltip("the curve used to animate the distance value on")]
	public AnimationCurve ShakeDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 1f)]
	public float RemapDistanceZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 1f)]
	public float RemapDistanceOne = 1f;

	protected Volume _volume;

	protected PaniniProjection _paniniProjection;

	protected float _initialDistance;

	protected float _originalShakeDuration;

	protected AnimationCurve _originalShakeDistance;

	protected float _originalRemapDistanceZero;

	protected float _originalRemapDistanceOne;

	protected bool _originalRelativeDistance;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<PaniniProjection>(out _paniniProjection);
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeDistance, RemapDistanceZero, RemapDistanceOne, RelativeDistance, _initialDistance);
		_paniniProjection.distance.Override(x);
	}

	protected override void GrabInitialValues()
	{
		_initialDistance = _paniniProjection.distance.value;
	}

	public virtual void OnPaniniProjectionShakeEvent(AnimationCurve distance, float duration, float remapMin, float remapMax, bool relativeDistance = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
		{
			return;
		}
		if (stop)
		{
			Stop();
			return;
		}
		_resetShakerValuesAfterShake = resetShakerValuesAfterShake;
		_resetTargetValuesAfterShake = resetTargetValuesAfterShake;
		if (resetShakerValuesAfterShake)
		{
			_originalShakeDuration = ShakeDuration;
			_originalShakeDistance = ShakeDistance;
			_originalRemapDistanceZero = RemapDistanceZero;
			_originalRemapDistanceOne = RemapDistanceOne;
			_originalRelativeDistance = RelativeDistance;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeDistance = distance;
		RemapDistanceZero = remapMin * attenuation;
		RemapDistanceOne = remapMax * attenuation;
		RelativeDistance = relativeDistance;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_paniniProjection.distance.Override(_initialDistance);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeDistance = _originalShakeDistance;
		RemapDistanceZero = _originalRemapDistanceZero;
		RemapDistanceOne = _originalRemapDistanceOne;
		RelativeDistance = _originalRelativeDistance;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMPaniniProjectionShakeEvent_URP.Register(OnPaniniProjectionShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMPaniniProjectionShakeEvent_URP.Unregister(OnPaniniProjectionShakeEvent);
	}
}
