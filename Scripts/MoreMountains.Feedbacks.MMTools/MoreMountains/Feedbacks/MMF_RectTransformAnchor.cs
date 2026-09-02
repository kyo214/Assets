using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the min and max anchors of a RectTransform over time. That's the normalized position in the parent RectTransform that the lower left and upper right corners are anchored to.")]
[FeedbackPath("UI/RectTransform Anchor")]
public class MMF_RectTransformAnchor : MMF_FeedbackBase
{
	[MMFInspectorGroup("Target RectTransform", true, 37, true, false)]
	[Tooltip("the target RectTransform to control")]
	public RectTransform TargetRectTransform;

	[MMFInspectorGroup("Anchor Min", true, 43, false, false)]
	[Tooltip("whether or not to modify the min anchor")]
	public bool ModifyAnchorMin = true;

	[Tooltip("the curve to animate the min anchor on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType AnchorMinCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

	[Tooltip("the value to remap the min anchor curve's 0 on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Vector2 AnchorMinRemapZero = Vector2.zero;

	[Tooltip("the value to remap the min anchor curve's 1 on")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 AnchorMinRemapOne = Vector2.one;

	[MMFInspectorGroup("Anchor Max", true, 44, false, false)]
	[Tooltip("whether or not to modify the max anchor")]
	public bool ModifyAnchorMax = true;

	[Tooltip("the curve to animate the max anchor on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType AnchorMaxCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

	[Tooltip("the value to remap the max anchor curve's 0 on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Vector2 AnchorMaxRemapZero = Vector2.zero;

	[Tooltip("the value to remap the max anchor curve's 1 on")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 AnchorMaxRemapOne = Vector2.one;

	protected override void FillTargets()
	{
		if (!(TargetRectTransform == null))
		{
			MMF_FeedbackBaseTarget mMF_FeedbackBaseTarget = new MMF_FeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
			mMPropertyReceiver.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver.TargetComponent = TargetRectTransform;
			mMPropertyReceiver.TargetPropertyName = "anchorMin";
			mMPropertyReceiver.RelativeValue = RelativeValues;
			mMPropertyReceiver.Vector2RemapZero = AnchorMinRemapZero;
			mMPropertyReceiver.Vector2RemapOne = AnchorMinRemapOne;
			mMPropertyReceiver.ShouldModifyValue = ModifyAnchorMin;
			mMF_FeedbackBaseTarget.Target = mMPropertyReceiver;
			mMF_FeedbackBaseTarget.LevelCurve = AnchorMinCurve;
			mMF_FeedbackBaseTarget.RemapLevelZero = 0f;
			mMF_FeedbackBaseTarget.RemapLevelOne = 1f;
			mMF_FeedbackBaseTarget.InstantLevel = 1f;
			_targets.Add(mMF_FeedbackBaseTarget);
			MMF_FeedbackBaseTarget mMF_FeedbackBaseTarget2 = new MMF_FeedbackBaseTarget();
			MMPropertyReceiver mMPropertyReceiver2 = new MMPropertyReceiver();
			mMPropertyReceiver2.TargetObject = TargetRectTransform.gameObject;
			mMPropertyReceiver2.TargetComponent = TargetRectTransform;
			mMPropertyReceiver2.TargetPropertyName = "anchorMax";
			mMPropertyReceiver2.RelativeValue = RelativeValues;
			mMPropertyReceiver2.Vector2RemapZero = AnchorMaxRemapZero;
			mMPropertyReceiver2.Vector2RemapOne = AnchorMaxRemapOne;
			mMPropertyReceiver2.ShouldModifyValue = ModifyAnchorMax;
			mMF_FeedbackBaseTarget2.Target = mMPropertyReceiver2;
			mMF_FeedbackBaseTarget2.LevelCurve = AnchorMaxCurve;
			mMF_FeedbackBaseTarget2.RemapLevelZero = 0f;
			mMF_FeedbackBaseTarget2.RemapLevelOne = 1f;
			mMF_FeedbackBaseTarget2.InstantLevel = 1f;
			_targets.Add(mMF_FeedbackBaseTarget2);
		}
	}
}
