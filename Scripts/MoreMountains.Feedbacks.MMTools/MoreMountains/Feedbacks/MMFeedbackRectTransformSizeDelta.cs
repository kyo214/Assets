using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the size delta property (the size of this RectTransform relative to the distances between the anchors) of a RectTransform, over time")]
[FeedbackPath("UI/RectTransformSizeDelta")]
public class MMFeedbackRectTransformSizeDelta : MMFeedbackBase
{
	[Header("Target")]
	[Tooltip("the rect transform we want to impact")]
	public RectTransform TargetRectTransform;

	[Header("Size Delta")]
	[Tooltip("the speed at which we should animate the size delta")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType SpeedCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

	[Tooltip("the value to remap the curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Vector2 RemapZero = Vector2.zero;

	[Tooltip("the value to remap the curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 RemapOne = Vector2.one;

	protected override void FillTargets()
	{
		if (!(TargetRectTransform == null))
		{
			MMFeedbackBaseTarget mMFeedbackBaseTarget = new MMFeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver.TargetComponent = TargetRectTransform;
			mMPropertyReceiver.TargetPropertyName = "sizeDelta";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMPropertyReceiver.Vector2RemapZero = RemapZero;
			mMPropertyReceiver.Vector2RemapOne = RemapOne;
			mMFeedbackBaseTarget.Target = mMPropertyReceiver;
			mMFeedbackBaseTarget.LevelCurve = SpeedCurve;
			mMFeedbackBaseTarget.RemapLevelZero = 0f;
			mMFeedbackBaseTarget.RemapLevelOne = 1f;
			mMFeedbackBaseTarget.InstantLevel = 1f;
			_targets.Add(mMFeedbackBaseTarget);
		}
	}
}
