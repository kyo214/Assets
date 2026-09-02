using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGObjectPoolNTS<T> : BGObjectPool
{
	private readonly Queue<T> _objects;

	private readonly Func<T> _objectGenerator;

	private readonly Action<T> _dispose;

	public BGObjectPoolNTS(Func<T> objectGenerator, Action<T> dispose = null)
	{
		_objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");
		_objects = new Queue<T>();
		_dispose = dispose;
	}

	public T Get()
	{
		if (_objects.Count <= 0)
		{
			return _objectGenerator();
		}
		return _objects.Dequeue();
	}

	public void Return(T item)
	{
		_dispose?.Invoke(item);
		_objects.Enqueue(item);
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
