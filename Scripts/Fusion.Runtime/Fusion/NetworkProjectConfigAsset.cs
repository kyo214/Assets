using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion;

[ScriptHelp]
public class NetworkProjectConfigAsset : AssetObject
{
	[SerializeField]
	public NetworkProjectConfig Config = new NetworkProjectConfig();

	[SerializeField]
	[HideInInspector]
	[Obsolete("Field moved to Config.AssembliesToWeave. Will be removed in 1.1.")]
	public string[] AssembliesToWeave = new string[2] { "Assembly-CSharp", "Assembly-CSharp-firstpass" };

	[SerializeField]
	[InlineHelp]
	public string PrefabAssetsContainerPath = string.Empty;

	[EditorDisabledGroup(true)]
	[SerializeField]
	[Header("Auto Generated")]
	[InlineHelp]
	[ResolveNetworkPrefabSourceUnity]
	public NetworkPrefabSourceUnityBase[] Prefabs = Array.Empty<NetworkPrefabSourceUnityBase>();

	[EditorDisabledGroup(false)]
	[DrawIf("_dummy", Hide = true)]
	[SerializeField]
	private bool _dummy;

	[Obsolete("Use NetworkPrefabConfig.FusionVersionInfo. Will be removed in 1.1.")]
	public static (NetworkRunner.BuildTypes, FileVersionInfo) FusionVersionInfo => NetworkProjectConfig.FusionVersionInfo;

	[Obsolete("Use NetworkProjectConfig.Global.ToJson. Will be removed in 1.1.")]
	public static string GetSerializedConfigForRelay()
	{
		return NetworkProjectConfig.Serialize(NetworkProjectConfig.Global);
	}

	[Obsolete("Use NetworkProjectConfig.ConvertPhysicsMode instead. Will be removed in 1.1.")]
	public static LocalPhysicsMode ConvertPhysicsMode(NetworkProjectConfig.PhysicsEngines engine)
	{
		return NetworkProjectConfig.ConvertPhysicsMode(engine);
	}
}
