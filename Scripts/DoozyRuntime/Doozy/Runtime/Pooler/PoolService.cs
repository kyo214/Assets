using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Pooler;

public class PoolService<T> where T : IPoolable, new()
{
	public UnityAction OnPoolUpdate;

	public UnityAction<T> OnItemAddedToPool;

	public UnityAction<T> OnItemRetrievedFromPool;

	public bool CallbacksEnabled;

	public int maxPoolSize { get; private set; }

	private List<T> pool { get; set; } = new List<T>();

	private bool initialized { get; set; }

	private void Initialize()
	{
		if (!initialized)
		{
			if (pool == null)
			{
				List<T> list = (pool = new List<T>());
			}
			maxPoolSize = -1;
			initialized = true;
		}
	}

	public T Get()
	{
		Initialize();
		RemoveNulls();
		T val = default;
		bool flag = false;
		foreach (T item in pool)
		{
			if (item != null)
			{
				T val2 = item;
				pool.Remove(val2);
				val = val2;
				flag = true;
				break;
			}
		}
		val = (flag ? val : new T());
		val.inPool = false;
		val.Reset();
		if (!CallbacksEnabled)
		{
			return val;
		}
		OnPoolUpdate?.Invoke();
		OnItemRetrievedFromPool?.Invoke(val);
		return val;
	}

	public void AddToPool(T item)
	{
		Initialize();
		if (item == null)
		{
			return;
		}
		if (maxPoolSize > 0 && pool.Count > maxPoolSize)
		{
			item.Dispose();
			return;
		}
		item.Reset();
		if (!pool.Contains(item))
		{
			pool.Add(item);
		}
		item.inPool = true;
		if (CallbacksEnabled)
		{
			OnPoolUpdate?.Invoke();
			OnItemAddedToPool?.Invoke(item);
		}
	}

	public void PreloadPool(int numberOfItems)
	{
		Initialize();
		numberOfItems = Mathf.Max(0, numberOfItems);
		for (int i = 0; i < numberOfItems; i++)
		{
			T item = new T
			{
				inPool = false
			};
			AddToPool(item);
		}
		if (CallbacksEnabled)
		{
			OnPoolUpdate?.Invoke();
		}
	}

	public void TrimPool(int targetPoolSize)
	{
		Initialize();
		targetPoolSize = Mathf.Max(0, targetPoolSize);
		if (pool.Count > targetPoolSize)
		{
			while (pool.Count > targetPoolSize)
			{
				int index = pool.Count - 1;
				pool[index].Dispose();
				pool.RemoveAt(index);
			}
			if (CallbacksEnabled)
			{
				OnPoolUpdate?.Invoke();
			}
		}
	}

	public void SetMaximumPoolSize(int size)
	{
		Initialize();
		maxPoolSize = size;
		TrimPool(maxPoolSize);
		if (CallbacksEnabled)
		{
			OnPoolUpdate?.Invoke();
		}
	}

	public void ClearMaxPoolSize()
	{
		Initialize();
		maxPoolSize = -1;
	}

	public void ClearPool(bool clearMaxPoolSize = false)
	{
		Initialize();
		foreach (T item in pool)
		{
			item.Dispose();
		}
		pool.Clear();
		if (clearMaxPoolSize)
		{
			ClearMaxPoolSize();
		}
		if (CallbacksEnabled)
		{
			OnPoolUpdate?.Invoke();
		}
	}

	private void RemoveNulls()
	{
		Initialize();
		for (int num = pool.Count - 1; num >= 0; num--)
		{
			if (pool[num] == null)
			{
				pool.RemoveAt(num);
			}
		}
	}
}
