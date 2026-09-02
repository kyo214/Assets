using UnityEngine;

namespace MoreMountains.Tools;

[ExecuteAlways]
[AddComponentMenu("More Mountains/Tools/Particles/MMDelayParticles")]
public class MMDelayParticles : MonoBehaviour
{
	[Header("Delay")]
	public float Delay;

	public bool DelayChildren = true;

	public bool ApplyDelayOnStart;

	[MMInspectorButton("ApplyDelay")]
	public bool ApplyDelayButton;

	protected Component[] particleSystems;

	protected virtual void Start()
	{
		if (ApplyDelayOnStart)
		{
			ApplyDelay();
		}
	}

	protected virtual void ApplyDelay()
	{
		if (base.gameObject.GetComponent<ParticleSystem>() != null)
		{
			ParticleSystem.MainModule main = base.gameObject.GetComponent<ParticleSystem>().main;
			main.startDelay = main.startDelay.constant + Delay;
		}
		Component[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		particleSystems = componentsInChildren;
		componentsInChildren = particleSystems;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem.MainModule main2 = ((ParticleSystem)componentsInChildren[i]).main;
			main2.startDelay = main2.startDelay.constant + Delay;
		}
	}
}
