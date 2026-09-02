using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.CustomTrack.DeactivateTrack;

[Serializable]
public class DeactivationClip : PlayableAsset, ITimelineClipAsset
{
	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		return Playable.Create(graph);
	}
}
