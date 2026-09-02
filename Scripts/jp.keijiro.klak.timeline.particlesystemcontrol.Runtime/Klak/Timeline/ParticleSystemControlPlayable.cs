using System;
using UnityEngine.Playables;

namespace Klak.Timeline;

[Serializable]
public class ParticleSystemControlPlayable : PlayableBehaviour
{
	public float rateOverTime = 10f;

	public float rateOverDistance;
}
