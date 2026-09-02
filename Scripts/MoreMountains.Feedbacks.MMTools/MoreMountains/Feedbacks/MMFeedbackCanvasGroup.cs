using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the opacity of a canvas group over time.")]
[FeedbackPath("UI/CanvasGroup")]
public class MMFeedbackCanvasGroup : MMFeedbackBase
{
	[Header("Target")]
	[Tooltip("the receiver to write the level to")]
	public CanvasGroup TargetCanvasGroup;

	[Header("Level")]
	[Tooltip("the curve to tween the opacity on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType AlphaCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the opacity curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapZero;

	[Tooltip("the value to remap the opacity curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapOne = 1f;

	[Tooltip("the value to move the opacity to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantAlpha;

	protected override void FillTargets()
	{
		if (!(TargetCanvasGroup == null))
		{
			MMFeedbackBaseTarget mMFeedbackBaseTarget = new MMFeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetCanvasGroup.gameObject;
			mMPropertyReceiver.TargetComponent = TargetCanvasGroup;
			mMPropertyReceiver.TargetPropertyName = "alpha";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMFeedbackBaseTarget.Target = mMPropertyReceiver;
			mMFeedbackBaseTarget.LevelCurve = AlphaCurve;
			mMFeedbackBaseTarget.RemapLevelZero = RemapZero;
			mMFeedbackBaseTarget.RemapLevelOne = RemapOne;
			mMFeedbackBaseTarget.InstantLevel = InstantAlpha;
			_targets.Add(mMFeedbackBaseTarget);
		}
	}
}
