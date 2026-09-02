using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public abstract class MMObjectPooler : MonoBehaviour
{
	public static MMObjectPooler Instance;

	public bool MutualizeWaitingPools;

	public bool NestWaitingPool = true;

	[MMCondition("NestWaitingPool", true)]
	public bool NestUnderThis;

	protected GameObject _waitingPool;

	protected MMObjectPool _objectPool;

	protected const int _initialPoolsListCapacity = 5;

	protected bool _onSceneLoadedRegistered;

	public static List<MMObjectPool> _pools = new List<MMObjectPool>(5);

	public static void AddPool(MMObjectPool pool)
	{
		if (_pools == null)
		{
			_pools = new List<MMObjectPool>(5);
		}
		if (!_pools.Contains(pool))
		{
			_pools.Add(pool);
		}
	}

	public static void RemovePool(MMObjectPool pool)
	{
		_pools?.Remove(pool);
	}

	protected virtual void Awake()
	{
		Instance = this;
		FillObjectPool();
	}

	protected virtual bool CreateWaitingPool()
	{
		if (!MutualizeWaitingPools)
		{
			_waitingPool = new GameObject(DetermineObjectPoolName());
			SceneManager.MoveGameObjectToScene(_waitingPool, base.gameObject.scene);
			_objectPool = _waitingPool.AddComponent<MMObjectPool>();
			_objectPool.PooledGameObjects = new List<GameObject>();
			ApplyNesting();
			return true;
		}
		MMObjectPool mMObjectPool = ExistingPool(DetermineObjectPoolName());
		if (mMObjectPool != null)
		{
			_objectPool = mMObjectPool;
			_waitingPool = mMObjectPool.gameObject;
			return false;
		}
		_waitingPool = new GameObject(DetermineObjectPoolName());
		SceneManager.MoveGameObjectToScene(_waitingPool, base.gameObject.scene);
		_objectPool = _waitingPool.AddComponent<MMObjectPool>();
		_objectPool.PooledGameObjects = new List<GameObject>();
		ApplyNesting();
		AddPool(_objectPool);
		return true;
	}

	public virtual MMObjectPool ExistingPool(string poolName)
	{
		if (_pools == null)
		{
			_pools = new List<MMObjectPool>(5);
		}
		if (_pools.Count == 0)
		{
			MMObjectPool[] array = Object.FindObjectsOfType<MMObjectPool>();
			if (array.Length != 0)
			{
				_pools.AddRange(array);
			}
		}
		foreach (MMObjectPool pool in _pools)
		{
			if (pool != null && pool.name == poolName && pool.gameObject.scene == base.gameObject.scene)
			{
				return pool;
			}
		}
		return null;
	}

	protected virtual void ApplyNesting()
	{
		if (NestWaitingPool && NestUnderThis && _waitingPool != null)
		{
			_waitingPool.transform.SetParent(base.transform);
		}
	}

	protected virtual string DetermineObjectPoolName()
	{
		return "[ObjectPooler] " + base.name;
	}

	public virtual void FillObjectPool()
	{
	}

	public virtual GameObject GetPooledGameObject()
	{
		return null;
	}

	public virtual void DestroyObjectPool()
	{
		if (_waitingPool != null)
		{
			Object.Destroy(_waitingPool.gameObject);
		}
	}

	protected virtual void OnEnable()
	{
		if (!_onSceneLoadedRegistered)
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if ((_objectPool == null || _waitingPool == null) && this != null)
		{
			FillObjectPool();
		}
	}

	private void OnDestroy()
	{
		if (_objectPool != null && NestUnderThis)
		{
			RemovePool(_objectPool);
		}
		if (_onSceneLoadedRegistered)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			_onSceneLoadedRegistered = false;
		}
	}
}
