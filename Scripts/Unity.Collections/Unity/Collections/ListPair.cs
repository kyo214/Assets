using System.Collections;

namespace Unity.Collections;

internal struct ListPair<Key, Value>(Key k, Value v) where Value : IList
{
	public Key key = k;

	public Value value = v;

	public override string ToString()
	{
		string text = $"{key} = [";
		for (int i = 0; i < value.Count; i++)
		{
			text += value[i];
			if (i < value.Count - 1)
			{
				text += ", ";
			}
		}
		return text + "]";
	}
}
