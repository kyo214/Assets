#define DEBUG
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion;

[AddComponentMenu("Fusion/Network Object")]
[DisallowMultipleComponent]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object")]
[DefaultExecutionOrder(500)]
[ScriptHelp(Url = "https://doc.photonengine.com/fusion/current/manual/network-object", BackColor = EditorHeaderBackColor.Orange, Icon = EditorHeaderIcon.FusionBlue)]
public class NetworkObject : Behaviour, ILogBuilder
{
	public struct PredictionData
	{
		public Tick Tick;

		public NetworkPrefabId Prefab;

		public NetworkObjectPredictionKey Key;
	}

	internal enum ObjectInterestModes
	{
		AreaOfInterest = 0,
		AllPlayers = 1,
		ExplicitPlayers = 2
	}

	public const int DefaultExecutionOrder = 500;

	[NonSerialized]
	internal unsafe int* Ptr;

	[NonSerialized]
	internal unsafe int* Changed;

	[NonSerialized]
	internal FastReferenceList<NetworkBehaviour> CallbackBehaviours;

	[NonSerialized]
	public NetworkId Id;

	[NonSerialized]
	public bool IsResume;

	[NonSerialized]
	[EditorDisabled(false)]
	public NetworkRunner Runner;

	[InlineHelp]
	[SerializeField]
	[FormerlySerializedAs("AoiMode")]
	[MultiPropertyDrawersFix]
	internal ObjectInterestModes ObjectInterest = ObjectInterestModes.AllPlayers;

	[InlineHelp]
	[SerializeField]
	[FormerlySerializedAs("DefaultPropertyGroups")]
	[MultiPropertyDrawersFix]
	internal string[] DefaultInterestGroups;

	[SerializeField]
	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	internal bool DestroyWhenStateAuthorityLeaves;

	[SerializeField]
	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	internal bool AllowStateAuthorityOverride = true;

	[DrawIf("ObjectInterest", 0.0)]
	[SerializeField]
	[InlineHelp]
	[FormerlySerializedAs("AoiPosition")]
	internal NetworkAreaOfInterestBehaviour AoiPositionSource;

	[InlineHelp]
	public NetworkObjectFlags Flags;

	[InlineHelp]
	public NetworkObjectGuid NetworkGuid;

	[InlineHelp]
	public PredictionData PredictedSpawn;

	[InlineHelp]
	public NetworkObject[] NestedObjects;

	[InlineHelp]
	public NetworkBehaviour[] NetworkedBehaviours;

	[InlineHelp]
	public SimulationBehaviour[] SimulationBehaviours;

	[NonSerialized]
	internal bool InSimulation;

	public Tick LastReceiveTick { get; internal set; }

	public string Name
	{
		get
		{
			NetworkId id = Id;
			return id.ToString() + (BehaviourUtils.IsAlive(this) ? ("(" + base.name + ")") : "");
		}
	}

	public bool IsSceneObject => Flags.IsSceneObject();

	public bool IsSpawnedPrefabRoot => (Flags & NetworkObjectFlags.TypeSpawnedPrefab) == NetworkObjectFlags.TypeSpawnedPrefab;

	public bool IsSpawnedPrefabNestedObject => (Flags & NetworkObjectFlags.TypeSpawnedPrefabChild) == NetworkObjectFlags.TypeSpawnedPrefabChild;

	public bool IsValid => BehaviourUtils.IsAlive(Runner) && Runner.Exists(this);

	public bool IsPredictedSpawn => PredictedSpawn.Key;

	public unsafe bool IsPredictedDespawn => PredictedSpawn.Tick != 0 && Header != null;

	public bool IsInSimulation => InSimulation;

	public unsafe PlayerRef InputAuthority => (Header == null) ? PlayerRef.None : Header->InputAuthority;

	public unsafe PlayerRef StateAuthority => (Header == null) ? PlayerRef.None : Header->StateAuthority;

	public unsafe int AreaOfInterestLayerMask => (Header != null) ? Header->AreaOfInterestLayerMask : 0;

