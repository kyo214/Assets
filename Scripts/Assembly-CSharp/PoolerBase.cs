using System;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolerBase<T> : MonoBehaviour where T : MonoBehaviour
{
	private T _prefab;

	private bool _initialized;

	private ObjectPool<T> _pool;

	private ObjectPool<T> Pool
	{
		get
		{
			if (_pool == null)
			{
				throw new InvalidOperationException("You need to call InitPool before using it.");
			}
			return _pool;
		}
		set
		{
			_pool = value;
		}
	}

	protected void InitPool(T prefab, int initial = 10, int max = 999, bool collectionChecks = false)
	{
		_prefab = prefab;
		Pool = new ObjectPool<T>(CreateSetup, GetSetup, ReleaseSetup, DestroySetup, collectionChecks, initial, max);
		_initialized = true;
	}

	protected virtual T CreateSetup()
	{
		return UnityEngine.Object.Instantiate(_prefab);
	}

	protected virtual void GetSetup(T obj)
	{
		obj.gameObject.SetActive(value: true);
	}

	protected virtual void ReleaseSetup(T obj)
	{
		obj.gameObject.SetActive(value: false);
	}

	protected virtual void DestroySetup(T obj)
	{
		UnityEngine.Object.Destroy(obj);
	}

	public T Get()
	{
		return Pool.Get();
	}

	public void Release(T obj)
	{
		Pool.Release(obj);
	}

	private void OnDestroy()
	{
		if (_initialized)
		{
			Pool.Dispose();
		}
	}
}
