using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class ObjectPool : MonoBehaviour
{
	public List<PoolEntry> prefabsToPool;

	public bool suppressWarnings;

	private GameObject[][] Pool;

	private Dictionary<int, GameObject> autoPooledObjects;

	private GameObject container;

	private static ObjectPool _instance;

	private bool isInitialized;

	public static ObjectPool Instance
	{
		get
		{
			if (_instance == null)
			{
				CreateInstance();
			}
			if (!_instance.isInitialized)
			{
				_instance.Start();
			}
			return _instance;
		}
	}

	private ObjectPool()
	{
	}

	private static void CreateInstance()
	{
		ObjectPool[] array = Object.FindObjectsOfType<ObjectPool>();
		if (array.Length > 1)
		{
			Debug.LogError("Multiple ObjectPool scripts found in scene. There can be only one.");
		}
		if (array.Length == 0)
		{
			Debug.LogError("ObjectPool script not found in scene. This is required for DestroyIt to work properly.");
		}
		_instance = array[0];
	}

	private void Start()
	{
		if (isInitialized || prefabsToPool == null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("DestroyIt_ObjectPool");
		container = ((gameObject != null) ? gameObject : new GameObject("DestroyIt_ObjectPool"));
		autoPooledObjects = new Dictionary<int, GameObject>();
		Pool = new GameObject[prefabsToPool.Count][];
		for (int i = 0; i < prefabsToPool.Count; i++)
		{
			PoolEntry poolEntry = prefabsToPool[i];
			Pool[i] = new GameObject[poolEntry.Count];
			for (int j = 0; j < poolEntry.Count; j++)
			{
				if (!(poolEntry.Prefab == null))
				{
					GameObject gameObject2 = Object.Instantiate(poolEntry.Prefab);
					gameObject2.name = poolEntry.Prefab.name;
					PoolObject(gameObject2);
				}
			}
		}
		isInitialized = true;
		CreateInstance();
	}

	public void AddDestructibleObjectToPool(Destructible destObj)
	{
		if (destObj.destroyedPrefab != null && destObj.autoPoolDestroyedPrefab)
		{
			GameObject gameObject = Object.Instantiate(destObj.destroyedPrefab);
			Destructible[] componentsInChildren = gameObject.GetComponentsInChildren<Destructible>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				AddDestructibleObjectToPool(componentsInChildren[i]);
			}
			gameObject.transform.parent = container.transform;
			gameObject.name = destObj.destroyedPrefab.name;
			gameObject.AddTag(Tag.Pooled);
			DestructibleHelper.TransferMaterials(destObj, gameObject);
			if (gameObject.GetComponentsInChildren<ClingPoint>().Length == 0)
			{
				destObj.CheckForClingingDebris = false;
			}
			destObj.PooledRigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
			destObj.PooledRigidbodyGos = new GameObject[destObj.PooledRigidbodies.Length];
			for (int j = 0; j < destObj.PooledRigidbodies.Length; j++)
			{
				destObj.PooledRigidbodyGos[j] = destObj.PooledRigidbodies[j].gameObject;
			}
			gameObject.SetActive(value: false);
			autoPooledObjects.Add(destObj.GetInstanceID(), gameObject);
		}
	}

	public GameObject SpawnFromOriginal(string prefabName)
	{
		foreach (PoolEntry item in prefabsToPool)
		{
			if (item.Prefab != null && item.Prefab.name == prefabName)
			{
				GameObject obj = Object.Instantiate(item.Prefab);
				obj.name = prefabName;
				return obj;
			}
		}
		return null;
	}

	private static GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject gameObject = Object.Instantiate(prefab, position, rotation);
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.parent = parent;
		if (parent != null)
		{
			gameObject.transform.localPosition = position;
		}
		else
		{
			gameObject.transform.position = position;
		}
		return gameObject;
	}

	public GameObject Spawn(GameObject originalPrefab, Vector3 position, Quaternion rotation, Transform parent, int autoPoolID = 0)
	{
		if (autoPooledObjects != null && autoPoolID != 0 && autoPooledObjects.ContainsKey(autoPoolID))
		{
			GameObject gameObject = autoPooledObjects[autoPoolID];
			if (gameObject != null)
			{
				gameObject.transform.parent = parent;
				if (parent != null)
				{
					gameObject.transform.localPosition = position;
					gameObject.transform.localRotation = rotation;
				}
				else
				{
					gameObject.transform.position = position;
					gameObject.transform.rotation = rotation;
				}
				gameObject.SetActive(value: true);
				return gameObject;
			}
		}
		string text = originalPrefab.name;
		for (int i = 0; i < prefabsToPool.Count; i++)
		{
			GameObject prefab = prefabsToPool[i].Prefab;
			if (prefab == null || prefab.name != text)
			{
				continue;
			}
			if (Pool != null && Pool[i].Length != 0)
			{
				for (int j = 0; j < Pool[i].Length; j++)
				{
					if (Pool[i][j] != null)
					{
						GameObject gameObject2 = Pool[i][j];
						Pool[i][j] = null;
						gameObject2.transform.parent = parent;
						if (parent != null)
						{
							gameObject2.transform.localPosition = position;
						}
						else
						{
							gameObject2.transform.position = position;
						}
						gameObject2.transform.rotation = rotation;
						gameObject2.SetActive(value: true);
						return gameObject2;
					}
				}
			}
			if (Pool == null)
			{
				GameObject result = InstantiateObject(prefabsToPool[i].Prefab, position, rotation, parent);
				Debug.LogWarning("[" + text + " was instantiated instead of spawned from pool. Reason: Pool is null.");
				return result;
			}
			if (!prefabsToPool[i].OnlyPooled)
			{
				GameObject gameObject3 = InstantiateObject(prefabsToPool[i].Prefab, position, rotation, parent);
				gameObject3.name = prefabsToPool[i].Prefab.name;
				gameObject3.AddTag(Tag.Pooled);
				if (!suppressWarnings)
				{
					Debug.LogWarning("[" + text + " was instantiated instead of spawned from pool. Reason: No objects remaining in the pool (size: " + Pool[i].Length + "). Consider increasing the pool size.");
				}
				return gameObject3;
			}
			return null;
		}
		return InstantiateObject(originalPrefab, position, rotation, parent);
	}

	public GameObject Spawn(GameObject originalPrefab, Vector3 position, Quaternion rotation, int autoPoolID = 0)
	{
		return Spawn(originalPrefab, position, rotation, null, autoPoolID);
	}

	public void PoolObject(GameObject obj, bool reenableChildren = false)
	{
		for (int i = 0; i < prefabsToPool.Count; i++)
		{
			if (prefabsToPool[i].Prefab == null || prefabsToPool[i].Prefab.name != obj.name)
			{
				continue;
			}
			obj.transform.parent = container.transform;
			ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].Stop();
				componentsInChildren[j].Clear();
				ParticleSystem.EmissionModule emission = componentsInChildren[j].emission;
				emission.enabled = true;
			}
			if (reenableChildren)
			{
				Transform[] componentsInChildren2 = obj.GetComponentsInChildren<Transform>(includeInactive: true);
				for (int k = 0; k < componentsInChildren2.Length; k++)
				{
					componentsInChildren2[k].gameObject.SetActive(value: true);
				}
			}
			obj.AddTag(Tag.Pooled);
			obj.SetActive(value: false);
			for (int l = 0; l < Pool[i].Length; l++)
			{
				if (Pool[i][l] == null)
				{
					Pool[i][l] = obj;
					return;
				}
			}
			Object.Destroy(obj);
			if (!suppressWarnings)
			{
				Debug.LogWarning("[" + obj.name + "] was destroyed instead of pooled. Reason: The pool size for this prefab was too small (" + Pool[i].Length + "). Consider increasing the pool size.");
			}
			return;
		}
		Object.Destroy(obj);
		if (!suppressWarnings)
		{
			Debug.LogWarning("[" + obj.name + "] was destroyed instead of pooled. Reason: Prefab not found in pool.");
		}
	}
}
