using System;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;

namespace Unity.Services.Analytics;

public abstract class Event
{
	private protected readonly Dictionary<string, string> m_Strings;

	private protected readonly Dictionary<string, long> m_Integers;

	private protected readonly Dictionary<string, bool> m_Booleans;

	private protected readonly Dictionary<string, double> m_Floats;

	internal readonly string Name;

	internal readonly bool StandardEvent;

	internal readonly int EventVersion;

	protected Event(string name)
	{
		Name = name;
		m_Strings = new Dictionary<string, string>(StringComparer.Ordinal);
		m_Integers = new Dictionary<string, long>(StringComparer.Ordinal);
		m_Booleans = new Dictionary<string, bool>(StringComparer.Ordinal);
		m_Floats = new Dictionary<string, double>(StringComparer.Ordinal);
	}

	internal Event(string name, bool standardEvent, int eventVersion)
		: this(name)
	{
		StandardEvent = standardEvent;
		EventVersion = eventVersion;
	}

	protected void SetParameter(string name, string value)
	{
		m_Strings[name] = value;
	}

	protected void SetParameter(string name, bool value)
	{
		m_Booleans[name] = value;
	}

	protected void SetParameter(string name, int value)
	{
		SetParameter(name, (long)value);
	}

	protected void SetParameter(string name, long value)
	{
		m_Integers[name] = value;
	}

	protected void SetParameter(string name, float value)
	{
		SetParameter(name, (double)value);
	}

	protected void SetParameter(string name, double value)
	{
		m_Floats[name] = value;
	}

	internal virtual void Serialize(IBuffer buffer)
	{
		foreach (KeyValuePair<string, string> @string in m_Strings)
		{
			buffer.PushString(@string.Key, @string.Value);
		}
		foreach (KeyValuePair<string, long> integer in m_Integers)
		{
			buffer.PushInt64(integer.Key, integer.Value);
		}
		foreach (KeyValuePair<string, double> @float in m_Floats)
		{
			buffer.PushDouble(@float.Key, @float.Value);
		}
		foreach (KeyValuePair<string, bool> boolean in m_Booleans)
		{
			buffer.PushBool(boolean.Key, boolean.Value);
		}
	}

	public virtual void Validate()
	{
	}

	protected bool ParameterHasBeenSet(string name)
	{
		if (!m_Strings.ContainsKey(name) && !m_Integers.ContainsKey(name) && !m_Floats.ContainsKey(name))
		{
			return m_Booleans.ContainsKey(name);
		}
		return true;
	}

	public virtual void Reset()
	{
		m_Strings.Clear();
		m_Integers.Clear();
		m_Booleans.Clear();
		m_Floats.Clear();
	}

	internal static string[] BakeEnum2String<T>(bool toUpper = false) where T : Enum
	{
		Array values = Enum.GetValues(typeof(T));
		string[] array = new string[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			if (toUpper)
			{
				array[i] = values.GetValue(i).ToString().ToUpperInvariant();
			}
			else
			{
				array[i] = values.GetValue(i).ToString();
			}
		}
		return array;
	}
}
