using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the line spacing of a target TMP over time.")]
[FeedbackPath("TextMesh Pro/TMP Line Spacing")]
public class MMFeedbackTMPLineSpacing : MMFeedbackBase
{
	[Header("Target")]
	[Tooltip("the TMP_Text component to control")]
	public TMP_Text TargetTMPText;

	[Header("Paragraph Spacing")]
	[Tooltip("the curve to tween on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType LineSpacingCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapOne = 10f;

	[Tooltip("the value to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantFontSize;

	protected override void FillTargets()
	{
		if (!(TargetTMPText == null))
		{
			MMFeedbackBaseTarget mMFeedbackBaseTarget = new MMFeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetTMPText.gameObject;
			mMPropertyReceiver.TargetComponent = TargetTMPText;
			mMPropertyReceiver.TargetPropertyName = "lineSpacing";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMFeedbackBaseTarget.Target = mMPropertyReceiver;
			mMFeedbackBaseTarget.LevelCurve = LineSpacingCurve;
			mMFeedbackBaseTarget.RemapLevelZero = RemapZero;
			mMFeedbackBaseTarget.RemapLevelOne = RemapOne;
			mMFeedbackBaseTarget.InstantLevel = InstantFontSize;
			_targets.Add(mMFeedbackBaseTarget);
		}
	}
}
