using System.Collections.Generic;

namespace Fusion.Collections;

internal class SynchronizedQueue<T>
{
	private readonly Queue<T> _queue;

	public int Count
	{
		get
		{
			lock (_queue)
			{
				return _queue.Count;
			}
		}
	}

	public SynchronizedQueue()
	{
		_queue = new Queue<T>(1024);
	}

	public bool Pop(out T item)
	{
		lock (_queue)
		{
			if (_queue.Count > 0)
			{
				item = _queue.Dequeue();
				return true;
			}
		}
		item = default;
		return false;
	}

	public void Push(T item)
	{
		if (item == null)
		{
			return;
		}
		lock (_queue)
		{
			_queue.Enqueue(item);
		}
	}
}
