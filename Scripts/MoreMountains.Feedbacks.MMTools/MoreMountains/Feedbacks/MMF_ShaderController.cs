using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you trigger a one time play on a target ShaderController.")]
[FeedbackPath("Renderer/ShaderController")]
public class MMF_ShaderController : MMF_Feedback
{
	public enum Modes
	{
		OneTime = 0,
		ToDestination = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Shader Controller", true, 37, true, false)]
	[Tooltip("the mode this controller is in")]
	public Modes Mode;

	[Tooltip("the float controller to trigger a one time play on")]
	public ShaderController TargetShaderController;

	[Tooltip("an optional list of float controllers to trigger a one time play on")]
	public List<ShaderController> TargetShaderControllerList;

	[Tooltip("whether this should revert to original at the end")]
	public bool RevertToInitialValueAfterEnd;

	[Tooltip("the duration of the One Time shake")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeDuration = 1f;

	[Tooltip("the amplitude of the One Time shake (this will be multiplied by the curve's height)")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeAmplitude = 1f;

	[Tooltip("the low value to remap the normalized curve value to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeRemapMin;

	[Tooltip("the high value to remap the normalized curve value to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeRemapMax = 1f;

	[Tooltip("the curve to apply to the one time shake")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public AnimationCurve OneTimeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the new value towards which to move the current value")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float ToDestinationValue = 1f;

	[Tooltip("the duration over which to interpolate the target value")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float ToDestinationDuration = 1f;

	[Tooltip("the color to aim for (when targetting a Color property")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public Color ToDestinationColor = Color.red;

	[Tooltip("the curve over which to interpolate the value")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	protected float _oneTimeDurationStorage;

	protected float _oneTimeAmplitudeStorage;

	protected float _oneTimeRemapMinStorage;

	protected float _oneTimeRemapMaxStorage;

	protected AnimationCurve _oneTimeCurveStorage;

	protected float _toDestinationValueStorage;

	protected float _toDestinationDurationStorage;

	protected AnimationCurve _toDestinationCurveStorage;

	protected bool _revertToInitialValueAfterEndStorage;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != Modes.OneTime)
			{
				return ApplyTimeMultiplier(ToDestinationDuration);
			}
			return ApplyTimeMultiplier(OneTimeDuration);
		}
		set
		{
			OneTimeDuration = value;
			ToDestinationDuration = value;
		}
	}

	protected override void CustomInitialization(MMF_Player owner)
	{
		if (Active && TargetShaderController != null)
		{
			_oneTimeDurationStorage = TargetShaderController.OneTimeDuration;
			_oneTimeAmplitudeStorage = TargetShaderController.OneTimeAmplitude;
			_oneTimeCurveStorage = TargetShaderController.OneTimeCurve;
			_oneTimeRemapMinStorage = TargetShaderController.OneTimeRemapMin;
			_oneTimeRemapMaxStorage = TargetShaderController.OneTimeRemapMax;
			_toDestinationCurveStorage = TargetShaderController.ToDestinationCurve;
			_toDestinationDurationStorage = TargetShaderController.ToDestinationDuration;
			_toDestinationValueStorage = TargetShaderController.ToDestinationValue;
			_revertToInitialValueAfterEndStorage = TargetShaderController.RevertToInitialValueAfterEnd;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetShaderController == null)
		{
			return;
		}
		float intensityMultiplier = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		PerformPlay(TargetShaderController, intensityMultiplier);
		foreach (ShaderController targetShaderController in TargetShaderControllerList)
		{
			PerformPlay(targetShaderController, intensityMultiplier);
		}
	}

	protected virtual void PerformPlay(ShaderController shaderController, float intensityMultiplier)
	{
		shaderController.RevertToInitialValueAfterEnd = RevertToInitialValueAfterEnd;
		if (Mode == Modes.OneTime)
		{
			shaderController.OneTimeDuration = FeedbackDuration;
			shaderController.OneTimeAmplitude = OneTimeAmplitude;
			shaderController.OneTimeCurve = OneTimeCurve;
			if (NormalPlayDirection)
			{
				shaderController.OneTimeRemapMin = OneTimeRemapMin * intensityMultiplier;
				shaderController.OneTimeRemapMax = OneTimeRemapMax * intensityMultiplier;
			}
			else
			{
				shaderController.OneTimeRemapMin = OneTimeRemapMax * intensityMultiplier;
				shaderController.OneTimeRemapMax = OneTimeRemapMin * intensityMultiplier;
			}
			shaderController.OneTime();
		}
		if (Mode == Modes.ToDestination)
		{
			shaderController.ToColor = ToDestinationColor;
			shaderController.ToDestinationCurve = ToDestinationCurve;
			shaderController.ToDestinationDuration = FeedbackDuration;
			shaderController.ToDestinationValue = ToDestinationValue;
			shaderController.ToDestination();
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		base.CustomStopFeedback(position, feedbacksIntensity);
		if (TargetShaderController != null)
		{
			TargetShaderController.Stop();
		}
		foreach (ShaderController targetShaderController in TargetShaderControllerList)
		{
			targetShaderController.Stop();
		}
	}

	protected override void CustomReset()
	{
		base.CustomReset();
		if (Active && FeedbackTypeAuthorized && TargetShaderController != null)
		{
			PerformReset(TargetShaderController);
		}
		foreach (ShaderController targetShaderController in TargetShaderControllerList)
		{
			PerformReset(targetShaderController);
		}
	}

	protected virtual void PerformReset(ShaderController shaderController)
	{
		shaderController.OneTimeDuration = _oneTimeDurationStorage;
		shaderController.OneTimeAmplitude = _oneTimeAmplitudeStorage;
		shaderController.OneTimeCurve = _oneTimeCurveStorage;
		shaderController.OneTimeRemapMin = _oneTimeRemapMinStorage;
		shaderController.OneTimeRemapMax = _oneTimeRemapMaxStorage;
		shaderController.ToDestinationCurve = _toDestinationCurveStorage;
		shaderController.ToDestinationDuration = _toDestinationDurationStorage;
		shaderController.ToDestinationValue = _toDestinationValueStorage;
		shaderController.RevertToInitialValueAfterEnd = _revertToInitialValueAfterEndStorage;
	}
}