	internal unsafe NetworkObjectHeader* Header
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (NetworkObjectHeader*)Ptr;
		}
	}

	public unsafe bool HasInputAuthority => Header != null && Header->InputAuthority == Runner.Simulation.LocalPlayer && Header->InputAuthority.IsValid;

	public unsafe bool HasStateAuthority => Header != null && (Header->StateAuthority == Runner.Simulation.LocalPlayer || (Header->StateAuthority.IsNone && Runner.Simulation.IsServer));

	public bool IsProxy => (GetLocalAuthorityMask() & 4) == 4;

	public bool IsSpawnable
	{
		get
		{
			return !Flags.IsIgnored();
		}
		set
		{
			Flags.SetIgnored(!value);
		}
	}

	protected virtual void Awake()
	{
		DebugAwake();
		if (Id.IsValid && Flags.IsActivatedByUser())
		{
			if (BehaviourUtils.IsAlive(Runner))
			{
				Runner.AttachActivatedByUser(this);
			}
			else
			{
				Log.DebugWarn(this, "Expected to be activated while the runner is active");
			}
		}
	}

	protected virtual void OnDestroy()
	{
		OnDestroyInternal();
		DebugOnDestroy();
	}

	internal void OnDestroyNeverActive()
	{
		OnDestroyInternal();
		Assert.Check(Flags.IsActivatedByUser(), "Should only happen for initially inactive objects");
		Assert.Check((Flags & NetworkObjectFlags.Spawned) == 0, "Never should have become active");
	}

	private unsafe void OnDestroyInternal()
	{
		if (BehaviourUtils.IsAlive(Runner))
		{
			if (Runner.Exists(this))
			{
				NetworkObjectDestroyFlags networkObjectDestroyFlags = NetworkObjectDestroyFlags.DestroyedByEngine;
				if (HasStateAuthority)
				{
					networkObjectDestroyFlags |= NetworkObjectDestroyFlags.DestroyState;
				}
				Runner.Destroy(this, networkObjectDestroyFlags);
			}
			else if (Runner.Simulation != null && Id.IsValid && Changed != null)
			{
				Runner.DestroyOrphaned(this, destroyedByEngine: true);
			}
			else
			{
				Runner.DestroyOrphanedUnattached(this);
			}
		}
		else if (BehaviourUtils.IsNotNull(Runner) && Id.IsValid && Changed != null)
		{
			Log.DebugWarn(this, "Runner has been destroyed, but the object has not been despawned.");
		}
		Ptr = null;
	}

	internal unsafe void ResetNetworkState()
	{
		Id = default;
		Ptr = default;
		Runner = null;
	}

	internal unsafe void Defaults()
	{
		if (IsSceneObject)
		{
			Assert.Check(NetworkGuid.IsValid);
		}
		Header->SceneGuid = (IsSceneObject ? NetworkGuid : default(NetworkObjectGuid));
		Header->InputAuthority = default;
		Header->StateAuthority = default;
	}

	internal static int GetWordCount(NetworkObject obj)
	{
		if (BehaviourUtils.IsAlive(obj))
		{
			int num = NetworkStructUtils.GetWordCount<NetworkObjectHeader>();
			for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
			{
				if (BehaviourUtils.IsAlive(obj.NetworkedBehaviours[i]))
				{
					num += NetworkBehaviourUtils.GetWordCount(obj.NetworkedBehaviours[i]);
					continue;
				}
				throw new Exception("Found missing NetworkBehaviour reference in NetworkBehaviours[] list on " + obj.Name + ". Re-baking of object required. Please check prefab or scene object and make sure NetworkBehaviour list is up to date.");
			}
			return num;
		}
		return 0;
	}

	public int GetLocalAuthorityMask()
	{
		if (BehaviourUtils.IsNotAlive(Runner))
		{
			return 0;
		}
		return AuthorityMasks.Create(HasStateAuthority, HasInputAuthority);
	}

	internal int GetRpcSourceAuthorityMask(PlayerRef player)
	{
		Assert.Check(BehaviourUtils.IsAlive(Runner));
		bool state = ((!StateAuthority.IsValid) ? (player.IsNone || Runner.IsHostPlayer(player)) : (StateAuthority == player || (player.IsNone && Runner.IsHostPlayer(StateAuthority))));
		bool input = InputAuthority.IsValid && (InputAuthority == player || (player.IsNone && Runner.IsHostPlayer(InputAuthority)));
		return AuthorityMasks.Create(state, input);
	}

	public unsafe void AssignInputAuthority(PlayerRef player)
	{
		Assert.Check(BehaviourUtils.IsAlive(Runner));
		Assert.Check(Runner.Exists(this));
		if (Runner.Topology == SimulationConfig.Topologies.ClientServer || HasStateAuthority)
		{
			Header->InputAuthority = player;
		}
	}

	public void RequestStateAuthority()
	{
		Assert.Check(BehaviourUtils.IsAlive(Runner));
		Assert.Check(Runner.Exists(this));
		if (Runner.IsClient && !HasStateAuthority)
		{
			Runner.Simulation.RequestStateAuthority(Id, wants: true);
		}
	}

	public void ReleaseStateAuthority()
	{
		Assert.Check(BehaviourUtils.IsAlive(Runner));
		Assert.Check(Runner.Exists(this));
		if (Runner.IsClient && HasStateAuthority)
		{
			Runner.Simulation.RequestStateAuthority(Id, wants: false);
		}
	}

	public void RemoveInputAuthority()
	{
		AssignInputAuthority(default);
	}

	public static implicit operator NetworkId(NetworkObject obj)
	{
		return BehaviourUtils.IsNull(obj) ? default(NetworkId) : obj.Id;
	}

	public void SetPlayerAlwaysInterested(PlayerRef player, bool alwaysInterested)
	{
		Runner.Simulation.SetPlayerAlwaysInterested(player, this, alwaysInterested);
	}

	public unsafe void CopyStateFrom(NetworkObject source)
	{
		Assert.Check(Header->Type.Equals(source.Header->Type), "NetworkObjects must be of the same type");
		Assert.Check(source.Id.IsValid, "Invalid NetworkId from source NetworkObject");
		Assert.Check(source.Id.Equals(Id), "NetworkObjects must have the same NetworkIds");
		Native.MemCpy(Ptr + 20, source.Ptr + 20, (Header->WordCount - 20) * 4);
		for (int i = 0; i < NestedObjects.Length; i++)
		{
			NestedObjects[i].CopyStateFrom(source.NestedObjects[i]);
		}
	}

	public unsafe void CopyStateFromSceneObject(NetworkObjectHeaderPtr source)
	{
		Assert.Check(Header->Type.Equals(source.Ptr->Type), "NetworkObjects must be of the same type");
		Assert.Check(Header->SceneGuid.Equals(source.Ptr->SceneGuid), "Scene NetworkObjects must have the same SceneGuid");
		Native.MemCpy(Ptr + 20, (byte*)source.Ptr + (nint)20 * (nint)4, (Header->WordCount - 20) * 4);
	}

	public void SetInterestGroup(PlayerRef player, string group, bool interested)
	{
		Runner.SetInterestGroup(this, player, group, interested);
	}

	[NetworkSerializeMethod]
	public static NetworkId NetworkWrap(NetworkRunner runner, NetworkObject obj)
	{
		if (BehaviourUtils.IsNotAlive(obj))
		{
			return default;
		}
		return obj.Id;
	}

	[NetworkDeserializeMethod]
	public static void NetworkUnwrap(NetworkRunner runner, NetworkId wrapper, ref NetworkObject result)
	{
		if (!wrapper.IsValid)
		{
			result = null;
		}
		else if (!runner.TryFindObject(wrapper, out result))
		{
			Assert.Check(BehaviourUtils.IsNotAlive(result));
		}
	}

	private void DebugAwake()
	{
		if ((Flags & NetworkObjectFlags.Spawned) != NetworkObjectFlags.None)
		{
			Log.Error(this, "Spawned before Awake");
		}
	}

	private void DebugOnDestroy()
	{
		if ((Flags & NetworkObjectFlags.Spawned) != NetworkObjectFlags.None)
		{
			Log.Error(this, "Not despawned before OnDestroy");
		}
	}

	[Conditional("DEBUG")]
	internal void DebugNotifySpawned()
	{
	}

	[Conditional("DEBUG")]
	internal void DebugNotifyDespawning()
	{
	}

	void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		AddDebugMessagePrefix(builder, in options);
		builder.Append(": ");
		builder.Append(message);
	}

	internal void AddDebugMessagePrefix(StringBuilder builder, in LogOptions options, bool addGuid = false, bool addHashCode = false)
	{
		builder.Append(BehaviourUtils.IsAlive(this) ? base.name : "(destroyed)");
		if (Id.IsValid)
		{
			builder.Append(" ");
			builder.Append(Id.ToString());
		}
		if (addGuid)
		{
			builder.Append("[guid:").Append(NetworkGuid).Append("]");
		}
		if (addHashCode)
		{
			builder.Append("[hashCode:").Append(GetHashCode()).Append("]");
		}
		int length = builder.Length;
		if (NetworkRunner.TryGetPrettyRunnerName(builder, Runner, in options))
		{
			builder.Insert(length, '@');
		}
	}
}
