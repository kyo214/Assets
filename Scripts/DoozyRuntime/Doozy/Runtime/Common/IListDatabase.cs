using System.Collections.Generic;

namespace Doozy.Runtime.Common;

public interface IListDatabase<TKey, TValue>
{
	void Add(TKey key);

	void Add(TKey key, TValue value);

	void Clear();

	bool ContainsKey(TKey key);

	bool ContainsValue(TKey key, TValue value);

	bool ContainsValue(TValue value);

	int CountKeys();

	int CountValues(TKey key);

	List<TValue> GetValues(TKey key);

	List<TKey> GetKeys();

	void Remove(TKey key);

	void Remove(TKey key, TValue value, bool deleteEmptyKey = true);

	void Remove(TValue value, bool deleteEmptyKey = true);

	void Validate(bool deleteEmptyKeys = true);
}
