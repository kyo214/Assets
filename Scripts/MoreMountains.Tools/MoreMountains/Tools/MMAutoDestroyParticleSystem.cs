using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Particles/MMAutoDestroyParticleSystem")]
public class MMAutoDestroyParticleSystem : MonoBehaviour
{
	public bool DestroyParent;

	public float DestroyDelay;

	protected ParticleSystem _particleSystem;

	protected float _startTime;

	protected virtual void Start()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		if (DestroyDelay != 0f)
		{
			_startTime = Time.time;
		}
	}

	protected virtual void Update()
	{
		if (DestroyDelay != 0f && Time.time - _startTime > DestroyDelay)
		{
			DestroyParticleSystem();
		}
		if (!_particleSystem.isPlaying)
		{
			DestroyParticleSystem();
		}
	}

	protected virtual void DestroyParticleSystem()
	{
		if (base.transform.parent != null && DestroyParent)
		{
			Object.Destroy(base.transform.parent.gameObject);
		}
		Object.Destroy(base.gameObject);
	}
}
