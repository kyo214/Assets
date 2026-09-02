using System;
using System.Collections;
using System.Collections.Generic;

namespace NPOI.Util.Collections;

public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly Dictionary<T, object> impl = new Dictionary<T, object>();

	public int Count => impl.Count;

	public bool IsReadOnly { get; set; }

	public void Add(T o)
	{
		if (IsReadOnly)
		{
			throw new InvalidOperationException("this hashset is readonly");
		}
		impl[o] = null;
	}

	public bool Contains(T o)
	{
		if (o == null)
		{
			return false;
		}
		return impl.ContainsKey(o);
	}

	public void CopyTo(T[] array, int index)
	{
		impl.Keys.CopyTo(array, index);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return impl.Keys.GetEnumerator();
	}

	public bool Remove(T o)
	{
		if (IsReadOnly)
		{
			throw new InvalidOperationException("this hashset is readonly");
		}
		impl.Remove(o);
		return true;
	}

	public void Clear()
	{
		impl.Clear();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return impl.GetEnumerator();
	}
}
