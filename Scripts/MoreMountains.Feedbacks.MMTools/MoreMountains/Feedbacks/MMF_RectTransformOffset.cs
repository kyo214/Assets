using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the offset of the lower left corner of the rectangle relative to the lower left anchor, and the offset of the upper right corner of the rectangle relative to the upper right anchor.")]
[FeedbackPath("UI/RectTransform Offset")]
public class MMF_RectTransformOffset : MMF_FeedbackBase
{
	[MMFInspectorGroup("Target RectTransform", true, 37, true, false)]
	public RectTransform TargetRectTransform;

	[MMFInspectorGroup("Offset Min", true, 40, false, false)]
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

	[MMFInspectorGroup("Offset Max", true, 41, false, false)]
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
			MMF_FeedbackBaseTarget mMF_FeedbackBaseTarget = new MMF_FeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver.TargetComponent = TargetRectTransform;
			mMPropertyReceiver.TargetPropertyName = "offsetMin";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMPropertyReceiver.Vector2RemapZero = OffsetMinRemapZero;
			mMPropertyReceiver.Vector2RemapOne = OffsetMinRemapOne;
			mMPropertyReceiver.ShouldModifyValue = ModifyOffsetMin;
			mMF_FeedbackBaseTarget.Target = mMPropertyReceiver;
			mMF_FeedbackBaseTarget.LevelCurve = OffsetMinCurve;
			mMF_FeedbackBaseTarget.RemapLevelZero = 0f;
			mMF_FeedbackBaseTarget.RemapLevelOne = 1f;
			mMF_FeedbackBaseTarget.InstantLevel = 1f;
			_targets.Add(mMF_FeedbackBaseTarget);
			MMF_FeedbackBaseTarget mMF_FeedbackBaseTarget2 = new MMF_FeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver2 = new MMPropertyReceiver();
			mMPropertyReceiver2.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver2.TargetComponent = TargetRectTransform;
			mMPropertyReceiver2.TargetPropertyName = "offsetMax";
			mMPropertyReceiver2.RelativeValue = RelativeValues;
			mMPropertyReceiver2.Vector2RemapZero = OffsetMaxRemapZero;
			mMPropertyReceiver2.Vector2RemapOne = OffsetMaxRemapOne;
			mMPropertyReceiver2.ShouldModifyValue = ModifyOffsetMax;
			mMF_FeedbackBaseTarget2.Target = mMPropertyReceiver2;
			mMF_FeedbackBaseTarget2.LevelCurve = OffsetMaxCurve;
			mMF_FeedbackBaseTarget2.RemapLevelZero = 0f;
			mMF_FeedbackBaseTarget2.RemapLevelOne = 1f;
			mMF_FeedbackBaseTarget2.InstantLevel = 1f;
			_targets.Add(mMF_FeedbackBaseTarget2);
		}
	}
}
