using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMShufflebag<T>
{
	protected List<T> _contents;

	protected T _currentItem;

	protected int _currentIndex = -1;

	public int Capacity => _contents.Capacity;

	public int Size => _contents.Count;

	public MMShufflebag(int initialCapacity)
	{
		_contents = new List<T>(initialCapacity);
	}

	public virtual void Add(T item, int quantity)
	{
		for (int i = 0; i < quantity; i++)
		{
			_contents.Add(item);
		}
		_currentIndex = Size - 1;
	}

	public T Pick()
	{
		if (_currentIndex < 1)
		{
			_currentIndex = Size - 1;
			_currentItem = _contents[0];
			return _currentItem;
		}
		int index = Random.Range(0, _currentIndex);
		_currentItem = _contents[index];
		_contents[index] = _contents[_currentIndex];
		_contents[_currentIndex] = _currentItem;
		_currentIndex--;
		return _currentItem;
	}
}
