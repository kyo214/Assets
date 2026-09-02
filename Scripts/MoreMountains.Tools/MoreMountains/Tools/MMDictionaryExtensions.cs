using System.Collections.Generic;

namespace MoreMountains.Tools;

public static class MMDictionaryExtensions
{
	public static T KeyByValue<T, W>(this Dictionary<T, W> dictionary, T value)
	{
		T result = default;
		foreach (KeyValuePair<T, W> item in dictionary)
		{
			if (item.Value.Equals(value))
			{
				result = item.Key;
				return result;
			}
		}
		return result;
	}
}
