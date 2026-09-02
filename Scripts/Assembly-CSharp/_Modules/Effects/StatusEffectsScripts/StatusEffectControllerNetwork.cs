using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.CodeGen;
using Toked.StatusEffect;
using UnityEngine;
using UnityEngine.Scripting;
using _Modules.CharacterSkin.Scripts;

namespace _Modules.Effects.StatusEffectsScripts;

[RequireComponent(typeof(StatusEffectController))]
[NetworkBehaviourWeaved(563)]
public class StatusEffectControllerNetwork : NetworkBehaviour
{
	[SerializeField]
	private StatusEffectController _statusEffectController;

	[SerializeField]
	private bool _syncOnSpawn;

	private bool _applyingFromNetwork;

	private static Changed<StatusEffectControllerNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<StatusEffectControllerNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<StatusEffectControllerNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[SerializeField]
	[DefaultForProperty("_statusEffectData", 0, 563)]
	private NetworkString<_32>[] __statusEffectData;

	public StatusEffectController StatusEffectController => _statusEffectController;

	[Networked(OnChanged = "OnStatusEffectChanged")]
	[Capacity(16)]
	[NetworkedWeaved(0, 563)]
	public unsafe NetworkLinkedList<NetworkString<_32>> _statusEffectData
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing StatusEffectControllerNetwork._statusEffectData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkLinkedList<NetworkString<_32>>((byte*)Ptr + 0, 16, ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__32_003E.GetInstance());
		}
	}

	public NetworkLinkedList<NetworkString<_32>> GetStatusEffectData()
	{
		return _statusEffectData;
	}

	private void OnEnable()
	{
		_statusEffectController.OnAddedStatusEffectEvent += OnAddedStatusEffectAction;
		_statusEffectController.OnRemoveStatusEffectEvent += OnRemoveStatusEffectAction;
		_statusEffectController.OnSyncStatusEffectEvent += OnSyncStatusEffectAction;
		_statusEffectController.OnSwapStatusEffectEvent += OnSwapStatusEffectEvent;
	}

	private void OnDisable()
	{
		_statusEffectController.OnAddedStatusEffectEvent -= OnAddedStatusEffectAction;
		_statusEffectController.OnRemoveStatusEffectEvent -= OnRemoveStatusEffectAction;
		_statusEffectController.OnSyncStatusEffectEvent -= OnSyncStatusEffectAction;
		_statusEffectController.OnSwapStatusEffectEvent -= OnSwapStatusEffectEvent;
	}

	[Preserve]
	private static void OnStatusEffectChanged(Changed<StatusEffectControllerNetwork> changed)
	{
		changed.Behaviour.SyncNetworkToLocal(applyingFromNetwork: true);
	}

	public void SyncNetworkToLocal(bool applyingFromNetwork = false)
	{
		_applyingFromNetwork = applyingFromNetwork;
		try
		{
			StatusEffectController statusEffectController = StatusEffectController;
			NetworkLinkedList<NetworkString<_32>> statusEffectData = GetStatusEffectData();
			StatusEffectLibraryScriptableObject statusEffectLibraryScriptableObject = DataManager.Instance.Get<StatusEffectLibraryScriptableObject>();
			if (statusEffectLibraryScriptableObject == null)
			{
				return;
			}
			HashSet<string> hashSet = statusEffectData.Select((NetworkString<_32> x) => x.Value).ToHashSet();
			HashSet<string> hashSet2 = statusEffectController.StatusEffectsList.Select((KeyValuePair<string, StatusEffectController.StatusEffect> x) => x.Key).ToHashSet();
			foreach (string item4 in hashSet)
			{
				if (hashSet2.Contains(item4))
				{
					continue;
				}
				(string, string, string) tuple = SplitKey(item4);
				StatusEffectScriptableObject data = statusEffectLibraryScriptableObject.GetData(tuple.Item1);
				if (!data)
				{
					Debug.LogWarning("StatusEffect not found in library: " + tuple.Item1);
				}
				else
				{
					if (!data.InfiniteDuration)
					{
						continue;
					}
					StatusEffectScriptableObject statusEffectScriptableObject = data.CloneStatusEffectSO(destroyOnRemove: true);
					if (statusEffectScriptableObject is IItemEffect itemEffect)
					{
						if (!string.IsNullOrEmpty(tuple.Item2) && !string.IsNullOrEmpty(tuple.Item3))
						{
							if (int.TryParse(tuple.Item3, out var result) && int.TryParse(tuple.Item2, out var result2))
							{
								itemEffect.Init(result2, result);
								statusEffectScriptableObject.StatusEffectData.SetAdditionalName(tuple.Item2, tuple.Item3);
							}
							else
							{
								itemEffect.Init(-1, -1);
								statusEffectScriptableObject.StatusEffectData.SetAdditionalName("-1", "-1");
							}
						}
					}
					else
					{
						statusEffectScriptableObject.StatusEffectData.SetAdditionalName(tuple.Item2, tuple.Item3);
					}
					statusEffectController.ApplyStatus(statusEffectController.PlayerController, statusEffectScriptableObject, executeEvent: false);
				}
			}
			foreach (KeyValuePair<string, StatusEffectController.StatusEffect> item5 in statusEffectController.StatusEffectsList.ToList())
			{
				if (!hashSet.Contains(item5.Key) && item5.Value.statusEffectScriptableObject.InfiniteDuration)
				{
					statusEffectController.ClearStatus(item5.Key, executeEvent: false);
				}
			}
		}
		finally
		{
			_applyingFromNetwork = false;
		}
		static (string statusName, string itemId, string uniqueId) SplitKey(string keyName)
		{
			string[] array = keyName.Split("_");
			string item = array[0];
			string item2 = "";
			string item3 = "";
			if (array.Length >= 3)
			{
				item2 = array[1];
				item3 = array[2];
			}
			return (statusName: item, itemId: item2, uniqueId: item3);
		}
	}

	private void OnAddedStatusEffectAction(StatusEffectController.StatusEffect statusEffect)
	{
		if (!_applyingFromNetwork)
		{
			StatusEffectData statusEffectData = statusEffect.statusEffectScriptableObject.StatusEffectData;
			if (NetworkGameManager.Instance.isServer)
			{
				SetStatusEffect(statusEffectData.Name);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_SetStatusEffect(statusEffectData.Name);
			}
		}
	}

	private void SetStatusEffect(string statusName)
	{
		if (!_statusEffectData.Contains(statusName))
		{
			_statusEffectData.Add(statusName);
		}
	}

	private void OnRemoveStatusEffectAction(string statusName)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				RemoveStatusEffect(statusName);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_RemoveStatusEffect(statusName);
			}
		}
	}

	private void RemoveStatusEffect(string statusName)
	{
		_statusEffectData.Remove(statusName);
	}

	private void OnSyncStatusEffectAction()
	{
		SyncNetworkToLocal();
	}

	private void SwapStatusEffect(string oldName, string newName)
	{
		RemoveStatusEffect(oldName);
		SetStatusEffect(newName);
	}

	private void OnSwapStatusEffectEvent(string oldName, string newName)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SwapStatusEffect(oldName, newName);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_ReplaceStatusEffect(oldName, newName);
			}
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetStatusEffect(string statusName)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_SetStatusEffect(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(statusName) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_SetStatusEffect(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, statusName) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetStatusEffect(statusName);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_RemoveStatusEffect(string statusName)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_RemoveStatusEffect(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(statusName) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_RemoveStatusEffect(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, statusName) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		RemoveStatusEffect(statusName);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_ReplaceStatusEffect(string oldStatusName, string newStatusName)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_ReplaceStatusEffect(System.String,System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(oldStatusName) + 3) & -4;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(newStatusName) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Effects.StatusEffectsScripts.StatusEffectControllerNetwork::Rpc_ReplaceStatusEffect(System.String,System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, oldStatusName) + 3) & -4) + num2;
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, newStatusName) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SwapStatusEffect(oldStatusName, newStatusName);
	}

	public void SetReference(bool withDebugUi)
	{
		_statusEffectController = GetComponent<StatusEffectController>();
		_statusEffectController?.SetReference(withDebugUi);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		NetworkBehaviourUtils.InitializeNetworkList(_statusEffectData, __statusEffectData, "_statusEffectData");
	}

	public override void CopyStateToBackingFields()
	{
		NetworkBehaviourUtils.CopyFromNetworkList(_statusEffectData, ref __statusEffectData);
	}

	[NetworkRpcWeavedInvoker(1, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetStatusEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((StatusEffectControllerNetwork)behaviour).Rpc_SetStatusEffect(result);
	}

	[NetworkRpcWeavedInvoker(2, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_RemoveStatusEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((StatusEffectControllerNetwork)behaviour).Rpc_RemoveStatusEffect(result);
	}

	[NetworkRpcWeavedInvoker(3, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_ReplaceStatusEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result2) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((StatusEffectControllerNetwork)behaviour).Rpc_ReplaceStatusEffect(result, result2);
	}
}
