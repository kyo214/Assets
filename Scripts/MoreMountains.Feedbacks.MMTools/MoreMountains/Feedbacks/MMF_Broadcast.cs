using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you broadcast a float value to the MMRadio system.")]
[FeedbackPath("GameObject/Broadcast")]
public class MMF_Broadcast : MMF_FeedbackBase
{
	[Header("Level")]
	[Tooltip("the curve to tween the intensity on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType Curve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the intensity curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapZero;

	[Tooltip("the value to remap the intensity curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapOne = 1f;

	[Tooltip("the value to move the intensity to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantChange;

	protected MMF_BroadcastProxy _proxy;

	public override bool HasChannel => true;

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		_proxy = Owner.gameObject.AddComponent<MMF_BroadcastProxy>();
		_proxy.Channel = Channel;
		PrepareTargets();
	}

	protected override void FillTargets()
	{
		MMF_FeedbackBaseTarget mMF_FeedbackBaseTarget = new MMF_FeedbackBaseTarget();
		MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
		mMPropertyReceiver.TargetObject = Owner.gameObject;
		mMPropertyReceiver.TargetComponent = _proxy;
		mMPropertyReceiver.TargetPropertyName = "ThisLevel";
		mMPropertyReceiver.RelativeValue = RelativeValues;
		mMF_FeedbackBaseTarget.Target = mMPropertyReceiver;
		mMF_FeedbackBaseTarget.LevelCurve = Curve;
		mMF_FeedbackBaseTarget.RemapLevelZero = RemapZero;
		mMF_FeedbackBaseTarget.RemapLevelOne = RemapOne;
		mMF_FeedbackBaseTarget.InstantLevel = InstantChange;
		_targets.Add(mMF_FeedbackBaseTarget);
	}
}
