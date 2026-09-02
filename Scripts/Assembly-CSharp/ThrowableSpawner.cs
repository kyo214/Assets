using UnityEngine;

public class ThrowableSpawner : PoolerBase<ObjectThrowable>
{
	[SerializeField]
	private ObjectThrowable _object;

	public Transform parentSpawn;

	public static ThrowableSpawner Instance { get; private set; }

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

	public ObjectThrowable Get(ObjectThrowable.ThrowableType objectThrowableType)
	{
		ObjectThrowable objectThrowable = Get();
		objectThrowable.Init(objectThrowableType);
		return objectThrowable;
	}

	protected override void GetSetup(ObjectThrowable theObject)
	{
		base.GetSetup(theObject);
		theObject.transform.parent = parentSpawn;
	}
}
