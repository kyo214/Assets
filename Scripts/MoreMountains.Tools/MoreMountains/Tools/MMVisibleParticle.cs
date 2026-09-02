using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Particles/MMVisibleParticle")]
public class MMVisibleParticle : MonoBehaviour
{
	protected virtual void Start()
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "VisibleParticles";
	}
}
