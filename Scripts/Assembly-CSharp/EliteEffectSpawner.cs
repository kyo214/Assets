using UnityEngine;

public class EliteEffectSpawner : PoolerBase<EliteEffectPool>
{
	[SerializeField]
	private EliteEffectPool _object;

	public Transform parentSpawn;

	public static EliteEffectSpawner Instance { get; private set; }

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

	protected override void GetSetup(EliteEffectPool theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
