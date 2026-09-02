using System;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Modules.Cutscene.Scripts;

[NetworkBehaviourWeaved(9)]
public class CutsceneNetworkManager : NetworkBehaviour
{
	public CutsceneManager cutsceneManager;

	[SerializeField]
	[DefaultForProperty("showCutscene", 0, 1)]
	private bool _showCutscene;

	[SerializeField]
	[DefaultForProperty("arrPlayerSkipCutscene", 1, 8)]
	private bool[] _arrPlayerSkipCutscene;

	[HideInInspector]
	public int playerCount;

	[HideInInspector]
	public int playerSkipCount;

	[HideInInspector]
	public bool allPlayerSkip;

	private static Changed<CutsceneNetworkManager> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<CutsceneNetworkManager> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<CutsceneNetworkManager> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked(OnChanged = "OnShowCutsceneChanged")]
	[NetworkedWeaved(0, 1)]
	public unsafe bool showCutscene
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CutsceneNetworkManager.showCutscene. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean((int*)((byte*)Ptr + 0));
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CutsceneNetworkManager.showCutscene. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean((int*)((byte*)Ptr + 0), value);
		}
	}

	[Networked(OnChanged = "OnSkipChanged")]
	[Capacity(8)]
	[NetworkedWeaved(1, 8)]
	public unsafe NetworkArray<bool> arrPlayerSkipCutscene
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CutsceneNetworkManager.arrPlayerSkipCutscene. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<bool>((byte*)Ptr + 4, 8, ReaderWriter_0040System_Boolean.GetInstance());
		}
	}

	public static event Action<CutsceneNetworkManager> OnShowHideCutsceneEvent;

	public static event Action<CutsceneNetworkManager> OnPlayerSkipCutsceneEvent;

	private void Awake()
	{
		cutsceneManager = GenericSingleton<CutsceneManager>.Instance;
		base.transform.SetParent(cutsceneManager.transform);
		cutsceneManager.CutsceneNetworkManager = this;
	}

	public bool GetOwnSkipStatus()
	{
		return arrPlayerSkipCutscene[NetworkGameManager.Instance.ownPlayer.network.GetIDX()];
	}

	[Preserve]
	private static void OnShowCutsceneChanged(Changed<CutsceneNetworkManager> changed)
	{
		if (changed.Behaviour.showCutscene)
		{
			changed.Behaviour.playerCount = NetworkGameManager.Instance.arrPlayerController.Count;
			changed.Behaviour.playerSkipCount = 0;
			CutsceneNetworkManager behaviour = changed.Behaviour;
			bool flag = (changed.Behaviour.cutsceneManager.AllSkip = false);
			behaviour.allPlayerSkip = flag;
		}
		OnShowHideCutsceneEvent?.Invoke(changed.Behaviour);
	}

	[Preserve]
	private static void OnSkipChanged(Changed<CutsceneNetworkManager> changed)
	{
		if (changed.Behaviour.showCutscene)
		{
			(bool, int) tuple = changed.Behaviour.GetAllPlayerSkip();
			CutsceneNetworkManager behaviour = changed.Behaviour;
			bool flag = (changed.Behaviour.cutsceneManager.AllSkip = tuple.Item1);
			behaviour.allPlayerSkip = flag;
			changed.Behaviour.playerSkipCount = tuple.Item2;
			OnPlayerSkipCutsceneEvent?.Invoke(changed.Behaviour);
		}
	}

	private (bool AllSkip, int SkipCount) GetAllPlayerSkip()
	{
		int num = 0;
		bool item = true;
		for (int i = 0; i < playerCount; i++)
		{
			if (arrPlayerSkipCutscene[i])
			{
				num++;
			}
			else
			{
				item = false;
			}
		}
		return (AllSkip: item, SkipCount: num);
	}

	public void SetSkipCutscene(byte id, bool value)
	{
		arrPlayerSkipCutscene.Set(id, value);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_SetSkipCutscene(byte id, bool value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_SetSkipCutscene(System.Byte,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_SetSkipCutscene(System.Byte,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				data[num2] = id;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		SetSkipCutscene(id, value);
	}

	public void SetShowCutscene(bool value)
	{
		showCutscene = value;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_SetShowCutscene(bool setShow)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_SetShowCutscene(System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_SetShowCutscene(System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), setShow);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		SetShowCutscene(setShow);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_PlayCutscene(string cutsceneKey)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_PlayCutscene(System.String)", Object, 7);
				return;
			}
			int num = 8;
			num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(cutsceneKey) + 3) & -4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Cutscene.Scripts.CutsceneNetworkManager::Rpc_PlayCutscene(System.String)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
				num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, cutsceneKey) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		cutsceneManager.Play(cutsceneKey);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		showCutscene = _showCutscene;
		NetworkBehaviourUtils.InitializeNetworkArray(arrPlayerSkipCutscene, _arrPlayerSkipCutscene, "arrPlayerSkipCutscene");
	}

	public override void CopyStateToBackingFields()
	{
		_showCutscene = showCutscene;
		NetworkBehaviourUtils.CopyFromNetworkArray(arrPlayerSkipCutscene, ref _arrPlayerSkipCutscene);
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SetSkipCutscene_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte id = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num3;
		behaviour.InvokeRpc = true;
		((CutsceneNetworkManager)behaviour).Rpc_SetSkipCutscene(id, value);
	}

	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SetShowCutscene_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool setShow = num2;
		behaviour.InvokeRpc = true;
		((CutsceneNetworkManager)behaviour).Rpc_SetShowCutscene(setShow);
	}

	[NetworkRpcWeavedInvoker(3, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_PlayCutscene_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CutsceneNetworkManager)behaviour).Rpc_PlayCutscene(result);
	}
}
