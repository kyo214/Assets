using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you trigger a fade event.")]
[FeedbackPath("Camera/Fade")]
public class MMF_Fade : MMF_Feedback
{
	public enum FadeTypes
	{
		FadeIn = 0,
		FadeOut = 1,
		Custom = 2
	}

	public enum PositionModes
	{
		FeedbackPosition = 0,
		Transform = 1,
		WorldPosition = 2,
		Script = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Fade", true, 43, false, false)]
	[Tooltip("the type of fade we want to use when this feedback gets played")]
	public FadeTypes FadeType;

	[Tooltip("the ID of the fader(s) to pilot")]
	public int ID;

	[Tooltip("the duration (in seconds) of the fade")]
	public float Duration = 1f;

	[Tooltip("the curve to use for this fade")]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	[Tooltip("whether or not this fade should ignore timescale")]
	public bool IgnoreTimeScale = true;

	[Header("Custom")]
	[Tooltip("the target alpha we're aiming for with this fade")]
	public float TargetAlpha;

	[Header("Position")]
	[Tooltip("the chosen way to position the fade")]
	public PositionModes PositionMode;

	[Tooltip("the transform on which to center the fade")]
	[MMFEnumCondition("PositionMode", new int[] { 1 })]
	public Transform TargetTransform;

	[Tooltip("the coordinates on which to center the fade")]
	[MMFEnumCondition("PositionMode", new int[] { 2 })]
	public Vector3 TargetPosition;

	[Tooltip("the position offset to apply when centering the fade")]
	public Vector3 PositionOffset;

	protected Vector3 _position;

	protected FadeTypes _fadeType;

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

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		_position = GetPosition(position);
		_fadeType = FadeType;
		if (!NormalPlayDirection)
		{
			if (FadeType == FadeTypes.FadeIn)
			{
				_fadeType = FadeTypes.FadeOut;
			}
			else if (FadeType == FadeTypes.FadeOut)
			{
				_fadeType = FadeTypes.FadeIn;
			}
		}
		switch (_fadeType)
		{
		case FadeTypes.Custom:
			MMFadeEvent.Trigger(FeedbackDuration, TargetAlpha, Curve, ID, IgnoreTimeScale, _position);
			break;
		case FadeTypes.FadeIn:
			MMFadeInEvent.Trigger(FeedbackDuration, Curve, ID, IgnoreTimeScale, _position);
			break;
		case FadeTypes.FadeOut:
			MMFadeOutEvent.Trigger(FeedbackDuration, Curve, ID, IgnoreTimeScale, _position);
			break;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMFadeStopEvent.Trigger(ID);
		}
	}

	protected virtual Vector3 GetPosition(Vector3 position)
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => Owner.transform.position + PositionOffset, 
			PositionModes.Transform => TargetTransform.position + PositionOffset, 
			PositionModes.WorldPosition => TargetPosition + PositionOffset, 
			PositionModes.Script => position + PositionOffset, 
			_ => position + PositionOffset, 
		};
	}
}
