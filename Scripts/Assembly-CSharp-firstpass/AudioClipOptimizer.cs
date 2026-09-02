using System.Collections.Generic;
using UnityEngine;

public static class AudioClipOptimizer
{
	private static readonly Dictionary<int, string> AudioClipNameByInstanceId = new Dictionary<int, string>();

	public static string CachedName(this AudioClip clip)
	{
		int instanceID = clip.GetInstanceID();
		if (AudioClipNameByInstanceId.ContainsKey(instanceID))
		{
			return AudioClipNameByInstanceId[instanceID];
		}
		string name = clip.name;
		AudioClipNameByInstanceId.Add(instanceID, name);
		return name;
	}
}
