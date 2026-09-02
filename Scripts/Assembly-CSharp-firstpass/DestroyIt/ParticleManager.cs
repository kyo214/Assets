using System;
using System.Linq;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class ParticleManager : MonoBehaviour
{
	public int maxDestroyedParticles = 20;

	public int maxPerDestructible = 5;

	public float withinSeconds = 4f;

	public float updateFrequency = 0.5f;

	private float _nextUpdate;

	private ActiveParticle[] _activeParticles;

	public static ParticleManager Instance { get; private set; }

	public ActiveParticle[] ActiveParticles
	{
		get
		{
			return _activeParticles;
		}
		private set
		{
			_activeParticles = value;
		}
	}

	public bool IsMaxActiveParticles => ActiveParticles.Length >= maxDestroyedParticles;

	public event Action ActiveParticlesCounterChangedEvent;

	private ParticleManager()
	{
	}

	public void Awake()
	{
		ActiveParticles = new ActiveParticle[0];
		Instance = this;
		_nextUpdate = Time.time + updateFrequency;
	}

	public void Update()
	{
		if (!(Time.time > _nextUpdate) || _activeParticles.Length == 0)
		{
			return;
		}
		int num = 0;
		int[] array = new int[0];
		bool flag = false;
		for (int i = 0; i < ActiveParticles.Length; i++)
		{
			if (Time.time >= ActiveParticles[i].InstantiatedTime + withinSeconds)
			{
				flag = true;
				num++;
				Array.Resize(ref array, num);
				array[num - 1] = i;
			}
		}
		_activeParticles = _activeParticles.RemoveAllAt(array);
		if (flag)
		{
			FireActiveParticlesCounterChangedEvent();
		}
		_nextUpdate = Time.time + updateFrequency;
	}

	public void PlayEffect(ParticleSystem particle, Destructible destObj, Vector3 pos, Quaternion rot, int parentId)
	{
		if (particle == null)
		{
			particle = DestructionManager.Instance.defaultParticle;
		}
		if (IsMaxActiveParticles || ActiveParticles.Count((ActiveParticle x) => x.ParentId == parentId) > maxPerDestructible)
		{
			return;
		}
		GameObject gameObject = ObjectPool.Instance.Spawn(particle.gameObject, pos, rot);
		if (gameObject == null || gameObject.GetComponent<ParticleSystem>() == null)
		{
			return;
		}
		ActiveParticle activeParticle = new ActiveParticle
		{
			GameObject = gameObject,
			InstantiatedTime = Time.time,
			ParentId = parentId
		};
		Array.Resize(ref _activeParticles, _activeParticles.Length + 1);
		ActiveParticles[_activeParticles.Length - 1] = activeParticle;
		FireActiveParticlesCounterChangedEvent();
		if (destObj.fallbackParticleScale != Vector3.one)
		{
			ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
			for (int num = 0; num < componentsInChildren.Length; num++)
			{
				ParticleSystem.MainModule main = componentsInChildren[num].main;
				main.scalingMode = ParticleSystemScalingMode.Hierarchy;
			}
			gameObject.transform.localScale = destObj.fallbackParticleScale;
			PoolAfter component = gameObject.GetComponent<PoolAfter>();
			if (component != null)
			{
				component.resetToPrefab = true;
			}
		}
		if (destObj.fallbackParticleMatOption == 1 || gameObject.GetComponent<ParticleSystem>() == null)
		{
			return;
		}
		ParticleSystemRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
		ParticleSystemRenderer[] array;
		if (destObj.fallbackParticleMatOption == 0)
		{
			array = componentsInChildren2;
			foreach (ParticleSystemRenderer particleSystemRenderer in array)
			{
				if (particleSystemRenderer.renderMode == ParticleSystemRenderMode.Mesh)
				{
					particleSystemRenderer.material = destObj.GetDestroyedParticleEffectMaterial();
					if (particleSystemRenderer.sharedMaterial.IsProgressiveDamageCapable())
					{
						Texture2D detailMask = DestructionManager.Instance.GetDetailMask(particleSystemRenderer.sharedMaterial, destObj.damageLevels[destObj.damageLevels.Count - 1]);
						particleSystemRenderer.material.SetTexture("_DetailMask", detailMask);
					}
				}
			}
		}
		if (destObj.fallbackParticleMatOption != 2)
		{
			return;
		}
		array = componentsInChildren2;
		foreach (ParticleSystemRenderer particleRenderer in array)
		{
			if (particleRenderer.renderMode == ParticleSystemRenderMode.Mesh)
			{
				MaterialMapping materialMapping = destObj.replaceParticleMats.Find((MaterialMapping x) => x.SourceMaterial == particleRenderer.sharedMaterial);
				Material material = ((materialMapping == null) ? particleRenderer.sharedMaterial : materialMapping.ReplacementMaterial);
				particleRenderer.material = material ?? destObj.GetDestroyedParticleEffectMaterial();
				if (particleRenderer.sharedMaterial.IsProgressiveDamageCapable())
				{
					Texture2D detailMask2 = DestructionManager.Instance.GetDetailMask(particleRenderer.sharedMaterial, destObj.damageLevels[destObj.damageLevels.Count - 1]);
					particleRenderer.material.SetTexture("_DetailMask", detailMask2);
				}
			}
		}
	}

	public void FireActiveParticlesCounterChangedEvent()
	{
		if (ActiveParticlesCounterChangedEvent != null)
		{
			ActiveParticlesCounterChangedEvent();
		}
	}
}
