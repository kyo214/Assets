using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you trigger cross fades on a target Graphic.")]
[FeedbackPath("UI/Graphic CrossFade")]
public class MMF_GraphicCrossFade : MMF_Feedback
{
	public enum Modes
	{
		Alpha = 0,
		Color = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Graphic Cross Fade", true, 54, true, false)]
	[Tooltip("the Graphic to affect when playing the feedback")]
	public Graphic TargetGraphic;

	[Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the Graphic should change over time")]
	public float Duration = 0.2f;

	[Tooltip("the target alpha")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float TargetAlpha = 0.2f;

	[Tooltip("the target color")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public Color TargetColor = Color.red;

	[Tooltip("whether or not the crossfade should also tween the alpha channel")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool UseAlpha = true;

	[Tooltip("if this is true, the target will be disabled when this feedbacks is stopped")]
	public bool DisableOnStop = true;

	protected Coroutine _coroutine;

	protected Color _initialColor;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(Duration);
		}
		set
		{
			Duration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetGraphic == null))
		{
			Turn(status: true);
			bool ignoreTimeScale = Timing.TimescaleMode == TimescaleModes.Unscaled;
			switch (Mode)
			{
			case Modes.Alpha:
				_initialColor = TargetGraphic.color;
				_initialColor.a = 1f;
				TargetGraphic.color = _initialColor;
				TargetGraphic.CrossFadeAlpha(0f, 0f, ignoreTimeScale: true);
				TargetGraphic.CrossFadeAlpha(TargetAlpha, Duration, ignoreTimeScale);
				break;
			case Modes.Color:
				TargetGraphic.CrossFadeColor(TargetColor, Duration, ignoreTimeScale, UseAlpha);
				break;
			}
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
