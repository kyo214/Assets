using System;
using System.Collections.Concurrent;

namespace BansheeGz.BGDatabase;

public abstract class BGObjectPool
{
	public abstract object GetObject();

	public abstract void Return(object obj);
}
public class BGObjectPool<T> : BGObjectPool
{
	private readonly ConcurrentBag<T> _objects;

	private readonly Func<T> _objectGenerator;

	private readonly Action<T> _dispose;

	public BGObjectPool(Func<T> objectGenerator, Action<T> dispose = null)
	{
		_objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");
		_objects = new ConcurrentBag<T>();
		_dispose = dispose;
	}

	public T Get()
	{
		if (!_objects.TryTake(out var result))
		{
			return _objectGenerator();
		}
		return result;
	}

	public void Return(T item)
	{
		_dispose?.Invoke(item);
		_objects.Add(item);
	}

	public override object GetObject()
	{
		return Get();
	}

	public override void Return(object obj)
	{
		Return((T)obj);
	}
}
