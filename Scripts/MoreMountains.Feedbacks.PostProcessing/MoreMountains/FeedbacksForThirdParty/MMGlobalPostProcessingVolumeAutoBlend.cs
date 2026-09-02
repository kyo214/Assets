using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMGlobalPostProcessingVolumeAutoBlend")]
public class MMGlobalPostProcessingVolumeAutoBlend : MonoBehaviour
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
	[Tooltip("the trigger mode for this MMGlobalPostProcessingVolumeAutoBlend")]
	public BlendTriggerModes BlendTriggerMode;

	[Tooltip("the duration of the blend (in seconds)")]
	public float BlendDuration = 1f;

	[Tooltip("the curve to use to blend")]
	public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Header("Weight")]
	[Tooltip("the weight at the start of the blend")]
	[Range(0f, 1f)]
	public float InitialWeight;

	[Tooltip("the desired weight at the end of the blend")]
	[Range(0f, 1f)]
	public float FinalWeight = 1f;

	[Header("Behaviour")]
	[Tooltip("the timescale to operate on")]
	public TimeScales TimeScale = TimeScales.Unscaled;

	[Tooltip("whether or not the associated volume should be disabled at 0")]
	public bool DisableVolumeOnZeroWeight = true;

	[Tooltip("whether or not this blender should disable itself at 0")]
	public bool DisableSelfAfterEnd = true;

	[Tooltip("whether or not this blender can be interrupted")]
	public bool Interruptable = true;

	[Tooltip("whether or not this blender should pick the current value as its starting point")]
	public bool StartFromCurrentValue = true;

	[Tooltip("reset to initial value on end ")]
	public bool ResetToInitialValueOnEnd;

	[Header("Tests")]
	[Tooltip("test blend button")]
	[MMFInspectorButton("Blend")]
	public bool TestBlend;

	[Tooltip("test blend back button")]
	[MMFInspectorButton("BlendBack")]
	public bool TestBlendBackwards;

	protected float _initial;

	protected float _destination;

	protected float _startTime;

	protected bool _blending;

	protected float GetTime()
	{
		if (TimeScale != TimeScales.Unscaled)
		{
			return Time.time;
		}
		return Time.unscaledTime;
	}
}
