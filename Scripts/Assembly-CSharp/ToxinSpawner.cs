using Toked.Weapon.Throwable;
using UnityEngine;

public class ToxinSpawner : PoolerBase<ToxinImpactItem>
{
	[SerializeField]
	private ToxinImpactItem _object;

	public Transform parentSpawn;

	public static ToxinSpawner Instance { get; private set; }

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

	public ToxinImpactItem GetItem(PlayerController playerController, Vector3 pos, float toxinDuration = -1f, float toxinDps = -1f)
	{
		ToxinImpactItem toxinImpactItem = Get();
		toxinImpactItem.transform.position = new Vector3(pos.x, 0f, pos.z);
		toxinImpactItem.Init(playerController, toxinDuration, toxinDps);
		return toxinImpactItem;
	}

	protected override void GetSetup(ToxinImpactItem theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
