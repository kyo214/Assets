using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMFeedbackBaseTarget
{
	public MMPropertyReceiver Target;

	public MMTweenType LevelCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	public float RemapLevelZero;

	public float RemapLevelOne = 1f;

	public float InstantLevel;

	public float InitialLevel;
}
