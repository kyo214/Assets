using System;

namespace MoreMountains.Feedbacks;

[Serializable]
public class BlinkPhase
{
	public float PhaseDuration = 1f;

	public float OffDuration = 0.2f;

	public float OnDuration = 0.1f;

	public float OffLerpDuration = 0.05f;

	public float OnLerpDuration = 0.05f;
}
