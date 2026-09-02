using System;
using System.Collections.Generic;
using System.Linq;

namespace Doozy.Runtime.Reactor.ScriptableObjects.Internal;

[Serializable]
public class PresetCategory
{
	public string Category;

	public List<string> Names;

	public PresetCategory(string category)
	{
		Category = category;
	}

	public PresetCategory AddName(string value)
	{
		if (Names == null)
		{
			Names = new List<string>();
		}
		Names.Add(value);
		Names = Names.Distinct().ToList();
		Names.Sort();
		return this;
	}
}
