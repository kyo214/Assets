using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

[NetworkBehaviourWeaved(75)]
public class ItemBoxNetwork : NetworkBehaviour
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	[NetworkStructWeaved(5)]
	public struct InventoryObjectNetwork : INetworkStruct
	{
		[FieldOffset(0)]
		public int ID;

		[FieldOffset(4)]
		public int Amount;

		[FieldOffset(8)]
		public int Ammo;

		[FieldOffset(12)]
		public int Durability;

		[FieldOffset(16)]
		public int MaxItemInSlot;
	}

	[SerializeField]
	[DefaultForProperty("arrItem", 0, 75)]
	private InventoryObjectNetwork[] _arrItem = new InventoryObjectNetwork[15];

	public bool _initalized;

	public static ItemBoxNetwork instance;

	private static Changed<ItemBoxNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<ItemBoxNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<ItemBoxNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked]
	[Capacity(15)]
	[NetworkedWeaved(0, 75)]
	public unsafe NetworkArray<InventoryObjectNetwork> arrItem
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ItemBoxNetwork.arrItem. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<InventoryObjectNetwork>((byte*)Ptr + 0, 15, ReaderWriter_0040ItemBoxNetwork__InventoryObjectNetwork.GetInstance());
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (!_initalized && NetworkGameManager.Instance.photonNetworking._runner.IsRunning && NetworkGameManager.Instance.photonNetworking._runner.IsServer)
		{
			_initalized = true;
			Invoke("InitItemBox", 1f);
		}
	}

	public string GetItemType(int id)
	{
		return DataManager.Instance.GetItemType(id);
	}

	public void InitItemBox()
	{
		for (int i = 0; i < arrItem.Length; i++)
		{
			InventoryObjectNetwork value = arrItem.Get(i);
			value.ID = -1;
			value.Amount = 0;
			value.MaxItemInSlot = 0;
			arrItem.Set(i, value);
		}
		for (int j = 0; j < 4; j++)
		{
			InventoryObjectNetwork value2 = arrItem.Get(j);
			value2.ID = 201;
			value2.Amount = 1;
			value2.MaxItemInSlot = 1;
			arrItem.Set(j, value2);
		}
	}

	public void AddItem(InventoryObjectNetwork newObject)
	{
		for (int i = 0; i < arrItem.Length; i++)
		{
			InventoryObjectNetwork value = arrItem.Get(i);
			if (value.ID == -1)
			{
				value.ID = newObject.ID;
				value.Amount = newObject.Amount;
				value.Ammo = newObject.Ammo;
				value.MaxItemInSlot = newObject.MaxItemInSlot;
				arrItem.Set(i, value);
				break;
			}
		}
	}

	public void RemoveItem(int idx)
	{
		if (arrItem.Get(idx).ID != -1)
		{
			for (int i = idx; i < arrItem.Length - 1; i++)
			{
				InventoryObjectNetwork value = arrItem.Get(i);
				InventoryObjectNetwork inventoryObjectNetwork = arrItem.Get(i + 1);
				value.ID = inventoryObjectNetwork.ID;
				value.Amount = inventoryObjectNetwork.Amount;
				value.Ammo = inventoryObjectNetwork.Ammo;
				value.MaxItemInSlot = inventoryObjectNetwork.MaxItemInSlot;
				arrItem.Set(i, value);
			}
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		NetworkBehaviourUtils.InitializeNetworkArray(arrItem, _arrItem, "arrItem");
	}

	public override void CopyStateToBackingFields()
	{
		NetworkBehaviourUtils.CopyFromNetworkArray(arrItem, ref _arrItem);
	}
}
