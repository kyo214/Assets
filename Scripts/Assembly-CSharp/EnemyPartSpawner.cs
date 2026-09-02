using UnityEngine;

public class EnemyPartSpawner : PoolerBase<EnemyPartPool>
{
	[SerializeField]
	private EnemyPartPool _object;

	public Transform parentSpawn;

	public static EnemyPartSpawner Instance { get; private set; }

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

	protected override void GetSetup(EnemyPartPool theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
