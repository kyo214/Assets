#define DEBUG
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Fusion;

[Serializable]
public class NetworkProjectConfig
{
	public enum PeerModes
	{
		Single = 0,
		Multiple = 1
	}

	public enum PhysicsEngines
	{
		Physics3D = 0,
		Physics2D = 1,
		None = 2
	}

	public enum PhysicsModes
	{
		ServerOnly = 0,
		ClientPrediction = 1
	}

	public enum SceneLoadSpawnModes
	{
		NotAllowed = 0,
		Allowed = 1,
		Queued = 2
	}

	public enum DeltaCompressors
	{
		Managed = 0,
		Burst = 1,
		DebugUncompressed = 2
	}

	public delegate NetworkProjectConfigAsset AssetLoadingDelegate();

	public delegate void AssetUndloadingDelegate(NetworkProjectConfigAsset asset);

	private static class Static
	{
		private delegate NetworkProjectConfigAsset LoadConfigDelegate();

		private static readonly LoadConfigDelegate LoadEditMode;

		public static Lazy<NetworkProjectConfigAsset> Instance;

		public static AssetLoadingDelegate CustomLoadConfig;

		public static AssetUndloadingDelegate CustomUnloadConfig;

		static Static()
		{
			if (Application.isEditor)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					if (assembly.FullName.StartsWith("Fusion.Editor,"))
					{
						Type type = assembly.GetType("Fusion.Editor.NetworkProjectConfigUtilities", throwOnError: true);
						MethodInfo method = type.GetMethod("EditTimeLoadGlobalConfigWrapper", BindingFlags.Static | BindingFlags.NonPublic);
						LoadEditMode = (LoadConfigDelegate)Delegate.CreateDelegate(typeof(LoadConfigDelegate), method);
						break;
					}
				}
				if (LoadEditMode == null)
				{
					throw new InvalidOperationException("Editor assembly starting with \"Fusion.Editor,\" not found");
				}
			}
			Reload();
		}

		public static void Reload()
		{
			Lazy<NetworkProjectConfigAsset> instance = Instance;
			NetworkProjectConfigAsset networkProjectConfigAsset = ((instance != null && instance.IsValueCreated) ? Instance.Value : null);
			Instance = new Lazy<NetworkProjectConfigAsset>(() =>
			{
				NetworkProjectConfigAsset networkProjectConfigAsset2 = null;
				try
				{
					if (!Application.isPlaying)
					{
						Assert.Check(Application.isEditor);
						Assert.Check(LoadEditMode != null);
						networkProjectConfigAsset2 = LoadEditMode();
						Assert.Always(networkProjectConfigAsset2 != null, "Failed to config in edit mode.");
					}
					else
					{
						bool flag = false;
						if (CustomLoadConfig != null)
						{
							flag = true;
							networkProjectConfigAsset2 = CustomLoadConfig();
						}
						if (networkProjectConfigAsset2 == null)
						{
							networkProjectConfigAsset2 = Resources.Load<NetworkProjectConfigAsset>("NetworkProjectConfig");
						}
						if (networkProjectConfigAsset2 == null)
						{
							if (flag)
							{
								throw new InvalidOperationException("Failed to load the global config using GlobalAssetLoading event and \"NetworkProjectConfig\" Resource");
							}
							throw new InvalidOperationException("Failed to load the global config from \"NetworkProjectConfig\" Resource");
						}
					}
					NetworkProjectConfig config = networkProjectConfigAsset2.Config;
					if (config == null)
					{
						throw new InvalidOperationException("Wrapper contains null config");
					}
					InitPrefabs(config.PrefabTable, networkProjectConfigAsset2);
					return networkProjectConfigAsset2;
				}
				catch (Exception innerException)
				{
					if (networkProjectConfigAsset2 != null)
					{
						UnloadConfigAsset(networkProjectConfigAsset2);
					}
					throw new InvalidOperationException("Failed to load global config", innerException);
				}
			}, isThreadSafe: false);
			if (networkProjectConfigAsset != null)
			{
				UnloadConfigAsset(networkProjectConfigAsset);
			}
		}

		private static void UnloadConfigAsset(NetworkProjectConfigAsset asset)
		{
			CustomUnloadConfig?.Invoke(asset);
			NetworkProjectConfig config = asset.Config;
			if (config != null && config.PrefabTable?.Count > 0)
			{
				asset.Config.PrefabTable.Clear();
			}
		}

		private static void InitPrefabs(NetworkPrefabTable table, NetworkProjectConfigAsset wrapper)
		{
			NetworkPrefabSourceUnityBase[] prefabs = wrapper.Prefabs;
			foreach (NetworkPrefabSourceUnityBase networkPrefabSourceUnityBase in prefabs)
			{
				if (!table.TryAdd(networkPrefabSourceUnityBase.AssetGuid, networkPrefabSourceUnityBase, out var _))
				{
					table.TryGetPrefabEntry(networkPrefabSourceUnityBase.AssetGuid, out var prefab);
					Log.Error($"Failed to add prefab asset {networkPrefabSourceUnityBase.AssetGuid}, there is already a prefab entry with same guid: {prefab}");
				}
			}
		}
	}

	public const string DefaultResourceName = "NetworkProjectConfig";

	public const string CurrentTypeId = "NetworkProjectConfig";

	public const int CurrentVersion = 1;

	[HideInInspector]
	public int Version = 1;

	[HideInInspector]
	public string TypeId = "NetworkProjectConfig";

	[Header("Scene Settings")]
	[FormerlySerializedAs("InstanceMode")]
	[InlineHelp]
	public PeerModes PeerMode;

	[Header("Physics Settings")]
	[InlineHelp]
	public PhysicsEngines PhysicsEngine;

	[DrawIf("PhysicsEngine", 2.0, Compare = DoIfCompareOperator.NotEqual)]
	[FormerlySerializedAs("PhysicsMode")]
	[InlineHelp]
	public PhysicsModes ServerPhysicsMode;

	[Header("Lag Compensation Settings")]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public bool UseLagCompensation = true;

	[DrawIf("UseLagCompensation")]
	[InlineHelp]
	public LagCompensationSettings LagCompensation = new LagCompensationSettings();

	[Header("Object Settings")]
	[InlineHelp]
	public SceneLoadSpawnModes SceneLoadSpawnMode = SceneLoadSpawnModes.NotAllowed;

	[InlineHelp]
	public DeltaCompressors DeltaCompressor;

	[InlineHelp]
	public bool InvokeRenderInBatchMode = true;

	[InlineHelp]
	public ushort MaxNetworkedObjectCount = 8192;

	[InlineHelp]
	public bool NetworkIdIsObjectName;

	[InlineHelp]
	public bool HideNetworkObjectInactivityGuard = false;

	[Header("Host Migration")]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public bool EnableHostMigration = false;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public uint HostMigrationSnapshotInterval = 60u;

	public NetworkPrefabTable PrefabTable = new NetworkPrefabTable();

	[InlineHelp]
	public SimulationConfig Simulation = new SimulationConfig();

	[InlineHelp]
	public InterpolationConfiguration Interpolation = new InterpolationConfiguration();

	[InlineHelp]
	public NetworkConfiguration Network = new NetworkConfiguration();

	[InlineHelp]
	public NetworkSimulationConfiguration NetworkConditions = new NetworkSimulationConfiguration();

	[InlineHelp]
	public HeapConfiguration Heap = new HeapConfiguration();

	[InlineHelp]
	public AccuracyDefaults AccuracyDefaults = new AccuracyDefaults();

	[Header("Weaver Settings")]
	[AssemblyName]
	[InlineHelp]
	public string[] AssembliesToWeave = new string[2] { "Assembly-CSharp", "Assembly-CSharp-firstpass" };

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool UseSerializableDictionary = true;

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool NullChecksForNetworkedProperties = true;

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool CheckRpcAttributeUsage = false;

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool CheckNetworkedPropertiesBeingEmpty = false;

	public static NetworkProjectConfig Global => Static.Instance.Value.Config;

	public static (NetworkRunner.BuildTypes, FileVersionInfo) FusionVersionInfo
	{
		get
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				string fullName = assembly.FullName;
				if (fullName.StartsWith("Fusion.Runtime,"))
				{
					return (NetworkRunner.BuildType, FileVersionInfo.GetVersionInfo(assembly.Location));
				}
			}
			return ((NetworkRunner.BuildTypes)(-1), null);
		}
	}

	public static event AssetLoadingDelegate GlobalAssetLoading
	{
		add
		{
			Static.CustomLoadConfig = (AssetLoadingDelegate)Delegate.Combine(Static.CustomLoadConfig, value);
		}
		remove
		{
			Static.CustomLoadConfig = (AssetLoadingDelegate)Delegate.Remove(Static.CustomLoadConfig, value);
		}
	}

	public static event AssetUndloadingDelegate GlobalAssetUnloading
	{
		add
		{
			Static.CustomUnloadConfig = (AssetUndloadingDelegate)Delegate.Combine(Static.CustomUnloadConfig, value);
		}
		remove
		{
			Static.CustomUnloadConfig = (AssetUndloadingDelegate)Delegate.Remove(Static.CustomUnloadConfig, value);
		}
	}

	public static void UnloadGlobal()
	{
		Static.Reload();
	}

	internal static void ResetStatics()
	{
		Static.Reload();
		Static.CustomUnloadConfig = null;
		Static.CustomLoadConfig = null;
	}

	internal NetworkProjectConfig Init(int globalSize, int? playerCountOverride, int? inputWordCount)
	{
		NetworkProjectConfig networkProjectConfig = Copy();
		networkProjectConfig.Heap = networkProjectConfig.Heap.Init(globalSize);
		networkProjectConfig.Network = networkProjectConfig.Network.Init();
		networkProjectConfig.Simulation = networkProjectConfig.Simulation.Init(playerCountOverride, inputWordCount);
		networkProjectConfig.HostMigrationSnapshotInterval = Math.Max(HostMigrationSnapshotInterval, 1u);
		return networkProjectConfig;
	}

	internal NetworkProjectConfig Copy()
	{
		NetworkProjectConfig networkProjectConfig = (NetworkProjectConfig)MemberwiseClone();
		networkProjectConfig.Simulation = Simulation.Copy();
		return networkProjectConfig;
	}

	public override string ToString()
	{
		return Serialize(this);
	}

	public static string Serialize(NetworkProjectConfig config)
	{
		return JsonUtility.ToJson(config);
	}

	public static NetworkProjectConfig Deserialize(string data)
	{
		return JsonUtility.FromJson<NetworkProjectConfig>(data);
	}

	internal static string SerializeMinimal(NetworkProjectConfig config)
	{
		return JsonUtils.RemoveExtraReferences(Serialize(config));
	}

	public static LocalPhysicsMode ConvertPhysicsMode(PhysicsEngines engine)
	{
		return engine switch
		{
			PhysicsEngines.Physics2D => LocalPhysicsMode.Physics2D, 
			PhysicsEngines.Physics3D => LocalPhysicsMode.Physics3D, 
			_ => LocalPhysicsMode.None, 
		};
	}
}
