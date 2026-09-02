using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will instantiate the specified ParticleSystem at the specified position on Start or on Play, optionally nesting them.")]
[FeedbackPath("Particles/Particles Instantiation")]
public class MMF_ParticlesInstantiation : MMF_Feedback
{
	public enum PositionModes
	{
		FeedbackPosition = 0,
		Transform = 1,
		WorldPosition = 2,
		Script = 3
	}

	public enum Modes
	{
		Cached = 0,
		OnDemand = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Particles Instantiation", true, 37, true, false)]
	[Tooltip("whether the particle system should be cached or created on demand the first time")]
	public Modes Mode;

	[Tooltip("if this is false, a brand new particle system will be created every time")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public bool CachedRecycle = true;

	[Tooltip("the particle system to spawn")]
	public ParticleSystem ParticlesPrefab;

	[Tooltip("the possible random particle systems")]
	public List<ParticleSystem> RandomParticlePrefabs;

	[Tooltip("if this is true, the particle system game object will be activated on Play, useful if you've somehow disabled it in a past Play")]
	public bool ForceSetActiveOnPlay;

	[MMFInspectorGroup("Position", true, 29, false, false)]
	[Tooltip("the selected position mode")]
	public PositionModes PositionMode;

	[Tooltip("the position at which to spawn this particle system")]
	[MMFEnumCondition("PositionMode", new int[] { 1 })]
	public Transform InstantiateParticlesPosition;

	[Tooltip("the world position to move to when in WorldPosition mode")]
	[MMFEnumCondition("PositionMode", new int[] { 2 })]
	public Vector3 TargetWorldPosition;

	[Tooltip("an offset to apply to the instantiation position")]
	public Vector3 Offset;

	[Tooltip("whether or not the particle system should be nested in hierarchy or floating on its own")]
	[MMFEnumCondition("PositionMode", new int[] { 1, 0 })]
	public bool NestParticles = true;

	[Tooltip("whether or not to also apply rotation")]
	public bool ApplyRotation;

	[Tooltip("whether or not to also apply scale")]
	public bool ApplyScale;

	protected ParticleSystem _instantiatedParticleSystem;

	protected List<ParticleSystem> _instantiatedRandomParticleSystems;

	protected override void CustomInitialization(MMF_Player owner)
	{
		if (Active && Mode == Modes.Cached)
		{
			InstantiateParticleSystem();
		}
	}

	protected virtual void InstantiateParticleSystem()
	{
		if (ParticlesPrefab == null)
		{
			return;
		}
		if (CachedRecycle && _instantiatedParticleSystem != null)
		{
			PositionParticleSystem(_instantiatedParticleSystem);
			return;
		}
		Transform transform = null;
		if (NestParticles)
		{
			if (PositionMode == PositionModes.FeedbackPosition)
			{
				transform = Owner.transform;
			}
			if (PositionMode == PositionModes.Transform)
			{
				transform = InstantiateParticlesPosition;
			}
		}
		if (RandomParticlePrefabs.Count > 0)
		{
			if (Mode == Modes.Cached)
			{
				_instantiatedRandomParticleSystems = new List<ParticleSystem>();
				foreach (ParticleSystem randomParticlePrefab in RandomParticlePrefabs)
				{
					ParticleSystem particleSystem = Object.Instantiate(randomParticlePrefab, transform);
					if (transform == null)
					{
						SceneManager.MoveGameObjectToScene(particleSystem.gameObject, Owner.gameObject.scene);
					}
					_instantiatedRandomParticleSystems.Add(particleSystem);
				}
			}
			else
			{
				int index = Random.Range(0, RandomParticlePrefabs.Count);
				_instantiatedParticleSystem = Object.Instantiate(RandomParticlePrefabs[index], transform);
				if (transform == null)
				{
					SceneManager.MoveGameObjectToScene(_instantiatedParticleSystem.gameObject, Owner.gameObject.scene);
				}
			}
		}
		else
		{
			_instantiatedParticleSystem = Object.Instantiate(ParticlesPrefab, transform);
			if (transform == null)
			{
				SceneManager.MoveGameObjectToScene(_instantiatedParticleSystem.gameObject, Owner.gameObject.scene);
			}
		}
		if (_instantiatedParticleSystem != null)
		{
			PositionParticleSystem(_instantiatedParticleSystem);
		}
		if (_instantiatedRandomParticleSystems == null || _instantiatedRandomParticleSystems.Count <= 0)
		{
			return;
		}
		foreach (ParticleSystem instantiatedRandomParticleSystem in _instantiatedRandomParticleSystems)
		{
			PositionParticleSystem(instantiatedRandomParticleSystem);
		}
	}

	protected virtual void PositionParticleSystem(ParticleSystem system)
	{
		if (InstantiateParticlesPosition == null && Owner != null)
		{
			InstantiateParticlesPosition = Owner.transform;
		}
		if (system != null)
		{
			system.Stop();
		}
		system.transform.position = GetPosition(Owner.transform.position);
		if (ApplyRotation)
		{
			system.transform.rotation = GetRotation(Owner.transform);
		}
		if (ApplyScale)
		{
			system.transform.localScale = GetScale(Owner.transform);
		}
		system.Clear();
	}

	protected virtual Quaternion GetRotation(Transform target)
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => Owner.transform.rotation, 
			PositionModes.Transform => InstantiateParticlesPosition.rotation, 
			PositionModes.WorldPosition => Quaternion.identity, 
			PositionModes.Script => Owner.transform.rotation, 
			_ => Owner.transform.rotation, 
		};
	}

