using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class Rocket : MonoBehaviour
{
	[Tooltip("The amount of constant force applied to the missile. This directly affects the missile's overall speed.")]
	[Range(1f, 100f)]
	public int speed = 30;

	[Tooltip("The maximum amount of damage the blast can do. This is separate from, and does not affect, the force of the blast on rigidbodies.")]
	public float blastDamage = 200f;

	[Tooltip("The strength (or force) of the blast. Higher numbers push rigidbodies around more.")]
	public float blastForce = 250f;

	[Tooltip("The distance from point of impact where objects are considered to be hit at point blank range. Point Blank radius is checked first, then Near, then Far.")]
	public float pointBlankBlastRadius = 2f;

	[Tooltip("The percentage of blast damage applied to objects hit at point blank distance from the rocket's impact point.")]
	[Range(0f, 1f)]
	public float pointBlankDamagePercent = 1f;

	[Tooltip("The distance from the point of impact where objects are nearby, but not considered point blank. Point Blank radius is checked first, then Near, then Far.")]
	public float nearBlastRadius = 4f;

	[Tooltip("The percentage of blast damage applied to objects hit at a distance near to the rocket's impact point.")]
	[Range(0f, 1f)]
	public float nearDamagePercent = 0.5f;

	[Tooltip("The distance from the point of impact where objects are far away, but still considered to be in the blast zone. Point Blank radius is checked first, then Near, then Far.")]
	public float farBlastRadius = 8f;

	[Tooltip("The percentage of blast damage applied to objects hit within maximum effective distance from the rocket's impact point.")]
	[Range(0f, 1f)]
	public float farDamagePercent = 0.2f;

	[Tooltip("The amount of upward \"push\" explosions have. Higher numbers make debris fly up in the air, but can get unrealistic.")]
	[Range(0f, 3f)]
	public float explosionUpwardPush = 1f;

	[Tooltip("The particle effect to play when this object collides with something.")]
	public GameObject explosionPrefab;

	public ParticleSystem smokeTrailPrefab;

	[Tooltip("How long the rocket will fly (in seconds) before running out of fuel.")]
	[Range(0f, 6f)]
	public float flightTime = 2f;

	[Tooltip("Remove the rocket from the scene after this many seconds, regardless if it's out of fuel or not.")]
	[Range(0f, 10f)]
	public float maxLifetime = 5f;

	private float checkFrequency = 0.1f;

	private float nextUpdateCheck;

	private bool outOfFuel;

	private float flightTimer;

	private GameObject smokeTrailObj;

	private bool isExploding;

	private bool isInitialized;

	private bool isStarted;

	private float smokeTrailDistance = 0.27f;

	private List<Rigidbody> affectedRigidbodies;

	private Dictionary<ChipAwayDebris, float> affectedChipAwayDebris;

	private Dictionary<Destructible, ExplosiveDamage> affectedDestructibles;

	private void Start()
	{
		isInitialized = true;
	}

	private void OnEnable()
	{
		isStarted = false;
		affectedRigidbodies = new List<Rigidbody>();
		affectedChipAwayDebris = new Dictionary<ChipAwayDebris, float>();
		affectedDestructibles = new Dictionary<Destructible, ExplosiveDamage>();
		nextUpdateCheck = Time.time + checkFrequency;
	}

	private void Update()
	{
		if (!isInitialized)
		{
			return;
		}
		if (!isStarted)
		{
			EngineStartUp();
			isStarted = true;
		}
		if (Time.time > nextUpdateCheck)
		{
			float num = Time.time - flightTimer;
			if (num > maxLifetime)
			{
				StartCoroutine(Recover());
			}
			if (!outOfFuel && num > flightTime)
			{
				EngineShutDown();
			}
			nextUpdateCheck = Time.time + checkFrequency;
		}
	}

	private void EngineStartUp()
	{
		flightTimer = Time.time;
		isExploding = false;
		outOfFuel = false;
		GetComponent<ConstantForce>().relativeForce = new Vector3(0f, 0f, speed);
		smokeTrailObj = ObjectPool.Instance.Spawn(smokeTrailPrefab.gameObject, new Vector3(0f, 0f, smokeTrailDistance * -1f), Quaternion.identity, base.transform);
	}

	private void EngineShutDown()
	{
		if (GetComponent<ConstantForce>() != null)
		{
			GetComponent<ConstantForce>().relativeForce = Vector3.zero;
		}
		GetComponent<Rigidbody>().useGravity = true;
		Transform transform = base.transform.Find("exhaust");
		if (transform != null)
		{
			transform.gameObject.SetActive(value: false);
		}
		outOfFuel = true;
		Transform transform2 = base.transform.Find("point light");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(value: false);
		}
	}

	private void TurnOffSmokeTrail()
	{
		if (!(smokeTrailObj == null))
		{
			smokeTrailObj.transform.parent = null;
			ParticleSystem.EmissionModule emission = smokeTrailObj.GetComponent<ParticleSystem>().emission;
			emission.enabled = false;
			PoolAfter poolAfter = smokeTrailObj.AddComponent<PoolAfter>();
			poolAfter.seconds = 7f;
			poolAfter.removeWhenPooled = true;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!isExploding)
		{
			Explode();
		}
	}

	public void Explode()
	{
		Vector3 position = base.transform.position;
		isExploding = true;
		TurnOffSmokeTrail();
		ObjectPool.Instance.Spawn(explosionPrefab, position, GetComponent<Rigidbody>().rotation);
		int colliderCount = Physics.OverlapSphereNonAlloc(position, pointBlankBlastRadius, DestructionManager.Instance.overlapColliders);
		ExplosiveDamage explosiveDamage = new ExplosiveDamage
		{
			Position = position,
			DamageAmount = blastDamage * pointBlankDamagePercent,
			BlastForce = blastForce,
			Radius = farBlastRadius,
			UpwardModifier = explosionUpwardPush
		};
		AddAffectedObjects(colliderCount, explosiveDamage, 0.75f);
		int colliderCount2 = Physics.OverlapSphereNonAlloc(position, nearBlastRadius, DestructionManager.Instance.overlapColliders);
		ExplosiveDamage explosiveDamage2 = new ExplosiveDamage
		{
			Position = position,
			DamageAmount = blastDamage * nearDamagePercent,
			BlastForce = blastForce,
			Radius = farBlastRadius,
			UpwardModifier = explosionUpwardPush
		};
		AddAffectedObjects(colliderCount2, explosiveDamage2, 0.5f);
		int colliderCount3 = Physics.OverlapSphereNonAlloc(position, farBlastRadius, DestructionManager.Instance.overlapColliders);
		ExplosiveDamage explosiveDamage3 = new ExplosiveDamage
		{
			Position = position,
			DamageAmount = blastDamage * farDamagePercent,
			BlastForce = blastForce,
			Radius = farBlastRadius,
			UpwardModifier = explosionUpwardPush
		};
		AddAffectedObjects(colliderCount3, explosiveDamage3, 0.25f);
		foreach (Rigidbody affectedRigidbody in affectedRigidbodies)
		{
			affectedRigidbody.AddExplosionForce(blastForce, base.transform.position, farBlastRadius, explosionUpwardPush);
		}
		foreach (KeyValuePair<ChipAwayDebris, float> affectedChipAwayDebri in affectedChipAwayDebris)
		{
			if ((float)Random.Range(1, 100) <= 100f * affectedChipAwayDebri.Value)
			{
				affectedChipAwayDebri.Key.BreakOff(blastForce, farBlastRadius, explosionUpwardPush);
			}
		}
		foreach (KeyValuePair<Destructible, ExplosiveDamage> affectedDestructible in affectedDestructibles)
		{
			if (affectedDestructible.Value.DamageAmount > 0f)
			{
				affectedDestructible.Key.ApplyDamage(affectedDestructible.Value);
			}
		}
		StartCoroutine(Recover());
	}

	private void AddAffectedObjects(int colliderCount, ExplosiveDamage explosiveDamage, float chipAwayPercentage)
	{
		for (int i = 0; i < colliderCount; i++)
		{
			Collider collider = DestructionManager.Instance.overlapColliders[i];
			if (collider is TerrainCollider || collider == GetComponent<Collider>())
			{
				continue;
			}
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null && !attachedRigidbody.isKinematic && !affectedRigidbodies.Contains(attachedRigidbody))
			{
				affectedRigidbodies.Add(attachedRigidbody);
			}
			ChipAwayDebris component = collider.gameObject.GetComponent<ChipAwayDebris>();
			if (component != null && !affectedChipAwayDebris.ContainsKey(component))
			{
				affectedChipAwayDebris.Add(component, chipAwayPercentage);
			}
			if (component != null)
			{
				continue;
			}
			Destructible[] componentsInParent = collider.gameObject.GetComponentsInParent<Destructible>(includeInactive: false);
			foreach (Destructible destructible in componentsInParent)
			{
				if (!affectedDestructibles.ContainsKey(destructible) && (destructible.isActiveAndEnabled || destructible.isTerrainTree))
				{
					affectedDestructibles.Add(destructible, explosiveDamage);
				}
			}
		}
	}

	private IEnumerator Recover()
	{
		yield return new WaitForFixedUpdate();
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().Sleep();
		GetComponent<Rigidbody>().useGravity = false;
		ObjectPool.Instance.PoolObject(base.gameObject, reenableChildren: true);
		StopAllCoroutines();
	}
}
