using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Services.Analytics;

public class CustomEvent : Event, IEnumerable
{
	public object this[string key]
	{
		set
		{
			Add(key, value);
		}
	}

	public CustomEvent(string name)
		: base(name)
	{
	}

	public void Add(string key, object value)
	{
		Type type = value.GetType();
		if (type == typeof(string))
		{
			SetParameter(key, (string)value);
			return;
		}
		if (type == typeof(int))
		{
			SetParameter(key, (int)value);
			return;
		}
		if (type == typeof(long))
		{
			SetParameter(key, (long)value);
			return;
		}
		if (type == typeof(float))
		{
			SetParameter(key, (float)value);
			return;
		}
		if (type == typeof(double))
		{
			SetParameter(key, (double)value);
			return;
		}
		if (type == typeof(bool))
		{
			SetParameter(key, (bool)value);
			return;
		}
		throw new ArgumentException($"Values of type {type} cannot be included as event parameters.");
	}

	public IEnumerator GetEnumerator()
	{
		foreach (KeyValuePair<string, string> @string in m_Strings)
		{
			yield return new KeyValuePair<string, object>(@string.Key, @string.Value);
		}
		foreach (KeyValuePair<string, long> integer in m_Integers)
		{
			yield return new KeyValuePair<string, object>(integer.Key, integer.Value);
		}
		foreach (KeyValuePair<string, double> @float in m_Floats)
		{
			yield return new KeyValuePair<string, object>(@float.Key, @float.Value);
		}
		foreach (KeyValuePair<string, bool> boolean in m_Booleans)
		{
			yield return new KeyValuePair<string, object>(boolean.Key, boolean.Value);
		}
	}
}
