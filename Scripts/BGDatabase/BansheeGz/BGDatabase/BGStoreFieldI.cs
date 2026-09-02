using System;

namespace BansheeGz.BGDatabase;

public interface BGStoreFieldI<T>
{
	T this[int index] { get; set; }

	int Count { get; }

	int MinSize { set; }

	int MinCapacity { set; }

	void ForEachKey(Action<int> action);

	void ForEachKeyValue(Action<int, T> action);

	T[] CopyRawValues();

	T Get(int index);

	void DeleteAt(int index);

	void Clear();

	void Add(T item);

	void Swap(int index1, int index2);

	void MoveValues(int fromIndex, int toIndex, int numberOfElements);
}
