using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the color of a target Graphic over time.")]
[FeedbackPath("UI/Graphic")]
public class MMF_Graphic : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Graphic", true, 54, true, false)]
	[Tooltip("the Graphic to affect when playing the feedback")]
	public Graphic TargetGraphic;

	[Tooltip("whether the feedback should affect the Graphic instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the Graphic should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float Duration = 0.2f;

	[Tooltip("whether or not that Graphic should be turned off on start")]
	public bool StartsOff;

	[Tooltip("if this is true, the target will be disabled when this feedbacks is stopped")]
	public bool DisableOnStop = true;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("whether or not to modify the color of the Graphic")]
	public bool ModifyColor = true;

	[Tooltip("the colors to apply to the Graphic over time")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Gradient ColorOverTime;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public Color InstantColor;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != Modes.Instant)
			{
				return ApplyTimeMultiplier(Duration);
			}
			return 0f;
		}
		set
		{
			Duration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (Active && StartsOff)
		{
			Turn(status: false);
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		Turn(status: true);
		switch (Mode)
		{
		case Modes.Instant:
			if (ModifyColor)
			{
				TargetGraphic.color = InstantColor;
			}
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(GraphicSequence());
			}
			break;
		}
	}

	protected virtual IEnumerator GraphicSequence()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float graphicValues = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetGraphicValues(graphicValues);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetGraphicValues(FinalNormalizedTime);
		if (StartsOff)
		{
			Turn(status: false);
		}
		IsPlaying = false;
		_coroutine = null;
		yield return null;
	}

	protected virtual void SetGraphicValues(float time)
	{
		if (ModifyColor)
		{
			TargetGraphic.color = ColorOverTime.Evaluate(time);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			IsPlaying = false;
			base.CustomStopFeedback(position, feedbacksIntensity);
			if (Active && DisableOnStop)
			{
				Turn(status: false);
			}
		}
	}

	protected virtual void Turn(bool status)
	{
		TargetGraphic.gameObject.SetActive(status);
		TargetGraphic.enabled = status;
	}
}
