using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGPartitionSaveModel
{
	private readonly Dictionary<string, byte[]> path2content = new Dictionary<string, byte[]>();

	public void Add(string key, byte[] content)
	{
		path2content[key] = content;
	}

	public byte[] Get(string key)
	{
		return BGUtil.Get(path2content, key);
	}

	public void ForEach(Action<string, byte[]> action)
	{
		foreach (KeyValuePair<string, byte[]> item in path2content)
		{
			action(item.Key, item.Value);
		}
	}
}
