using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMF_BroadcastProxy : MonoBehaviour
{
	[Tooltip("the channel on which to broadcast")]
	[MMReadOnly]
	public int Channel;

	[Tooltip("a debug view of the current level being broadcasted")]
	[MMReadOnly]
	public float DebugLevel;

	[Tooltip("whether or not a broadcast is in progress (will be false while the value is not changing, and thus not broadcasting)")]
	[MMReadOnly]
	public bool BroadcastInProgress;

	protected float _levelLastFrame;

	public float ThisLevel { get; set; }

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
