using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the offset of the lower left corner of the rectangle relative to the lower left anchor, and the offset of the upper right corner of the rectangle relative to the upper right anchor.")]
[FeedbackPath("UI/RectTransform Offset")]
public class MMFeedbackRectTransformOffset : MMFeedbackBase
{
	[Header("Target")]
	public RectTransform TargetRectTransform;

	[Header("Offset Min")]
	[Tooltip("whether we should modify the offset min or not")]
	public bool ModifyOffsetMin = true;

	[Tooltip("the curve to animate the min offset on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType OffsetMinCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

	[Tooltip("the value to remap the min curve's 0 on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Vector2 OffsetMinRemapZero = Vector2.zero;

	[Tooltip("the value to remap the min curve's 1 on")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 OffsetMinRemapOne = Vector2.one;

	[Header("Offset Max")]
	[Tooltip("whether we should modify the offset max or not")]
	public bool ModifyOffsetMax = true;

	[Tooltip("the curve to animate the max offset on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType OffsetMaxCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

	[Tooltip("the value to remap the max curve's 0 on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Vector2 OffsetMaxRemapZero = Vector2.zero;

	[Tooltip("the value to remap the max curve's 1 on")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 OffsetMaxRemapOne = Vector2.one;

	protected override void FillTargets()
	{
		if (!(TargetRectTransform == null))
		{
			MMFeedbackBaseTarget mMFeedbackBaseTarget = new MMFeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver.TargetComponent = TargetRectTransform;
			mMPropertyReceiver.TargetPropertyName = "offsetMin";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMPropertyReceiver.Vector2RemapZero = OffsetMinRemapZero;
			mMPropertyReceiver.Vector2RemapOne = OffsetMinRemapOne;
			mMPropertyReceiver.ShouldModifyValue = ModifyOffsetMin;
			mMFeedbackBaseTarget.Target = mMPropertyReceiver;
			mMFeedbackBaseTarget.LevelCurve = OffsetMinCurve;
			mMFeedbackBaseTarget.RemapLevelZero = 0f;
			mMFeedbackBaseTarget.RemapLevelOne = 1f;
			mMFeedbackBaseTarget.InstantLevel = 1f;
			_targets.Add(mMFeedbackBaseTarget);
			MMFeedbackBaseTarget mMFeedbackBaseTarget2 = new MMFeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver2 = new MMPropertyReceiver();
			mMPropertyReceiver2.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver2.TargetComponent = TargetRectTransform;
			mMPropertyReceiver2.TargetPropertyName = "offsetMax";
			mMPropertyReceiver2.RelativeValue = RelativeValues;
			mMPropertyReceiver2.Vector2RemapZero = OffsetMaxRemapZero;
			mMPropertyReceiver2.Vector2RemapOne = OffsetMaxRemapOne;
			mMPropertyReceiver2.ShouldModifyValue = ModifyOffsetMax;
			mMFeedbackBaseTarget2.Target = mMPropertyReceiver2;
			mMFeedbackBaseTarget2.LevelCurve = OffsetMaxCurve;
			mMFeedbackBaseTarget2.RemapLevelZero = 0f;
			mMFeedbackBaseTarget2.RemapLevelOne = 1f;
			mMFeedbackBaseTarget2.InstantLevel = 1f;
			_targets.Add(mMFeedbackBaseTarget2);
		}
	}
}
