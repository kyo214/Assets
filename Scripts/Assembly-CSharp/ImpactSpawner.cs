using UnityEngine;

public class ImpactSpawner : PoolerBase<ObjectImpactPool>
{
	[SerializeField]
	private ObjectImpactPool _object;

	public Transform parentSpawn;

	public static ImpactSpawner Instance { get; private set; }

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

	protected override void GetSetup(ObjectImpactPool theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
