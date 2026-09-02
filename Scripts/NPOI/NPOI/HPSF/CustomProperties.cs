using System;
using System.Collections;
using System.Text;
using NPOI.Util;

namespace NPOI.HPSF;

public class CustomProperties : Hashtable
{
	private Hashtable dictionaryIDToName = new Hashtable();

	private Hashtable dictionaryNameToID = new Hashtable();

	private bool isPure = true;

	public object this[string name]
	{
		get
		{
			object obj = dictionaryNameToID[name];
			if (obj == null)
			{
				IEnumerator enumerator = dictionaryNameToID.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string s = ((DictionaryEntry)enumerator.Current).Key as string;
					int num = Codepage;
					if (num < 0)
					{
						num = 1200;
					}
					byte[] bytes = Encoding.GetEncoding(num).GetBytes(s);
					byte[] bytes2 = Encoding.UTF8.GetBytes(name);
					if (Arrays.Equals(bytes, bytes2))
					{
						obj = ((DictionaryEntry)enumerator.Current).Value;
					}
				}
				if (obj == null)
				{
					return null;
				}
			}
			long num2 = (long)obj;
			return ((CustomProperty)base[num2])?.Value;
		}
	}

	public IDictionary Dictionary => dictionaryIDToName;

	public int Codepage
	{
		get
		{
			int num = -1;
			IEnumerator enumerator = Values.GetEnumerator();
			while (num == -1 && enumerator.MoveNext())
			{
				CustomProperty customProperty = (CustomProperty)enumerator.Current;
				if (customProperty.ID == 1)
				{
					num = (int)customProperty.Value;
				}
			}
			return num;
		}
		set
		{
			MutableProperty mutableProperty = new MutableProperty();
			mutableProperty.ID = 1L;
			mutableProperty.Type = 2L;
			mutableProperty.Value = value;
			Put(new CustomProperty(mutableProperty));
		}
	}

	public bool IsPure
	{
		get
		{
			return isPure;
		}
		set
		{
			isPure = value;
		}
	}

	public CustomProperty Put(string name, CustomProperty cp)
	{
		if (string.IsNullOrEmpty(name))
		{
			isPure = false;
			return null;
		}
		if (name == null)
		{
			throw new ArgumentException("The name of a custom property must be a String, but it is a " + name.GetType().Name);
		}
		if (!name.Equals(cp.Name))
		{
			throw new ArgumentException("Parameter \"name\" (" + name + ") and custom property's name (" + cp.Name + ") do not match.");
		}
		long iD = cp.ID;
		object obj = dictionaryNameToID[name];
		if (obj != null)
		{
			dictionaryIDToName.Remove(obj);
		}
		dictionaryNameToID[name] = iD;
		dictionaryIDToName[iD] = name;
		if (obj != null)
		{
			base.Remove(obj);
		}
		base[iD] = cp;
		return cp;
	}

	public ICollection KeySet()
	{
		return dictionaryNameToID.Keys;
	}

	public ICollection NameSet()
	{
		return dictionaryNameToID.Keys;
	}

	public ICollection IdSet()
	{
		return dictionaryNameToID.Keys;
	}

	private object Put(CustomProperty customProperty)
	{
		string name = customProperty.Name;
		object obj = dictionaryNameToID[name];
		if (obj != null)
		{
			customProperty.ID = (long)obj;
		}
		else
		{
			long num = 1L;
			IEnumerator enumerator = dictionaryIDToName.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				long num2 = (long)enumerator.Current;
				if (num2 > num)
				{
					num = num2;
				}
			}
			customProperty.ID = num + 1;
		}
		return Put(name, customProperty);
	}

	public object Remove(string name)
	{
		if (dictionaryNameToID[name] == null)
		{
			return null;
		}
		long num = (long)dictionaryNameToID[name];
		dictionaryIDToName.Remove(num);
		dictionaryNameToID.Remove(name);
		CustomProperty result = (CustomProperty)this[num];
		Remove(num);
		return result;
	}

	public object Put(string name, string value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 31L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public object Put(string name, long value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 20L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public object Put(string name, double value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 5L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public object Put(string name, int value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 3L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public object Put(string name, bool value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 11L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public object Put(string name, DateTime value)
	{
		CustomProperty customProperty = new CustomProperty(new MutableProperty
		{
			ID = -1L,
			Type = 64L,
			Value = value
		}, name);
		return Put(customProperty);
	}

	public override bool ContainsKey(object key)
	{
		if (key is long)
		{
			return base.ContainsKey((long)key);
		}
		if (key is string)
		{
			return base.ContainsKey((long)dictionaryNameToID[key]);
		}
		return false;
	}

	public override bool ContainsValue(object value)
	{
		if (value is CustomProperty)
		{
			return base.ContainsValue(value);
		}
		foreach (object value2 in base.Values)
		{
			if ((value2 as CustomProperty).Value == value)
			{
				return true;
			}
		}
		return false;
	}
}
