namespace System.Collections;

[Serializable]
public struct DictionaryEntry(object key, object value)
{
	private object _key = key;

	private object _value = value;

	public object Key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	public object Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public void Deconstruct(out object key, out object value)
	{
		key = Key;
		value = Value;
	}
}
