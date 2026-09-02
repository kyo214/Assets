namespace Unity.Collections;

internal struct Pair<Key, Value>(Key k, Value v)
{
	public Key key = k;

	public Value value = v;

	public override string ToString()
	{
		return $"{key} = {value}";
	}
}
