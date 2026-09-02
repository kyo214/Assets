using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you broadcast a float value to the MMRadio system.")]
[FeedbackPath("GameObject/Broadcast")]
public class MMFeedbackBroadcast : MMFeedbackBase
{
	[Header("Target Channel")]
	[Tooltip("the channel to write the level to")]
	public int Channel;

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

	[Tooltip("a debug view of the current level being broadcasted")]
	[MMReadOnly]
	public float DebugLevel;

	[Tooltip("whether or not a broadcast is in progress (will be false while the value is not changing, and thus not broadcasting)")]
	[MMReadOnly]
	public bool BroadcastInProgress;

	protected float _levelLastFrame;

	public float ThisLevel { get; set; }

	protected override void FillTargets()
	{
		MMFeedbackBaseTarget mMFeedbackBaseTarget = new MMFeedbackBaseTarget();
		MMPropertyReceiver mMPropertyReceiver = new MMPropertyReceiver();
		mMPropertyReceiver.TargetObject = base.gameObject;
		mMPropertyReceiver.TargetComponent = this;
		mMPropertyReceiver.TargetPropertyName = "ThisLevel";
		mMPropertyReceiver.RelativeValue = RelativeValues;
		mMFeedbackBaseTarget.Target = mMPropertyReceiver;
		mMFeedbackBaseTarget.LevelCurve = Curve;
		mMFeedbackBaseTarget.RemapLevelZero = RemapZero;
		mMFeedbackBaseTarget.RemapLevelOne = RemapOne;
		mMFeedbackBaseTarget.InstantLevel = InstantChange;
		_targets.Add(mMFeedbackBaseTarget);
	}

	protected virtual void Update()
	{
		ProcessBroadcast();
	}

	protected virtual void ProcessBroadcast()
	{
		BroadcastInProgress = false;
		if (ThisLevel != _levelLastFrame)
		{
			MMRadioLevelEvent.Trigger(Channel, ThisLevel);
			BroadcastInProgress = true;
		}
		DebugLevel = ThisLevel;
		_levelLastFrame = ThisLevel;
	}
}
