using System;
using System.Collections.Generic;
using Fusion;
using Fusion.CodeGen;
using Toked.Crafting;
using UnityEngine;
using UnityEngine.Scripting;

namespace Toked.Inventory;

[NetworkBehaviourWeaved(71)]
public class MaterialInventoryNetwork : NetworkBehaviour
{
	[SerializeField]
	private MaterialInventory _materialInventory;

	private static Changed<MaterialInventoryNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<MaterialInventoryNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<MaterialInventoryNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[SerializeField]
	[DefaultForProperty("_materialInventoryDic", 0, 71)]
	private SerializableDictionary<int, int> __materialInventoryDic;

	public MaterialInventory MaterialInventory => _materialInventory;

	[Networked]
	[Capacity(8)]
	[UnitySerializeField]
	[NetworkedWeaved(0, 71)]
	private unsafe NetworkDictionary<int, int> _materialInventoryDic
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MaterialInventoryNetwork._materialInventoryDic. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkDictionary<int, int>((int*)((byte*)Ptr + 0), 17, ReaderWriter_0040System_Int32.GetInstance(), ReaderWriter_0040System_Int32.GetInstance());
		}
	}

	public NetworkDictionary<int, int> GetMaterialInventoryDic()
	{
		return _materialInventoryDic;
	}

	public int GetMaterialAmount(int key)
	{
		_materialInventoryDic.TryGet(key, out var value);
		return value;
	}

	public Dictionary<string, MaterialInventoryData> GetMaterialData()
	{
		Dictionary<string, MaterialInventoryData> dictionary = new Dictionary<string, MaterialInventoryData>();
		foreach (KeyValuePair<int, int> item in _materialInventoryDic)
		{
			CraftMaterialScriptableObject materialSoByInventoryItemKey = MaterialInventory.GetMaterialSoByInventoryItemKey(item.Key);
			if (!(materialSoByInventoryItemKey == null))
			{
				if (dictionary.TryGetValue(materialSoByInventoryItemKey.ID, out var value))
				{
					value.Amount += item.Value;
				}
				else
				{
					dictionary.Add(materialSoByInventoryItemKey.ID, new MaterialInventoryData(materialSoByInventoryItemKey, item.Value));
				}
			}
		}
		return dictionary;
	}

	private void OnEnable()
	{
		_materialInventory.OnChangedMaterialEvent += OnChangedMaterialAction;
		_materialInventory.OnResetMaterialEvent += OnResetMaterialAction;
	}

	private void OnDisable()
	{
		_materialInventory.OnChangedMaterialEvent -= OnChangedMaterialAction;
		_materialInventory.OnResetMaterialEvent -= OnResetMaterialAction;
	}

	private void OnChangedMaterialAction(CraftMaterialScriptableObject so, int amount)
	{
		int itemInventoryId = so.ItemInventoryId;
		if (NetworkGameManager.Instance.isServer)
		{
			SetMaterialNetwork((short)itemInventoryId, (short)amount);
		}
		else
		{
			Rpc_SetMaterialNetwork((short)itemInventoryId, (short)amount);
		}
	}

	private void OnResetMaterialAction()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			ResetData();
		}
		else
		{
			Rpc_ResetData();
		}
	}

	private void ResetData()
	{
		_materialInventoryDic.Clear();
	}

	public void SyncToLocalData()
	{
		_materialInventory.MaterialInventoryDic.Clear();
		foreach (KeyValuePair<int, int> item in _materialInventoryDic)
		{
			_materialInventory.SetMaterial(item.Key, item.Value);
		}
	}

	private void SetMaterialNetwork(short key, short amount)
	{
		if (_materialInventoryDic.ContainsKey(key))
		{
			_materialInventoryDic.Set(key, amount);
		}
		else
		{
			_materialInventoryDic.Add(key, amount);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_SnycToLocalData()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_SnycToLocalData()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_SnycToLocalData()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		SyncToLocalData();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_SetMaterialNetwork(short key, short amount)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_SetMaterialNetwork(System.Int16,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_SetMaterialNetwork(System.Int16,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
				*(short*)(data + num2) = key;
				num2 += 5 & -4;
				*(short*)(data + num2) = amount;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		SetMaterialNetwork(key, amount);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void Rpc_ResetData()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_ResetData()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Toked.Inventory.MaterialInventoryNetwork::Rpc_ResetData()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ResetData();
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		NetworkBehaviourUtils.InitializeNetworkDictionary(_materialInventoryDic, __materialInventoryDic, "_materialInventoryDic");
	}

	public override void CopyStateToBackingFields()
	{
		NetworkBehaviourUtils.CopyFromNetworkDictionary(_materialInventoryDic, ref __materialInventoryDic);
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SnycToLocalData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((MaterialInventoryNetwork)behaviour).Rpc_SnycToLocalData();
	}

	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SetMaterialNetwork_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short key = num2;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short amount = num3;
		behaviour.InvokeRpc = true;
		((MaterialInventoryNetwork)behaviour).Rpc_SetMaterialNetwork(key, amount);
	}

	[NetworkRpcWeavedInvoker(3, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_ResetData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((MaterialInventoryNetwork)behaviour).Rpc_ResetData();
	}
}
