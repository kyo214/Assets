using System;
using System.Collections;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace Dissonance.Integrations.PhotonFusion;

[NetworkBehaviourWeaved(66)]
public class DissonanceFusionPlayer : NetworkBehaviour, IDissonancePlayer
{
	private static readonly Log Log = Logs.Create(LogCategory.Core, "DissonanceFusionPlayer");

	[CanBeNull]
	private Transform _transform;

	[DefaultForProperty("NetworkedPlayerName", 0, 66)]
	private string _NetworkedPlayerName;

	[CanBeNull]
	private DissonanceComms _dissonance;

	private static Changed<DissonanceFusionPlayer> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<DissonanceFusionPlayer> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<DissonanceFusionPlayer> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache_NetworkedPlayerName;

	private Transform Transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = base.transform;
			}
			return _transform;
		}
	}

	public Vector3 Position => Transform.position;

	public Quaternion Rotation => Transform.rotation;

	public NetworkPlayerType Type
	{
		get
		{
			if (Runner == null)
			{
				return NetworkPlayerType.Unknown;
			}
			if (Runner.IsClient || Runner.IsServer)
			{
				if (HasInputAuthority)
				{
					return NetworkPlayerType.Local;
				}
				return NetworkPlayerType.Remote;
			}
			return NetworkPlayerType.Unknown;
		}
	}

	[Networked(OnChanged = "OnNetworkedPlayerNameChangedStatic", OnChangedTargets = OnChangedTargets.All)]
	[Capacity(64)]
	[NetworkedWeaved(0, 66)]
	private unsafe string NetworkedPlayerName
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing DissonanceFusionPlayer.NetworkedPlayerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash((int*)((byte*)Ptr + 0), 64, ref cache_NetworkedPlayerName);
			return cache_NetworkedPlayerName;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing DissonanceFusionPlayer.NetworkedPlayerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash((int*)((byte*)Ptr + 0), 64, value, ref cache_NetworkedPlayerName);
		}
	}

	public bool IsTracking { get; private set; }

	public string PlayerId { get; private set; }

	public override void Spawned()
	{
		base.Spawned();
		StartCoroutine(OnSpawnedCo());
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		StopAllCoroutines();
		if (IsTracking && _dissonance != null)
		{
			_dissonance.StopTracking(this);
			IsTracking = false;
		}
		if (_dissonance != null)
		{
			_dissonance.LocalPlayerNameChanged -= SetLocalPlayerName;
		}
	}

	private IEnumerator OnSpawnedCo()
	{
		if (_dissonance == null)
		{
			while (_dissonance == null)
			{
				_dissonance = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
				yield return null;
			}
		}
		if (HasInputAuthority)
		{
			_dissonance.LocalPlayerNameChanged += SetLocalPlayerName;
			SetLocalPlayerName(_dissonance.LocalPlayerName);
		}
	}

	[Preserve]
	protected static void OnNetworkedPlayerNameChangedStatic(Changed<DissonanceFusionPlayer> changed)
	{
		changed.Behaviour.OnNetworkedPlayerNameChanged(changed);
	}

	protected void OnNetworkedPlayerNameChanged(Changed<DissonanceFusionPlayer> changed)
	{
		StopAllCoroutines();
		if (IsTracking)
		{
			StopTracking();
		}
		PlayerId = NetworkedPlayerName;
		StartTracking();
	}

	private void SetLocalPlayerName(string dissonanceName)
	{
		Rpc_SetName(dissonanceName);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetName(string name)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Dissonance.Integrations.PhotonFusion.DissonanceFusionPlayer::Rpc_SetName(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(name) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Dissonance.Integrations.PhotonFusion.DissonanceFusionPlayer::Rpc_SetName(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, name) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		NetworkedPlayerName = name;
	}

	private void StartTracking()
	{
		if (IsTracking)
		{
			throw Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "0663D808-ACCC-4D13-8913-03F9BA0C8578");
		}
		StopAllCoroutines();
		StartCoroutine(StartTrackingCo());
	}

	private IEnumerator StartTrackingCo()
	{
		if (_dissonance == null)
		{
			while (_dissonance == null)
			{
				_dissonance = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
				yield return null;
			}
		}
		while (PlayerId == null)
		{
			yield return null;
		}
		_dissonance.TrackPlayerPosition(this);
		IsTracking = true;
	}

	private void StopTracking()
	{
		if (!IsTracking)
		{
			throw Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "48802E32-C840-4C4B-BC58-4DC741464B9A");
		}
		StopAllCoroutines();
		if (_dissonance != null)
		{
			_dissonance.StopTracking(this);
			IsTracking = false;
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		NetworkedPlayerName = _NetworkedPlayerName;
	}

	public override void CopyStateToBackingFields()
	{
		_NetworkedPlayerName = NetworkedPlayerName;
	}

	[NetworkRpcWeavedInvoker(1, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetName_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((DissonanceFusionPlayer)behaviour).Rpc_SetName(result);
	}
}
