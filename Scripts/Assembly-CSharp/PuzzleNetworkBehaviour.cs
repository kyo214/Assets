using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(21)]
public class PuzzleNetworkBehaviour : NetworkBehaviour
{
	[SerializeField]
	[DefaultForProperty("currentIdx", 0, 1)]
	private int _currentIdx;

	[SerializeField]
	[DefaultForProperty("currentPass", 1, 18)]
	private string _currentPass;

	[SerializeField]
	[DefaultForProperty("isComplete", 19, 1)]
	private bool _isComplete;

	[SerializeField]
	[DefaultForProperty("isInitialized", 20, 1)]
	private bool _isInitialized;

	public bool IsSpawned;

	private static Changed<PuzzleNetworkBehaviour> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<PuzzleNetworkBehaviour> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<PuzzleNetworkBehaviour> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache_currentPass;

	[Networked(OnChanged = "OnIdxChanged")]
	[NetworkedWeaved(0, 1)]
	public unsafe int currentIdx
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.currentIdx. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(int*)((byte*)Ptr + 0);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.currentIdx. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(int*)((byte*)Ptr + 0) = value;
		}
	}

	[Networked(OnChanged = "OnCurrentPassChanged")]
	[NetworkedWeaved(1, 18)]
	public unsafe string currentPass
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.currentPass. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 1, 16, ref cache_currentPass);
			return cache_currentPass;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.currentPass. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 1, 16, value, ref cache_currentPass);
		}
	}

	[Networked(OnChanged = "OnIsCompleteChanged")]
	[NetworkedWeaved(19, 1)]
	public unsafe bool isComplete
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.isComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 19);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.isComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 19, value);
		}
	}

	[Networked(OnChanged = "OnIsCompleteChanged")]
	[NetworkedWeaved(20, 1)]
	public unsafe bool isInitialized
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.isInitialized. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 20);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PuzzleNetworkBehaviour.isInitialized. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 20, value);
		}
	}

	public event Action<int> OnIdxChange;

	public event Action OnPassChanged;

	public event Action OnPuzzleComplete;

	public event Action OnSpawned;

	public override void Spawned()
	{
		IsSpawned = true;
		OnSpawned?.Invoke();
	}

	[Preserve]
	public static void OnIdxChanged(Changed<PuzzleNetworkBehaviour> changed)
	{
		changed.Behaviour.OnIdxChange?.Invoke(changed.Behaviour.currentIdx);
	}

	[Preserve]
	public static void OnCurrentPassChanged(Changed<PuzzleNetworkBehaviour> changed)
	{
		changed.Behaviour.OnPassChanged?.Invoke();
	}

	[Preserve]
	public static void OnIsCompleteChanged(Changed<PuzzleNetworkBehaviour> changed)
	{
		changed.Behaviour.OnPuzzleComplete?.Invoke();
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCChangeIdx(int idx)
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
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PuzzleNetworkBehaviour::RPCChangeIdx(System.Int32)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PuzzleNetworkBehaviour::RPCChangeIdx(System.Int32)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					*(int*)(data + num2) = idx;
					num2 += 4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		isInitialized = true;
		currentIdx = idx;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCSetComplete()
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
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PuzzleNetworkBehaviour::RPCSetComplete()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PuzzleNetworkBehaviour::RPCSetComplete()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		isComplete = true;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCCompleteAndCheckMap(short uidInteractableItem, byte playerIdx)
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
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PuzzleNetworkBehaviour::RPCCompleteAndCheckMap(System.Int16,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PuzzleNetworkBehaviour::RPCCompleteAndCheckMap(System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
				*(short*)(data + num2) = uidInteractableItem;
				num2 += 5 & -4;
				data[num2] = playerIdx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			isComplete = true;
		}
		GameManager.Instance.GetItemInteractable(uidInteractableItem).IsSolved = true;
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIdx);
		RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(player.RoomName);
		if ((bool)roomCollider)
		{
			roomCollider.CheckMap(player);
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		currentIdx = _currentIdx;
		currentPass = _currentPass;
		isComplete = _isComplete;
		isInitialized = _isInitialized;
	}

	public override void CopyStateToBackingFields()
	{
		_currentIdx = currentIdx;
		_currentPass = currentPass;
		_isComplete = isComplete;
		_isInitialized = isInitialized;
	}

	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	protected unsafe static void RPCChangeIdx_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int idx = num2;
		behaviour.InvokeRpc = true;
		((PuzzleNetworkBehaviour)behaviour).RPCChangeIdx(idx);
	}

	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	protected unsafe static void RPCSetComplete_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PuzzleNetworkBehaviour)behaviour).RPCSetComplete();
	}

	[NetworkRpcWeavedInvoker(3, 7, 7)]
	[Preserve]
	protected unsafe static void RPCCompleteAndCheckMap_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uidInteractableItem = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte playerIdx = num3;
		behaviour.InvokeRpc = true;
		((PuzzleNetworkBehaviour)behaviour).RPCCompleteAndCheckMap(uidInteractableItem, playerIdx);
	}
}