	protected virtual Vector3 GetScale(Transform target)
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => Owner.transform.localScale, 
			PositionModes.Transform => InstantiateParticlesPosition.localScale, 
			PositionModes.WorldPosition => Owner.transform.localScale, 
			PositionModes.Script => Owner.transform.localScale, 
			_ => Owner.transform.localScale, 
		};
	}

	protected virtual Vector3 GetPosition(Vector3 position)
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => Owner.transform.position + Offset, 
			PositionModes.Transform => InstantiateParticlesPosition.position + Offset, 
			PositionModes.WorldPosition => TargetWorldPosition + Offset, 
			PositionModes.Script => position + Offset, 
			_ => position + Offset, 
		};
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		if (Mode == Modes.OnDemand)
		{
			InstantiateParticleSystem();
		}
		if (_instantiatedParticleSystem != null)
		{
			if (ForceSetActiveOnPlay)
			{
				_instantiatedParticleSystem.gameObject.SetActive(value: true);
			}
			_instantiatedParticleSystem.Stop();
			_instantiatedParticleSystem.transform.position = GetPosition(position);
			_instantiatedParticleSystem.Play();
		}
		if (_instantiatedRandomParticleSystems == null || _instantiatedRandomParticleSystems.Count <= 0)
		{
			return;
		}
		foreach (ParticleSystem instantiatedRandomParticleSystem in _instantiatedRandomParticleSystems)
		{
			if (ForceSetActiveOnPlay)
			{
				instantiatedRandomParticleSystem.gameObject.SetActive(value: true);
			}
			instantiatedRandomParticleSystem.Stop();
			instantiatedRandomParticleSystem.transform.position = GetPosition(position);
		}
		int index = Random.Range(0, _instantiatedRandomParticleSystems.Count);
		_instantiatedRandomParticleSystems[index].Play();
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		if (_instantiatedParticleSystem != null)
		{
			_instantiatedParticleSystem?.Stop();
		}
		if (_instantiatedRandomParticleSystems == null || _instantiatedRandomParticleSystems.Count <= 0)
		{
			return;
		}
		foreach (ParticleSystem instantiatedRandomParticleSystem in _instantiatedRandomParticleSystems)
		{
			instantiatedRandomParticleSystem.Stop();
		}
	}

	protected override void CustomReset()
	{
		base.CustomReset();
		if (!Active || !FeedbackTypeAuthorized || InCooldown)
		{
			return;
		}
		if (_instantiatedParticleSystem != null)
		{
			_instantiatedParticleSystem?.Stop();
		}
		if (_instantiatedRandomParticleSystems == null || _instantiatedRandomParticleSystems.Count <= 0)
		{
			return;
		}
		foreach (ParticleSystem instantiatedRandomParticleSystem in _instantiatedRandomParticleSystems)
		{
			instantiatedRandomParticleSystem.Stop();
		}
	}
}
