using Toked.Weapon.Throwable;
using UnityEngine;

public class LandmineSpawner : PoolerBase<LandmineItem>
{
	[SerializeField]
	private LandmineItem _object;

	public Transform parentSpawn;

	public static LandmineSpawner Instance { get; private set; }

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

	public LandmineItem GetItem(PlayerController playerController, Vector3 targetPosition)
	{
		LandmineItem landmineItem = Get();
		landmineItem.transform.position = new Vector3(targetPosition.x, 0f, targetPosition.z);
		landmineItem.Init(playerController);
		return landmineItem;
	}

	protected override void GetSetup(LandmineItem obj)
	{
		base.GetSetup(obj);
		obj.transform.parent = parentSpawn;
	}
}
