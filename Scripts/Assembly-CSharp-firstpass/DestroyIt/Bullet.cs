using UnityEngine;

namespace DestroyIt;

public class Bullet : MonoBehaviour
{
	[Tooltip("The bullet's speed in game units per second.")]
	public float speed = 400f;

	[Tooltip("How many seconds the bullet will live, regardless of distance traveled.")]
	public float timeToLive = 0.5f;

	public Renderer streak;

	[Range(1f, 10f)]
	[Tooltip("How often the bullet streak is visibile. 1 = 10% of the time. 10 = 100% of the time.")]
	public int streakVisibleFreq = 6;

	[Range(1f, 50f)]
	[Tooltip("Once turned on or off, the bullet streak will remain stable (unchanged) for this many frames.")]
	public int streakMinFramesStable = 3;

	private float spawnTime;

	private bool hitSomething;

	private bool isInitialized;

	private int streakFramesStable;

	public Vector3 StartingPosition { get; set; }

	public float DistanceTraveled => Vector3.Distance(StartingPosition, base.transform.position);

	public void OnEnable()
	{
		spawnTime = Time.time;
		hitSomething = false;
		StartingPosition = base.transform.position;
		if (streak != null)
		{
			streak.gameObject.SetActive(Random.Range(1, 11) <= streakVisibleFreq);
		}
		isInitialized = true;
	}

	public void Update()
	{
		if (!isInitialized)
		{
			return;
		}
		if (Time.time > spawnTime + timeToLive || hitSomething)
		{
			ObjectPool.Instance.PoolObject(base.gameObject);
			return;
		}
		if (streak != null)
		{
			if (streakFramesStable > streakMinFramesStable)
			{
				streak.gameObject.SetActive(Random.Range(1, 11) <= streakVisibleFreq);
				streakFramesStable = 0;
			}
			else
			{
				streakFramesStable++;
			}
		}
		Vector3 end = base.transform.position + base.transform.forward * speed * Time.deltaTime;
		Debug.DrawLine(base.transform.position, end, Color.red, 5f);
		RaycastHit[] array = Physics.RaycastAll(base.transform.position, base.transform.forward, speed * Time.deltaTime);
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].collider.isTrigger && array[i].distance < num2)
			{
				num = i;
				num2 = array[i].distance;
			}
		}
		if (num > -1)
		{
			ProcessBulletHit(array[num], base.transform.forward);
			hitSomething = true;
		}
		else
		{
			base.transform.position += base.transform.forward * speed * Time.deltaTime;
		}
	}

	private static void ProcessBulletHit(RaycastHit hitInfo, Vector3 bulletDirection)
	{
		HitEffects componentInParent = hitInfo.collider.gameObject.GetComponentInParent<HitEffects>();
		if (componentInParent != null && componentInParent.effects.Count > 0)
		{
			componentInParent.PlayEffect(HitBy.Bullet, hitInfo.point, hitInfo.normal);
		}
		Destructible[] componentsInParent = hitInfo.collider.gameObject.GetComponentsInParent<Destructible>(includeInactive: false);
		foreach (Destructible destructible in componentsInParent)
		{
			if (destructible.isActiveAndEnabled || destructible.isTerrainTree)
			{
				ImpactDamage damage = new ImpactDamage
				{
					DamageAmount = InputManager.Instance.bulletDamage,
					AdditionalForce = InputManager.Instance.bulletForcePerSecond,
					AdditionalForcePosition = hitInfo.point,
					AdditionalForceRadius = 0.5f
				};
				destructible.ApplyDamage(damage);
				break;
			}
		}
		Vector3 vector = bulletDirection * (InputManager.Instance.bulletForcePerSecond / InputManager.Instance.bulletForceFrequency);
		Rigidbody attachedRigidbody = hitInfo.collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			attachedRigidbody.AddForceAtPosition(vector, hitInfo.point, ForceMode.Impulse);
		}
		ChipAwayDebris component = hitInfo.collider.gameObject.GetComponent<ChipAwayDebris>();
		if (component != null)
		{
			component.BreakOff(-1.5f * vector, hitInfo.point);
		}
	}
}
