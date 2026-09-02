using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class DestructionManager : MonoBehaviour
{
	[Tooltip("If true, Destructible scripts will be deactivated on start, and will activate any time they are inside a trigger collider with the ActivateDestructibles script on it.")]
	[HideInInspector]
	public bool autoDeactivateDestructibles;

	[Tooltip("If true, Destructible terrain object scripts will be deactivated on start, and will activate any time they are inside a trigger collider with the ActivateDestructibles script on it.")]
	[HideInInspector]
	public bool autoDeactivateDestructibleTerrainObjects = true;

	[Tooltip("If true, Destructible terrain tree scripts will not be activated by ActivateDestructibles scripts. Recommended to leave this true for performance, unless you need to move trees during the game or use progressive damage textures on them.")]
	[HideInInspector]
	public bool destructibleTreesStayDeactivated = true;

	[Tooltip("The time in seconds to automatically deactivate Destructible scripts when they are outside an ActivateDestructibles trigger area.")]
	[HideInInspector]
	public float deactivateAfter = 2f;

	[Tooltip("Maximum allowed persistent debris pieces in the scene.")]
	public int maxPersistentDebris = 400;

	[Tooltip("Maximum allowed destroyed prefabs within [withinSeconds] seconds. When this limit is reached, a particle effect will be used instead.")]
	public int destroyedPrefabLimit = 15;

	[Tooltip("Number of seconds within which no more than [destroyedPrefabLimit] destructions will be instantiated.")]
	public int withinSeconds = 4;

	[Tooltip("The default particle effect to use when an object is destroyed.")]
	public ParticleSystem defaultParticle;

	[Tooltip("If damage done to an object is more than this many times its hit points, it will be obliterated into a particle effect if CanBeObliterated=true.")]
	public int obliterateMultiplier = 3;

	[Tooltip("If true, persistent debris is allowed to be culled even if the camera is currently rendering it.")]
	public bool removeVisibleDebris = true;

	[Tooltip("The time (in seconds) this script processes updates.")]
	public float updateFrequency = 0.5f;

	[HideInInspector]
	public bool useCameraDistanceLimit = true;

	[HideInInspector]
	public int cameraDistanceLimit = 100;

	[HideInInspector]
	public int debrisLayer = -1;

	[HideInInspector]
	public Collider[] overlapColliders;

	private float _nextUpdate;

	private List<Destructible> _destroyedObjects;

	private List<Debris> _debrisPieces;

	private List<Texture2D> _detailMasks;

	private static DestructionManager _instance;

	public List<float> DestroyedPrefabCounter { get; private set; }

	public bool IsDestroyedPrefabLimitReached => DestroyedPrefabCounter.Count >= destroyedPrefabLimit;

	public int ActiveDebrisCount
	{
		get
		{
			int num = 0;
			foreach (Debris debrisPiece in _debrisPieces)
			{
				if (debrisPiece.IsActive)
				{
					num++;
				}
			}
			return num;
		}
	}

	public static DestructionManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<DestructionManager>();
			}
			return _instance;
		}
	}

	public event Action DestroyedPrefabCounterChangedEvent;

	public event Action ActiveDebrisCounterChangedEvent;

	private DestructionManager()
	{
	}

	private void Awake()
	{
		DestroyedPrefabCounter = new List<float>();
		overlapColliders = new Collider[100];
		_detailMasks = Resources.LoadAll<Texture2D>("ProgressiveDamage").ToList();
		debrisLayer = LayerMask.NameToLayer("DestroyItDebris");
		_debrisPieces = new List<Debris>();
		_destroyedObjects = new List<Destructible>();
		_nextUpdate = Time.time + updateFrequency;
		if (defaultParticle == null)
		{
			defaultParticle = Resources.Load<ParticleSystem>("Default_Particles/DefaultLargeParticle");
		}
		Check.IsDefaultParticleAssigned();
		if (!Check.LayerExists("DestroyItDebris", logMessage: false))
		{
			Debug.LogWarning("DestroyItDebris layer not found. Add a layer named 'DestroyItDebris' to your project if you want debris to ignore other debris when using Cling Points.");
		}
	}

	private void Update()
	{
		if (Time.time < _nextUpdate)
		{
			return;
		}
		DestroyedPrefabCounter.Update(withinSeconds);
		if (_debrisPieces.Count > 0)
		{
			if (_debrisPieces.RemoveAll((Debris x) => x == null || !x.IsActive) > 0)
			{
				FireActiveDebrisCounterChangedEvent();
			}
			if (ActiveDebrisCount > maxPersistentDebris)
			{
				int num = ActiveDebrisCount - maxPersistentDebris;
				foreach (Debris debrisPiece in _debrisPieces)
				{
					if (num <= 0)
					{
						break;
					}
					if (debrisPiece.IsActive && (removeVisibleDebris || (!(debrisPiece.Rigidbody.GetComponent<Renderer>() == null) && !debrisPiece.Rigidbody.GetComponent<Renderer>().isVisible)))
					{
						debrisPiece.Disable();
						num--;
					}
				}
			}
		}
		if (_destroyedObjects.Count > 0)
		{
			int num2 = ((_destroyedObjects.Count > 5) ? 5 : _destroyedObjects.Count);
			for (int num3 = 0; num3 < num2; num3++)
			{
				if (_destroyedObjects[num3] != null && _destroyedObjects[num3].gameObject != null)
				{
					UnityEngine.Object.Destroy(_destroyedObjects[num3].gameObject);
				}
			}
			_destroyedObjects.RemoveRange(0, num2);
		}
		_nextUpdate = Time.time + updateFrequency;
	}

	public void ProcessDestruction<T>(Destructible oldObj, GameObject destroyedPrefab, T damageInfo, bool isObliterated)
	{
		if (oldObj == null || !oldObj.canBeDestroyed)
		{
			return;
		}
		oldObj.FireDestroyedEvent();
		oldObj.ReleaseClingingDebris();
		Joint[] componentsInChildren = oldObj.GetComponentsInChildren<Joint>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i]);
		}
		if (oldObj.damageEffects != null)
		{
			foreach (DamageEffect damageEffect in oldObj.damageEffects)
			{
				if (!damageEffect.UnparentOnDestroy || damageEffect.GameObject == null)
				{
					continue;
				}
				damageEffect.GameObject.transform.SetParent(null, worldPositionStays: true);
				if (damageEffect.StopEmittingOnDestroy && damageEffect.ParticleSystems != null && damageEffect.ParticleSystems.Length != 0)
				{
					ParticleSystem[] particleSystems = damageEffect.ParticleSystems;
					for (int i = 0; i < particleSystems.Length; i++)
					{
						ParticleSystem.EmissionModule emission = particleSystems[i].emission;
						emission.enabled = false;
					}
				}
			}
		}
		if (oldObj.gameObject.HasTagInParent(Tag.TerrainTree))
		{
			TreeManager.Instance.DestroyTreeAt(oldObj.transform.position);
		}
		if (oldObj.sinkWhenDestroyed)
		{
			DestructibleHelper.SinkAndDestroy(oldObj);
			return;
		}
		if ((destroyedPrefab == null || (IsDestroyedPrefabLimitReached && oldObj.canBeObliterated)) | isObliterated)
		{
			DestroyWithParticleEffect(oldObj, oldObj.fallbackParticle, damageInfo);
			return;
		}
		if (useCameraDistanceLimit && oldObj.canBeObliterated && Vector3.Distance(oldObj.transform.position, Camera.main.transform.position) > (float)cameraDistanceLimit)
		{
			DestroyWithParticleEffect(oldObj, oldObj.fallbackParticle, damageInfo);
			return;
		}
		DestroyedPrefabCounter.Add(Time.time);
		FireDestroyedPrefabCounterChangedEvent();
		UnparentSpecifiedChildren(oldObj);
		if (debrisLayer != -1)
		{
			oldObj.gameObject.layer = debrisLayer;
		}
		GameObject newObj = ObjectPool.Instance.Spawn(destroyedPrefab, oldObj.PositionFixedUpdate, oldObj.RotationFixedUpdate, oldObj.GetInstanceID());
		InstantiateDebris(newObj, oldObj, damageInfo);
		oldObj.gameObject.SetActive(value: false);
		_destroyedObjects.Add(oldObj);
	}

	private void DestroyWithParticleEffect<T>(Destructible oldObj, ParticleSystem customParticle, T damageInfo)
	{
		if (oldObj.useFallbackParticle)
		{
			int instanceID = (oldObj.gameObject.GetHighestParentWithTag(Tag.DestructibleGroup) ?? oldObj.gameObject).GetInstanceID();
			Vector3 position = oldObj.MeshCenterPoint;
			if (oldObj.centerPointOverride != Vector3.zero)
			{
				position = oldObj.centerPointOverride;
			}
			position = oldObj.transform.TransformPoint(position);
			ParticleManager.Instance.PlayEffect(customParticle ?? defaultParticle, oldObj, position, oldObj.transform.rotation, instanceID);
			customParticle.Play();
		}
		UnparentSpecifiedChildren(oldObj);
		oldObj.gameObject.SetActive(value: false);
		_destroyedObjects.Add(oldObj);
		if (damageInfo.GetType() == typeof(ImpactDamage))
		{
			DestructibleHelper.ReapplyImpactForce(damageInfo as ImpactDamage, oldObj.VelocityReduction);
		}
	}

	private static void UnparentSpecifiedChildren(Destructible obj)
	{
		if (obj.unparentOnDestroy == null)
		{
			return;
		}
		foreach (GameObject item in obj.unparentOnDestroy)
		{
			if (item == null)
			{
				continue;
			}
			item.transform.parent = null;
			DelayedRigidbody[] componentsInChildren = item.GetComponentsInChildren<DelayedRigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Initialize();
			}
			if (obj.disableKinematicOnUparentedChildren)
			{
				Rigidbody[] componentsInChildren2 = item.GetComponentsInChildren<Rigidbody>();
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].isKinematic = false;
				}
			}
			Animation[] componentsInChildren3 = item.GetComponentsInChildren<Animation>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].enabled = false;
			}
		}
	}

	private void InstantiateDebris<T>(GameObject newObj, Destructible oldObj, T damageInfo)
	{
		if (!oldObj.autoPoolDestroyedPrefab)
		{
			DestructibleHelper.TransferMaterials(oldObj, newObj);
		}
		if (oldObj.isTerrainTree)
		{
			newObj.gameObject.LockHueVariation();
		}
		if (oldObj.transform.lossyScale != new Vector3(1f, 1f, 1f))
		{
			newObj.transform.localScale = oldObj.transform.lossyScale;
		}
		if (oldObj.destroyedPrefabParent != null)
		{
			newObj.transform.parent = oldObj.destroyedPrefabParent.transform;
		}
		if (oldObj.isDebrisChipAway)
		{
			Collider[] componentsInChildren = newObj.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				if (!(collider.gameObject.GetComponent<ChipAwayDebris>() != null))
				{
					if (collider.attachedRigidbody != null && !collider.attachedRigidbody.isKinematic)
					{
						collider.attachedRigidbody.gameObject.RemoveComponent<Rigidbody>();
					}
					ChipAwayDebris chipAwayDebris = collider.gameObject.AddComponent<ChipAwayDebris>();
					chipAwayDebris.debrisMass = oldObj.chipAwayDebrisMass;
					chipAwayDebris.debrisDrag = oldObj.chipAwayDebrisDrag;
					chipAwayDebris.debrisAngularDrag = oldObj.chipAwayDebrisAngularDrag;
				}
			}
			return;
		}
		oldObj.ReparentChildren(newObj);
		Rigidbody[] array = oldObj.PooledRigidbodies;
		GameObject[] array2 = oldObj.PooledRigidbodyGos;
		if (array == null || array.Length == 0)
		{
			array = newObj.GetComponentsInChildren<Rigidbody>();
			array2 = new GameObject[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = array[j].gameObject;
			}
		}
		if (array.Length != 0)
		{
			for (int k = 0; k < array.Length; k++)
			{
				if (debrisLayer != -1)
				{
					array[k].gameObject.layer = debrisLayer;
				}
				if (oldObj.debrisToReParentByName != null && oldObj.debrisToReParentByName.Count > 0 && oldObj.transform.parent != null && (oldObj.debrisToReParentByName.Contains("ALL DEBRIS") || oldObj.debrisToReParentByName.Contains(array[k].name)))
				{
					array[k].gameObject.transform.parent = oldObj.transform.parent;
					array[k].isKinematic = oldObj.debrisToReParentIsKinematic;
				}
				if (!array[k].isKinematic)
				{
					array[k].velocity = oldObj.VelocityFixedUpdate;
					array[k].angularVelocity = oldObj.AngularVelocityFixedUpdate;
				}
				Debris item = new Debris
				{
					Rigidbody = array[k],
					GameObject = array2[k]
				};
				_debrisPieces.Add(item);
				FireActiveDebrisCounterChangedEvent();
			}
		}
		if (oldObj.CheckForClingingDebris)
		{
			newObj.MakeDebrisCling();
		}
		if (damageInfo.GetType() == typeof(ImpactDamage))
		{
			DestructibleHelper.ReapplyImpactForce(damageInfo as ImpactDamage, oldObj.VelocityReduction);
		}
		if (damageInfo.GetType() == typeof(ExplosiveDamage) || damageInfo.GetType() == typeof(ImpactDamage))
		{
			ExplosionHelper.ApplyForcesToDebris(newObj, 1f, damageInfo);
		}
	}

	public void SetProgressiveDamageTexture(Renderer rend, Material sourceMat, DamageLevel damageLevel)
	{
		if (sourceMat == null || !sourceMat.HasProperty("_DetailMask"))
		{
			return;
		}
		Texture texture = sourceMat.GetTexture("_DetailMask");
		if (texture == null || _detailMasks == null || _detailMasks.Count == 0)
		{
			return;
		}
		string arg = Regex.Replace(texture.name, "_D[0-9]*$", "");
		Texture texture2 = null;
		foreach (Texture2D detailMask in _detailMasks)
		{
			if (detailMask.name == $"{arg}_D{damageLevel.visibleDamageLevel}")
			{
				texture2 = detailMask;
				break;
			}
		}
		if (!(texture2 == null))
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			rend.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetTexture("_DetailMask", texture2);
			rend.SetPropertyBlock(materialPropertyBlock);
		}
	}

	public Texture2D GetDetailMask(Material sourceMat, DamageLevel damageLevel)
	{
		if (sourceMat == null)
		{
			return null;
		}
		if (!sourceMat.HasProperty("_DetailMask"))
		{
			return null;
		}
		Texture texture = sourceMat.GetTexture("_DetailMask");
		if (texture == null)
		{
			return null;
		}
		if (_detailMasks == null || _detailMasks.Count == 0)
		{
			return null;
		}
		string arg = Regex.Replace(texture.name, "_D[0-9]*$", "");
		foreach (Texture2D detailMask in _detailMasks)
		{
			if (detailMask.name == $"{arg}_D{damageLevel.visibleDamageLevel - 1}")
			{
				return detailMask;
			}
		}
		return null;
	}

	public void FireDestroyedPrefabCounterChangedEvent()
	{
		if (DestroyedPrefabCounterChangedEvent != null)
		{
			DestroyedPrefabCounterChangedEvent();
		}
	}

	public void FireActiveDebrisCounterChangedEvent()
	{
		if (ActiveDebrisCounterChangedEvent != null)
		{
			ActiveDebrisCounterChangedEvent();
		}
	}
}
