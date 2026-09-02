using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

public abstract class SingletonScriptable<InstanceType> : ScriptableObject where InstanceType : ScriptableObject
{
	protected static string AssetNameToLoad;

	protected static string ResourceNameToLoad;

	protected static List<string> FoldersToCreate = new List<string>();
}
