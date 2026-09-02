using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Modules.CharacterSkin.Scripts;

[NetworkBehaviourWeaved(73)]
public class CharacterSkinPhotonNetwork : NetworkBehaviour
{
	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	[DefaultForProperty("characterHeadSkinId", 0, 18)]
	private string _characterHeadSkinId;

	[SerializeField]
	[DefaultForProperty("characterBodySkinId", 18, 18)]
	private string _characterBodySkinId;

	[SerializeField]
	[DefaultForProperty("characterColorPaletteSkinId", 36, 18)]
	private string _characterColorPaletteSkinId;

	[SerializeField]
	[DefaultForProperty("characterSkinColorId", 54, 18)]
	private string _characterSkinColorId;

	[SerializeField]
	[DefaultForProperty("characterGenderSkinId", 72, 1)]
	private int _characterGenderSkinId;

	private bool _applyingFromNetwork;

	private static Changed<CharacterSkinPhotonNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<CharacterSkinPhotonNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<CharacterSkinPhotonNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache_characterHeadSkinId;

	private string cache_characterBodySkinId;

	private string cache_characterColorPaletteSkinId;

	private string cache_characterSkinColorId;

	[Networked(OnChanged = "OnHeadSkinIdChanged")]
	[NetworkedWeaved(0, 18)]
	public unsafe string characterHeadSkinId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterHeadSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash((int*)((byte*)Ptr + 0), 16, ref cache_characterHeadSkinId);
			return cache_characterHeadSkinId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterHeadSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash((int*)((byte*)Ptr + 0), 16, value, ref cache_characterHeadSkinId);
		}
	}

	[Networked(OnChanged = "OnBodySkinIdChanged")]
	[NetworkedWeaved(18, 18)]
	public unsafe string characterBodySkinId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterBodySkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 18, 16, ref cache_characterBodySkinId);
			return cache_characterBodySkinId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterBodySkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 18, 16, value, ref cache_characterBodySkinId);
		}
	}

	[Networked(OnChanged = "OnMaterialSkinIdChanged")]
	[NetworkedWeaved(36, 18)]
	public unsafe string characterColorPaletteSkinId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterColorPaletteSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 36, 16, ref cache_characterColorPaletteSkinId);
			return cache_characterColorPaletteSkinId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterColorPaletteSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 36, 16, value, ref cache_characterColorPaletteSkinId);
		}
	}

	[Networked(OnChanged = "OnSkinColorIdChanged")]
	[NetworkedWeaved(54, 18)]
	public unsafe string characterSkinColorId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterSkinColorId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 54, 16, ref cache_characterSkinColorId);
			return cache_characterSkinColorId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterSkinColorId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 54, 16, value, ref cache_characterSkinColorId);
		}
	}

	[Networked]
	[NetworkedWeaved(72, 1)]
	public unsafe int characterGenderSkinId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterGenderSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[72];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CharacterSkinPhotonNetwork.characterGenderSkinId. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[72] = value;
		}
	}

	[Preserve]
	private static void OnHeadSkinIdChanged(Changed<CharacterSkinPhotonNetwork> changed)
	{
		changed.Behaviour.SyncHeadDataNetworkToLocal(applyingFromNetwork: true);
	}

	[Preserve]
	private static void OnBodySkinIdChanged(Changed<CharacterSkinPhotonNetwork> changed)
	{
		changed.Behaviour.SyncBodyDataNetworkToLocal(applyingFromNetwork: true);
	}

	[Preserve]
	private static void OnMaterialSkinIdChanged(Changed<CharacterSkinPhotonNetwork> changed)
	{
		changed.Behaviour.SyncColorDataNetworkToLocal(applyingFromNetwork: true);
	}

	[Preserve]
	private static void OnSkinColorIdChanged(Changed<CharacterSkinPhotonNetwork> changed)
	{
		changed.Behaviour.SyncSkinColorDataNetworkToLocal(applyingFromNetwork: true);
	}

	public void SyncSkinDataNetworkToLocal(bool applyingFromNetwork = false)
	{
		SyncHeadDataNetworkToLocal(applyingFromNetwork);
		SyncBodyDataNetworkToLocal(applyingFromNetwork);
		SyncColorDataNetworkToLocal(applyingFromNetwork);
		SyncSkinColorDataNetworkToLocal(applyingFromNetwork);
	}

	private void OnEnable()
	{
		PlayerSkinData playerSkinData = _playerController.data.PlayerSkinData;
		playerSkinData.OnHeadDataSkinChangedEvents += OnChangeHeadSkin;
		playerSkinData.OnBodyDataSkinChangedEvents += OnChangeBodySkin;
		playerSkinData.OnMaterialDataSkinChangedEvents += OnChangeMaterialSkin;
		playerSkinData.OnSkinColorSkinChangedEvents += OnChangeSkinColor;
	}

	private void OnDisable()
	{
		PlayerSkinData playerSkinData = _playerController.data.PlayerSkinData;
		playerSkinData.OnHeadDataSkinChangedEvents -= OnChangeHeadSkin;
		playerSkinData.OnBodyDataSkinChangedEvents -= OnChangeBodySkin;
		playerSkinData.OnMaterialDataSkinChangedEvents -= OnChangeMaterialSkin;
		playerSkinData.OnSkinColorSkinChangedEvents -= OnChangeSkinColor;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private unsafe void Rpc_SetSkinData(string characterSkinId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetSkinData(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(characterSkinId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetSkinData(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, characterSkinId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		string text = (characterBodySkinId = characterSkinId);
		characterHeadSkinId = text;
		CharacterSkinData heroSkinById = SkinManager.Instance.GetHeroSkinById(characterSkinId);
		characterColorPaletteSkinId = heroSkinById.skinColorPaletteSo.CharacterColorSkinId;
		_playerController.data.PlayerSkinData.SetSkinData(heroSkinById);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private unsafe void Rpc_SetHeadSkinData(string characterSkinId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetHeadSkinData(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(characterSkinId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetHeadSkinData(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, characterSkinId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetHeadData(characterSkinId);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private unsafe void Rpc_SetBodySkinData(string characterSkinId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetBodySkinData(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(characterSkinId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetBodySkinData(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, characterSkinId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetBodyData(characterSkinId);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private unsafe void Rpc_SetMaterialSkinData(string skinColorId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetMaterialSkinData(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(skinColorId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetMaterialSkinData(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 4), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, skinColorId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetColorData(skinColorId);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private unsafe void Rpc_SetSkinColorData(string skinColorId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetSkinColorData(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(skinColorId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void _Modules.CharacterSkin.Scripts.CharacterSkinPhotonNetwork::Rpc_SetSkinColorData(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 5), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, skinColorId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetSkinColorData(skinColorId);
	}

	private void SetHeadData(string characterSkinId)
	{
		characterHeadSkinId = characterSkinId;
	}

	public void SyncHeadDataNetworkToLocal(bool applyingFromNetwork = false)
	{
		_applyingFromNetwork = applyingFromNetwork;
		try
		{
			CharacterSkinData heroSkinById = SkinManager.Instance.GetHeroSkinById(characterHeadSkinId);
			if (heroSkinById != null)
			{
				_playerController.data.PlayerSkinData.SetHeadSkinData(heroSkinById, executeEvent: false);
				SetGenderData(heroSkinById);
			}
		}
		finally
		{
			_applyingFromNetwork = false;
		}
	}

	private void SetBodyData(string characterSkinId)
	{
		characterBodySkinId = characterSkinId;
	}

	public void SyncBodyDataNetworkToLocal(bool applyingFromNetwork = false)
	{
		_applyingFromNetwork = applyingFromNetwork;
		try
		{
			CharacterSkinData heroSkinBodyById = SkinManager.Instance.GetHeroSkinBodyById(characterBodySkinId);
			if (heroSkinBodyById != null)
			{
				_playerController.data.PlayerSkinData.SetBodySkinData(heroSkinBodyById, executeEvent: false);
			}
		}
		finally
		{
			_applyingFromNetwork = false;
		}
	}

	private void SetColorData(string skinColorId)
	{
		characterColorPaletteSkinId = skinColorId;
	}

	public void SyncColorDataNetworkToLocal(bool applyingFromNetwork = false)
	{
		_applyingFromNetwork = applyingFromNetwork;
		try
		{
			SkinColorPaletteScriptableObject heroColorPaletteById = SkinManager.Instance.GetHeroColorPaletteById(characterColorPaletteSkinId);
			if (heroColorPaletteById != null)
			{
				_playerController.data.PlayerSkinData.SetMaterialSkinData(heroColorPaletteById, executeEvent: false);
			}
		}
		finally
		{
			_applyingFromNetwork = false;
		}
	}

	private void SetSkinColorData(string skinColorId)
	{
		characterSkinColorId = skinColorId;
	}

	public void SyncSkinColorDataNetworkToLocal(bool applyingFromNetwork = false)
	{
		_applyingFromNetwork = applyingFromNetwork;
		try
		{
			SkinColorScriptableObject heroSkinColorSOById = SkinManager.Instance.GetHeroSkinColorSOById(characterSkinColorId);
			if (heroSkinColorSOById != null)
			{
				_playerController.data.PlayerSkinData.SetSkinColorData(heroSkinColorSOById, executeEvent: false);
			}
		}
		finally
		{
			_applyingFromNetwork = false;
		}
	}

	private void SetGenderData(CharacterSkinData characterSkinData)
	{
		characterGenderSkinId = (int)characterSkinData.skinGender;
	}

	private void OnChangeHeadSkin(CharacterSkinData characterSkinData)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SetHeadData(characterSkinData.CharacterSkinId);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_SetHeadSkinData(characterSkinData.CharacterSkinId);
			}
		}
	}

	private void OnChangeBodySkin(CharacterSkinData characterSkinData)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SetBodyData(characterSkinData.CharacterSkinId);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_SetBodySkinData(characterSkinData.CharacterSkinId);
			}
		}
	}

	private void OnChangeMaterialSkin(SkinColorPaletteScriptableObject characterSkinData)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SetColorData(characterSkinData.CharacterColorSkinId);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_SetMaterialSkinData(characterSkinData.CharacterColorSkinId);
			}
		}
	}

	private void OnChangeSkinColor(SkinColorScriptableObject characterSkinData)
	{
		if (!_applyingFromNetwork)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SetSkinColorData(characterSkinData.SkinColorId);
			}
			else if (Object.HasInputAuthority)
			{
				Rpc_SetSkinColorData(characterSkinData.SkinColorId);
			}
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		characterHeadSkinId = _characterHeadSkinId;
		characterBodySkinId = _characterBodySkinId;
		characterColorPaletteSkinId = _characterColorPaletteSkinId;
		characterSkinColorId = _characterSkinColorId;
		characterGenderSkinId = _characterGenderSkinId;
	}

	public override void CopyStateToBackingFields()
	{
		_characterHeadSkinId = characterHeadSkinId;
		_characterBodySkinId = characterBodySkinId;
		_characterColorPaletteSkinId = characterColorPaletteSkinId;
		_characterSkinColorId = characterSkinColorId;
		_characterGenderSkinId = characterGenderSkinId;
	}

	[NetworkRpcWeavedInvoker(1, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetSkinData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CharacterSkinPhotonNetwork)behaviour).Rpc_SetSkinData(result);
	}

	[NetworkRpcWeavedInvoker(2, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetHeadSkinData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CharacterSkinPhotonNetwork)behaviour).Rpc_SetHeadSkinData(result);
	}

	[NetworkRpcWeavedInvoker(3, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetBodySkinData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CharacterSkinPhotonNetwork)behaviour).Rpc_SetBodySkinData(result);
	}

	[NetworkRpcWeavedInvoker(4, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetMaterialSkinData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CharacterSkinPhotonNetwork)behaviour).Rpc_SetMaterialSkinData(result);
	}

	[NetworkRpcWeavedInvoker(5, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetSkinColorData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((CharacterSkinPhotonNetwork)behaviour).Rpc_SetSkinColorData(result);
	}
}
