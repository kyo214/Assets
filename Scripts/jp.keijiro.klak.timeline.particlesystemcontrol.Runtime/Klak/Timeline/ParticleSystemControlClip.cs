using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline;

[Serializable]
public class ParticleSystemControlClip : PlayableAsset, ITimelineClipAsset
{
	public ParticleSystemControlPlayable template = new ParticleSystemControlPlayable();

	public ClipCaps clipCaps => ClipCaps.Blending;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		return ScriptPlayable<ParticleSystemControlPlayable>.Create(graph, template);
	}
}
