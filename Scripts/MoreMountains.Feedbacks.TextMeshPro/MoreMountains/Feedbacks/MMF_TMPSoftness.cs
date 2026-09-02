using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you tweak the softness of a TMP text over time.")]
[FeedbackPath("TextMesh Pro/TMP Softness")]
public class MMF_TMPSoftness : MMF_Feedback
{
	[MMFInspectorGroup("Target", true, 12, true, false)]
	[Tooltip("the TMP_Text component to control")]
	public TMP_Text TargetTMPText;

	[MMFInspectorGroup("Softness", true, 13, false, false)]
	[Tooltip("whether or not values should be relative")]
	public bool RelativeValues = true;

	[Tooltip("the selected mode")]
	public MMFeedbackBase.Modes Mode;

	[Tooltip("the duration of the feedback, in seconds")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float Duration = 0.5f;

	[Tooltip("the curve to tween on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType SoftnessCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapOne = 1f;

	[Tooltip("the value to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantSoftness;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	protected float _initialSoftness;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != MMFeedbackBase.Modes.Instant)
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

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (Active)
		{
			_initialSoftness = TargetTMPText.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (TargetTMPText == null || !Active)
		{
			return;
		}
		switch (Mode)
		{
		case MMFeedbackBase.Modes.Instant:
			TargetTMPText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, InstantSoftness);
			TargetTMPText.UpdateMeshPadding();
			break;
		case MMFeedbackBase.Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(ApplyValueOverTime());
			}
			break;
		}
	}

	protected virtual IEnumerator ApplyValueOverTime()
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float value = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetValue(value);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetValue(FinalNormalizedTime);
		_coroutine = null;
		IsPlaying = false;
		yield return null;
	}

	protected virtual void SetValue(float time)
	{
		float num = MMTween.Tween(time, 0f, 1f, RemapZero, RemapOne, SoftnessCurve);
		if (RelativeValues)
		{
			num += _initialSoftness;
		}
		TargetTMPText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, num);
		TargetTMPText.UpdateMeshPadding();
	}
}
