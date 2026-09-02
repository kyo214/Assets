using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the color of a target sprite renderer over time, and flip it on X or Y. You can also use it to command one or many MMSpriteRendererShakers.")]
[FeedbackPath("Renderer/SpriteRenderer")]
public class MMFeedbackSpriteRenderer : MMFeedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1,
		ShakerEvent = 2,
		ToDestinationColor = 3,
		ToDestinationColorAndBack = 4
	}

	public enum InitialColorModes
	{
		InitialColorOnInit = 0,
		InitialColorOnPlay = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Sprite Renderer")]
	[Tooltip("the SpriteRenderer to affect when playing the feedback")]
	public SpriteRenderer BoundSpriteRenderer;

	[Tooltip("whether the feedback should affect the sprite renderer instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the sprite renderer should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2, 3, 4 })]
	public float Duration = 0.2f;

	[Tooltip("whether or not that sprite renderer should be turned off on start")]
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

	[Tooltip("whether to grab the initial color to (potentially) go back to at init or when the feedback plays")]
	public InitialColorModes InitialColorMode = InitialColorModes.InitialColorOnPlay;

	[Header("Color")]
	[Tooltip("whether or not to modify the color of the sprite renderer")]
	public bool ModifyColor = true;

	[Tooltip("the colors to apply to the sprite renderer over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public Gradient ColorOverTime;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1, 2 })]
	public Color InstantColor;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1, 3, 4 })]
	public Color ToDestinationColor = Color.red;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1, 3, 4 })]
	public AnimationCurve ToDestinationColorCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Header("Flip")]
	[Tooltip("whether or not to flip the sprite on X")]
	public bool FlipX;

	[Tooltip("whether or not to flip the sprite on Y")]
	public bool FlipY;

	protected Coroutine _coroutine;

	protected Color _initialColor;

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
		if (BoundSpriteRenderer != null && InitialColorMode == InitialColorModes.InitialColorOnInit)
		{
			_initialColor = BoundSpriteRenderer.color;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		if (BoundSpriteRenderer != null && InitialColorMode == InitialColorModes.InitialColorOnPlay)
		{
			_initialColor = BoundSpriteRenderer.color;
		}
		float feedbacksIntensity2 = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		Turn(status: true);
		switch (Mode)
		{
		case Modes.Instant:
			if (ModifyColor)
			{
				BoundSpriteRenderer.color = InstantColor;
			}
			Flip();
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(SpriteRendererSequence());
			}
			break;
		case Modes.ShakerEvent:
			MMSpriteRendererShakeEvent.Trigger(FeedbackDuration, ModifyColor, ColorOverTime, FlipX, FlipY, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, UseRange, EventRange, EventOriginTransform.position);
			break;
		case Modes.ToDestinationColor:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(SpriteRendererToDestinationSequence(andBack: false));
			}
			break;
		case Modes.ToDestinationColorAndBack:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(SpriteRendererToDestinationSequence(andBack: true));
			}
			break;
		}
	}

	protected virtual IEnumerator SpriteRendererSequence()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		Flip();
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float spriteRendererValues = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetSpriteRendererValues(spriteRendererValues);
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		SetSpriteRendererValues(FinalNormalizedTime);
		if (StartsOff)
		{
			Turn(status: false);
		}
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual IEnumerator SpriteRendererToDestinationSequence(bool andBack)
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		Flip();
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float num = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			if (andBack)
			{
				num = ((num < 0.5f) ? MMFeedbacksHelpers.Remap(num, 0f, 0.5f, 0f, 1f) : MMFeedbacksHelpers.Remap(num, 0.5f, 1f, 1f, 0f));
			}
			float t = ToDestinationColorCurve.Evaluate(num);
			if (ModifyColor)
			{
				BoundSpriteRenderer.color = Color.LerpUnclamped(_initialColor, ToDestinationColor, t);
			}
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		if (ModifyColor)
		{
			BoundSpriteRenderer.color = (andBack ? _initialColor : ToDestinationColor);
		}
		if (StartsOff)
		{
			Turn(status: false);
		}
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void Flip()
	{
		if (FlipX)
		{
			BoundSpriteRenderer.flipX = !BoundSpriteRenderer.flipX;
		}
		if (FlipY)
		{
			BoundSpriteRenderer.flipY = !BoundSpriteRenderer.flipY;
		}
	}

	protected virtual void SetSpriteRendererValues(float time)
	{
		if (ModifyColor)
		{
			BoundSpriteRenderer.color = ColorOverTime.Evaluate(time);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && _coroutine != null)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			StopCoroutine(_coroutine);
			IsPlaying = false;
			_coroutine = null;
		}
	}

	protected virtual void Turn(bool status)
	{
		BoundSpriteRenderer.gameObject.SetActive(status);
		BoundSpriteRenderer.enabled = status;
	}

	protected virtual void OnDisable()
	{
		_coroutine = null;
	}
}
