using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGHashtableIdKey<T> : BGHashtableForSerialization<BGId, T>
{
	public List<string> myKeys = new List<string>();

	protected override void FromKeys()
	{
		myKeys.Clear();
		foreach (BGId key in keys)
		{
			myKeys.Add(key.ToString());
		}
	}

	protected override void ToKeys()
	{
		foreach (string myKey in myKeys)
		{
			keys.Add(new BGId(myKey));
		}
		myKeys.Clear();
	}
}
