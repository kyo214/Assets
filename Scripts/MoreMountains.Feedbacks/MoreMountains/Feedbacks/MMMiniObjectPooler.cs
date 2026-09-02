using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

public class MMMiniObjectPooler : MonoBehaviour
{
	public GameObject GameObjectToPool;

	public int PoolSize = 20;

	public bool PoolCanExpand = true;

	public bool MutualizeWaitingPools;

	public bool NestWaitingPool = true;

	protected GameObject _waitingPool;

	protected MMMiniObjectPool _objectPool;

	protected List<GameObject> _pooledGameObjects;

	protected const int _initialPoolsListCapacity = 5;

	private static List<MMMiniObjectPool> _pools = new List<MMMiniObjectPool>(5);

	public static void AddPool(MMMiniObjectPool pool)
	{
		if (_pools == null)
		{
			_pools = new List<MMMiniObjectPool>(5);
		}
		if (!_pools.Contains(pool))
		{
			_pools.Add(pool);
		}
	}

	public static void RemovePool(MMMiniObjectPool pool)
	{
		_pools?.Remove(pool);
	}

	protected virtual void Awake()
	{
		FillObjectPool();
	}

	private void OnDestroy()
	{
		if (_objectPool != null)
		{
			RemovePool(_objectPool);
		}
	}

	public virtual MMMiniObjectPool ExistingPool(string poolName)
	{
		if (_pools == null)
		{
			_pools = new List<MMMiniObjectPool>(5);
		}
		if (_pools.Count == 0)
		{
			MMMiniObjectPool[] array = Object.FindObjectsOfType<MMMiniObjectPool>();
			if (array.Length != 0)
			{
				_pools.AddRange(array);
			}
		}
		foreach (MMMiniObjectPool pool in _pools)
		{
			if (pool != null && pool.name == poolName && pool.gameObject.scene == base.gameObject.scene)
			{
				return pool;
			}
		}
		return null;
	}

	protected virtual void CreateWaitingPool()
	{
		if (!MutualizeWaitingPools)
		{
			_objectPool = base.gameObject.AddComponent<MMMiniObjectPool>();
			_objectPool.PooledGameObjects = new List<GameObject>();
			return;
		}
		MMMiniObjectPool mMMiniObjectPool = ExistingPool(DetermineObjectPoolName(GameObjectToPool));
		if (mMMiniObjectPool != null)
		{
			_waitingPool = mMMiniObjectPool.gameObject;
			_objectPool = mMMiniObjectPool;
			return;
		}
		GameObject gameObject = new GameObject();
		gameObject.name = DetermineObjectPoolName(GameObjectToPool);
		_objectPool = gameObject.AddComponent<MMMiniObjectPool>();
		_objectPool.PooledGameObjects = new List<GameObject>();
		AddPool(_objectPool);
	}

	public static string DetermineObjectPoolName(GameObject gameObjectToPool)
	{
		return gameObjectToPool.name + "_pool";
	}

	public virtual void FillObjectPool()
	{
		if (!(GameObjectToPool == null))
		{
			CreateWaitingPool();
			_pooledGameObjects = new List<GameObject>();
			int num = PoolSize;
			if (_objectPool != null)
			{
				num -= _objectPool.PooledGameObjects.Count;
				_pooledGameObjects = new List<GameObject>(_objectPool.PooledGameObjects);
			}
			for (int i = 0; i < num; i++)
			{
				AddOneObjectToThePool();
			}
		}
	}

	public virtual GameObject GetPooledGameObject()
	{
		for (int i = 0; i < _pooledGameObjects.Count; i++)
		{
			if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
			{
				return _pooledGameObjects[i];
			}
		}
		if (PoolCanExpand)
		{
			return AddOneObjectToThePool();
		}
		return null;
	}

	protected virtual GameObject AddOneObjectToThePool()
	{
		if (GameObjectToPool == null)
		{
			Debug.LogWarning("The " + base.gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", base.gameObject);
			return null;
		}
		GameObjectToPool.gameObject.SetActive(value: false);
		GameObject gameObject = Object.Instantiate(GameObjectToPool);
		SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
		if (NestWaitingPool)
		{
			gameObject.transform.SetParent(_objectPool.transform);
		}
		gameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
		_pooledGameObjects.Add(gameObject);
		_objectPool.PooledGameObjects.Add(gameObject);
		return gameObject;
	}

	public virtual void DestroyObjectPool()
	{
		if (_waitingPool != null)
		{
			Object.Destroy(_waitingPool.gameObject);
		}
	}
}
