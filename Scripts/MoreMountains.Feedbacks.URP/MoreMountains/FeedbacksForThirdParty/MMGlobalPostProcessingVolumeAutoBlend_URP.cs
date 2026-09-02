using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMGlobalPostProcessingVolumeAutoBlend_URP")]
public class MMGlobalPostProcessingVolumeAutoBlend_URP : MonoBehaviour
{
	public enum TimeScales
	{
		Scaled = 0,
		Unscaled = 1
	}

	public enum BlendTriggerModes
	{
		OnEnable = 0,
		Script = 1
	}

	[Header("Blend")]
	public BlendTriggerModes BlendTriggerMode;

	public float BlendDuration = 1f;

	public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Header("Weight")]
	[Range(0f, 1f)]
	public float InitialWeight;

	[Range(0f, 1f)]
	public float FinalWeight = 1f;

	[Header("Behaviour")]
	public TimeScales TimeScale = TimeScales.Unscaled;

	public bool DisableVolumeOnZeroWeight = true;

	public bool DisableSelfAfterEnd = true;

	public bool Interruptable = true;

	public bool StartFromCurrentValue = true;

	[Header("Tests")]
	[MMFInspectorButton("Blend")]
	public bool TestBlend;

	[MMFInspectorButton("BlendBack")]
	public bool TestBlendBackwards;

	protected float _initial;

	protected float _destination;

	protected float _startTime;

	protected bool _blending;

	protected Volume _volume;

	protected float GetTime()
	{
		if (TimeScale != TimeScales.Unscaled)
		{
			return Time.time;
		}
		return Time.unscaledTime;
	}

	protected virtual void Awake()
	{
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.weight = InitialWeight;
	}

	protected virtual void OnEnable()
	{
		if (BlendTriggerMode == BlendTriggerModes.OnEnable && !_blending)
		{
			Blend();
		}
	}

	public virtual void Blend()
	{
		if (!_blending || Interruptable)
		{
			_initial = (StartFromCurrentValue ? _volume.weight : InitialWeight);
			_destination = FinalWeight;
			StartBlending();
		}
	}

	public virtual void BlendBack()
	{
		if (!_blending || Interruptable)
		{
			_initial = (StartFromCurrentValue ? _volume.weight : FinalWeight);
			_destination = InitialWeight;
			StartBlending();
		}
	}

	protected virtual void StartBlending()
	{
		_startTime = GetTime();
		_blending = true;
		base.enabled = true;
		if (DisableVolumeOnZeroWeight)
		{
			_volume.enabled = true;
		}
	}

	public virtual void StopBlending()
	{
		_blending = false;
	}

	protected virtual void Update()
	{
		if (!_blending)
		{
			return;
		}
		float num = GetTime() - _startTime;
		if (num < BlendDuration)
		{
			float time = MMFeedbacksHelpers.Remap(num, 0f, BlendDuration, 0f, 1f);
			_volume.weight = Mathf.LerpUnclamped(_initial, _destination, Curve.Evaluate(time));
			return;
		}
		_volume.weight = _destination;
		_blending = false;
		if (DisableVolumeOnZeroWeight && _volume.weight == 0f)
		{
			_volume.enabled = false;
		}
		if (DisableSelfAfterEnd)
		{
			base.enabled = false;
		}
	}
}
