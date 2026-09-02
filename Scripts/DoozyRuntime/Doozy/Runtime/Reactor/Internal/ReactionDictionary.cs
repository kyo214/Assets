using System.Collections.Generic;
using System.Linq;

namespace Doozy.Runtime.Reactor.Internal;

internal class ReactionDictionary<T>
{
	private Dictionary<T, HashSet<Reaction>> dictionary { get; set; }

	private bool initialized { get; set; }

	private void Initialize()
	{
		if (dictionary == null)
		{
			initialized = false;
		}
		if (!initialized)
		{
			dictionary = new Dictionary<T, HashSet<Reaction>>();
			initialized = true;
		}
	}

	internal void Validate()
	{
		Initialize();
		RemoveEmptyKeys();
	}

	internal List<Reaction> GetReactions(T targetObject)
	{
		Initialize();
		if (targetObject == null || !dictionary.ContainsKey(targetObject))
		{
			return new List<Reaction>();
		}
		return dictionary[targetObject].ToList();
	}

	internal void AddReaction(T key, Reaction value)
	{
		Initialize();
		if (key == null || value == null)
		{
			return;
		}
		if (dictionary.ContainsKey(key))
		{
			if (dictionary[key] == null)
			{
				dictionary[key] = new HashSet<Reaction> { value };
			}
			else
			{
				dictionary[key].Add(value);
			}
		}
		else
		{
			dictionary.Add(key, new HashSet<Reaction> { value });
		}
	}

	internal void RemoveReaction(T key, Reaction value)
	{
		Initialize();
		if (key != null && dictionary.ContainsKey(key))
		{
			HashSet<Reaction> hashSet = dictionary[key];
			hashSet.Remove(null);
			hashSet.Remove(value);
			if (hashSet.Count == 0)
			{
				dictionary.Remove(key);
			}
		}
	}

	internal void RemoveReaction(Reaction value)
	{
		Initialize();
		foreach (T key in dictionary.Keys)
		{
			dictionary[key].Remove(value);
		}
		RemoveEmptyKeys();
	}

	private void RemoveEmptyKeys()
	{
		foreach (T item in dictionary.Keys.Where((T key) => dictionary[key] == null || dictionary[key].Count == 0).ToList())
		{
			dictionary.Remove(item);
		}
	}
}
