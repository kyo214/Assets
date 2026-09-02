using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGIdDictionary<T> : Dictionary<BGId, T>
{
	public BGIdDictionary()
	{
	}

	public BGIdDictionary(int capacity)
		: base(capacity)
	{
	}

	public BGIdDictionary(BGIdDictionary<T> source)
		: base((IDictionary<BGId, T>)source)
	{
	}
}
