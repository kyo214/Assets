using System;
using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class Destructible : MonoBehaviour
{
	[HideInInspector]
	public float totalHitPoints = 50f;

	[HideInInspector]
	public float currentHitPoints = 50f;

	[HideInInspector]
	public List<DamageLevel> damageLevels;

	[HideInInspector]
	public GameObject destroyedPrefab;

	[HideInInspector]
	public GameObject destroyedPrefabParent;

	[HideInInspector]
	public ParticleSystem fallbackParticle;

	[HideInInspector]
	public int fallbackParticleMatOption;

	[HideInInspector]
	public List<DamageEffect> damageEffects;

	[HideInInspector]
	public float velocityReduction = 0.5f;

	[HideInInspector]
	public float ignoreCollisionsUnder = 2f;

	[HideInInspector]
	public List<GameObject> unparentOnDestroy;

	[HideInInspector]
	public bool disableKinematicOnUparentedChildren = true;

	[HideInInspector]
	public List<MaterialMapping> replaceMaterials;

	[HideInInspector]
	public List<MaterialMapping> replaceParticleMats;

	[HideInInspector]
	public bool canBeDestroyed = true;

	[HideInInspector]
	public bool canBeRepaired = true;

	[HideInInspector]
	public bool canBeObliterated = true;

	[HideInInspector]
	public List<string> debrisToReParentByName;

	[HideInInspector]
	public bool debrisToReParentIsKinematic;

	[HideInInspector]
	public List<string> childrenToReParentByName;

	[HideInInspector]
	public int destructibleGroupId;

	[HideInInspector]
	public bool isDebrisChipAway;

	[HideInInspector]
	public float chipAwayDebrisMass = 1f;

	[HideInInspector]
	public float chipAwayDebrisDrag;

	[HideInInspector]
	public float chipAwayDebrisAngularDrag = 0.05f;

	[HideInInspector]
	public bool autoPoolDestroyedPrefab = true;

	[HideInInspector]
	public bool useFallbackParticle = true;

	[HideInInspector]
	public Vector3 centerPointOverride;

	[HideInInspector]
	public Vector3 fallbackParticleScale = Vector3.one;

	[HideInInspector]
	public bool sinkWhenDestroyed;

	[HideInInspector]
	public bool shouldDeactivate;

	[HideInInspector]
	public bool isTerrainTree;

	private const float InvulnerableTimer = 0.5f;

	private DamageLevel _currentDamageLevel;

	private bool _isObliterated;

	private bool _isInitialized;

	private float _deactivateTimer;

	private bool _firstFixedUpdate = true;

	private Rigidbody _rigidBody;

	private bool _isInvulnerable;

	public bool UseProgressiveDamage { get; set; } = true;

	public bool CheckForClingingDebris { get; set; } = true;

	public Rigidbody[] PooledRigidbodies { get; set; }

	public GameObject[] PooledRigidbodyGos { get; set; }

	public float VelocityReduction => Mathf.Abs(velocityReduction - 1f);

	public Quaternion RotationFixedUpdate { get; private set; }

	public Vector3 PositionFixedUpdate { get; private set; }

	public Vector3 VelocityFixedUpdate { get; private set; }

	public Vector3 AngularVelocityFixedUpdate { get; private set; }

	public float LastRepairedAmount { get; private set; }

	public float LastDamagedAmount { get; private set; }

	public bool IsDestroyed
	{
		get
		{
			if (!_isInvulnerable && canBeDestroyed)
			{
				return currentHitPoints <= 0f;
			}
			return false;
		}
	}

	public Vector3 MeshCenterPoint { get; private set; }

	public event Action DamagedEvent;

	public event Action DestroyedEvent;

	public event Action RepairedEvent;

	public void Start()
	{
		CheckForClingingDebris = true;
		if (damageLevels == null || damageLevels.Count == 0)
		{
			damageLevels = DestructibleHelper.DefaultDamageLevels();
		}
		damageLevels.CalculateDamageLevels(totalHitPoints);
		_rigidBody = GetComponent<Rigidbody>();
		if (useFallbackParticle)
		{
			MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
			MeshCenterPoint = base.gameObject.GetMeshCenterPoint(componentsInChildren);
			if (base.gameObject.IsAnyMeshPartOfStaticBatch(componentsInChildren) && centerPointOverride == Vector3.zero)
			{
				Debug.Log("[" + base.gameObject.name + "] is a Destructible object with one or more static meshes, but no position override for the fallback particle effect. Particle effect may not spawn where you expect.");
			}
		}
		PlayDamageEffects();
		_isInvulnerable = true;
		Invoke("RemoveInvulnerability", 0.5f);
		if (base.gameObject.HasTagInParent(Tag.TerrainTree))
		{
			isTerrainTree = true;
		}
		if (autoPoolDestroyedPrefab)
		{
			ObjectPool.Instance.AddDestructibleObjectToPool(this);
		}
		_isInitialized = true;
	}

	public void RemoveInvulnerability()
	{
		_isInvulnerable = false;
	}

	public void FixedUpdate()
	{
		if (!_isInitialized)
		{
			return;
		}
		DestructionManager instance = DestructionManager.Instance;
		if (!(instance == null))
		{
			PositionFixedUpdate = base.transform.position;
			RotationFixedUpdate = base.transform.rotation;
			if (_rigidBody != null)
			{
				VelocityFixedUpdate = _rigidBody.velocity;
				AngularVelocityFixedUpdate = _rigidBody.angularVelocity;
			}
			SetDamageLevel();
			PlayDamageEffects();
			if (instance.autoDeactivateDestructibles && !isTerrainTree && shouldDeactivate)
			{
				UpdateDeactivation(instance.deactivateAfter);
			}
			else if (instance.autoDeactivateDestructibleTerrainObjects && isTerrainTree && shouldDeactivate)
			{
				UpdateDeactivation(instance.deactivateAfter);
			}
			if (IsDestroyed)
			{
				instance.ProcessDestruction(this, destroyedPrefab, new ExplosiveDamage(), _isObliterated);
			}
			if (_firstFixedUpdate)
			{
				this.SetActiveOrInactive(instance);
			}
			_firstFixedUpdate = false;
		}
	}

	private void UpdateDeactivation(float deactivateAfter)
	{
		if (_deactivateTimer > deactivateAfter)
		{
			_deactivateTimer = 0f;
			shouldDeactivate = false;
			base.enabled = false;
		}
		else
		{
			_deactivateTimer += Time.fixedDeltaTime;
		}
	}

	public void ApplyDamage(float amount)
	{
		if (IsDestroyed || _isInvulnerable)
		{
			return;
		}
		LastDamagedAmount = amount;
		FireDamagedEvent();
		currentHitPoints -= amount;
		CheckForObliterate(amount);
		if (!(currentHitPoints > 0f))
		{
			if (currentHitPoints < 0f)
			{
				currentHitPoints = 0f;
			}
			PlayDamageEffects();
			if (IsDestroyed)
			{
				DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, new DirectDamage
				{
					DamageAmount = amount
				}, _isObliterated);
			}
		}
	}

	public void ApplyDamage(Damage damage)
	{
		if (IsDestroyed || _isInvulnerable)
		{
			return;
		}
		LastDamagedAmount = damage.DamageAmount;
		FireDamagedEvent();
		currentHitPoints -= damage.DamageAmount;
		CheckForObliterate(damage.DamageAmount);
		if (!(currentHitPoints > 0f))
		{
			if (currentHitPoints < 0f)
			{
				currentHitPoints = 0f;
			}
			PlayDamageEffects();
			if (IsDestroyed)
			{
				DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, damage, _isObliterated);
			}
		}
	}

	public void RepairDamage(float amount)
	{
		if (!IsDestroyed && canBeRepaired)
		{
			LastRepairedAmount = amount;
			currentHitPoints += amount;
			if (currentHitPoints > totalHitPoints)
			{
				currentHitPoints = totalHitPoints;
			}
			PlayDamageEffects();
			FireRepairedEvent();
		}
	}

	public void Destroy()
	{
		if (!IsDestroyed && !_isInvulnerable)
		{
			LastDamagedAmount = currentHitPoints;
			FireDamagedEvent();
			currentHitPoints = 0f;
			PlayDamageEffects();
			DestructionManager.Instance.ProcessDestruction(this, destroyedPrefab, currentHitPoints, _isObliterated);
		}
	}

	private void CheckForObliterate(float damage)
	{
		if (!_isInvulnerable && canBeDestroyed && canBeObliterated && damage >= (float)DestructionManager.Instance.obliterateMultiplier * totalHitPoints)
		{
			_isObliterated = true;
		}
	}

	private void SetDamageLevel()
	{
		DamageLevel damageLevel = damageLevels?.GetDamageLevel(currentHitPoints);
		if (damageLevel == null || (_currentDamageLevel != null && damageLevel == _currentDamageLevel))
		{
			return;
		}
		_currentDamageLevel = damageLevel;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!(renderer.GetComponentInParent<Destructible>() != this) && (renderer is MeshRenderer || renderer is SkinnedMeshRenderer) && !renderer.gameObject.HasTag(default(Tag)) && renderer.gameObject.layer != DestructionManager.Instance.debrisLayer)
			{
				for (int j = 0; j < renderer.sharedMaterials.Length; j++)
				{
					DestructionManager.Instance.SetProgressiveDamageTexture(renderer, renderer.sharedMaterials[j], _currentDamageLevel);
				}
			}
		}
		PlayDamageEffects();
	}

	public Material GetDestroyedParticleEffectMaterial()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!(renderer.GetComponentInParent<Destructible>() != this) && (renderer is MeshRenderer || renderer is SkinnedMeshRenderer))
			{
				return renderer.sharedMaterial;
			}
		}
		return null;
	}

	private void PlayDamageEffects()
	{
		if (damageEffects == null || damageEffects.Count == 0)
		{
			return;
		}
		int num = 0;
		if (_currentDamageLevel != null)
		{
			num = damageLevels.IndexOf(_currentDamageLevel);
		}
		foreach (DamageEffect damageEffect in damageEffects)
		{
			if (damageEffect == null || damageEffect.Prefab == null)
			{
				continue;
			}
			Quaternion rotation = base.transform.rotation;
			if (damageEffect.Rotation != Vector3.zero)
			{
				rotation = base.transform.rotation * Quaternion.Euler(damageEffect.Rotation);
			}
			if (damageEffect.HasTagDependency && !base.gameObject.HasTag(damageEffect.TagDependency))
			{
				continue;
			}
			if (_currentDamageLevel != null && damageEffect.TriggeredAt < damageLevels.Count)
			{
				if (num >= damageEffect.TriggeredAt && !damageEffect.HasStarted)
				{
					if (damageEffect.GameObject != null)
					{
						for (int i = 0; i < damageEffect.ParticleSystems.Length; i++)
						{
							ParticleSystem.EmissionModule emission = damageEffect.ParticleSystems[i].emission;
							emission.enabled = true;
						}
					}
					else
					{
						damageEffect.GameObject = ObjectPool.Instance.Spawn(damageEffect.Prefab, damageEffect.Offset, rotation, base.transform);
						if (damageEffect.GameObject != null)
						{
							damageEffect.ParticleSystems = damageEffect.GameObject.GetComponentsInChildren<ParticleSystem>();
						}
					}
					damageEffect.HasStarted = true;
				}
				if (num < damageEffect.TriggeredAt && damageEffect.HasStarted)
				{
					if (damageEffect.GameObject != null)
					{
						for (int j = 0; j < damageEffect.ParticleSystems.Length; j++)
						{
							ParticleSystem.EmissionModule emission2 = damageEffect.ParticleSystems[j].emission;
							emission2.enabled = false;
						}
					}
					damageEffect.HasStarted = false;
				}
			}
			if (damageEffect.TriggeredAt == damageLevels.Count && IsDestroyed && !damageEffect.HasStarted)
			{
				damageEffect.GameObject = (canBeDestroyed ? ObjectPool.Instance.Spawn(damageEffect.Prefab, base.transform.TransformPoint(damageEffect.Offset), rotation) : ObjectPool.Instance.Spawn(damageEffect.Prefab, damageEffect.Offset, rotation, base.transform));
				if (damageEffect.GameObject != null)
				{
					damageEffect.ParticleSystems = damageEffect.GameObject.GetComponentsInChildren<ParticleSystem>();
				}
				damageEffect.HasStarted = true;
			}
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (DestructionManager.Instance == null || !base.isActiveAndEnabled)
		{
			return;
		}
		this.ProcessDestructibleCollision(collision, GetComponent<Rigidbody>());
		if (collision.contacts.Length != 0)
		{
			Destructible componentInParent = collision.contacts[0].otherCollider.gameObject.GetComponentInParent<Destructible>();
			if (componentInParent != null && collision.contacts[0].otherCollider.attachedRigidbody == null)
			{
				componentInParent.ProcessDestructibleCollision(collision, GetComponent<Rigidbody>());
			}
		}
	}

	public void OnDrawGizmos()
	{
		damageEffects.DrawGizmos(base.transform);
		centerPointOverride.DrawGizmos(base.transform);
	}

	public void FireDestroyedEvent()
	{
		DestroyedEvent?.Invoke();
	}

	public void FireRepairedEvent()
	{
		RepairedEvent?.Invoke();
	}

	public void FireDamagedEvent()
	{
		DamagedEvent?.Invoke();
	}
}
