using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback triggers a MMTimeScaleEvent, which, if you have a MMTimeManager object in your scene, will be caught and used to modify the timescale according to the specified settings. These settings are the new timescale (0.5 will be twice slower than normal, 2 twice faster, etc), the duration of the timescale modification, and the optional speed at which to transition between normal and altered time scale.")]
[FeedbackPath("Time/Timescale Modifier")]
public class MMF_TimescaleModifier : MMF_Feedback
{
	public enum Modes
	{
		Shake = 0,
		Change = 1,
		Reset = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Timescale Modifier", true, 63, false, false)]
	[Tooltip("the selected mode : shake : changes the timescale for a certain duration- change : sets the timescale to a new value, forever (until you change it again)- reset : resets the timescale to its previous value")]
	public Modes Mode;

	[Tooltip("the new timescale to apply")]
	public float TimeScale = 0.5f;

	[Tooltip("the duration of the timescale modification")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float TimeScaleDuration = 1f;

	[Tooltip("whether or not we should lerp the timescale")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public bool TimeScaleLerp;

	[Tooltip("the speed at which to lerp the timescale")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public float TimeScaleLerpSpeed = 1f;

	[Tooltip("whether to reset the timescale on Stop or not")]
	public bool ResetTimescaleOnStop;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(TimeScaleDuration);
		}
		set
		{
			TimeScaleDuration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			switch (Mode)
			{
			case Modes.Shake:
				MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, FeedbackDuration, TimeScaleLerp, TimeScaleLerpSpeed, infinite: false);
				break;
			case Modes.Change:
				MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, 0f, TimeScaleLerp, TimeScaleLerpSpeed, infinite: true);
				break;
			case Modes.Reset:
				MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, TimeScale, 0f, lerp: false, 0f, infinite: true);
				break;
			}
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && ResetTimescaleOnStop)
		{
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, TimeScale, 0f, lerp: false, 0f, infinite: true);
		}
	}
}
