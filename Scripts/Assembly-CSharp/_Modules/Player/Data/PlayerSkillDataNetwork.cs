using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.CodeGen;
using Toked.Skill;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Modules.Player.Data;

[NetworkBehaviourWeaved(2469)]
public class PlayerSkillDataNetwork : NetworkBehaviour
{
	[SerializeField]
	private PlayerSkillData _playerSkillData;

	[SerializeField]
	[DefaultForProperty("_skillLearnData", 0, 303)]
	private int[] __skillLearnData;

	[SerializeField]
	[DefaultForProperty("_skillPoint", 303, 1)]
	private int __skillPoint;

	[SerializeField]
	[DefaultForProperty("_perkId", 304, 18)]
	private string __perkId;

	[SerializeField]
	[DefaultForProperty("_additionalPerkSkillList", 322, 2147)]
	private NetworkString<_64>[] __additionalPerkSkillList;

	private PlayerController _playerController;

	private static Changed<PlayerSkillDataNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<PlayerSkillDataNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<PlayerSkillDataNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache__perkId;

	public PlayerSkillData PlayerSkillData => _playerSkillData;

	[UnitySerializeField]
	[Networked(OnChanged = "OnSkillLearnChanged")]
	[Capacity(100)]
	[NetworkedWeaved(0, 303)]
	private unsafe NetworkLinkedList<int> _skillLearnData
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._skillLearnData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkLinkedList<int>((byte*)Ptr + 0, 100, ReaderWriter_0040System_Int32.GetInstance());
		}
	}

	[UnitySerializeField]
	[Networked]
	[NetworkedWeaved(303, 1)]
	private unsafe int _skillPoint
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._skillPoint. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[303];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._skillPoint. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[303] = value;
		}
	}

	[UnitySerializeField]
	[Networked(OnChanged = "OnPerkChanged")]
	[NetworkedWeaved(304, 18)]
	private unsafe string _perkId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._perkId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 304, 16, ref cache__perkId);
			return cache__perkId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._perkId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 304, 16, value, ref cache__perkId);
		}
	}

	[UnitySerializeField]
	[Networked]
	[Capacity(32)]
	[NetworkedWeaved(322, 2147)]
	private unsafe NetworkLinkedList<NetworkString<_64>> _additionalPerkSkillList
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerSkillDataNetwork._additionalPerkSkillList. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkLinkedList<NetworkString<_64>>((byte*)Ptr + 1288, 32, ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__64_003E.GetInstance());
		}
	}

	public PlayerController PlayerController => _playerController ?? (_playerController = GetComponentInParent<PlayerController>());

	public static event Action<PlayerController, string> OnPerkNetworkChangedEvent;

	public static event Action<string> OnPerkDestroyEvent;

	public void SyncToLocalVariable()
	{
		_playerSkillData.ResetSkillLearnData(executeEvent: false);
		_playerSkillData.SetAdditionalPerkSkill(ConvertAdditionalPerkSkillList());
		_playerSkillData.SetSkillLearn(ConvertDataToIdSkill());
		_playerSkillData.SetSkillPoint(_skillPoint, executeEvent: false);
		_playerSkillData.SetPerk(_perkId, executeEvent: false);
	}

	public NetworkLinkedList<int> GetSkillLearnData()
	{
		return _skillLearnData;
	}

	public string GetPerkId()
	{
		return _perkId;
	}

	public List<string> ConvertAdditionalPerkSkillList()
	{
		List<string> list = new List<string>();
		foreach (NetworkString<_64> additionalPerkSkill in _additionalPerkSkillList)
		{
			list.Add(additionalPerkSkill.ToString());
		}
		return list.Distinct().ToList();
	}

	public List<string> ConvertDataToIdSkill()
	{
		List<string> list = new List<string>();
		SkillLibraryScriptableObject skillLibraryScriptableObject = DataManager.Instance.Get<SkillLibraryScriptableObject>();
		if ((bool)skillLibraryScriptableObject)
		{
			foreach (int skillLearnDatum in _skillLearnData)
			{
				SkillScriptableObject dataByIndex = skillLibraryScriptableObject.GetDataByIndex(skillLearnDatum);
				if (!list.Contains(dataByIndex.ID))
				{
					list.Add(dataByIndex.ID);
				}
			}
		}
		return list;
	}

	private void OnEnable()
	{
		_playerSkillData.OnChangedSkillLearnEvent += OnChangedSkillLearnAction;
		_playerSkillData.OnResetSkillLearnEvent += OnResetSkillLearnAction;
		_playerSkillData.OnChangedSkillPointEvent += OnChangedSkillPointAction;
		_playerSkillData.OnResetSkillPointEvent += OnResetSkillPointAction;
		_playerSkillData.OnChangedPerkEvent += OnChangedPerksIdAction;
		_playerSkillData.OnChangedAdditionalPerkSkillEvent += OnChangedAdditionalPerkSkillAction;
		_playerSkillData.OnSetAdditionalPerkSkillEvent += OnSetAdditionalPerkSkillAction;
	}

	private void OnDisable()
	{
		_playerSkillData.OnChangedSkillLearnEvent -= OnChangedSkillLearnAction;
		_playerSkillData.OnResetSkillLearnEvent -= OnResetSkillLearnAction;
		_playerSkillData.OnChangedSkillPointEvent -= OnChangedSkillPointAction;
		_playerSkillData.OnResetSkillPointEvent -= OnResetSkillPointAction;
		_playerSkillData.OnChangedPerkEvent -= OnChangedPerksIdAction;
		_playerSkillData.OnChangedAdditionalPerkSkillEvent -= OnChangedAdditionalPerkSkillAction;
		_playerSkillData.OnSetAdditionalPerkSkillEvent -= OnSetAdditionalPerkSkillAction;
	}

	private void OnDestroy()
	{
		OnPerkDestroyEvent?.Invoke(_playerSkillData?.PerkId);
	}

	private void SetPerk(string perkId)
	{
		_perkId = perkId;
	}

	private void OnChangedPerksIdAction(string id)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetPerk(id);
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetPerk(id);
		}
	}

	private void SetAdditionalPerkSkill(string perkList)
	{
		if (!_additionalPerkSkillList.Contains(perkList))
		{
			_additionalPerkSkillList.Add(perkList);
		}
	}

	private void OnChangedAdditionalPerkSkillAction(string perkList)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetAdditionalPerkSkill(perkList);
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetAdditionalPerkSkill(perkList);
		}
	}

	private void OnSetAdditionalPerkSkillAction(List<string> perkList)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			foreach (string perk in perkList)
			{
				SetAdditionalPerkSkill(perk);
			}
			return;
		}
		if (!Object.HasInputAuthority)
		{
			return;
		}
		foreach (string perk2 in perkList)
		{
			Rpc_SetAdditionalPerkSkill(perk2);
		}
	}

	[Preserve]
	private static void OnPerkChanged(Changed<PlayerSkillDataNetwork> changed)
	{
		PlayerController player = changed.Behaviour.PlayerController;
		string skillDataNetwork = changed.Behaviour.GetPerkId();
		Debug.Log(player.network.GetPlayerName() + "  " + player.network.GetIDX() + " Choose " + skillDataNetwork);
		if (string.IsNullOrWhiteSpace(player.data.SkillData.PerkId))
		{
			UniTaskUtil.DelayedCall(changed.Behaviour, 1f, OnChangeAction).Forget();
			OnPerkNetworkChangedEvent?.Invoke(player, skillDataNetwork);
		}
		void OnChangeAction()
		{
			player.data.PlayerSkillNetworkData.SyncToLocalVariable();
			DataManager.Instance.Get<PerkLibraryScriptableObject>()?.GetData(skillDataNetwork)?.ExecuteEffectSkill(player);
			UIGameManager.Instance?.SetPerksUIInfo(changed.Behaviour.PlayerController);
		}
	}

	[Preserve]
	private static void OnSkillLearnChanged(Changed<PlayerSkillDataNetwork> changed)
	{
		PlayerController playerController = changed.Behaviour.PlayerController;
		NetworkLinkedList<int> skillLearnData = changed.Behaviour.GetSkillLearnData();
		SkillLibraryScriptableObject skillLibraryScriptableObject = DataManager.Instance.Get<SkillLibraryScriptableObject>();
		foreach (int item in skillLearnData)
		{
			SkillScriptableObject skillScriptableObject = skillLibraryScriptableObject?.GetDataByIndex(item);
			if ((bool)skillScriptableObject && !playerController.data.CheckSkillLearn(skillScriptableObject.ID))
			{
				skillScriptableObject.ExecuteEffectSkill(playerController);
			}
		}
		UIGameManager.Instance?.SetSkillUIInfo(changed.Behaviour.PlayerController);
	}

	private void SetSkillPoint(int point)
	{
		_skillPoint = point;
	}

	private void OnChangedSkillPointAction(int point)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetSkillPoint(point);
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetSkillPoint(point);
		}
	}

	private void OnResetSkillPointAction(int point)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetSkillPoint(point);
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetSkillPoint(point);
		}
	}

	private void OnChangedSkillLearnAction(string idSkill)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetSkillLearn(idSkill);
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetSkillLearn(idSkill);
		}
	}

	private void OnResetSkillLearnAction()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			ResetSkillLearn();
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_ResetSkillLearn();
		}
	}

	private void SetSkillLearn(string idSkill)
	{
		SkillScriptableObject skillScriptableObject = DataManager.Instance.Get<SkillLibraryScriptableObject>()?.GetData(idSkill);
		if ((bool)skillScriptableObject && !_skillLearnData.Contains(skillScriptableObject.SortIndex))
		{
			_skillLearnData.Add(skillScriptableObject.SortIndex);
		}
	}

	private void ResetSkillLearn()
	{
		_skillLearnData.Clear();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_ResetSkillLearn()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_ResetSkillLearn()", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_ResetSkillLearn()", num);
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
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		ResetSkillLearn();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetSkillLearn(string idSkill)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetSkillLearn(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(idSkill) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetSkillLearn(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, idSkill) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetSkillLearn(idSkill);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetSkillPoint(int point)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetSkillPoint(System.Int32)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetSkillPoint(System.Int32)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
					*(int*)(data + num2) = point;
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
		SetSkillPoint(point);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetPerk(string id)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetPerk(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(id) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetPerk(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 4), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, id) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetPerk(id);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetAdditionalPerkSkill(string perkList)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetAdditionalPerkSkill(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(perkList) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.Player.Data.PlayerSkillDataNetwork::Rpc_SetAdditionalPerkSkill(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 5), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, perkList) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetAdditionalPerkSkill(perkList);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		NetworkBehaviourUtils.InitializeNetworkList(_skillLearnData, __skillLearnData, "_skillLearnData");
		_skillPoint = __skillPoint;
		_perkId = __perkId;
		NetworkBehaviourUtils.InitializeNetworkList(_additionalPerkSkillList, __additionalPerkSkillList, "_additionalPerkSkillList");
	}

	public override void CopyStateToBackingFields()
	{
		NetworkBehaviourUtils.CopyFromNetworkList(_skillLearnData, ref __skillLearnData);
		__skillPoint = _skillPoint;
		__perkId = _perkId;
		NetworkBehaviourUtils.CopyFromNetworkList(_additionalPerkSkillList, ref __additionalPerkSkillList);
	}

	[NetworkRpcWeavedInvoker(1, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_ResetSkillLearn_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerSkillDataNetwork)behaviour).Rpc_ResetSkillLearn();
	}

	[NetworkRpcWeavedInvoker(2, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetSkillLearn_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerSkillDataNetwork)behaviour).Rpc_SetSkillLearn(result);
	}

	[NetworkRpcWeavedInvoker(3, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetSkillPoint_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int point = num2;
		behaviour.InvokeRpc = true;
		((PlayerSkillDataNetwork)behaviour).Rpc_SetSkillPoint(point);
	}

	[NetworkRpcWeavedInvoker(4, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetPerk_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerSkillDataNetwork)behaviour).Rpc_SetPerk(result);
	}

	[NetworkRpcWeavedInvoker(5, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetAdditionalPerkSkill_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerSkillDataNetwork)behaviour).Rpc_SetAdditionalPerkSkill(result);
	}
}
