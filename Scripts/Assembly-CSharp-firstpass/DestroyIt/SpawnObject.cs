using UnityEngine;

namespace DestroyIt;

public class SpawnObject : MonoBehaviour
{
	[Tooltip("The prefab of the object you want to spawn into the scene from the object pool.")]
	public GameObject prefab;

	private ObjectPool _objectPool;

	private void Start()
	{
		_objectPool = ObjectPool.Instance;
		if (_objectPool == null)
		{
			Debug.LogWarning("Object Pool was not found or could not be created. Removing script and exiting.");
			Object.Destroy(this);
		}
		else
		{
			_objectPool.Spawn(prefab, base.transform.localPosition, base.transform.localRotation, base.transform.parent);
			base.gameObject.SetActive(value: false);
		}
	}
}
