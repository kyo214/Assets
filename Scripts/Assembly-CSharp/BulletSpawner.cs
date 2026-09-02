using UnityEngine;

public class BulletSpawner : PoolerBase<BulletImpactPool>
{
	[SerializeField]
	private BulletImpactPool _object;

	public Transform parentSpawn;

	public static BulletSpawner Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		InitPool(_object);
	}

	protected override void GetSetup(BulletImpactPool theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
