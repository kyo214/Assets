using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[Serializable]
public class MMSequenceTrack
{
	public int ID;

	public Color TrackColor;

	public KeyCode Key = KeyCode.Space;

	public bool Active = true;

	[MMFReadOnly]
	public MMSequenceTrackStates State;

	[HideInInspector]
	public bool Initialized;

	public virtual void SetDefaults(int index)
	{
		if (!Initialized)
		{
			ID = index;
			TrackColor = MMSequence.RandomSequenceColor();
			Key = KeyCode.Space;
			Active = true;
			State = MMSequenceTrackStates.Idle;
			Initialized = true;
		}
	}
}
