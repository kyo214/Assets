using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the color of a target Image over time. You can also use it to command one or many MMImageShakers.")]
[FeedbackPath("UI/Image")]
public class MMFeedbackImage : MMFeedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1,
		ShakerEvent = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Sprite Renderer")]
	[Tooltip("the Image to affect when playing the feedback")]
	public Image BoundImage;

	[Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the Image should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float Duration = 0.2f;

	[Tooltip("whether or not that Image should be turned off on start")]
	public bool StartsOff;

	[Tooltip("the channel to broadcast on")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public int Channel;

	[Tooltip("whether or not to reset shaker values after shake")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to broadcast a range to only affect certain shakers")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool UseRange;

	[Tooltip("the range of the event, in units")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float EventRange = 100f;

	[Tooltip("the transform to use to broadcast the event as origin point")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public Transform EventOriginTransform;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, the target will be disabled when this feedbacks is stopped")]
	public bool DisableOnStop = true;

	[Header("Color")]
	[Tooltip("whether or not to modify the color of the image")]
	public bool ModifyColor = true;

	[Tooltip("the colors to apply to the Image over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public Gradient ColorOverTime;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1, 2 })]
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

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (EventOriginTransform == null)
		{
			EventOriginTransform = base.transform;
		}
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
				BoundImage.color = InstantColor;
			}
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(ImageSequence());
			}
			break;
		case Modes.ShakerEvent:
			break;
		}
	}

	protected virtual IEnumerator ImageSequence()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float imageValues = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetImageValues(imageValues);
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		SetImageValues(FinalNormalizedTime);
		if (StartsOff)
		{
			Turn(status: false);
		}
		IsPlaying = false;
		_coroutine = null;
		yield return null;
	}

	protected virtual void SetImageValues(float time)
	{
		if (ModifyColor)
		{
			BoundImage.color = ColorOverTime.Evaluate(time);
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
			_coroutine = null;
		}
	}

	protected virtual void Turn(bool status)
	{
		BoundImage.gameObject.SetActive(status);
		BoundImage.enabled = status;
	}
}
