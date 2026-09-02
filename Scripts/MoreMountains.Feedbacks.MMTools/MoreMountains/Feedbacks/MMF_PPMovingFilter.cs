using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will trigger a post processing moving filter event, meant to be caught by a MMPostProcessingMovableFilter object")]
[FeedbackPath("PostProcess/PPMovingFilter")]
public class MMF_PPMovingFilter : MMF_Feedback
{
	public enum Modes
	{
		Toggle = 0,
		On = 1,
		Off = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("PostProcessing Profile Moving Filter", true, 54, false, false)]
	[Tooltip("the selected mode for this feedback")]
	public Modes Mode;

	[Tooltip("the duration of the transition")]
	public float TransitionDuration = 1f;

	[Tooltip("the curve to move along to")]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	protected bool _active;

	protected bool _toggle;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(TransitionDuration);
		}
		set
		{
			TransitionDuration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			_active = Mode == Modes.On;
			_toggle = Mode == Modes.Toggle;
			MMPostProcessingMovingFilterEvent.Trigger(Curve, _active, _toggle, FeedbackDuration, Channel);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMPostProcessingMovingFilterEvent.Trigger(Curve, _active, _toggle, FeedbackDuration, 0, stop: true);
		}
	}
}
