using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you pilot a Global PostProcessing Volume AutoBlend component. A GPPVAB component is placed on a PostProcessing Volume, and will let you control and blend its weight over time on demand.")]
[FeedbackPath("PostProcess/Global PP Volume Auto Blend")]
public class MMFeedbackGlobalPPVolumeAutoBlend : MMFeedback
{
	public enum Modes
	{
		Default = 0,
		Override = 1
	}

	public enum Actions
	{
		Blend = 0,
		BlendBack = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("PostProcess Volume Blend")]
	[Tooltip("the target auto blend to pilot with this feedback")]
	public MMGlobalPostProcessingVolumeAutoBlend TargetAutoBlend;

	[Tooltip("the chosen mode")]
	public Modes Mode;

	[Tooltip("the chosen action when in default mode")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Actions BlendAction;

	[Tooltip("the duration of the blend, in seconds when in override mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float BlendDuration = 1f;

	[Tooltip("the curve to apply to the blend")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public AnimationCurve BlendCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Tooltip("the weight to blend from")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InitialWeight;

	[Tooltip("the weight to blend to")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float FinalWeight = 1f;

	[Tooltip("whether or not to reset to the initial value at the end of the shake")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool ResetToInitialValueOnEnd = true;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode == Modes.Override)
			{
				return BlendDuration;
			}
			if (TargetAutoBlend == null)
			{
				return 0.1f;
			}
			return TargetAutoBlend.BlendDuration;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active)
		{
			_ = FeedbackTypeAuthorized;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active)
		{
			_ = FeedbackTypeAuthorized;
		}
	}
}
