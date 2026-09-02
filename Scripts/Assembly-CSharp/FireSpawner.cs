using Toked.Weapon.Throwable;
using UnityEngine;

public class FireSpawner : PoolerBase<AreaImpactItem>
{
	[SerializeField]
	private AreaImpactItem _object;

	public Transform parentSpawn;

	public static FireSpawner Instance { get; private set; }

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

	public AreaImpactItem GetItem(PlayerController playerController, Vector3 pos, float fireDuration = -1f, float fireDps = -1f)
	{
		AreaImpactItem areaImpactItem = Get();
		areaImpactItem.transform.position = new Vector3(pos.x, 0f, pos.z);
		areaImpactItem.Init(playerController, fireDuration, fireDps);
		return areaImpactItem;
	}

	protected override void GetSetup(AreaImpactItem theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
