using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using I2.Loc;
using Steamworks;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Inventory;
using UnityEngine;
using UnityEngine.Scripting;
using _Modules.Achievement.Scripts;

[NetworkBehaviourWeaved(106)]
public class PlayerPhotonNetwork : NetworkBehaviour
{
	public PlayerNetwork playerNetwork;

	[SerializeField]
	[DefaultForProperty("disconnected", 0, 1)]
	private NetworkBool _disconnected;

	[SerializeField]
	[DefaultForProperty("isQuitGame", 1, 1)]
	private NetworkBool _isQuitGame;

	[SerializeField]
	[DefaultForProperty("isKicked", 2, 1)]
	private NetworkBool _isKicked;

	[SerializeField]
	[DefaultForProperty("inGame", 3, 1)]
	private NetworkBool _inGame;

	[SerializeField]
	[DefaultForProperty("IsSurvive", 4, 1)]
	private NetworkBool _IsSurvive;

	[SerializeField]
	[DefaultForProperty("ModeGame", 5, 18)]
	private string _ModeGame;

	[SerializeField]
	[DefaultForProperty("enableControl", 23, 1)]
	private NetworkBool _enableControl;

	[SerializeField]
	[DefaultForProperty("IsDisconnected", 24, 1)]
	private NetworkBool _IsDisconnected;

	[SerializeField]
	[DefaultForProperty("IsDisconnectedOnLobby", 25, 1)]
	private NetworkBool _IsDisconnectedOnLobby;

	[SerializeField]
	[DefaultForProperty("dataInputMove", 26, 1)]
	private byte _dataInputMove;

	[SerializeField]
	[DefaultForProperty("dataInputClick", 27, 1)]
	private short _dataInputClick;

	[SerializeField]
	[DefaultForProperty("voiceChatName", 28, 18)]
	private string _voiceChatName;

	[SerializeField]
	[DefaultForProperty("targetIdxCam", 46, 1)]
	private byte _targetIdxCam;

	[SerializeField]
	[DefaultForProperty("reviveTimer", 47, 1)]
	private byte _reviveTimer;

	[SerializeField]
	[DefaultForProperty("reviveTimerSecond", 48, 1)]
	private byte _reviveTimerSecond;

	[SerializeField]
	[DefaultForProperty("weaponSelect", 49, 1)]
	private byte _weaponSelect;

	[SerializeField]
	[DefaultForProperty("idx", 50, 1)]
	private byte _idx;

	[SerializeField]
	[DefaultForProperty("userUniqueId", 51, 18)]
	private string _userUniqueId;

	[SerializeField]
	[DefaultForProperty("playerName", 69, 18)]
	private string _playerName;

	[SerializeField]
	[DefaultForProperty("health", 87, 1)]
	private short _health;

	[SerializeField]
	[DefaultForProperty("Life", 88, 1)]
	private byte _Life;

	[SerializeField]
	[DefaultForProperty("idWeapon0", 89, 1)]
	private short _idWeapon0;

	[SerializeField]
	[DefaultForProperty("idWeapon1", 90, 1)]
	private short _idWeapon1;

	[SerializeField]
	[DefaultForProperty("godMode", 91, 1)]
	private NetworkBool _godMode;

	[SerializeField]
	[DefaultForProperty("MaxInventorySlot", 92, 1)]
	private byte _MaxInventorySlot;

	[SerializeField]
	[DefaultForProperty("SyncCurrentPosition", 93, 3)]
	private Vector3 _SyncCurrentPosition;

	[SerializeField]
	[DefaultForProperty("MissionVote", 96, 1)]
	private byte _MissionVote;

	[SerializeField]
	[DefaultForProperty("ButtonsPrevious", 97, 1)]
	private NetworkButtons _ButtonsPrevious;

	[SerializeField]
	[DefaultForProperty("IsDialogueOnboardingNPCShowed", 98, 1)]
	private bool _IsDialogueOnboardingNPCShowed;

	[SerializeField]
	[DefaultForProperty("healingValue", 99, 1)]
	private byte _healingValue;

	[SerializeField]
	[DefaultForProperty("IsInteractingPuzzle", 100, 1)]
	private bool _IsInteractingPuzzle;

	[SerializeField]
	[DefaultForProperty("SteamIDUlong", 101, 2)]
	private ulong _SteamIDUlong;

	private static readonly int IsThrowingAnim = Animator.StringToHash("isThrowing");

	[SerializeField]
	[DefaultForProperty("_bonusLootMaterial", 103, 1)]
	private float __bonusLootMaterial;

	[SerializeField]
	[DefaultForProperty("_discountCraft", 104, 1)]
	private float __discountCraft;

	[SerializeField]
	[DefaultForProperty("IsFriendPass", 105, 1)]
	private bool _IsFriendPass;

	private static Changed<PlayerPhotonNetwork> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<PlayerPhotonNetwork> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<PlayerPhotonNetwork> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache_ModeGame;

	private string cache_voiceChatName;

	private string cache_userUniqueId;

	private string cache_playerName;

	[Networked(OnChanged = "OnDisconnectedChanged")]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool disconnected
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.disconnected. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)((byte*)Ptr + 0);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.disconnected. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)((byte*)Ptr + 0) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(1, 1)]
	public unsafe NetworkBool isQuitGame
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.isQuitGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 1);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.isQuitGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 1) = value;
		}
	}

	[Networked(OnChanged = "OnKickedChanged")]
	[NetworkedWeaved(2, 1)]
	public unsafe NetworkBool isKicked
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.isKicked. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 2);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.isKicked. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 2) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(3, 1)]
	public unsafe NetworkBool inGame
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.inGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 3);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.inGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 3) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(4, 1)]
	public unsafe NetworkBool IsSurvive
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsSurvive. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 4);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsSurvive. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 4) = value;
		}
	}

	[Networked(OnChanged = "OnModeGameChanged")]
	[NetworkedWeaved(5, 18)]
	public unsafe string ModeGame
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.ModeGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 5, 16, ref cache_ModeGame);
			return cache_ModeGame;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.ModeGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 5, 16, value, ref cache_ModeGame);
		}
	}

	[Networked(OnChanged = "OnEnableControlChanged")]
	[NetworkedWeaved(23, 1)]
	public unsafe NetworkBool enableControl
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.enableControl. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 23);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.enableControl. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 23) = value;
		}
	}

	[Networked(OnChanged = "OnStatusDisconnectedChanged")]
	[NetworkedWeaved(24, 1)]
	public unsafe NetworkBool IsDisconnected
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDisconnected. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 24);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDisconnected. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 24) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(25, 1)]
	public unsafe NetworkBool IsDisconnectedOnLobby
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDisconnectedOnLobby. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 25);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDisconnectedOnLobby. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 25) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(26, 1)]
	public unsafe byte dataInputMove
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.dataInputMove. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[104];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.dataInputMove. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[104] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnDataInputChanged")]
	[NetworkedWeaved(27, 1)]
	public unsafe short dataInputClick
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.dataInputClick. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[54];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.dataInputClick. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[54] = value;
		}
	}

	[Networked]
	[NetworkedWeaved(28, 18)]
	public unsafe string voiceChatName
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.voiceChatName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 28, 16, ref cache_voiceChatName);
			return cache_voiceChatName;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.voiceChatName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 28, 16, value, ref cache_voiceChatName);
		}
	}

	[Networked]
	[NetworkedWeaved(46, 1)]
	public unsafe byte targetIdxCam
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.targetIdxCam. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[184];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.targetIdxCam. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[184] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(47, 1)]
	public unsafe byte reviveTimer
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.reviveTimer. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[188];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.reviveTimer. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[188] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnReviveTimeChanged")]
	[NetworkedWeaved(48, 1)]
	public unsafe byte reviveTimerSecond
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.reviveTimerSecond. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[192];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.reviveTimerSecond. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[192] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnWeaponSelectChanged")]
	[NetworkedWeaved(49, 1)]
	public unsafe byte weaponSelect
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.weaponSelect. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[196];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.weaponSelect. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[196] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnIdxChanged")]
	[NetworkedWeaved(50, 1)]
	public unsafe byte idx
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idx. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[200];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idx. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[200] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(51, 18)]
	public unsafe string userUniqueId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.userUniqueId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 51, 16, ref cache_userUniqueId);
			return cache_userUniqueId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.userUniqueId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 51, 16, value, ref cache_userUniqueId);
		}
	}

	[Networked(OnChanged = "OnPlayerNameChanged")]
	[NetworkedWeaved(69, 18)]
	public unsafe string playerName
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.playerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 69, 16, ref cache_playerName);
			return cache_playerName;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.playerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 69, 16, value, ref cache_playerName);
		}
	}

	[Networked(OnChanged = "OnHealthChanged")]
	[NetworkedWeaved(87, 1)]
	public unsafe short health
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.health. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[174];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.health. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[174] = value;
		}
	}

	[Networked]
	[NetworkedWeaved(88, 1)]
	public unsafe byte Life
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.Life. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[352];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.Life. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[352] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnWeapon0Changed")]
	[NetworkedWeaved(89, 1)]
	public unsafe short idWeapon0
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idWeapon0. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[178];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idWeapon0. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[178] = value;
		}
	}

	[Networked(OnChanged = "OnWeapon1Changed")]
	[NetworkedWeaved(90, 1)]
	public unsafe short idWeapon1
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idWeapon1. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[180];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.idWeapon1. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[180] = value;
		}
	}

	[Networked(OnChanged = "OnGodModeChanged")]
	[NetworkedWeaved(91, 1)]
	public unsafe NetworkBool godMode
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.godMode. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 91);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.godMode. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 91) = value;
		}
	}

	[Networked(OnChanged = "OnMaxInventorySlot")]
	[NetworkedWeaved(92, 1)]
	public unsafe byte MaxInventorySlot
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.MaxInventorySlot. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[368];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.MaxInventorySlot. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[368] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnSyncPositionChanged")]
	[NetworkedWeaved(93, 3)]
	public unsafe Vector3 SyncCurrentPosition
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.SyncCurrentPosition. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3(Ptr + 93, 0.001f);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.SyncCurrentPosition. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3(Ptr + 93, 999.99994f, value);
		}
	}

	[Networked(OnChanged = "OnMissionVote")]
	[NetworkedWeaved(96, 1)]
	public unsafe byte MissionVote
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.MissionVote. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[384];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.MissionVote. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[384] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(97, 1)]
	public unsafe NetworkButtons ButtonsPrevious
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.ButtonsPrevious. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkButtons*)(Ptr + 97);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.ButtonsPrevious. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkButtons*)(Ptr + 97) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(98, 1)]
	public unsafe bool IsDialogueOnboardingNPCShowed
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDialogueOnboardingNPCShowed. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 98);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsDialogueOnboardingNPCShowed. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 98, value);
		}
	}

	[Networked]
	[NetworkedWeaved(99, 1)]
	public unsafe byte healingValue
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.healingValue. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[396];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.healingValue. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[396] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnInteractingPuzzle")]
	[NetworkedWeaved(100, 1)]
	public unsafe bool IsInteractingPuzzle
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsInteractingPuzzle. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 100);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsInteractingPuzzle. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 100, value);
		}
	}

	[Networked]
	[NetworkedWeaved(101, 2)]
	public unsafe ulong SteamIDUlong
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.SteamIDUlong. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ulong*)(Ptr + 101);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.SteamIDUlong. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ulong*)(Ptr + 101) = value;
		}
	}

	[UnitySerializeField]
	[Networked]
	[NetworkedWeaved(103, 1)]
	private unsafe float _bonusLootMaterial
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork._bonusLootMaterial. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (float)Ptr[103] * 0.001f;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork._bonusLootMaterial. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteFloat(Ptr + 103, 999.99994f, value);
		}
	}

	[UnitySerializeField]
	[Networked]
	[NetworkedWeaved(104, 1)]
	public unsafe float _discountCraft
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork._discountCraft. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (float)Ptr[104] * 0.001f;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork._discountCraft. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteFloat(Ptr + 104, 999.99994f, value);
		}
	}

	public string PlayerDeviceID { get; set; }

	[Networked]
	[NetworkedWeaved(105, 1)]
	public unsafe bool IsFriendPass
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsFriendPass. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 105);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing PlayerPhotonNetwork.IsFriendPass. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 105, value);
		}
	}

	private void Awake()
	{
		if ((object)playerNetwork == null)
		{
			playerNetwork = GetComponent<PlayerNetwork>();
		}
	}

	private IEnumerator Start()
	{
		if (Object.HasInputAuthority)
		{
			CameraGame.Instance.mainCam.GetComponent<AudioListener>().enabled = false;
			playerNetwork.isLocalPlayer = true;
			playerNetwork.playerController.fov.enabled = true;
			playerNetwork.playerController.fov2.enabled = true;
		}
		if (Object.HasStateAuthority)
		{
			NetworkGameManager.Instance.isServer = true;
		}
		playerNetwork.playerController.Init().Forget();
		if (!GameModes.Instance.isShowingDisclaimer)
		{
			enableControl = true;
		}
		playerNetwork.playerController.enableMoveChar = true;
		yield return new WaitForSeconds(0.5f);
		if (NetworkGameManager.Instance.isServer)
		{
			reviveTimer = 90;
		}
		playerNetwork.playerController.healArea.GetComponent<ItemInteractable>().UniqueID = (short)(10010 + playerNetwork.GetIDX());
		if (SteamManager.Initialized)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SteamIDUlong = SteamClient.SteamId.Value;
			}
			else if (playerNetwork.isLocalPlayer)
			{
				RpcSetSteamID(SteamClient.SteamId.Value);
			}
		}
		if (!NetworkGameManager.Instance.isServer || playerNetwork.isLocalPlayer)
		{
			yield break;
		}
		playerNetwork.ExecSyncMission();
		playerNetwork.SetTriggerInteractableObject();
		playerNetwork.ExecSyncPickableObject();
		playerNetwork.ExecSyncInventory();
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			for (int num = NetworkGameManager.Instance.ListPlayerTempInventory.Count - 1; num >= 0; num--)
			{
				if (NetworkGameManager.Instance.ListPlayerTempInventory[num].DeviceID == playerNetwork.playerPhoton.PlayerDeviceID)
				{
					playerNetwork.playerController.data.arrInventory.Clear();
					playerNetwork.playerController.data.arrInventory = NetworkGameManager.Instance.ListPlayerTempInventory[num].ArrInventory.ToList();
					playerNetwork.ExecSyncInventory(isTargettedLocalPlayer: true, isToAllPlayer: true);
					NetworkGameManager.Instance.ListPlayerTempInventory.RemoveAt(num);
					break;
				}
			}
		}).Forget();
		UniTaskUtil.DelayedCall(this, 3f, () =>
		{
			playerNetwork.ExecSyncMaxHealth();
			playerNetwork.ExecSyncMaxStamina();
		}).Forget();
	}

	private void OnEnable()
	{
		MaterialInventoryManager materialInventoryManager = playerNetwork.playerController.data.MaterialInventoryManager;
		materialInventoryManager.OnBonusLootMaterialChangedEvent = (Action<float>)Delegate.Combine(materialInventoryManager.OnBonusLootMaterialChangedEvent, new Action<float>(SetBonusLootMaterial));
		MaterialInventoryManager materialInventoryManager2 = playerNetwork.playerController.data.MaterialInventoryManager;
		materialInventoryManager2.OnDiscountCraftChangedEvent = (Action<float>)Delegate.Combine(materialInventoryManager2.OnDiscountCraftChangedEvent, new Action<float>(SetDiscountCraft));
	}

	private void OnDisable()
	{
		MaterialInventoryManager materialInventoryManager = playerNetwork.playerController.data.MaterialInventoryManager;
		materialInventoryManager.OnBonusLootMaterialChangedEvent = (Action<float>)Delegate.Remove(materialInventoryManager.OnBonusLootMaterialChangedEvent, new Action<float>(SetBonusLootMaterial));
		MaterialInventoryManager materialInventoryManager2 = playerNetwork.playerController.data.MaterialInventoryManager;
		materialInventoryManager2.OnDiscountCraftChangedEvent = (Action<float>)Delegate.Remove(materialInventoryManager2.OnDiscountCraftChangedEvent, new Action<float>(SetDiscountCraft));
	}

	public void SyncVariableToLocal()
	{
		playerNetwork.playerController.data.MaterialInventoryManager.SetDiscountCraft(_discountCraft, executeEvent: false);
		playerNetwork.playerController.data.MaterialInventoryManager.SetBonusLootMaterial(_bonusLootMaterial, executeEvent: false);
	}

	public override void FixedUpdateNetwork()
	{
		if (Runner.IsServer && NetworkGameManager.Instance.arrPlayerController.Count > 1 && (bool)Object)
		{
			PlayerRef inputAuthority = Object.InputAuthority;
			if (inputAuthority.IsValid && (bool)NetworkGameManager.Instance?.GetPlayer(targetIdxCam))
			{
				Runner.AddPlayerAreaOfInterest(inputAuthority, NetworkGameManager.Instance.GetPlayer(targetIdxCam).transform.position, PhotonMultiplayerManager.Instance.areaOfInterest);
			}
		}
		if (GetInput<NetworkInputData>(out var input))
		{
			int num = dataInputMove / 10;
			if (num < 9 && num != input.inputDataMove / 10 && playerNetwork.isLocalPlayer)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					SyncCurrentPosition = base.transform.position;
				}
				else
				{
					RpcSetSyncPosition(base.transform.position);
				}
			}
			dataInputMove = input.inputDataMove;
			if (dataInputClick != input.inputDataClick)
			{
				dataInputClick = input.inputDataClick;
			}
		}
		playerNetwork.UpdateNetwork();
	}

	[Preserve]
	private static void OnDataInputChanged(Changed<PlayerPhotonNetwork> changed)
	{
		PlayerController playerController = changed.Behaviour.playerNetwork.playerController;
		PlayerNetwork playerNetwork = changed.Behaviour.playerNetwork;
		string text = changed.Behaviour.dataInputClick.ToString();
		byte b = changed.Behaviour.dataInputMove;
		if (!playerNetwork.isLocalPlayer)
		{
			if (text.Substring(3, 1) == "1")
			{
				playerController.SetAiming(value: true);
			}
			else if (text.Substring(3, 1) == "0")
			{
				playerController.SetAiming(value: false);
			}
			if (text.Substring(4, 1) == "1")
			{
				playerController.isLMBDown = true;
			}
			else if (text.Substring(4, 1) == "0")
			{
				playerController.isLMBDown = false;
			}
		}
		if (text.Substring(1, 1) == "1")
		{
			playerController.isSprintDown = true;
		}
		else if (text.Substring(1, 1) == "0")
		{
			playerController.isSprintDown = false;
		}
		if (text.Substring(2, 1) == "1" && !playerController.isDashDown)
		{
			playerController.isDashDown = true;
			if (playerController.canDash)
			{
				playerController.Dash().Forget();
			}
		}
		else if (text.Substring(2, 1) == "0" && playerController.isDashDown)
		{
			playerController.isDashDown = false;
			if (playerController.isSprinting)
			{
				playerController.StopSprint();
			}
		}
		if (playerController.isSprintDown && !playerController.isSprinting && playerNetwork.GetEnableControl() && playerController.canSprint && b / 10 < 9 && !playerController.isAiming && !playerController.isRMBDown && !playerController.isAttacking && !playerController.fsmUpperBody.GetBool("isMelee"))
		{
			playerController.StartSprint();
		}
		if (playerController.isSprinting && (b / 10 >= 9 || !playerController.isSprintDown))
		{
			playerController.StopSprint();
		}
	}

	[Preserve]
	private static void OnReviveTimeChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (!ChatSystem.Instance)
		{
			return;
		}
		PlayerController playerController = changed.Behaviour.playerNetwork.playerController;
		ChatSystem.Instance.ListChatPlayers[changed.Behaviour.idx].text = LocalizationManager.GetTranslation("Menu/ImDying") + "<BR>(" + changed.Behaviour.reviveTimerSecond + ")";
		if (UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextProgressMashButton.isActiveAndEnabled && changed.Behaviour.Life > 0)
		{
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextProgressMashButton.text = LocalizationManager.GetTranslation("Interaction/GetUp") + "<BR>(" + changed.Behaviour.reviveTimerSecond + ")";
		}
		if (changed.Behaviour.reviveTimerSecond != 0)
		{
			return;
		}
		if (NetworkGameManager.Instance.ownPlayer.fsmUpperBody.GetBool("isReviving"))
		{
			ItemInteractable component = playerController.reviveArea.gameObject.GetComponent<ItemInteractable>();
			if (NetworkGameManager.Instance.ownPlayer.itemCollision != null && NetworkGameManager.Instance.ownPlayer.itemCollision == component.gameObject)
			{
				NetworkGameManager.Instance.ownPlayer.StopInteractProgress();
			}
		}
		ChatSystem.Instance.InstantHideDialogueBox(playerController.network.GetIDX());
		ChatSystem.Instance.ListChatPlayers[playerController.network.GetIDX()].gameObject.SetActive(value: false);
		UIGameManager.Instance.ArrPlayerInfo[playerController.network.playerIdx].TextPlayerName.text = UIGameManager.Instance.ArrPlayerInfo[playerController.network.playerIdx].TextPlayerName.text + "<br><color=red>(DEAD)</color>";
		playerController.reviveArea.enabled = false;
		playerController.healArea.enabled = false;
		playerController.SetAnimLowerSpeed(0f);
		playerController.SetAnimUpperSpeed(0f);
		playerController.isPermadeath = true;
		playerController.bloodPool.gameObject.SetActive(value: true);
		playerController.bloodPool.transform.DOKill();
		playerController.bloodPool.transform.DOScale(0f, 0f);
		playerController.bloodPool.transform.DOScale(4f, 5f);
		if (playerController.network.isLocalPlayer)
		{
			if (playerController.IsMale)
			{
				AudioManager.StopSFX("male_dyingBreath");
			}
			else
			{
				AudioManager.StopSFX("female_dyingBreath");
			}
			AudioManager.StopSFX("ui-heartbeat");
			UIGameManager.Instance.flashRed2.DOKill();
			UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, 0f);
			AudioManager.ChangeLowPass(22000f);
			UIGameManager.Instance.txtReviveTimer.text = LocalizationManager.GetTranslation("Menu/Dead").ToUpper();
		}
		else
		{
			string translation = LocalizationManager.GetTranslation("Menu/PlayerDead");
			translation = translation.Replace("[n]", playerController.network.GetPlayerName());
			UIGameManager.Instance.ShowPlayerInfo(translation + " (" + LocalizationManager.GetTranslation("Locations/" + playerController.RoomName) + ")");
		}
		foreach (InventoryObject item in playerController.data.arrInventory)
		{
			if (item.Name != "Null" && item.IdxInventory != 0)
			{
				playerController.inventoryManager.FunctionItemDrop(item.IdxInventory, isSwapWeapon: false);
			}
		}
	}

	[Preserve]
	private static void OnIdxChanged(Changed<PlayerPhotonNetwork> changed)
	{
		PlayerController playerController = changed.Behaviour.playerNetwork.playerController;
		NetworkGameManager.Instance.arrPlayerNetworkController[changed.Behaviour.idx] = playerController;
		playerController.network.playerIdx = changed.Behaviour.idx;
		if ((!playerController.network.isLocalPlayer && LobbyManager.Instance == null) || (!playerController.network.isLocalPlayer && LobbyManager.Instance != null && LobbyManager.Instance.testMode) || (LobbyManager.Instance != null && !LobbyManager.Instance.testMode))
		{
			PlayerBoard.Instance.boardPlayerList[changed.Behaviour.idx].SetActive(value: true);
		}
		if ((bool)LobbyManager.Instance)
		{
			LobbyManager.Instance.allReady = false;
			LobbyManager.Instance.timerCountDown.StopDuration();
		}
		UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: false);
		playerController.data.PlayerSkinData.LoadSkinData(isInit: true);
		playerController.reviveArea.gameObject.GetComponent<ItemInteractable>().UniqueID = (short)(10000 + changed.Behaviour.idx);
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.Show();
		}
	}

	[Preserve]
	private static void OnEnableControlChanged(Changed<PlayerPhotonNetwork> changed)
	{
		changed.Behaviour.playerNetwork.playerController.fsmUpperBody.SetBool("isMoving", value: false);
		changed.Behaviour.playerNetwork.playerController.fsmLowerBody.SetBool("isMoving", value: false);
	}

	[Preserve]
	private static void OnPlayerNameChanged(Changed<PlayerPhotonNetwork> changed)
	{
		UIGameManager.Instance.ArrPlayerInfo[changed.Behaviour.idx].gameObject.SetActive(value: true);
		PlayerBoard.Instance.playerNameList[changed.Behaviour.idx].text = changed.Behaviour.playerName;
		UIGameManager.Instance.ArrPlayerInfo[changed.Behaviour.idx].TextPlayerName.text = changed.Behaviour.playerName;
		if (changed.Behaviour.idx == 0 && NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			UIGameManager.Instance.ArrPlayerInfo[changed.Behaviour.idx].IconHostObject.SetActive(value: true);
		}
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.ShowBoard();
		}
	}

	[Preserve]
	private static void OnWeaponSelectChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (!changed.Behaviour.playerNetwork.isLocalPlayer)
		{
			changed.Behaviour.playerNetwork.playerController.ChangeWeaponPlayer(changed.Behaviour.weaponSelect);
		}
	}

	[Preserve]
	private static void OnHealthChanged(Changed<PlayerPhotonNetwork> changed)
	{
		PlayerController playerController = changed.Behaviour.playerNetwork.playerController;
		changed.Behaviour.CheckPlayerDying(playerController);
		float num = playerController.network.GetHealth();
		if (changed.Behaviour.playerNetwork.isLocalPlayer)
		{
			if (num <= 50f)
			{
				float num2 = 1f - num / 50f;
				if (num <= playerController.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f)
				{
					UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0.15f);
				}
				else
				{
					UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0f);
				}
				CameraGame.Instance.colorA.saturation.value = num2 * -45f;
				UIGameManager.Instance.flashRed2.DOKill();
				if (num <= playerController.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f)
				{
					UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, num2 / 6f);
				}
				else
				{
					UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, 0f);
				}
				UIGameManager.Instance.flashRed2.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
				if (num > playerController.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f)
				{
					playerController.canSprint = true;
					playerController.canDash = true;
					AudioManager.ChangeLowPass(22000f);
					AudioManager.StopSFX("ui-heartbeat");
					playerController.animspeed = 1f;
					playerController.SetAnimLowerSpeed(playerController.animspeed);
					playerController.isLowHealth = false;
					if (playerController.isSprinting)
					{
						playerController.data.SetCurrentMoveSpeed(playerController.data.GetSprintSpeed());
					}
					else if (playerController.isAiming || playerController.isRMBDown || playerController.isAttackMelee)
					{
						playerController.SetAnimLowerSpeed(0.5f);
						if (!playerController.isAttackMelee && !playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
						{
							playerController.SetAnimUpperSpeed(0.5f);
						}
						playerController.data.SetCurrentMoveSpeed(playerController.data.GetMoveAimSpeed());
					}
					else
					{
						playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
					}
				}
				else
				{
					playerController.canSprint = false;
					playerController.canDash = false;
					AudioManager.PlaySFX("ui-heartbeat");
					AudioManager.ChangeLowPass(2000f);
					playerController.animspeed = 0.5f;
					playerController.SetAnimLowerSpeed(playerController.animspeed);
					playerController.isLowHealth = true;
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
				}
			}
			else
			{
				AudioManager.StopSFX("ui-heartbeat");
				UIGameManager.Instance.flashRed2.DOKill();
				UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, 0f);
				if ((bool)CameraGame.Instance && (bool)CameraGame.Instance.colorA)
				{
					CameraGame.Instance.colorA.saturation.value = 0f;
				}
				UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0f);
				AudioManager.ChangeLowPass(22000f);
				playerController.animspeed = 1f;
				playerController.SetAnimLowerSpeed(playerController.animspeed);
				playerController.canSprint = true;
				playerController.canDash = true;
				playerController.isLowHealth = false;
				if (playerController.isSprinting)
				{
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetSprintSpeed());
				}
				else if (playerController.isAiming || playerController.isRMBDown || playerController.isAttackMelee)
				{
					playerController.SetAnimLowerSpeed(0.5f);
					if (!playerController.isAttackMelee && !playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
					{
						playerController.SetAnimUpperSpeed(0.5f);
					}
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetMoveAimSpeed());
				}
				else
				{
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
				}
			}
			UIGameManager.Instance.txtHpValuePlayer.text = Mathf.RoundToInt(num) + "/" + changed.Behaviour.playerNetwork.playerController.data.GetMaxHealth();
			if (UIGameManager.Instance != null && UIGameManager.Instance.arrStatusHP[0].isActiveAndEnabled)
			{
				if (num > 60f)
				{
					UIGameManager.Instance.txtTermHpStatusPlayer.SetTerm("Menu/Fine");
					foreach (Animator item in UIGameManager.Instance.arrStatusHP)
					{
						item.Play("Fine");
					}
				}
				else if (num > 29f)
				{
					UIGameManager.Instance.txtTermHpStatusPlayer.SetTerm("Menu/Caution");
					foreach (Animator item2 in UIGameManager.Instance.arrStatusHP)
					{
						item2.Play("Caution");
					}
				}
				else
				{
					UIGameManager.Instance.txtTermHpStatusPlayer.SetTerm("Menu/Danger");
					foreach (Animator item3 in UIGameManager.Instance.arrStatusHP)
					{
						item3.Play("Danger");
					}
				}
			}
		}
		if (num <= 50f)
		{
			if (num > playerController.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f)
			{
				playerController.canSprint = true;
				playerController.canDash = true;
				playerController.animspeed = 1f;
				playerController.SetAnimLowerSpeed(playerController.animspeed);
				playerController.isLowHealth = false;
				if (playerController.isSprinting)
				{
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetSprintSpeed());
				}
				else if (playerController.isAiming || playerController.isRMBDown || playerController.isAttackMelee)
				{
					playerController.SetAnimLowerSpeed(0.5f);
					if (!playerController.isAttackMelee && !playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
					{
						playerController.SetAnimUpperSpeed(0.5f);
					}
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetMoveAimSpeed());
				}
				else
				{
					playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
				}
			}
			else
			{
				playerController.canSprint = false;
				playerController.canDash = false;
				playerController.animspeed = 0.5f;
				playerController.SetAnimLowerSpeed(playerController.animspeed);
				playerController.isLowHealth = true;
				playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
			}
		}
		else
		{
			if (playerController.network.isLocalPlayer)
			{
				AudioManager.StopSFX("ui-heartbeat");
				UIGameManager.Instance.flashRed2.DOKill();
				UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, 0f);
				if (CameraGame.Instance != null)
				{
					CameraGame.Instance.colorA.saturation.value = 0f;
				}
				UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0f);
				AudioManager.ChangeLowPass(22000f);
			}
			playerController.animspeed = 1f;
			playerController.SetAnimLowerSpeed(playerController.animspeed);
			playerController.canSprint = true;
			playerController.canDash = true;
			playerController.isLowHealth = false;
			if (playerController.isSprinting)
			{
				playerController.data.SetCurrentMoveSpeed(playerController.data.GetSprintSpeed());
			}
			else if (playerController.isAiming || playerController.isRMBDown || playerController.isAttackMelee)
			{
				playerController.SetAnimLowerSpeed(0.5f);
				if (!playerController.isAttackMelee && !playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
				{
					playerController.SetAnimUpperSpeed(0.5f);
				}
				playerController.data.SetCurrentMoveSpeed(playerController.data.GetMoveAimSpeed());
			}
			else
			{
				playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
			}
		}
		short num3 = changed.Behaviour.health;
		changed.LoadOld();
		short num4 = changed.Behaviour.health;
		changed.LoadNew();
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			if (num <= 0f && playerController.network.isLocalPlayer)
			{
				foreach (PlayerController item4 in NetworkGameManager.Instance.arrPlayerNetworkController)
				{
					if ((bool)item4)
					{
						item4.healArea.enabled = false;
					}
				}
			}
			else if (num3 > 0 && num4 <= 0 && playerController.network.isLocalPlayer)
			{
				foreach (PlayerController item5 in NetworkGameManager.Instance.arrPlayerNetworkController)
				{
					if (!item5)
					{
						continue;
					}
					if (item5.network.GetHealth() <= item5.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f && !item5.isPermadeath && !item5.reviveArea.enabled && item5 != playerController)
					{
						if (NetworkGameManager.Instance.ownPlayer.PlayerMultiplyStatsData.GetHealLowHpAmount() > 0f && item5.network.GetHealth() > 0f)
						{
							item5.healArea.enabled = true;
						}
						else
						{
							item5.healArea.enabled = false;
						}
					}
					else
					{
						item5.healArea.enabled = false;
					}
				}
			}
			else if (!playerController.network.isLocalPlayer)
			{
				if (num <= playerController.PlayerMultiplyStatsData.GetBoundLowHpSlowWalk() - 1f && !playerController.isPermadeath)
				{
					if (NetworkGameManager.Instance.ownPlayer.network.GetHealth() > 0f && NetworkGameManager.Instance.ownPlayer.PlayerMultiplyStatsData.GetHealLowHpAmount() > 0f && num3 > 0)
					{
						playerController.healArea.enabled = true;
					}
					else
					{
						playerController.healArea.enabled = false;
					}
				}
				else
				{
					playerController.healArea.enabled = false;
				}
			}
		}
		if (num3 > 0 && num4 <= 0)
		{
			if (ChatSystem.Instance != null)
			{
				ChatSystem.Instance.InstantHideDialogueBox(playerController.network.GetIDX());
				ChatSystem.Instance.ListChatPlayers[changed.Behaviour.idx].gameObject.SetActive(value: false);
			}
			if (playerController.network.isLocalPlayer)
			{
				playerController.fov.enabled = true;
			}
			playerController.SetActiveDeadIconChar(isActive: false);
			playerController.isEntangled = false;
			playerController.network.charControllerPhoton.charControl.enabled = true;
			playerController.network.charControllerPhoton.Collider.enabled = true;
			playerController.network.charControllerPhoton.charControl.detectCollisions = true;
			if ((bool)UIGameManager.Instance && playerController.network.isLocalPlayer)
			{
				UIGameManager.Instance.spectateObject.SetActive(value: false);
				UIGameManager.Instance.uIInGameController.SetPlayerStatusUI(setActive: true);
				UIGameManager.Instance.uIInGameController.SetInventoryStatusUI(setActive: true);
			}
			AudioManager.StopSFXTransform(playerController.transform);
			playerController.reviveArea.enabled = false;
			playerController.shadow.color = new Color(playerController.shadow.color.r, playerController.shadow.color.g, playerController.shadow.color.b, 0.7f);
			playerController.network.charControllerPhoton.SetLayerMask(GameManager.Instance.layerMaskLive);
			playerController.playerCollider.SetActive(value: true);
			playerController.isPermadeath = false;
			if (LobbyManager.Instance == null)
			{
				playerController.flashlight.SetActive(value: true);
			}
			playerController.network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("Character");
			playerController.sortGroup.sortingLayerName = "Default";
			playerController.enableMoveChar = true;
			playerController.fsmUpperBody.Play("Idle");
			playerController.fsmUpperBody.SetBool("isMoving", value: false);
			playerController.fsmUpperBody.SetBool("isMelee", value: false);
			playerController.fsmUpperBody.SetBool("isShooting", value: false);
			playerController.fsmUpperBody.SetBool("isReviving", value: false);
			playerController.fsmUpperBody.SetBool("isReloading", value: false);
			playerController.invincibleTimer.StartDuration(3f);
			if (NetworkGameManager.Instance.isServer)
			{
				changed.Behaviour.enableControl = true;
				if (changed.Behaviour.reviveTimer == 90)
				{
					changed.Behaviour.reviveTimer = 45;
				}
				else if (changed.Behaviour.reviveTimer == 45)
				{
					changed.Behaviour.reviveTimer = 20;
				}
				else if (changed.Behaviour.reviveTimer == 20)
				{
					changed.Behaviour.reviveTimer = 10;
				}
			}
			playerController.bloodPool.transform.DOKill();
			playerController.bloodPool.gameObject.SetActive(value: false);
			if (playerController.network.GetPlayerName() != "")
			{
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextPlayerName.text = playerController.network.GetPlayerName();
			}
			_ = playerController.network.isLocalPlayer;
			playerController.reviveTimer.StopDuration();
			PlayerStatusUI.Instance.SetDisableMashButton(playerController.network.GetIDX());
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextProgressMashButton.transform.parent.parent.gameObject.SetActive(value: false);
			if (NetworkGameManager.Instance.isServer)
			{
				playerController.network.playerPhoton.targetIdxCam = playerController.network.GetIDX();
			}
		}
		if (PlayerBoard.Instance != null)
		{
			PlayerBoard.Instance.Hp[playerController.network.GetIDX()].text = Mathf.FloorToInt(playerController.network.GetHealth()).ToString();
			if (playerController.network.GetHealth() > 60f)
			{
				PlayerBoard.Instance.Hp[playerController.network.GetIDX()].color = new Color(0.53f, 1f, 0.5f);
			}
			else if (playerController.network.GetHealth() > 29f)
			{
				PlayerBoard.Instance.Hp[playerController.network.GetIDX()].color = new Color(1f, 0.93f, 0.2f);
			}
			else
			{
				PlayerBoard.Instance.Hp[playerController.network.GetIDX()].color = new Color(1f, 0.31f, 0.2f);
			}
		}
	}

	public void CheckPlayerDying(PlayerController playerController)
	{
		if (!(playerController.network.GetHealth() <= 0f))
		{
			return;
		}
		playerController.SetActiveDeadIconChar(isActive: true);
		playerController.isDashingMove = false;
		playerController.isDashing = false;
		playerController.animspeed = 1f;
		playerController.SetAnimLowerSpeed(playerController.animspeed);
		playerController.directionDash = Vector3.zero;
		playerController.canDash = true;
		playerController.sortGroup.sortingLayerName = "Default";
		playerController.weaponController.meleeSprite.material.DOKill();
		playerController.animUpperChar.transform.DOKill();
		playerController.animUpperChar.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0f);
		playerController.weaponController.meleeSprite.material.DOFloat(0f, "_Brightness", 0f);
		playerController.weaponController.meleeSprite.material.DOColor(new Color(0f, 0f, 0f), "_Tint", 0f);
		if (playerController.angleRot == 0f)
		{
			playerController.angleRot = 45f;
		}
		else if (playerController.angleRot == 90f)
		{
			playerController.angleRot = 135f;
		}
		else if (playerController.angleRot == 180f)
		{
			playerController.angleRot = 135f;
		}
		else if (playerController.angleRot == 270f)
		{
			playerController.angleRot = 225f;
		}
		playerController.enableMoveChar = false;
		playerController.flashlight.SetActive(value: false);
		playerController.fsmUpperBody.SetBool("isMoving", value: false);
		playerController.fsmUpperBody.SetBool("isMelee", value: false);
		playerController.fsmUpperBody.SetBool("isShooting", value: false);
		playerController.fsmUpperBody.SetBool("isReviving", value: false);
		playerController.fsmUpperBody.SetBool("isReloading", value: false);
		playerController.isAttacking = false;
		playerController.isAttackMelee = false;
		playerController.isShooting = false;
		playerController.isThrowing = false;
		playerController.isAiming = false;
		playerController.SetAnimUpperSpeed(1f);
		playerController.isAttackBtnPressed = false;
		playerController.animLowerChar.Play("LegFall" + playerController.angleRot, -1, 0f);
		playerController.animUpperChar.Play("Fall" + playerController.angleRot, -1, 0f);
		playerController.SetAnimLowerSpeed(1f);
		playerController.SetAnimUpperSpeed(1f);
		playerController.shadow.color = new Color(playerController.shadow.color.r, playerController.shadow.color.g, playerController.shadow.color.b, 0f);
		playerController.reviveArea.enabled = true;
		playerController.network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("CharacterDeadCollider");
		playerController.network.charControllerPhoton.SetLayerMask(GameManager.Instance.layerMaskDead);
		playerController.direction = Vector3.zero;
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.reviveTimer.StartDuration((int)playerController.network.playerPhoton.reviveTimer);
		}
		if (playerController.network.isLocalPlayer)
		{
			GameStatistic.AddDeath();
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressObject.SetActive(value: false);
			playerController.weaponController.MeleeTween.Kill();
			PlayerStatusUI.Instance.SetDisableMashButton(playerController.network.GetIDX());
			if (!UIGameManager.Instance.uiInventory.isHidden)
			{
				UIGameManager.Instance.HideInventory();
			}
			if (Life <= 0)
			{
				playerController.delaySpectator.StartDuration(3f);
			}
			UIGameManager.Instance.txtReviveTimer.text = playerController.network.playerPhoton.reviveTimer.ToString();
			if (playerController.IsMale)
			{
				AudioManager.PlaySFXTransform("male_dyingBreath", playerController.transform, playerController.network.isLocalPlayer);
			}
			else
			{
				AudioManager.PlaySFXTransform("female_dyingBreath", playerController.transform, playerController.network.isLocalPlayer);
			}
			if (!UIGameManager.Instance.UIMenuNote.isHidden)
			{
				playerController.CloseNote();
			}
			else if (UIGameManager.Instance.UIMenuPuzzle != null && !UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				playerController.ClosePuzzle();
			}
			foreach (InventoryObject item in playerController.data.arrInventory)
			{
				if (item.Name != "Null" && item.ItemType == "Item")
				{
					playerController.inventoryManager.FunctionItemDrop(item.IdxInventory, isSwapWeapon: false);
				}
			}
			if (Life > 0)
			{
				PlayerStatusUI.Instance.SetEnableMashButton(playerController.network.GetIDX());
				PlayerStatusUI.Instance.ProgresBar[playerController.network.GetIDX()].value = 0f;
				playerController.ctrGetUp = playerController.maxCtrGetUp;
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextProgressMashButton.transform.parent.parent.gameObject.SetActive(value: true);
			}
			else if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo && GameModes.Instance.modeGame != "PVP")
			{
				playerController.network.ShowBaloonChat(ChatType.HELP_ME, -1, -1, -1, -1, 10);
			}
		}
		else
		{
			if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo && GameModes.Instance.modeGame != "PVP")
			{
				playerController.network.ShowBaloonChat(ChatType.HELP_ME, -1, -1, -1, -1, 10);
			}
			string translation = LocalizationManager.GetTranslation("Menu/PlayerDying");
			translation = translation.Replace("[n]", playerController.network.GetPlayerName());
			UIGameManager.Instance.ShowPlayerInfo(translation + " (" + LocalizationManager.GetTranslation("Locations/" + playerController.RoomName) + ")");
		}
		if ((NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo || NetworkGameManager.Instance.arrPlayerController.Count == 1) && Life <= 0)
		{
			UniTaskUtil.DelayedCall(this, 0.5f, () =>
			{
				if (NetworkGameManager.Instance.IsAllPlayerDead() && NetworkGameManager.Instance.isServer && !GameManager.Instance.IsCutscenePlaying)
				{
					GameManagerPhoton.Instance.IsWin = false;
					GameManagerPhoton.Instance.RPCExecuteResult();
				}
			}).Forget();
		}
		else if (NetworkGameManager.Instance.IsAllPlayerDead())
		{
			UniTaskUtil.DelayedCall(this, 0.5f, () =>
			{
				if (NetworkGameManager.Instance.IsAllPlayerDead() && NetworkGameManager.Instance.isServer && !GameManager.Instance.IsCutscenePlaying)
				{
					GameManagerPhoton.Instance.IsWin = false;
					GameManagerPhoton.Instance.RPCExecuteResult();
				}
			}).Forget();
		}
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.ScorePlayerNetwork.IncreaseDead();
			playerController.network.playerPhoton.dataInputMove = (byte)(Mathf.FloorToInt(playerController.network.playerPhoton.dataInputMove / 10) * 10);
			foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
			{
				if ((item2.aiTarget.target == playerController.targetedPoint || item2.aiTarget.target == playerController.transform) && item2.network.GetHealth() > 0f)
				{
					item2.movement.SetStateAfterPlayerDead();
				}
			}
		}
		if (GameModes.Instance.modeGame == "PVP" && NetworkGameManager.Instance.IsOnePlayerSurvive())
		{
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.IsWin = true;
			}
			playerController.network.StartCoroutine(playerController.network.ShowResultScene());
		}
		playerController.playerCollider.SetActive(value: false);
	}

	[Preserve]
	private static void OnWeapon0Changed(Changed<PlayerPhotonNetwork> changed)
	{
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.ShowBoard();
		}
		changed.Behaviour.playerNetwork.playerController.weaponController.idWeaponMelee = changed.Behaviour.idWeapon0;
		if (NetworkGameManager.Instance.ownPlayer != changed.Behaviour.playerNetwork.playerController)
		{
			if (changed.Behaviour.idWeapon0 != -1)
			{
				changed.Behaviour.playerNetwork.playerController.weaponController.EquipWeaponID(changed.Behaviour.idWeapon0, 0);
				PlayerBoard.Instance.Weapon0[changed.Behaviour.playerNetwork.GetIDX()].enabled = true;
				PlayerBoard.Instance.Weapon0[changed.Behaviour.playerNetwork.GetIDX()].sprite = DataManager.Instance.GetItemSprite(changed.Behaviour.idWeapon0.ToString());
			}
			else
			{
				changed.Behaviour.playerNetwork.playerController.weaponController.UnEquipWeapon(0, fromServer: true);
				PlayerBoard.Instance.Weapon0[changed.Behaviour.playerNetwork.GetIDX()].enabled = false;
			}
		}
	}

	[Preserve]
	private static void OnWeapon1Changed(Changed<PlayerPhotonNetwork> changed)
	{
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.ShowBoard();
		}
		changed.Behaviour.playerNetwork.playerController.weaponController.idWeaponRange = changed.Behaviour.idWeapon1;
		if (changed.Behaviour.idWeapon1 > 0)
		{
			changed.Behaviour.playerNetwork.playerController.weaponController.idBaseWeaponRange = DataManager.Instance.GetBaseWeapon(changed.Behaviour.idWeapon1);
		}
		else
		{
			changed.Behaviour.playerNetwork.playerController.weaponController.idBaseWeaponRange = changed.Behaviour.idWeapon1;
		}
		if (changed.Behaviour.idWeapon1 > 0)
		{
			changed.Behaviour.playerNetwork.playerController.isRangeActive = true;
		}
		if (NetworkGameManager.Instance.ownPlayer != changed.Behaviour.playerNetwork.playerController)
		{
			if (changed.Behaviour.idWeapon1 != -1)
			{
				changed.Behaviour.playerNetwork.playerController.weaponController.EquipWeaponID(changed.Behaviour.idWeapon1, 1);
				PlayerBoard.Instance.Weapon1[changed.Behaviour.playerNetwork.GetIDX()].enabled = true;
				PlayerBoard.Instance.Weapon1[changed.Behaviour.playerNetwork.GetIDX()].sprite = DataManager.Instance.GetItemSprite(changed.Behaviour.idWeapon1.ToString());
			}
			else
			{
				changed.Behaviour.playerNetwork.playerController.isRangeActive = false;
				changed.Behaviour.playerNetwork.playerController.weaponController.UnEquipWeapon(1, fromServer: true);
				PlayerBoard.Instance.Weapon1[changed.Behaviour.playerNetwork.GetIDX()].enabled = false;
			}
		}
	}

	[Preserve]
	private static void OnModeGameChanged(Changed<PlayerPhotonNetwork> changed)
	{
		GameModes.Instance.modeGame = changed.Behaviour.ModeGame;
		GameModes.Instance.Init();
	}

	[Preserve]
	private static void OnDisconnectedChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (!NetworkGameManager.Instance.isServer && !changed.Behaviour.Object.HasInputAuthority)
		{
			if ((bool)changed.Behaviour.disconnected)
			{
				changed.Behaviour.playerNetwork.playerController.Disconnected();
			}
			else
			{
				changed.Behaviour.playerNetwork.playerController.Reconnected();
			}
		}
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.ShowBoard();
		}
	}

	[Preserve]
	private static void OnKickedChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (changed.Behaviour.playerNetwork.isLocalPlayer)
		{
			if ((bool)UIFinalResultManager.Instance && UIFinalResultManager.Instance.gameObject.activeSelf)
			{
				UIFinalResultManager.Instance.gameObject.SetActive(value: false);
			}
			if ((bool)LobbyManager.Instance && LobbyManager.Instance.UIResult.activeSelf)
			{
				LobbyManager.Instance.UIResult.SetActive(value: false);
			}
			GameManager.Instance.isKicked = true;
			GlobalSaveData.instance.optionData.lastRoomCode = "";
			UIGameManager.Instance.ShowFailedConnect("ErrorDisconnectedFromServer");
			PhotonMultiplayerManager.Instance.Shutdown();
		}
	}

	[Preserve]
	private static void OnSyncPositionChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (!changed.Behaviour.playerNetwork.isLocalPlayer)
		{
			changed.Behaviour.transform.position = changed.Behaviour.SyncCurrentPosition;
			changed.Behaviour.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			changed.Behaviour.playerNetwork.charControllerPhoton.DisableMoveTemporary = true;
		}
	}

	[Preserve]
	private static void OnMissionVote(Changed<PlayerPhotonNetwork> changed)
	{
		if ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionBoard.ListPlayerVote.Count > 0 && changed.Behaviour.idx > 0 && changed.Behaviour.MissionVote > 0 && MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(changed.Behaviour.MissionVote)?.transform.position != Vector3.zero)
		{
			MissionLobbyManager.Instance.MissionBoard.ListPlayerVote[changed.Behaviour.idx - 1].gameObject.SetActive(value: true);
			MissionLobbyManager.Instance.MissionBoard.ListPlayerVote[changed.Behaviour.idx - 1].Play(changed.Behaviour.playerNetwork.playerController.data.PlayerSkinData.GetPlayerAvatarSkin());
			MissionSelection missionSelection = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(changed.Behaviour.MissionVote);
			if ((bool)missionSelection)
			{
				MissionLobbyManager.Instance.MissionBoard.ListPlayerVote[changed.Behaviour.idx - 1].transform.parent.position = missionSelection.transform.position;
			}
		}
	}

	[Preserve]
	private static void OnStatusDisconnectedChanged(Changed<PlayerPhotonNetwork> changed)
	{
		if (!changed.Behaviour.IsDisconnected)
		{
			changed.Behaviour.playerNetwork.playerController.Reconnected();
		}
		else
		{
			changed.Behaviour.playerNetwork.playerController.Disconnected();
		}
	}

	[Preserve]
	private static void OnGodModeChanged(Changed<PlayerPhotonNetwork> changed)
	{
		changed.Behaviour.playerNetwork.playerController.SetGodMode(changed.Behaviour.godMode);
	}

	[Preserve]
	private static void OnMaxInventorySlot(Changed<PlayerPhotonNetwork> changed)
	{
		changed.Behaviour.playerNetwork.playerController.data.SetMaxInventoryLocal(changed.Behaviour.MaxInventorySlot);
	}

	[Preserve]
	private static void OnInteractingPuzzle(Changed<PlayerPhotonNetwork> changed)
	{
		PlayerNetwork playerNetwork = changed.Behaviour.playerNetwork;
		if (!playerNetwork.isLocalPlayer)
		{
			UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()].SolvingPuzzleUI.SetActive(changed.Behaviour.IsInteractingPuzzle);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetUILobby(bool isActive, byte idxPlayer)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetUILobby(System.Boolean,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetUILobby(System.Boolean,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isActive);
				num2 += 4;
				data[num2] = idxPlayer;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if ((GameManager.Instance != null && !NetworkGameManager.Instance.GetPlayer(idxPlayer).network.isLocalPlayer) || LobbyManager.Instance != null)
		{
			PlayerBoard.Instance.boardPlayerList[idxPlayer].SetActive(isActive);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSetReady(bool value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetReady(System.Boolean)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetReady(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
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
		GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(idx, value);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetInteractableObject(short idx, bool arrTriggerInteractableObject, bool arrEnableCollider, bool arrIsTriggered, int hashName = -1)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetInteractableObject(System.Int16,System.Boolean,System.Boolean,System.Boolean,System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetInteractableObject(System.Int16,System.Boolean,System.Boolean,System.Boolean,System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
				*(short*)(data + num2) = idx;
				num2 += 5 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), arrTriggerInteractableObject);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), arrEnableCollider);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), arrIsTriggered);
				num2 += 4;
				*(int*)(data + num2) = hashName;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer || GameManager.Instance.initSyncInteractableLobby)
		{
			return;
		}
		ItemInteractable itemInteract = GameManager.Instance.GetItemInteractable(idx, itemOnly: true);
		if (!itemInteract)
		{
			return;
		}
		if (arrEnableCollider)
		{
			itemInteract.EnableCollider();
		}
		else if (idx < GameManager.Instance.arrItemInteractable.Count && itemInteract != null)
		{
			itemInteract.DisableCollider();
		}
		if (arrIsTriggered)
		{
			itemInteract.listItemToActivate.Clear();
			itemInteract.itemIDUnlock = -1;
			if (itemInteract.IsPuzzle)
			{
				itemInteract.IsSolved = true;
				if (itemInteract.isLocked)
				{
					itemInteract.isLocked = false;
					itemInteract.lockMap.SetActive(value: false);
				}
				RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(playerNetwork.playerController.RoomName);
				if ((bool)roomCollider)
				{
					roomCollider.CheckMap(playerNetwork.playerController);
				}
			}
			itemInteract.TriggerAnimation(isUsedByLocalPlayer: false);
			if (itemInteract.isAutomaticClose)
			{
				if (itemInteract.animatorTrigger1 != null)
				{
					if (itemInteract.animationName1[0] != "")
					{
						itemInteract.animatorTrigger1.SetFloat("speedAnimation", -1f);
						itemInteract.animatorTrigger1.Play(itemInteract.animationName1[0], -1, 1f);
					}
					else if (itemInteract.SetAnimatorTrigger1 != "")
					{
						UniTaskUtil.DelayedCall(this, (float)idx * 0.05f, () =>
						{
							itemInteract.animatorTrigger1.SetTrigger(itemInteract.SetAnimatorTrigger1);
						}).Forget();
					}
					if (itemInteract.isTriggerOnce)
					{
						UniTaskUtil.DelayedCall(this, 0.65f, () =>
						{
							itemInteract.animatorTrigger1.enabled = false;
						}).Forget();
					}
				}
				if (itemInteract.animatorTrigger2 != null && itemInteract.animationName2[0] != "")
				{
					itemInteract.animatorTrigger2.SetFloat("speedAnimation", -1f);
					itemInteract.animatorTrigger2.Play(itemInteract.animationName2[0], -1, 1f);
					itemInteract.triggerScanAstar = true;
					UniTaskUtil.DelayedCall(itemInteract, 1f, () =>
					{
						itemInteract.ScanAstarItemCollider();
					}).Forget();
					if (itemInteract.isTriggerOnce)
					{
						UniTaskUtil.DelayedCall(this, 0.65f, () =>
						{
							itemInteract.animatorTrigger2.enabled = false;
						}).Forget();
					}
				}
				if (itemInteract.isTriggerOnce)
				{
					itemInteract.DisableCollider();
				}
				else
				{
					itemInteract.isTriggered = false;
				}
			}
			else if (itemInteract.animatorTrigger1 != null && itemInteract.SetAnimatorTrigger1 != "")
			{
				UniTaskUtil.DelayedCall(this, (float)idx * 0.05f, () =>
				{
					itemInteract.animatorTrigger1.SetTrigger(itemInteract.SetAnimatorTrigger1);
				}).Forget();
			}
			if (itemInteract.isTriggerOnce)
			{
				itemInteract.DisableCollider();
				itemInteract.spawnItemID = -1;
			}
			if (itemInteract.isLocked)
			{
				itemInteract.isLocked = false;
			}
			if (itemInteract.isLockedFromOtherSide)
			{
				itemInteract.isLockedFromOtherSide = false;
			}
		}
		if (hashName != -1)
		{
			itemInteract.animatorTrigger1.Play(hashName);
		}
		if (itemInteract != null && itemInteract.triggerOnReverse != arrTriggerInteractableObject && itemInteract.isTriggerReverse)
		{
			itemInteract.listItemToActivate.Clear();
			itemInteract.itemIDUnlock = -1;
			itemInteract.TriggerAnimation(isUsedByLocalPlayer: false);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetInteractableObjectKeyItem(short arrKeyNeedItem, int arrNeedItemList, bool isLastItem = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetInteractableObjectKeyItem(System.Int16,System.Int32,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetInteractableObjectKeyItem(System.Int16,System.Int32,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 4), data);
				*(short*)(data + num2) = arrKeyNeedItem;
				num2 += 5 & -4;
				*(int*)(data + num2) = arrNeedItemList;
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isLastItem);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer)
		{
			return;
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(arrKeyNeedItem);
		if ((bool)itemInteractable)
		{
			itemInteractable.listItemToActivate.Clear();
			if (arrNeedItemList != 0)
			{
				if (arrNeedItemList >= 1000000)
				{
					itemInteractable.listItemToActivate.Add(arrNeedItemList / 1000000);
				}
				if (arrNeedItemList >= 1000)
				{
					itemInteractable.listItemToActivate.Add(arrNeedItemList / 1000 % 1000);
				}
				itemInteractable.listItemToActivate.Add(arrNeedItemList % 1000);
			}
		}
		if (isLastItem)
		{
			GameManager.Instance.initSyncInteractableLobby = true;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncPickableObject(byte idx, bool arrPickable_isActive, short arrPickable_id, byte arrPickable_uid, short arrPickable_amount, ulong arrPickable_pos, bool visibleMap, bool isLastItem = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncPickableObject(System.Byte,System.Boolean,System.Int16,System.Byte,System.Int16,System.UInt64,System.Boolean,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncPickableObject(System.Byte,System.Boolean,System.Int16,System.Byte,System.Int16,System.UInt64,System.Boolean,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 5), data);
				data[num2] = idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), arrPickable_isActive);
				num2 += 4;
				*(short*)(data + num2) = arrPickable_id;
				num2 += 5 & -4;
				data[num2] = arrPickable_uid;
				num2 += 4 & -4;
				*(short*)(data + num2) = arrPickable_amount;
				num2 += 5 & -4;
				*(ulong*)(data + num2) = arrPickable_pos;
				num2 += 8;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), visibleMap);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isLastItem);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer || GameManager.Instance.initSyncitemPickableLobby)
		{
			return;
		}
		if (isLastItem)
		{
			GameManager.Instance.initSyncitemPickableLobby = true;
		}
		bool flag = false;
		for (int i = 0; i < GameManager.Instance.arrItemPickable.Count; i++)
		{
			if (GameManager.Instance.arrItemPickable[i].uniqueID == arrPickable_uid && GameManager.Instance.arrItemPickable[i].itemID == arrPickable_id)
			{
				flag = true;
				if ((bool)GameManager.Instance.arrItemPickable[i].rigidbody)
				{
					GameManager.Instance.arrItemPickable[i].rigidbody.transform.localPosition = MathFunc.DecodeVector3FromULong(arrPickable_pos);
				}
				else
				{
					GameManager.Instance.arrItemPickable[i].transform.position = MathFunc.DecodeVector3FromULong(arrPickable_pos);
				}
				GameManager.Instance.arrItemPickable[i].itemCollider.enabled = arrPickable_isActive;
				if (!arrPickable_isActive)
				{
					GameManager.Instance.arrItemPickable[i].OnRemoveObjectCustomFunction?.Execute(playerNetwork.playerController);
				}
				GameManager.Instance.arrItemPickable[i].SetSpriteEnable(arrPickable_isActive);
				BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(arrPickable_id);
				if (arrPickable_id < 100)
				{
					GameManager.Instance.arrItemPickable[i].ammo = arrPickable_amount;
				}
				else if (entityByKeyid != null && entityByKeyid.Durability > 0)
				{
					GameManager.Instance.arrItemPickable[i].durability = arrPickable_amount;
				}
				else
				{
					GameManager.Instance.arrItemPickable[i].amount = (byte)arrPickable_amount;
				}
				ItemScriptableObject itemData = DataManager.Instance.GetItemData(arrPickable_id.ToString());
				if (itemData != null)
				{
					GameManager.Instance.arrItemPickable[i].itemMap.sprite = itemData.ItemSprite;
				}
				GameManager.Instance.arrItemPickable[i].itemMap.enabled = visibleMap;
				break;
			}
		}
		if (!flag)
		{
			BGDatabase_Item entityByKeyid2 = BGDatabase_Item.GetEntityByKeyid(arrPickable_id);
			int durability = -1;
			if (entityByKeyid2 != null && entityByKeyid2.Durability > 0)
			{
				durability = arrPickable_amount;
			}
			GameManager.Instance.SpawnNewItem(arrPickable_id, MathFunc.DecodeVector3FromULong(arrPickable_pos), isSpreading: false, (byte)arrPickable_amount, (byte)arrPickable_amount, arrPickable_uid, arrPickable_isActive, isFading: false, visibleMap, durability);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncInventoryLocalToAll(short[] inventoryIdP, byte[] amountIdP, short[] durabilityP)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncInventoryLocalToAll(System.Int16[],System.Byte[],System.Int16[])", Object, 7);
				return;
			}
			int num = 8;
			num += (inventoryIdP.Length * 2 + 4 + 3) & -4;
			num += (amountIdP.Length * 1 + 4 + 3) & -4;
			num += (durabilityP.Length * 2 + 4 + 3) & -4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncInventoryLocalToAll(System.Int16[],System.Byte[],System.Int16[])", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 6), data);
				*(int*)(data + num2) = inventoryIdP.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryIdP) + 3) & -4) + num2;
				*(int*)(data + num2) = amountIdP.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amountIdP) + 3) & -4) + num2;
				*(int*)(data + num2) = durabilityP.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durabilityP) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController playerController = playerNetwork.playerController;
		if (playerController != null && !playerNetwork.isLocalPlayer)
		{
			for (int i = 0; i < 12; i++)
			{
				short num3 = -1;
				byte b = 0;
				num3 = inventoryIdP[i];
				b = amountIdP[i];
				short durability = durabilityP[i];
				playerController.data.AddObject(num3, (byte)i, b, -1, durability);
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncInventory(bool isTargettedLocalPlayer, short[] inventoryUIdP0, short[] inventoryIdP0, short[] inventoryUIdP1, short[] inventoryIdP1, short[] inventoryUIdP2, short[] inventoryIdP2, short[] inventoryUIdP3, short[] inventoryIdP3, byte[] amountIdP0, byte[] amountIdP1, byte[] amountIdP2, byte[] amountIdP3, short[] durabilityP0, short[] durabilityP1, short[] durabilityP2, short[] durabilityP3, bool isToAllPlayer = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncInventory(System.Boolean,System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Byte[],System.Byte[],System.Byte[],System.Byte[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += (inventoryUIdP0.Length * 2 + 4 + 3) & -4;
			num += (inventoryIdP0.Length * 2 + 4 + 3) & -4;
			num += (inventoryUIdP1.Length * 2 + 4 + 3) & -4;
			num += (inventoryIdP1.Length * 2 + 4 + 3) & -4;
			num += (inventoryUIdP2.Length * 2 + 4 + 3) & -4;
			num += (inventoryIdP2.Length * 2 + 4 + 3) & -4;
			num += (inventoryUIdP3.Length * 2 + 4 + 3) & -4;
			num += (inventoryIdP3.Length * 2 + 4 + 3) & -4;
			num += (amountIdP0.Length * 1 + 4 + 3) & -4;
			num += (amountIdP1.Length * 1 + 4 + 3) & -4;
			num += (amountIdP2.Length * 1 + 4 + 3) & -4;
			num += (amountIdP3.Length * 1 + 4 + 3) & -4;
			num += (durabilityP0.Length * 2 + 4 + 3) & -4;
			num += (durabilityP1.Length * 2 + 4 + 3) & -4;
			num += (durabilityP2.Length * 2 + 4 + 3) & -4;
			num += (durabilityP3.Length * 2 + 4 + 3) & -4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncInventory(System.Boolean,System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Byte[],System.Byte[],System.Byte[],System.Byte[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 7), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isTargettedLocalPlayer);
				num2 += 4;
				*(int*)(data + num2) = inventoryUIdP0.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP0) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryIdP0.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryIdP0) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryUIdP1.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP1) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryIdP1.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryIdP1) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryUIdP2.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP2) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryIdP2.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryIdP2) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryUIdP3.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP3) + 3) & -4) + num2;
				*(int*)(data + num2) = inventoryIdP3.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryIdP3) + 3) & -4) + num2;
				*(int*)(data + num2) = amountIdP0.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amountIdP0) + 3) & -4) + num2;
				*(int*)(data + num2) = amountIdP1.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amountIdP1) + 3) & -4) + num2;
				*(int*)(data + num2) = amountIdP2.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amountIdP2) + 3) & -4) + num2;
				*(int*)(data + num2) = amountIdP3.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amountIdP3) + 3) & -4) + num2;
				*(int*)(data + num2) = durabilityP0.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durabilityP0) + 3) & -4) + num2;
				*(int*)(data + num2) = durabilityP1.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durabilityP1) + 3) & -4) + num2;
				*(int*)(data + num2) = durabilityP2.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durabilityP2) + 3) & -4) + num2;
				*(int*)(data + num2) = durabilityP3.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durabilityP3) + 3) & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isToAllPlayer);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		SetSyncInventory(isTargettedLocalPlayer, inventoryIdP0, inventoryIdP1, inventoryIdP2, inventoryIdP3, inventoryUIdP0, inventoryUIdP1, inventoryUIdP2, inventoryUIdP3, amountIdP0, amountIdP1, amountIdP2, amountIdP3, durabilityP0, durabilityP1, durabilityP2, durabilityP3, isToAllPlayer);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSyncInventoryToHost(bool isTargettedLocalPlayer, short[] inventoryUIdP0, short[] inventoryIdP0, short[] inventoryUIdP1, short[] inventoryIdP1, short[] inventoryUIdP2, short[] inventoryIdP2, short[] inventoryUIdP3, short[] inventoryIdP3, byte[] amountIdP0, byte[] amountIdP1, byte[] amountIdP2, byte[] amountIdP3, short[] durabilityP0, short[] durabilityP1, short[] durabilityP2, short[] durabilityP3, bool isToAllPlayer = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncInventoryToHost(System.Boolean,System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Byte[],System.Byte[],System.Byte[],System.Byte[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += (inventoryUIdP0.Length * 2 + 4 + 3) & -4;
				num += (inventoryIdP0.Length * 2 + 4 + 3) & -4;
				num += (inventoryUIdP1.Length * 2 + 4 + 3) & -4;
				num += (inventoryIdP1.Length * 2 + 4 + 3) & -4;
				num += (inventoryUIdP2.Length * 2 + 4 + 3) & -4;
				num += (inventoryIdP2.Length * 2 + 4 + 3) & -4;
				num += (inventoryUIdP3.Length * 2 + 4 + 3) & -4;
				num += (inventoryIdP3.Length * 2 + 4 + 3) & -4;
				num += (amountIdP0.Length * 1 + 4 + 3) & -4;
				num += (amountIdP1.Length * 1 + 4 + 3) & -4;
				num += (amountIdP2.Length * 1 + 4 + 3) & -4;
				num += (amountIdP3.Length * 1 + 4 + 3) & -4;
				num += (durabilityP0.Length * 2 + 4 + 3) & -4;
				num += (durabilityP1.Length * 2 + 4 + 3) & -4;
				num += (durabilityP2.Length * 2 + 4 + 3) & -4;
				num += (durabilityP3.Length * 2 + 4 + 3) & -4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncInventoryToHost(System.Boolean,System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Byte[],System.Byte[],System.Byte[],System.Byte[],System.Int16[],System.Int16[],System.Int16[],System.Int16[],System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 8), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isTargettedLocalPlayer);
					num2 += 4;
					*(int*)(data + num2) = inventoryUIdP0.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP0) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryIdP0.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryIdP0) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryUIdP1.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP1) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryIdP1.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryIdP1) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryUIdP2.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP2) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryIdP2.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryIdP2) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryUIdP3.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryUIdP3) + 3) & -4) + num2;
					*(int*)(data + num2) = inventoryIdP3.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, inventoryIdP3) + 3) & -4) + num2;
					*(int*)(data + num2) = amountIdP0.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, amountIdP0) + 3) & -4) + num2;
					*(int*)(data + num2) = amountIdP1.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, amountIdP1) + 3) & -4) + num2;
					*(int*)(data + num2) = amountIdP2.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, amountIdP2) + 3) & -4) + num2;
					*(int*)(data + num2) = amountIdP3.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, amountIdP3) + 3) & -4) + num2;
					*(int*)(data + num2) = durabilityP0.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, durabilityP0) + 3) & -4) + num2;
					*(int*)(data + num2) = durabilityP1.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, durabilityP1) + 3) & -4) + num2;
					*(int*)(data + num2) = durabilityP2.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, durabilityP2) + 3) & -4) + num2;
					*(int*)(data + num2) = durabilityP3.Length;
					num2 += 4;
					num2 = ((Native.CopyFromArray(data + num2, durabilityP3) + 3) & -4) + num2;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isToAllPlayer);
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
		SetSyncInventory(isTargettedLocalPlayer, inventoryIdP0, inventoryIdP1, inventoryIdP2, inventoryIdP3, inventoryUIdP0, inventoryUIdP1, inventoryUIdP2, inventoryUIdP3, amountIdP0, amountIdP1, amountIdP2, amountIdP3, durabilityP0, durabilityP1, durabilityP2, durabilityP3, isToAllPlayer);
	}

	public void SetSyncInventory(bool isTargettedLocalPlayer, short[] inventoryIdP0, short[] inventoryIdP1, short[] inventoryIdP2, short[] inventoryIdP3, short[] inventoryUIdP0, short[] inventoryUIdP1, short[] inventoryUIdP2, short[] inventoryUIdP3, byte[] amountIdP0, byte[] amountIdP1, byte[] amountIdP2, byte[] amountIdP3, short[] durabilityP0, short[] durabilityP1, short[] durabilityP2, short[] durabilityP3, bool isToAllPlayer = false)
	{
		if ((!isToAllPlayer || playerNetwork.isLocalPlayer) && (!playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer || GameManager.Instance.initSyncInventory))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(i);
			if (!(player != null))
			{
				continue;
			}
			if (player.network.isLocalPlayer & isTargettedLocalPlayer)
			{
				player.data.idHealing = -1;
				player.data.idThrowable = -1;
				UIGameManager.Instance.HideHealingShortcutSprite();
				UIGameManager.Instance.HideThrowableShortcutSprite();
				player.inventoryManager.txtAmountHealingItem.text = "";
				player.inventoryManager.txtAmountThrowableItem.text = "";
			}
			for (int j = 0; j < 12; j++)
			{
				if (!player.network.isLocalPlayer | isTargettedLocalPlayer)
				{
					short num = -1;
					short uniqueID = -1;
					byte amount = 0;
					short durability = 0;
					if (player.network.GetIDX() == 0)
					{
						uniqueID = inventoryUIdP0[j];
						num = inventoryIdP0[j];
						amount = amountIdP0[j];
						durability = durabilityP0[j];
					}
					if (player.network.GetIDX() == 1)
					{
						uniqueID = inventoryUIdP1[j];
						num = inventoryIdP1[j];
						amount = amountIdP1[j];
						durability = durabilityP1[j];
					}
					if (player.network.GetIDX() == 2)
					{
						uniqueID = inventoryUIdP2[j];
						num = inventoryIdP2[j];
						amount = amountIdP2[j];
						durability = durabilityP2[j];
					}
					if (player.network.GetIDX() == 3)
					{
						uniqueID = inventoryUIdP3[j];
						num = inventoryIdP3[j];
						amount = amountIdP3[j];
						durability = durabilityP3[j];
					}
					player.data.AddObject(num, (byte)j, amount, uniqueID, durability, isSyncReconnect: true);
					if (num == MissionManager.Instance.KeyItemToActivateCar && (bool)UIMissionObjective.Instance)
					{
						UIMissionObjective.Instance.SetCheckboxRetrieveKeyItem();
					}
				}
			}
		}
		if (isTargettedLocalPlayer && UIGameManager.Instance != null)
		{
			NetworkGameManager.Instance.ownPlayer.weaponController.EquipWeaponInventory(0);
			NetworkGameManager.Instance.ownPlayer.data.InitImageInventoryLocal();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncItemBox(short[] inventoryId, byte[] amount, short[] durability, short timer, bool isForLocalPlayer = true)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncItemBox(System.Int16[],System.Byte[],System.Int16[],System.Int16,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += (inventoryId.Length * 2 + 4 + 3) & -4;
			num += (amount.Length * 1 + 4 + 3) & -4;
			num += (durability.Length * 2 + 4 + 3) & -4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncItemBox(System.Int16[],System.Byte[],System.Int16[],System.Int16,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 9), data);
				*(int*)(data + num2) = inventoryId.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, inventoryId) + 3) & -4) + num2;
				*(int*)(data + num2) = amount.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, amount) + 3) & -4) + num2;
				*(int*)(data + num2) = durability.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, durability) + 3) & -4) + num2;
				*(short*)(data + num2) = timer;
				num2 += 5 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isForLocalPlayer);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if ((!isForLocalPlayer || !playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer) && (isForLocalPlayer || playerNetwork.isLocalPlayer))
		{
			return;
		}
		playerNetwork.playerController.ItemBoxController.arrItem.Clear();
		for (int i = 0; i < inventoryId.Length; i++)
		{
			if (inventoryId[i] > 0)
			{
				playerNetwork.playerController.data.AddItemBox(inventoryId[i], amount[i], durability[i]);
			}
		}
		if (isForLocalPlayer && playerNetwork.isLocalPlayer)
		{
			GameManager.Instance.timer.interval = timer;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncMission(byte missionID, byte missionIDByMap, bool isCleared, bool isLocked, bool isHide, bool isCurrentMission, WeaponMapStruct[] listWeapon, byte spawnIdx, byte objectiveID, MapModifierStruct[] listModifier, byte[] listMapPossibleToUnlock, bool isLastMissionList = false, bool isFirstMissionList = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMission(System.Byte,System.Byte,System.Boolean,System.Boolean,System.Boolean,System.Boolean,WeaponMapStruct[],System.Byte,System.Byte,MapModifierStruct[],System.Byte[],System.Boolean,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += (listWeapon.Length * 8 + 4 + 3) & -4;
			num += 4;
			num += 4;
			num += (listModifier.Length * 8 + 4 + 3) & -4;
			num += (listMapPossibleToUnlock.Length * 1 + 4 + 3) & -4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMission(System.Byte,System.Byte,System.Boolean,System.Boolean,System.Boolean,System.Boolean,WeaponMapStruct[],System.Byte,System.Byte,MapModifierStruct[],System.Byte[],System.Boolean,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 10), data);
				data[num2] = missionID;
				num2 += 4 & -4;
				data[num2] = missionIDByMap;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isCleared);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isLocked);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isHide);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isCurrentMission);
				num2 += 4;
				*(int*)(data + num2) = listWeapon.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, listWeapon) + 3) & -4) + num2;
				data[num2] = spawnIdx;
				num2 += 4 & -4;
				data[num2] = objectiveID;
				num2 += 4 & -4;
				*(int*)(data + num2) = listModifier.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, listModifier) + 3) & -4) + num2;
				*(int*)(data + num2) = listMapPossibleToUnlock.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, listMapPossibleToUnlock) + 3) & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isLastMissionList);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isFirstMissionList);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		StartCoroutine(SyncMission(missionID, missionIDByMap, isCleared, isLocked, isHide, isCurrentMission, listWeapon, spawnIdx, objectiveID, listModifier, listMapPossibleToUnlock, isLastMissionList, isFirstMissionList));
	}

	private IEnumerator SyncMission(byte missionID, byte missionIDByMap, bool isCleared, bool isLocked, bool isHide, bool isCurrentMission, WeaponMapStruct[] listWeapon, byte spawnIdx, byte objectiveID, MapModifierStruct[] listModifier, byte[] listMapPossibleToUnlock, bool isLastMissionList = false, bool isFirstMissionList = false)
	{
		while (!GameManagerPhoton.Instance)
		{
			yield return null;
		}
		if (!playerNetwork.isLocalPlayer)
		{
			yield break;
		}
		if (isFirstMissionList)
		{
			Debug.Log("--cek Clear Mission List");
			GameManagerPhoton.Instance.ListMission.Clear();
		}
		GameManagerPhoton.Instance.isInitializedRandomizeWeapon = true;
		List<SO_MissionMap> list = GlobalMissionManager.Instance.ListAllMission.ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].buildVersion > int.Parse(GlobalSaveData.instance.gameData.GameVersion))
			{
				list.RemoveAt(num);
			}
		}
		foreach (SO_MissionMap item in list)
		{
			if (item.MissionIDByMap != missionIDByMap)
			{
				continue;
			}
			SO_MissionMap sO_MissionMap;
			if (item.MissionID == missionID)
			{
				sO_MissionMap = item;
				GameManagerPhoton.Instance.ListMission.Add(item);
			}
			else
			{
				sO_MissionMap = UnityEngine.Object.Instantiate(item);
				sO_MissionMap.MissionID = missionID;
				GameManagerPhoton.Instance.ListMission.Add(sO_MissionMap);
			}
			sO_MissionMap.ListWeapon.Clear();
			for (int i = 0; i < listWeapon.Length; i++)
			{
				sO_MissionMap.ListWeapon.Add(new WeaponMapType());
				sO_MissionMap.ListWeapon[i].Weapon = listWeapon[i].Weapon;
				sO_MissionMap.ListWeapon[i].WeaponType = listWeapon[i].WeaponType;
			}
			sO_MissionMap.ListModifier.Clear();
			for (int j = 0; j < listModifier.Length; j++)
			{
				sO_MissionMap.ListModifier.Add(GlobalMissionManager.Instance.GetMissionModifier(listModifier[j].idMissionModifier));
			}
			if (!sO_MissionMap.IsFixedMissionObjective)
			{
				sO_MissionMap.MissionObjective = GlobalMissionManager.Instance.ListAllMissionObjective[objectiveID];
			}
			sO_MissionMap.IsCleared = isCleared;
			sO_MissionMap.IsLocked = isLocked;
			sO_MissionMap.IsHide = isHide;
			sO_MissionMap.PlayerSpawningIdx = spawnIdx;
			if (!isCurrentMission)
			{
				break;
			}
			GameManagerPhoton.Instance.CurrentMission = sO_MissionMap;
			foreach (SO_MissionModifierEffect item2 in GlobalMissionManager.Instance.ListAllMissionModifier)
			{
				item2.Init();
			}
			for (int k = 0; k < listModifier.Length; k++)
			{
				sO_MissionMap.ListModifier[k].SetValueByDifficulty(0);
			}
			if (!NetworkGameManager.Instance.isSyncingMissionMap)
			{
				break;
			}
			sO_MissionMap.ListPossibleMapToUnlock.Clear();
			for (int l = 0; l < listMapPossibleToUnlock.Length; l++)
			{
				foreach (SO_MissionMap item3 in list)
				{
					if (listMapPossibleToUnlock[l] == item3.MissionIDByMap)
					{
						sO_MissionMap.ListPossibleMapToUnlock.Add(item3);
						break;
					}
				}
			}
			break;
		}
		if (isLastMissionList)
		{
			if ((bool)MissionLobbyManager.Instance)
			{
				MissionLobbyManager.Instance.SetUIMission();
				UIGameManager.Instance.SetMissionLocation(UIGameManager.Instance.missionLocationText, null, UIGameManager.Instance.missionLocationTextField);
			}
			UIGameManager.Instance.txtTime.text = GameManagerPhoton.Instance.Wave.ToString();
			if ((bool)GameManagerPhoton.Instance.CurrentMission && (bool)GameManagerPhoton.Instance.CurrentMission.MissionObjective && GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave > 0)
			{
				TextMeshProUGUI txtTime = UIGameManager.Instance.txtTime;
				txtTime.text = txtTime.text + " / " + GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave;
			}
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RpcSyncModifier(MapModifierStruct[] listModifier)
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
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncModifier(MapModifierStruct[])", Object, 1);
				return;
			}
			int num = 8;
			num += (listModifier.Length * 8 + 4 + 3) & -4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncModifier(MapModifierStruct[])", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 11), data);
				*(int*)(data + num2) = listModifier.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data + num2, listModifier) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!GameManagerPhoton.Instance || !GameManagerPhoton.Instance.CurrentMission)
		{
			return;
		}
		GameManagerPhoton.Instance.CurrentMission.ListModifier.Clear();
		for (int i = 0; i < listModifier.Length; i++)
		{
			GameManagerPhoton.Instance.CurrentMission.ListModifier.Add(GlobalMissionManager.Instance.GetMissionModifier(listModifier[i].idMissionModifier));
		}
		foreach (SO_MissionModifierEffect item in GlobalMissionManager.Instance.ListAllMissionModifier)
		{
			item.Init();
		}
		if (!GameManagerPhoton.Instance || !GameManagerPhoton.Instance.CurrentMission)
		{
			return;
		}
		foreach (SO_MissionModifierEffect item2 in GameManagerPhoton.Instance.CurrentMission.ListModifier)
		{
			item2.SetValueByDifficulty(0);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RpcSyncMissionObjective(byte idMissionObjective, byte difficultyObjective)
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
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMissionObjective(System.Byte,System.Byte)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMissionObjective(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 12), data);
				data[num2] = idMissionObjective;
				num2 += 4 & -4;
				data[num2] = difficultyObjective;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!GameManagerPhoton.Instance || !GameManagerPhoton.Instance.CurrentMission || !MissionLobbyManager.Instance || NetworkGameManager.Instance.isServer)
		{
			return;
		}
		MissionSelection missionSelection = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(GameManagerPhoton.Instance.CurrentMission.MissionID);
		for (int i = 0; i < GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3.Count; i++)
		{
			if (difficultyObjective <= 3 && GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3[i].ID == idMissionObjective)
			{
				missionSelection.MissionData.MissionObjective = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3[i];
				break;
			}
			if (difficultyObjective >= 4 && GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4[i].ID == idMissionObjective)
			{
				missionSelection.MissionData.MissionObjective = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4[i];
				break;
			}
		}
		missionSelection.MissionData.PlayerSpawningIdx = 0;
		missionSelection.StickerObjective.sprite = missionSelection.MissionData.MissionObjective.IconSticker;
		missionSelection.SetUI();
		MissionLobbyManager.Instance.MissionBoard.TextScenarioLabel.SetTerm(missionSelection.MissionData.MapNameLocalization);
		MissionLobbyManager.Instance.MissionBoard.TextScenarioDesc.SetTerm(missionSelection.MissionData.MissionObjective.MissionModeDescLocalization);
		if (missionSelection.MissionData.MissionObjective.IsCountdownEndlessHordeEnable)
		{
			int num3 = 0;
			num3 = missionSelection.MissionData.MissionObjective.GetCountdownTimerEndlessHorde(NetworkGameManager.Instance.arrPlayerController.Count) / 60;
			MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text = MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", num3.ToString());
		}
		if (missionSelection.MissionData.MissionObjective.MinTargetDestroy > 0 && missionSelection.MissionData.MissionObjective.TargetType != "")
		{
			MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text = MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", missionSelection.MissionData.MissionObjective.MinTargetDestroy.ToString());
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncMaxHealth(float maxHealth)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMaxHealth(System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMaxHealth(System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 13), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, maxHealth);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.data.SetLocalMaxHealth(maxHealth);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncMaxStamina(float maxStamina)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMaxStamina(System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMaxStamina(System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 14), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, maxStamina);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.data.SetLocalMaxStamina(maxStamina);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncMaxHealthLocal(byte idxPlayer, float maxHealth)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMaxHealthLocal(System.Byte,System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMaxHealthLocal(System.Byte,System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 15), data);
				data[num2] = idxPlayer;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, maxHealth);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (playerNetwork.isLocalPlayer)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(idxPlayer);
			if (player != null)
			{
				player.data.SetLocalMaxHealth(maxHealth);
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncMaxStaminaLocal(byte idxPlayer, float maxStamina)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncMaxStaminaLocal(System.Byte,System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncMaxStaminaLocal(System.Byte,System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 16), data);
				data[num2] = idxPlayer;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, maxStamina);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (playerNetwork.isLocalPlayer)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(idxPlayer);
			if (player != null)
			{
				player.data.SetLocalMaxStamina(maxStamina);
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncRoom(byte idxRoom, byte stateRoom)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncRoom(System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncRoom(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 17), data);
				data[num2] = idxRoom;
				num2 += 4 & -4;
				data[num2] = stateRoom;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (playerNetwork.isLocalPlayer)
		{
			switch (stateRoom)
			{
			case 1:
				GameManager.Instance.arrRoom[idxRoom].SetRevealedMap();
				break;
			case 2:
				GameManager.Instance.arrRoom[idxRoom].SetCompleteMap();
				break;
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncingOff()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncingOff()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncingOff()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 18), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (playerNetwork.isLocalPlayer)
		{
			NetworkGameManager.Instance.isSyncingMissionMap = false;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcRemoveItemBox(byte idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcRemoveItemBox(System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcRemoveItemBox(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 19), data);
					data[num2] = idx;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (GameModes.Instance.isItemBoxGlobal)
		{
			ItemBoxNetwork.instance.RemoveItem(idx);
		}
		else if (idx < playerNetwork.playerController.ItemBoxController.arrItem.Count)
		{
			playerNetwork.playerController.ItemBoxController.arrItem.RemoveAt(idx);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcAddItemBox(short idInventory, byte amount, short durability)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcAddItemBox(System.Int16,System.Byte,System.Int16)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcAddItemBox(System.Int16,System.Byte,System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 20), data);
					*(short*)(data + num2) = idInventory;
					num2 += 5 & -4;
					data[num2] = amount;
					num2 += 4 & -4;
					*(short*)(data + num2) = durability;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		playerNetwork.playerController.data.AddItemBox(idInventory, amount, durability);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSetPlayerName(string _playerName, string uniqueId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetPlayerName(System.String,System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(_playerName) + 3) & -4;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(uniqueId) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetPlayerName(System.String,System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 21), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, _playerName) + 3) & -4) + num2;
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, uniqueId) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		playerName = _playerName;
		userUniqueId = uniqueId;
		GameManagerPhoton.Instance.UpdatePlayerList();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSetVoiceChatName(string _voiceChatName)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetVoiceChatName(System.String)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(_voiceChatName) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetVoiceChatName(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 22), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, _voiceChatName) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (_voiceChatName != null)
		{
			voiceChatName = _voiceChatName;
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSetAFK(bool value)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
			return;
		}
		NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
		if (Runner.Stage == SimulationStages.Resimulate)
		{
			return;
		}
		int localAuthorityMask = Object.GetLocalAuthorityMask();
		if ((localAuthorityMask & 2) == 0)
		{
			NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetAFK(System.Boolean)", Object, 2);
		}
		else
		{
			if ((localAuthorityMask & 1) == 1)
			{
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetAFK(System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 23), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 1) != 0)
			{
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetTargetIdxCam(byte playeridx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetTargetIdxCam(System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetTargetIdxCam(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 24), data);
					data[num2] = playeridx;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (playerNetwork.GetHealth() <= 0f || playeridx == playerNetwork.GetIDX())
		{
			targetIdxCam = playeridx;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetWeapon0(short idWeapon)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetWeapon0(System.Int16)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetWeapon0(System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 25), data);
					*(short*)(data + num2) = idWeapon;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		idWeapon0 = idWeapon;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetWeapon1(short idWeapon)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetWeapon1(System.Int16)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetWeapon1(System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 26), data);
					*(short*)(data + num2) = idWeapon;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		idWeapon1 = idWeapon;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSelectWeapon(byte idxWeapon)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSelectWeapon(System.Byte)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSelectWeapon(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 27), data);
					data[num2] = idxWeapon;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		weaponSelect = idxWeapon;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcUnequipWeapon0()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcUnequipWeapon0()", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcUnequipWeapon0()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 28), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		idWeapon0 = -1;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcUnequipWeapon1()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcUnequipWeapon1()", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcUnequipWeapon1()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 29), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		idWeapon1 = -1;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcAddInventory(short iD, byte idxInventory, byte amount, byte playerIDX, short uniqueID, short durability = -1)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcAddInventory(System.Int16,System.Byte,System.Byte,System.Byte,System.Int16,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcAddInventory(System.Int16,System.Byte,System.Byte,System.Byte,System.Int16,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 30), data);
				*(short*)(data + num2) = iD;
				num2 += 5 & -4;
				data[num2] = idxInventory;
				num2 += 4 & -4;
				data[num2] = amount;
				num2 += 4 & -4;
				data[num2] = playerIDX;
				num2 += 4 & -4;
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				*(short*)(data + num2) = durability;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIDX);
		if (iD == MissionManager.Instance.KeyItemToActivateCar)
		{
			GameManager.Instance.gameManagerPhoton.arrObjective.Set(0, value: true);
			GameManager.Instance.gameManagerPhoton.arrObjective.Set(1, value: true);
			UIMissionObjective.Instance?.SetCheckboxRetrieveKeyItem();
			GameManager.Instance.waveManager.cueHordeTimer.StopDuration();
			if ((!GameManager.Instance.waveManager.hordeTimer.isRunning || GameManager.Instance.waveManager.hordeTimer.interval > 5f) && !UIGameManager.Instance.LabelHordeInfiniteIncoming.activeSelf)
			{
				GameManager.Instance.waveManager.AlertHorde(5);
			}
			foreach (ItemInteractable item in GameManager.Instance.ListBrimCarInteractable)
			{
				item.lockMap.transform.GetChild(0).gameObject.SetActive(value: true);
			}
		}
		if (!player.network.isLocalPlayer)
		{
			player.data.AddObject(iD, idxInventory, amount, uniqueID, durability);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncDataInventory(byte idxInventory, byte amount, byte playerIDX)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncDataInventory(System.Byte,System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncDataInventory(System.Byte,System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 31), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				data[num2] = amount;
				num2 += 4 & -4;
				data[num2] = playerIDX;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIDX);
		if (!player.network.isLocalPlayer)
		{
			if (player.data.arrInventory[idxInventory].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[idxInventory].ID).WeaponType == "Range")
			{
				player.data.arrInventory[idxInventory].Ammo = amount;
			}
			else
			{
				player.data.arrInventory[idxInventory].Amount = amount;
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSwapItem(byte idx1, byte idx2, byte playerIDX)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSwapItem(System.Byte,System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSwapItem(System.Byte,System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 32), data);
				data[num2] = idx1;
				num2 += 4 & -4;
				data[num2] = idx2;
				num2 += 4 & -4;
				data[num2] = playerIDX;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIDX);
		if (!player.network.isLocalPlayer)
		{
			player.inventoryManager.FunctionSwapSlot(idx1, idx2, isLocal: false);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcRemoveObject(short uniqueID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcRemoveObject(System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcRemoveObject(System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 33), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.OnRemoveObject(uniqueID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcRemoveInventory(byte idxInventory)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcRemoveInventory(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcRemoveInventory(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 34), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer)
		{
			playerNetwork.playerController.data.RemoveInventoryOtherPlayer(idxInventory);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcRemoveInventoryData(byte idxInventory)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcRemoveInventoryData(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcRemoveInventoryData(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 35), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer)
		{
			playerNetwork.playerController.data.RemoveInventoryData(idxInventory, syncNetwork: false);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcRemoveInventoryDuplicate(byte idxInventory, byte itemAmount)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcRemoveInventoryDuplicate(System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcRemoveInventoryDuplicate(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 36), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				data[num2] = itemAmount;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.data.RemoveInventoryOtherPlayer(idxInventory, isDuplicateItem: true, itemAmount);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcItemInteract(short uniqueID, byte idxPlayer, bool triggerOnReverse, bool isForceInteract = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcItemInteract(System.Int16,System.Byte,System.Boolean,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcItemInteract(System.Int16,System.Byte,System.Boolean,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 37), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				data[num2] = idxPlayer;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), triggerOnReverse);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isForceInteract);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.ItemInteract(uniqueID, idxPlayer, triggerOnReverse, isForceInteract);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcUnlockItem(int uniqueId)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcUnlockItem(System.Int32)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcUnlockItem(System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 38), data);
				*(int*)(data + num2) = uniqueId;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.UnlockItem((byte)uniqueId);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcStartProgressInteract(short uniqueID, byte playerID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStartProgressInteract(System.Int16,System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStartProgressInteract(System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 39), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StartProgressInteract(uniqueID, playerID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcStopProgressInteract(short uniqueID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStopProgressInteract(System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStopProgressInteract(System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 40), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StopProgressInteract(uniqueID, idx);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcStopProgressInteract()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStopProgressInteract()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStopProgressInteract()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 41), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StopProgressInteract(-1, idx);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcDropItem(int uIDItem, byte amount, byte ammo, ulong pos, short idxItem, bool isFading = false, bool isSpreading = true)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcDropItem(System.Int32,System.Byte,System.Byte,System.UInt64,System.Int16,System.Boolean,System.Boolean)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				num += 4;
				num += 8;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcDropItem(System.Int32,System.Byte,System.Byte,System.UInt64,System.Int16,System.Boolean,System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 42), data);
					*(int*)(data + num2) = uIDItem;
					num2 += 4;
					data[num2] = amount;
					num2 += 4 & -4;
					data[num2] = ammo;
					num2 += 4 & -4;
					*(ulong*)(data + num2) = pos;
					num2 += 8;
					*(short*)(data + num2) = idxItem;
					num2 += 5 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isFading);
					num2 += 4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpreading);
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
		GameManager.Instance.gameManagerPhoton.RpcDropItem(uIDItem, amount, ammo, pos, idxItem, isFading, isSpreading);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcDropItemFromPlayer(int IDItem, byte amount, byte ammo, byte playerIDX, byte idxInventory = 0, bool isQuickDrop = false, short uniqueID = -1)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcDropItemFromPlayer(System.Int32,System.Byte,System.Byte,System.Byte,System.Byte,System.Boolean,System.Int16)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcDropItemFromPlayer(System.Int32,System.Byte,System.Byte,System.Byte,System.Byte,System.Boolean,System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 43), data);
					*(int*)(data + num2) = IDItem;
					num2 += 4;
					data[num2] = amount;
					num2 += 4 & -4;
					data[num2] = ammo;
					num2 += 4 & -4;
					data[num2] = playerIDX;
					num2 += 4 & -4;
					data[num2] = idxInventory;
					num2 += 4 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isQuickDrop);
					num2 += 4;
					*(short*)(data + num2) = uniqueID;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (uniqueID == -1)
		{
			uniqueID = (short)GameManager.Instance.GetIdxItemPool(IDItem, isQuickDrop);
		}
		GameManager.Instance.gameManagerPhoton.RpcDropItemFromPlayer(IDItem, amount, ammo, playerIDX, uniqueID, idxInventory);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSpawnItem(int uIDItem, Vector3 pos, short idxItem, bool isSpread)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSpawnItem(System.Int32,UnityEngine.Vector3,System.Int16,System.Boolean)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 12;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSpawnItem(System.Int32,UnityEngine.Vector3,System.Int16,System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 44), data);
					*(int*)(data + num2) = uIDItem;
					num2 += 4;
					ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, pos);
					num2 += 12;
					*(short*)(data + num2) = idxItem;
					num2 += 5 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
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
		if (idxItem < 0 && GameManager.Instance.arrItemPickable.Count > 0)
		{
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
			List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
			idxItem = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
		}
		GameManager.Instance.gameManagerPhoton.RpcSpawnItem(uIDItem, MathFunc.EncodeVector3ToULong(pos), idxItem, isSpread);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSpawnItemAmount(int uIDItem, Vector3 pos, short idxItem, byte amount, bool isSpread)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSpawnItemAmount(System.Int32,UnityEngine.Vector3,System.Int16,System.Byte,System.Boolean)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 12;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSpawnItemAmount(System.Int32,UnityEngine.Vector3,System.Int16,System.Byte,System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 45), data);
					*(int*)(data + num2) = uIDItem;
					num2 += 4;
					ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, pos);
					num2 += 12;
					*(short*)(data + num2) = idxItem;
					num2 += 5 & -4;
					data[num2] = amount;
					num2 += 4 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
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
		if (idxItem < 0 && GameManager.Instance.arrItemPickable.Count > 0)
		{
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
			List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
			idxItem = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
		}
		GameManager.Instance.gameManagerPhoton.RpcSpawnItemAmount(uIDItem, MathFunc.EncodeVector3ToULong(pos), idxItem, amount, isSpread);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSpawnItemAmountAmmo(int uIDItem, Vector3 pos, short idxItem, byte amount, byte ammo = 0, bool isSpread = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSpawnItemAmountAmmo(System.Int32,UnityEngine.Vector3,System.Int16,System.Byte,System.Byte,System.Boolean)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 12;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSpawnItemAmountAmmo(System.Int32,UnityEngine.Vector3,System.Int16,System.Byte,System.Byte,System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 46), data);
					*(int*)(data + num2) = uIDItem;
					num2 += 4;
					ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, pos);
					num2 += 12;
					*(short*)(data + num2) = idxItem;
					num2 += 5 & -4;
					data[num2] = amount;
					num2 += 4 & -4;
					data[num2] = ammo;
					num2 += 4 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
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
		if (idxItem < 0 && GameManager.Instance.arrItemPickable.Count > 0)
		{
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
			List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
			idxItem = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
		}
		GameManager.Instance.gameManagerPhoton.RpcSpawnItemAmountAmmo(uIDItem, MathFunc.EncodeVector3ToULong(pos), idxItem, amount, ammo, isSpread);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncItemMap(short uidItem)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncItemMap(System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncItemMap(System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 47), data);
				*(short*)(data + num2) = uidItem;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.ShowItemMap(uidItem);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetHealth(short value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetHealth(System.Int16)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetHealth(System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 48), data);
					*(short*)(data + num2) = value;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (value < 0)
		{
			value = 0;
		}
		health = value;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcAddHealth(short value, bool cantDead = false)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcAddHealth(System.Int16,System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcAddHealth(System.Int16,System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 49), data);
					*(short*)(data + num2) = value;
					num2 += 5 & -4;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), cantDead);
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
		PlayerData data2 = playerNetwork.playerController.data;
		short num3 = health;
		if (!GameManager.Instance.IsCutscenePlaying && ((playerNetwork.playerController.IsGod && value > 0) || !playerNetwork.playerController.IsGod))
		{
			num3 += value;
		}
		if (num3 <= 0)
		{
			if (cantDead)
			{
				num3 = 100;
			}
			else
			{
				num3 = 0;
				if (playerNetwork.playerController.itemCollision != null && playerNetwork.playerController.itemCollision.GetComponent<ItemInteractable>() != null)
				{
					ItemInteractable component = playerNetwork.playerController.itemCollision.GetComponent<ItemInteractable>();
					if (component.isNeedProgress)
					{
						playerNetwork.ExecStopProgressInteract((short)component.UniqueID);
					}
				}
			}
		}
		else if ((float)num3 / 100f > data2.GetMaxHealth())
		{
			num3 = (short)(data2.GetMaxHealth() * 100f);
		}
		health = num3;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecAttackTriggered(byte _idx, byte ammo, short aimDirection)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcExecAttackTriggered(System.Byte,System.Byte,System.Int16)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcExecAttackTriggered(System.Byte,System.Byte,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 50), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				*(short*)(data + num2) = aimDirection;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx)?.weaponController.AttackTriggered(ammo, aimDirection);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcSetAimDirection(byte _idx, short aimDirection)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetAimDirection(System.Byte,System.Int16)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetAimDirection(System.Byte,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 51), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				*(short*)(data + num2) = aimDirection;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).weaponController.dirAimOtherPlayer = aimDirection;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecThrowingTriggered(byte _idx, byte ammo, short aimDirection)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcExecThrowingTriggered(System.Byte,System.Byte,System.Int16)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcExecThrowingTriggered(System.Byte,System.Byte,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 52), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				*(short*)(data + num2) = aimDirection;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).weaponController.AttackTriggered(ammo, aimDirection);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcReleaseAttack(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcReleaseAttack(System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcReleaseAttack(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 53), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(_idx);
		if (player != null && !player.network.isLocalPlayer)
		{
			player.weaponController.ReleaseAttack();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcStopShoot(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStopShoot(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStopShoot(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 54), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(_idx);
		if (player != null)
		{
			player.data.arrInventory[player.weaponController.idxWeaponRange].Ammo = 0;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecHitEffect(byte _idx, bool isCloseInventory = true)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcExecHitEffect(System.Byte,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcExecHitEffect(System.Byte,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 55), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isCloseInventory);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		FeedbackPlayerController feedbackController = NetworkGameManager.Instance.GetPlayer(_idx).feedbackController;
		if (feedbackController != null)
		{
			feedbackController.Hurt(isCloseInventory).Forget();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSicknessEffect()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSicknessEffect()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSicknessEffect()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 56), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		FeedbackPlayerController feedbackController = playerNetwork.playerController.feedbackController;
		if (feedbackController != null)
		{
			feedbackController.SicknessVfx().Forget();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetEnableControl(bool value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetEnableControl(System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetEnableControl(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 57), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
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
		enableControl = value;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcHitEnemy(byte idxEnemy, short value, byte animationType)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcHitEnemy(System.Byte,System.Int16,System.Byte)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcHitEnemy(System.Byte,System.Int16,System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 58), data);
					data[num2] = idxEnemy;
					num2 += 4 & -4;
					*(short*)(data + num2) = value;
					num2 += 5 & -4;
					data[num2] = animationType;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		EnemyController enemy = GameManager.Instance.GetEnemy(idxEnemy);
		if (enemy != null)
		{
			if (enemy.network.networkPhoton.health - value < 0)
			{
				enemy.network.networkPhoton.health = 0;
			}
			else
			{
				enemy.network.networkPhoton.health = (short)(enemy.network.networkPhoton.health - value);
			}
			enemy.feedback.Hurt(0.03f, animationType, enemy.movement.angleAnim, playerNetwork.playerController).Forget();
			enemy.ChasingPlayer(playerNetwork.playerController);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcThrowPose(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcThrowPose(System.Byte)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcThrowPose(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 59), data);
					data[num2] = _idx;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).fsmUpperBody.SetBool(IsThrowingAnim, value: true);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcCancelThrow(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcCancelThrow(System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcCancelThrow(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 60), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(_idx);
		player.fsmUpperBody.SetBool(IsThrowingAnim, value: false);
		player.SetAnimUpperSpeed(1f);
		player.isThrowing = false;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcThrowGrenade(byte _idx, Vector3 posGrenade, byte idItem)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcThrowGrenade(System.Byte,UnityEngine.Vector3,System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcThrowGrenade(System.Byte,UnityEngine.Vector3,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 61), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, posGrenade);
				num2 += 12;
				data[num2] = idItem;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).ThrowWeapon(posGrenade, idItem);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecGrenadeLauncher(byte _idx, Vector3 direction)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcExecGrenadeLauncher(System.Byte,UnityEngine.Vector3)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcExecGrenadeLauncher(System.Byte,UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 62), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, direction);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).weaponController.GLauncherControl.ExecuteGrenadeLauncher(direction);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcEnTangled(byte idx, byte idxEnemy, short angleAnim)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcEnTangled(System.Byte,System.Byte,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcEnTangled(System.Byte,System.Byte,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 63), data);
				data[num2] = idx;
				num2 += 4 & -4;
				data[num2] = idxEnemy;
				num2 += 4 & -4;
				*(short*)(data + num2) = angleAnim;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(idx);
		EnemyController enemy = GameManager.Instance.GetEnemy(idxEnemy);
		if (player.network.isLocalPlayer)
		{
			PlayerStatusUI.Instance.SetEnableMashButton(idx);
			UIGameManager.Instance.ArrPlayerInfo[idx].ChargeMeleeProgressObject.SetActive(value: false);
		}
		player.network.SetEnableControl(value: false);
		player.enableMoveChar = false;
		player.fsmUpperBody.Play("Tangled");
		player.network.charControllerPhoton.charControl.detectCollisions = false;
		player.network.charControllerPhoton.Collider.enabled = false;
		int num3 = 0;
		if ((bool)enemy)
		{
			enemy.movement.angleAnim = angleAnim;
			num3 = enemy.movement.angleAnim + 180;
		}
		if (num3 >= 360)
		{
			num3 -= 360;
		}
		player.animUpperChar.Play("TangledStart" + num3, -1, 0f);
		player.animLowerChar.Play("LegDown" + num3);
		player.angleRot = num3;
		AudioManager.PlaySFXTransform("hairmaiden-aggro", base.transform, isLocalPlayerTrigger: false, 1f, 1f);
		UniTaskUtil.DelayedCall(this, 0.25f, () =>
		{
			if ((bool)enemy)
			{
				Vector3 position = enemy.enemyCharacterRenderController.transform.position;
				player.network.charControllerPhoton.SetPosition(new Vector3(position.x + 0.1f, player.network.charControllerPhoton.transform.position.y, position.z + 0.1f));
			}
		}).Forget();
		player.sortGroup.sortingLayerName = "Ground";
		player.isEntangled = true;
		player.network.charControllerPhoton.charControl.enabled = false;
		player.network.ExecHurtEffect(player.network.GetIDX());
		player.maxCtrReleaseEntangled = 15;
		player.ctrReleaseEntangled = player.maxCtrReleaseEntangled;
		player.fsmUpperBody.SetBool("isShooting", value: false);
		player.fsmUpperBody.SetBool("isMelee", value: false);
		player.fsmUpperBody.SetBool("isReviving", value: false);
		player.fsmUpperBody.SetBool("isReloading", value: false);
		player.isAttacking = false;
		player.isAttackMelee = false;
		player.isShooting = false;
		player.isThrowing = false;
		if (player.network.isLocalPlayer && UIGameManager.Instance.crosshair != null)
		{
			UIGameManager.Instance.crosshair.gameObject.SetActive(value: false);
		}
		player.data.SetCurrentMoveSpeed(player.data.GetInitialMoveSpeed());
		player.SetAnimLowerSpeed(player.animspeed);
		PlayerStatusUI.Instance.ProgresBar[player.network.GetIDX()].value = 0f;
		CameraGame.Instance.CameraShake();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcReleaseEnTangled()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcReleaseEnTangled()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcReleaseEnTangled()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 64), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.ReleaseEntangled();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcStartSprint(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStartSprint(System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStartSprint(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 65), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).StartSprint();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcStopSprint(byte _idx)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcStopSprint(System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcStopSprint(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 66), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.GetPlayer(_idx).StopSprint();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcShowBaloonChatUID(byte _idx, ChatType chatType, short itemID, short UIDItem)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcShowBaloonChatUID(System.Byte,ChatType,System.Int16,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcShowBaloonChatUID(System.Byte,ChatType,System.Int16,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 67), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				*(ChatType*)(data + num2) = chatType;
				num2 += 4;
				*(short*)(data + num2) = itemID;
				num2 += 5 & -4;
				*(short*)(data + num2) = UIDItem;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ChatSystem.Instance.ShowBaloonChat(_idx, chatType, itemID, -1, -1, UIDItem);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcShowBaloonChat1(byte _idx, ChatType chatType, short itemID, byte _idxTarget = 10)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcShowBaloonChat1(System.Byte,ChatType,System.Int16,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcShowBaloonChat1(System.Byte,ChatType,System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 68), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				*(ChatType*)(data + num2) = chatType;
				num2 += 4;
				*(short*)(data + num2) = itemID;
				num2 += 5 & -4;
				data[num2] = _idxTarget;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ChatSystem.Instance.ShowBaloonChat(_idx, chatType, itemID, -1, -1, -1, _idxTarget);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcShowBaloonChat2(byte _idx, ChatType chatType, short itemID, short itemID2)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcShowBaloonChat2(System.Byte,ChatType,System.Int16,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcShowBaloonChat2(System.Byte,ChatType,System.Int16,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 69), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				*(ChatType*)(data + num2) = chatType;
				num2 += 4;
				*(short*)(data + num2) = itemID;
				num2 += 5 & -4;
				*(short*)(data + num2) = itemID2;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ChatSystem.Instance.ShowBaloonChat(_idx, chatType, itemID, itemID2, -1, -1);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcShowBaloonChat3(byte _idx, ChatType chatType, short itemID, short itemID2, short itemID3)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcShowBaloonChat3(System.Byte,ChatType,System.Int16,System.Int16,System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcShowBaloonChat3(System.Byte,ChatType,System.Int16,System.Int16,System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 70), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				*(ChatType*)(data + num2) = chatType;
				num2 += 4;
				*(short*)(data + num2) = itemID;
				num2 += 5 & -4;
				*(short*)(data + num2) = itemID2;
				num2 += 5 & -4;
				*(short*)(data + num2) = itemID3;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ChatSystem.Instance.ShowBaloonChat(_idx, chatType, itemID, itemID2, itemID3, -1);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcKillAllEnemies()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcKillAllEnemies()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcKillAllEnemies()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 71), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		GameDebug.Instance.KillAllEnemies();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcUnlockAllDoors()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcUnlockAllDoors()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcUnlockAllDoors()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 72), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.UnlockAllDoor();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetGodMode(bool setGodMode)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetGodMode(System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetGodMode(System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 73), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), setGodMode);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		godMode = setGodMode;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetGodMode()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetGodMode()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetGodMode()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 74), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.SetGodMode(playerNetwork.playerController);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetGhostMode()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetGhostMode()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetGhostMode()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 75), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.SetGhostMode(playerNetwork.playerController);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcWallThrough()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcWallThrough()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcWallThrough()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 76), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.WallThrough();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSuperStamina()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSuperStamina()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSuperStamina()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 77), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.SuperStamina(playerNetwork.playerController);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSpeedMax()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSpeedMax()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSpeedMax()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 78), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.SpeedMax(playerNetwork.playerController);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcShowAllItem()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcShowAllItem()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcShowAllItem()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 79), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameDebug.Instance.ShowAllItem();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcTonicStamina(bool isUnlimitedStamina)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcTonicStamina(System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcTonicStamina(System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 80), data);
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isUnlimitedStamina);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.SuperStamina(isUnlimitedStamina);
		playerNetwork.playerController.IsSpeedIncreaseBy2 = isUnlimitedStamina;
		if (playerNetwork.playerController.isAiming)
		{
			playerNetwork.playerController.data.SetCurrentMoveSpeed(playerNetwork.playerController.data.GetMoveAimSpeed());
		}
		else if (playerNetwork.playerController.isSprinting)
		{
			playerNetwork.playerController.data.SetCurrentMoveSpeed(playerNetwork.playerController.data.GetSprintSpeed());
		}
		else
		{
			playerNetwork.playerController.data.SetCurrentMoveSpeed(playerNetwork.playerController.data.GetInitialMoveSpeed());
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetQuitGame()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetQuitGame()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetQuitGame()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 81), data);
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
			isQuitGame = true;
		}
		if (playerNetwork.GetIDX() == 0 && !NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.Shutdown();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCRequestSync()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCRequestSync()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCRequestSync()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 82), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		playerNetwork.ExecSyncMission();
		playerNetwork.SetTriggerInteractableObject();
		playerNetwork.playerPhoton.RpcSyncingOff();
		playerNetwork.ExecSyncEventTrigger();
		playerNetwork.ExecSyncPickableObject();
		playerNetwork.ExecSyncInventory(isTargettedLocalPlayer: true);
		playerNetwork.ExecSyncItemBox(playerNetwork.playerController, (short)Mathf.FloorToInt(GameManager.Instance.timer.interval));
		playerNetwork.ExecSyncMaxHealth();
		playerNetwork.ExecSyncMaxStamina();
		playerNetwork.ExecSyncMap(playerNetwork.playerController);
		GameManagerPhoton.Instance.RpcSyncTimerCountdown(ChatSystem.Instance.timerCountdown.interval);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCRequestSyncMap()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCRequestSyncMap()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCRequestSyncMap()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 83), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		playerNetwork.ExecSyncMap(playerNetwork.playerController);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetMaxInventory(byte value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetMaxInventory(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetMaxInventory(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 84), data);
				data[num2] = value;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		MaxInventorySlot = value;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetSyncPosition(Vector3 value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetSyncPosition(UnityEngine.Vector3)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 12;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetSyncPosition(UnityEngine.Vector3)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 85), data);
					ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, value);
					num2 += 12;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SyncCurrentPosition = value;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCVoteMission(byte missionDataMissionID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCVoteMission(System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCVoteMission(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 86), data);
					data[num2] = missionDataMissionID;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		MissionVote = missionDataMissionID;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCDialogueOnboardingNPCShowed()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCDialogueOnboardingNPCShowed()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCDialogueOnboardingNPCShowed()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 87), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		IsDialogueOnboardingNPCShowed = true;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCSetPlayerDeviceID(string deviceID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSetPlayerDeviceID(System.String)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(deviceID) + 3) & -4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSetPlayerDeviceID(System.String)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 88), data);
					num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash(data + num2, deviceID) + 3) & -4) + num2;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		PlayerDeviceID = deviceID;
	}

	private void SetBonusLootMaterial(float value)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			_bonusLootMaterial = value;
		}
		else
		{
			Rpc_SetBonusLootMaterial(value);
		}
	}

	private void SetDiscountCraft(float value)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			_discountCraft = value;
		}
		else
		{
			Rpc_SetDiscountCraft(value);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	private unsafe void Rpc_SetBonusLootMaterial(float value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::Rpc_SetBonusLootMaterial(System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::Rpc_SetBonusLootMaterial(System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 89), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, value);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		_bonusLootMaterial = value;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	private unsafe void Rpc_SetDiscountCraft(float value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::Rpc_SetDiscountCraft(System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::Rpc_SetDiscountCraft(System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 90), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, value);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		_discountCraft = value;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetLife(byte newLife)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetLife(System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetLife(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 91), data);
					data[num2] = newLife;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		Life = newLife;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCEquipWeaponInventory(byte idxInventory)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCEquipWeaponInventory(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCEquipWeaponInventory(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 92), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer)
		{
			playerNetwork.playerController.weaponController.EquipWeaponInventory(idxInventory, playerNetwork.playerController.data.arrInventory[idxInventory].Ammo);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSyncAmmoWeapon(byte ammo)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSyncAmmoWeapon(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSyncAmmoWeapon(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 93), data);
				data[num2] = ammo;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer && playerNetwork.playerController.weaponController.idxWeaponRange > 0)
		{
			playerNetwork.playerController.data.arrInventory[playerNetwork.playerController.weaponController.idxWeaponRange].Ammo = ammo;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSyncAmmoWeaponInventory(byte idxInventory, byte ammo)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSyncAmmoWeaponInventory(System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSyncAmmoWeaponInventory(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 94), data);
				data[num2] = idxInventory;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer && idxInventory < playerNetwork.playerController.data.arrInventory.Count)
		{
			playerNetwork.playerController.data.arrInventory[idxInventory].Ammo = ammo;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSubtractAmmoWeapon()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSubtractAmmoWeapon()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSubtractAmmoWeapon()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 95), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer && playerNetwork.playerController.weaponController.idxWeaponRange > 0)
		{
			playerNetwork.playerController.data.arrInventory[playerNetwork.playerController.weaponController.idxWeaponRange].Ammo--;
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RpcSyncLobbyObjectPickedUp(byte itemUid)
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
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncLobbyObjectPickedUp(System.Byte)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncLobbyObjectPickedUp(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 96), data);
				data[num2] = itemUid;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (playerNetwork.isLocalPlayer && !NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.ListItemUIDLobbyPickedUp.Add(itemUid);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCSyncAmountInventory(byte idxInventory, byte amount)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSyncAmountInventory(System.Byte,System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSyncAmountInventory(System.Byte,System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 97), data);
					data[num2] = idxInventory;
					num2 += 4 & -4;
					data[num2] = amount;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (idxInventory >= 0 && idxInventory < playerNetwork.playerController.data.arrInventory.Count)
		{
			playerNetwork.playerController.data.arrInventory[idxInventory].Amount = amount;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncEventTrigger(short uniqueID, bool isCollided)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSyncEventTrigger(System.Int16,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSyncEventTrigger(System.Int16,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 98), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isCollided);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!playerNetwork.isLocalPlayer || NetworkGameManager.Instance.isServer)
		{
			return;
		}
		for (int i = 0; i < GameManager.Instance.arrEventTrigger.Count; i++)
		{
			if (!(GameManager.Instance.arrEventTrigger[i] != null) || GameManager.Instance.arrEventTrigger[i].UniqueID < 0 || GameManager.Instance.arrEventTrigger[i].UniqueID != uniqueID)
			{
				continue;
			}
			if (isCollided)
			{
				GameManager.Instance.arrEventTrigger[i].IsCollided = true;
				if (GameManager.Instance.arrEventTrigger[i].Anim != null)
				{
					GameManager.Instance.arrEventTrigger[i].Anim.Play("Default");
				}
			}
			break;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetDisconnectedOnLobby(bool value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetDisconnectedOnLobby(System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetDisconnectedOnLobby(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 99), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
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
		IsDisconnectedOnLobby = value;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetHealingValue(byte value, byte idxBar = 200)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetHealingValue(System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetHealingValue(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 100), data);
				data[num2] = value;
				num2 += 4 & -4;
				data[num2] = idxBar;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		byte idxBar2 = idxBar;
		if (NetworkGameManager.Instance.isServer)
		{
			healingValue = value;
		}
		if (!UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()].HealBarObject.activeSelf || playerNetwork.isLocalPlayer)
		{
			return;
		}
		UIPlayerInfo playerInfo = UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()];
		if (idxBar2 < 100)
		{
			playerInfo.listTargetStitch[idxBar2].DOScale(0f, 0.2f).OnComplete(() =>
			{
				UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()].listTargetStitch[idxBar2].gameObject.SetActive(value: false);
			});
			playerInfo.RedBarBG.gameObject.SetActive(value: true);
			playerInfo.RedBarBG.DOKill();
			playerInfo.RedBarBG.color = new Color(0.13f, 1f, 0.25f, 1f);
			playerInfo.RedBarBG.DOFade(0f, 0.7f).OnComplete(() =>
			{
				playerInfo.RedBarBG.gameObject.SetActive(value: false);
			});
			playerInfo.TextHealingReviveValue.gameObject.SetActive(value: true);
			playerInfo.TextHealingReviveValue.text = "+5";
			playerInfo.TextHealingReviveValue.DOKill();
			playerInfo.TextHealingReviveValue.rectTransform.DOKill();
			playerInfo.TextHealingReviveValue.rectTransform.localPosition = Vector2.zero;
			playerInfo.TextHealingReviveValue.DOFade(0.6f, 0f);
			playerInfo.TextHealingReviveValue.rectTransform.DOLocalMoveY(18f, 0.6f);
			playerInfo.TextHealingReviveValue.DOFade(0f, 0.3f).SetDelay(1f).OnComplete(() =>
			{
				playerInfo.TextHealingReviveValue.gameObject.SetActive(value: false);
			});
		}
		else if (idxBar2 == 100)
		{
			playerInfo.RedBarBG.gameObject.SetActive(value: true);
			playerInfo.RedBarBG.DOKill();
			playerInfo.RedBarBG.color = new Color(0.56f, 0.13f, 0.07f, 1f);
			playerInfo.RedBarBG.DOFade(0f, 0.7f).OnComplete(() =>
			{
				playerInfo.RedBarBG.gameObject.SetActive(value: false);
			});
			playerInfo.TextHealingReviveValue.gameObject.SetActive(value: true);
			playerInfo.TextHealingReviveValue.text = "-10";
			playerInfo.TextHealingReviveValue.DOKill();
			playerInfo.TextHealingReviveValue.rectTransform.DOKill();
			playerInfo.TextHealingReviveValue.rectTransform.localPosition = Vector2.zero;
			playerInfo.TextHealingReviveValue.DOFade(0.6f, 0f);
			playerInfo.TextHealingReviveValue.rectTransform.DOLocalMoveY(18f, 0.6f);
			playerInfo.TextHealingReviveValue.DOFade(0f, 0.3f).SetDelay(1f).OnComplete(() =>
			{
				playerInfo.TextHealingReviveValue.gameObject.SetActive(value: false);
			});
			UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()].TextHealingValue.transform.DOShakePosition(0.2f, 2f, 30, 90f, snapping: true, fadeOut: false);
		}
		UIGameManager.Instance.ArrPlayerInfo[playerNetwork.GetIDX()].TextHealingValue.text = "+" + healingValue;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetInteractingPuzzle(bool value)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetInteractingPuzzle(System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetInteractingPuzzle(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 101), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), value);
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
		IsInteractingPuzzle = value;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetSteamID(ulong steamID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetSteamID(System.UInt64)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetSteamID(System.UInt64)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 102), data);
					*(ulong*)(data + num2) = steamID;
					num2 += 8;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SteamIDUlong = steamID;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSyncDurability(int idxInventory, float durability)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSyncDurability(System.Int32,System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSyncDurability(System.Int32,System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 103), data);
				*(int*)(data + num2) = idxInventory;
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, durability);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController playerController = playerNetwork.playerController;
		InventoryObject inventoryObject = playerController.data.arrInventory[idxInventory];
		inventoryObject.Durability = durability;
		if (playerController.network.isLocalPlayer)
		{
			playerController.ArmorManager.SyncArmorManager(inventoryObject);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcSetFriendPass(bool v)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RpcSetFriendPass(System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RpcSetFriendPass(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 104), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), v);
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
		IsFriendPass = true;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSetAdditionalSpeed(float speed)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void PlayerPhotonNetwork::RPCSetAdditionalSpeed(System.Single)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void PlayerPhotonNetwork::RPCSetAdditionalSpeed(System.Single)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 105), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, speed);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		playerNetwork.playerController.SetAdditionalSpeed(speed);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		disconnected = _disconnected;
		isQuitGame = _isQuitGame;
		isKicked = _isKicked;
		inGame = _inGame;
		IsSurvive = _IsSurvive;
		ModeGame = _ModeGame;
		enableControl = _enableControl;
		IsDisconnected = _IsDisconnected;
		IsDisconnectedOnLobby = _IsDisconnectedOnLobby;
		dataInputMove = _dataInputMove;
		dataInputClick = _dataInputClick;
		voiceChatName = _voiceChatName;
		targetIdxCam = _targetIdxCam;
		reviveTimer = _reviveTimer;
		reviveTimerSecond = _reviveTimerSecond;
		weaponSelect = _weaponSelect;
		idx = _idx;
		userUniqueId = _userUniqueId;
		playerName = _playerName;
		health = _health;
		Life = _Life;
		idWeapon0 = _idWeapon0;
		idWeapon1 = _idWeapon1;
		godMode = _godMode;
		MaxInventorySlot = _MaxInventorySlot;
		SyncCurrentPosition = _SyncCurrentPosition;
		MissionVote = _MissionVote;
		ButtonsPrevious = _ButtonsPrevious;
		IsDialogueOnboardingNPCShowed = _IsDialogueOnboardingNPCShowed;
		healingValue = _healingValue;
		IsInteractingPuzzle = _IsInteractingPuzzle;
		SteamIDUlong = _SteamIDUlong;
		_bonusLootMaterial = __bonusLootMaterial;
		_discountCraft = __discountCraft;
		IsFriendPass = _IsFriendPass;
	}

	public override void CopyStateToBackingFields()
	{
		_disconnected = disconnected;
		_isQuitGame = isQuitGame;
		_isKicked = isKicked;
		_inGame = inGame;
		_IsSurvive = IsSurvive;
		_ModeGame = ModeGame;
		_enableControl = enableControl;
		_IsDisconnected = IsDisconnected;
		_IsDisconnectedOnLobby = IsDisconnectedOnLobby;
		_dataInputMove = dataInputMove;
		_dataInputClick = dataInputClick;
		_voiceChatName = voiceChatName;
		_targetIdxCam = targetIdxCam;
		_reviveTimer = reviveTimer;
		_reviveTimerSecond = reviveTimerSecond;
		_weaponSelect = weaponSelect;
		_idx = idx;
		_userUniqueId = userUniqueId;
		_playerName = playerName;
		_health = health;
		_Life = Life;
		_idWeapon0 = idWeapon0;
		_idWeapon1 = idWeapon1;
		_godMode = godMode;
		_MaxInventorySlot = MaxInventorySlot;
		_SyncCurrentPosition = SyncCurrentPosition;
		_MissionVote = MissionVote;
		_ButtonsPrevious = ButtonsPrevious;
		_IsDialogueOnboardingNPCShowed = IsDialogueOnboardingNPCShowed;
		_healingValue = healingValue;
		_IsInteractingPuzzle = IsInteractingPuzzle;
		_SteamIDUlong = SteamIDUlong;
		__bonusLootMaterial = _bonusLootMaterial;
		__discountCraft = _discountCraft;
		_IsFriendPass = IsFriendPass;
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetUILobby_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isActive = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idxPlayer = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetUILobby(isActive, idxPlayer);
	}

	[NetworkRpcWeavedInvoker(2, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSetReady_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetReady(value);
	}

	[NetworkRpcWeavedInvoker(3, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetInteractableObject_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short num3 = num2;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool arrTriggerInteractableObject = num4;
		bool num5 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool arrEnableCollider = num5;
		bool num6 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool arrIsTriggered = num6;
		int num7 = *(int*)(data + num);
		num += 4;
		int hashName = num7;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetInteractableObject(num3, arrTriggerInteractableObject, arrEnableCollider, arrIsTriggered, hashName);
	}

	[NetworkRpcWeavedInvoker(4, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetInteractableObjectKeyItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short arrKeyNeedItem = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		int arrNeedItemList = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isLastItem = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetInteractableObjectKeyItem(arrKeyNeedItem, arrNeedItemList, isLastItem);
	}

	[NetworkRpcWeavedInvoker(5, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncPickableObject_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool arrPickable_isActive = num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short arrPickable_id = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte arrPickable_uid = num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short arrPickable_amount = num6;
		long num7 = *(long*)(data + num);
		num += 8;
		ulong arrPickable_pos = (ulong)num7;
		bool num8 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool visibleMap = num8;
		bool num9 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isLastItem = num9;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncPickableObject(b, arrPickable_isActive, arrPickable_id, arrPickable_uid, arrPickable_amount, arrPickable_pos, visibleMap, isLastItem);
	}

	[NetworkRpcWeavedInvoker(6, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncInventoryLocalToAll_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short[] array = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array2, data + num) + 3) & -4) + num;
		short[] array3 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array3, data + num) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncInventoryLocalToAll(array, array2, array3);
	}

	[NetworkRpcWeavedInvoker(7, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isTargettedLocalPlayer = num2;
		short[] array = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		short[] array2 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array2, data + num) + 3) & -4) + num;
		short[] array3 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array3, data + num) + 3) & -4) + num;
		short[] array4 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array4, data + num) + 3) & -4) + num;
		short[] array5 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array5, data + num) + 3) & -4) + num;
		short[] array6 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array6, data + num) + 3) & -4) + num;
		short[] array7 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array7, data + num) + 3) & -4) + num;
		short[] array8 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array8, data + num) + 3) & -4) + num;
		byte[] array9 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array9, data + num) + 3) & -4) + num;
		byte[] array10 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array10, data + num) + 3) & -4) + num;
		byte[] array11 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array11, data + num) + 3) & -4) + num;
		byte[] array12 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array12, data + num) + 3) & -4) + num;
		short[] array13 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array13, data + num) + 3) & -4) + num;
		short[] array14 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array14, data + num) + 3) & -4) + num;
		short[] array15 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array15, data + num) + 3) & -4) + num;
		short[] array16 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array16, data + num) + 3) & -4) + num;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isToAllPlayer = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncInventory(isTargettedLocalPlayer, array, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12, array13, array14, array15, array16, isToAllPlayer);
	}

	[NetworkRpcWeavedInvoker(8, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSyncInventoryToHost_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isTargettedLocalPlayer = num2;
		short[] array = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		short[] array2 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array2, data + num) + 3) & -4) + num;
		short[] array3 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array3, data + num) + 3) & -4) + num;
		short[] array4 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array4, data + num) + 3) & -4) + num;
		short[] array5 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array5, data + num) + 3) & -4) + num;
		short[] array6 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array6, data + num) + 3) & -4) + num;
		short[] array7 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array7, data + num) + 3) & -4) + num;
		short[] array8 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array8, data + num) + 3) & -4) + num;
		byte[] array9 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array9, data + num) + 3) & -4) + num;
		byte[] array10 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array10, data + num) + 3) & -4) + num;
		byte[] array11 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array11, data + num) + 3) & -4) + num;
		byte[] array12 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array12, data + num) + 3) & -4) + num;
		short[] array13 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array13, data + num) + 3) & -4) + num;
		short[] array14 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array14, data + num) + 3) & -4) + num;
		short[] array15 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array15, data + num) + 3) & -4) + num;
		short[] array16 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array16, data + num) + 3) & -4) + num;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isToAllPlayer = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncInventoryToHost(isTargettedLocalPlayer, array, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12, array13, array14, array15, array16, isToAllPlayer);
	}

	[NetworkRpcWeavedInvoker(9, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncItemBox_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short[] array = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array2, data + num) + 3) & -4) + num;
		short[] array3 = new short[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array3, data + num) + 3) & -4) + num;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short timer = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isForLocalPlayer = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncItemBox(array, array2, array3, timer, isForLocalPlayer);
	}

	[NetworkRpcWeavedInvoker(10, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMission_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte missionID = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte missionIDByMap = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isCleared = num4;
		bool num5 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isLocked = num5;
		bool num6 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isHide = num6;
		bool num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isCurrentMission = num7;
		WeaponMapStruct[] array = new WeaponMapStruct[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		byte num8 = data[num];
		num += 4 & -4;
		byte spawnIdx = num8;
		byte num9 = data[num];
		num += 4 & -4;
		byte objectiveID = num9;
		MapModifierStruct[] array2 = new MapModifierStruct[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array2, data + num) + 3) & -4) + num;
		byte[] array3 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array3, data + num) + 3) & -4) + num;
		bool num10 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isLastMissionList = num10;
		bool num11 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isFirstMissionList = num11;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMission(missionID, missionIDByMap, isCleared, isLocked, isHide, isCurrentMission, array, spawnIdx, objectiveID, array2, array3, isLastMissionList, isFirstMissionList);
	}

	[NetworkRpcWeavedInvoker(11, 1, 7)]
	[Preserve]
	protected unsafe static void RpcSyncModifier_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		MapModifierStruct[] array = new MapModifierStruct[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncModifier(array);
	}

	[NetworkRpcWeavedInvoker(12, 1, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMissionObjective_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idMissionObjective = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte difficultyObjective = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMissionObjective(idMissionObjective, difficultyObjective);
	}

	[NetworkRpcWeavedInvoker(13, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMaxHealth_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float maxHealth = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMaxHealth(maxHealth);
	}

	[NetworkRpcWeavedInvoker(14, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMaxStamina_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float maxStamina = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMaxStamina(maxStamina);
	}

	[NetworkRpcWeavedInvoker(15, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMaxHealthLocal_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxPlayer = num2;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float maxHealth = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMaxHealthLocal(idxPlayer, maxHealth);
	}

	[NetworkRpcWeavedInvoker(16, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncMaxStaminaLocal_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxPlayer = num2;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float maxStamina = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncMaxStaminaLocal(idxPlayer, maxStamina);
	}

	[NetworkRpcWeavedInvoker(17, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncRoom_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxRoom = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte stateRoom = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncRoom(idxRoom, stateRoom);
	}

	[NetworkRpcWeavedInvoker(18, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncingOff_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncingOff();
	}

	[NetworkRpcWeavedInvoker(19, 7, 1)]
	[Preserve]
	protected unsafe static void RpcRemoveItemBox_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcRemoveItemBox(b);
	}

	[NetworkRpcWeavedInvoker(20, 7, 1)]
	[Preserve]
	protected unsafe static void RpcAddItemBox_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short idInventory = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short durability = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcAddItemBox(idInventory, amount, durability);
	}

	[NetworkRpcWeavedInvoker(21, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSetPlayerName_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result2) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetPlayerName(result, result2);
	}

	[NetworkRpcWeavedInvoker(22, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSetVoiceChatName_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetVoiceChatName(result);
	}

	[NetworkRpcWeavedInvoker(23, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSetAFK_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetAFK(value);
	}

	[NetworkRpcWeavedInvoker(24, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetTargetIdxCam_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte playeridx = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetTargetIdxCam(playeridx);
	}

	[NetworkRpcWeavedInvoker(25, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetWeapon0_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short idWeapon = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetWeapon0(idWeapon);
	}

	[NetworkRpcWeavedInvoker(26, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetWeapon1_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short idWeapon = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetWeapon1(idWeapon);
	}

	[NetworkRpcWeavedInvoker(27, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSelectWeapon_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxWeapon = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSelectWeapon(idxWeapon);
	}

	[NetworkRpcWeavedInvoker(28, 2, 1)]
	[Preserve]
	protected unsafe static void RpcUnequipWeapon0_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcUnequipWeapon0();
	}

	[NetworkRpcWeavedInvoker(29, 2, 1)]
	[Preserve]
	protected unsafe static void RpcUnequipWeapon1_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcUnequipWeapon1();
	}

	[NetworkRpcWeavedInvoker(30, 7, 7)]
	[Preserve]
	protected unsafe static void RpcAddInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short iD = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idxInventory = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte amount = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte playerIDX = num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num6;
		short num7 = *(short*)(data + num);
		num += 5 & -4;
		short durability = num7;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcAddInventory(iD, idxInventory, amount, playerIDX, uniqueID, durability);
	}

	[NetworkRpcWeavedInvoker(31, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncDataInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte playerIDX = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncDataInventory(idxInventory, amount, playerIDX);
	}

	[NetworkRpcWeavedInvoker(32, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSwapItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idx = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idx2 = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte playerIDX = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSwapItem(idx, idx2, playerIDX);
	}

	[NetworkRpcWeavedInvoker(33, 7, 7)]
	[Preserve]
	protected unsafe static void RpcRemoveObject_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcRemoveObject(uniqueID);
	}

	[NetworkRpcWeavedInvoker(34, 7, 7)]
	[Preserve]
	protected unsafe static void RpcRemoveInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcRemoveInventory(idxInventory);
	}

	[NetworkRpcWeavedInvoker(35, 7, 7)]
	[Preserve]
	protected unsafe static void RpcRemoveInventoryData_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcRemoveInventoryData(idxInventory);
	}

	[NetworkRpcWeavedInvoker(36, 7, 7)]
	[Preserve]
	protected unsafe static void RpcRemoveInventoryDuplicate_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte itemAmount = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcRemoveInventoryDuplicate(idxInventory, itemAmount);
	}

	[NetworkRpcWeavedInvoker(37, 7, 7)]
	[Preserve]
	protected unsafe static void RpcItemInteract_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idxPlayer = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool triggerOnReverse = num4;
		bool num5 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isForceInteract = num5;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcItemInteract(uniqueID, idxPlayer, triggerOnReverse, isForceInteract);
	}

	[NetworkRpcWeavedInvoker(38, 2, 7)]
	[Preserve]
	protected unsafe static void RpcUnlockItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uniqueId = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcUnlockItem(uniqueId);
	}

	[NetworkRpcWeavedInvoker(39, 2, 7)]
	[Preserve]
	protected unsafe static void RpcStartProgressInteract_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte playerID = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStartProgressInteract(uniqueID, playerID);
	}

	[NetworkRpcWeavedInvoker(40, 7, 7)]
	[Preserve]
	protected unsafe static void RpcStopProgressInteract_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStopProgressInteract(uniqueID);
	}

	[NetworkRpcWeavedInvoker(41, 7, 7)]
	[Preserve]
	protected unsafe static void RpcStopProgressInteract_0040Invoker2(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStopProgressInteract();
	}

	[NetworkRpcWeavedInvoker(42, 2, 1)]
	[Preserve]
	protected unsafe static void RpcDropItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte ammo = num4;
		long num5 = *(long*)(data + num);
		num += 8;
		ulong pos = (ulong)num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num6;
		bool num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isFading = num7;
		bool num8 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpreading = num8;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcDropItem(uIDItem, amount, ammo, pos, idxItem, isFading, isSpreading);
	}

	[NetworkRpcWeavedInvoker(43, 7, 1)]
	[Preserve]
	protected unsafe static void RpcDropItemFromPlayer_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int iDItem = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte ammo = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte playerIDX = num5;
		byte num6 = data[num];
		num += 4 & -4;
		byte idxInventory = num6;
		bool num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isQuickDrop = num7;
		short num8 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num8;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcDropItemFromPlayer(iDItem, amount, ammo, playerIDX, idxInventory, isQuickDrop, uniqueID);
	}

	[NetworkRpcWeavedInvoker(44, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSpawnItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 pos = vector;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSpawnItem(uIDItem, pos, idxItem, isSpread);
	}

	[NetworkRpcWeavedInvoker(45, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSpawnItemAmount_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 pos = vector;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte amount = num4;
		bool num5 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num5;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSpawnItemAmount(uIDItem, pos, idxItem, amount, isSpread);
	}

	[NetworkRpcWeavedInvoker(46, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSpawnItemAmountAmmo_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 pos = vector;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte amount = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte ammo = num5;
		bool num6 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num6;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSpawnItemAmountAmmo(uIDItem, pos, idxItem, amount, ammo, isSpread);
	}

	[NetworkRpcWeavedInvoker(47, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncItemMap_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uidItem = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncItemMap(uidItem);
	}

	[NetworkRpcWeavedInvoker(48, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetHealth_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetHealth(value);
	}

	[NetworkRpcWeavedInvoker(49, 7, 1)]
	[Preserve]
	protected unsafe static void RpcAddHealth_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short value = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool cantDead = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcAddHealth(value, cantDead);
	}

	[NetworkRpcWeavedInvoker(50, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecAttackTriggered_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte ammo = num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short aimDirection = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcExecAttackTriggered(b, ammo, aimDirection);
	}

	[NetworkRpcWeavedInvoker(51, 2, 7)]
	[Preserve]
	protected unsafe static void RpcSetAimDirection_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short aimDirection = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetAimDirection(b, aimDirection);
	}

	[NetworkRpcWeavedInvoker(52, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecThrowingTriggered_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte ammo = num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short aimDirection = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcExecThrowingTriggered(b, ammo, aimDirection);
	}

	[NetworkRpcWeavedInvoker(53, 2, 7)]
	[Preserve]
	protected unsafe static void RpcReleaseAttack_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcReleaseAttack(b);
	}

	[NetworkRpcWeavedInvoker(54, 7, 7)]
	[Preserve]
	protected unsafe static void RpcStopShoot_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStopShoot(b);
	}

	[NetworkRpcWeavedInvoker(55, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecHitEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isCloseInventory = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcExecHitEffect(b, isCloseInventory);
	}

	[NetworkRpcWeavedInvoker(56, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSicknessEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSicknessEffect();
	}

	[NetworkRpcWeavedInvoker(57, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetEnableControl_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetEnableControl(value);
	}

	[NetworkRpcWeavedInvoker(58, 2, 1)]
	[Preserve]
	protected unsafe static void RpcHitEnemy_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxEnemy = num2;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short value = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte animationType = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcHitEnemy(idxEnemy, value, animationType);
	}

	[NetworkRpcWeavedInvoker(59, 2, 1)]
	[Preserve]
	protected unsafe static void RpcThrowPose_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcThrowPose(b);
	}

	[NetworkRpcWeavedInvoker(60, 2, 7)]
	[Preserve]
	protected unsafe static void RpcCancelThrow_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcCancelThrow(b);
	}

	[NetworkRpcWeavedInvoker(61, 2, 7)]
	[Preserve]
	protected unsafe static void RpcThrowGrenade_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 posGrenade = vector;
		byte num3 = data[num];
		num += 4 & -4;
		byte idItem = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcThrowGrenade(b, posGrenade, idItem);
	}

	[NetworkRpcWeavedInvoker(62, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecGrenadeLauncher_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 direction = vector;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcExecGrenadeLauncher(b, direction);
	}

	[NetworkRpcWeavedInvoker(63, 7, 7)]
	[Preserve]
	protected unsafe static void RpcEnTangled_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idxEnemy = num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short angleAnim = num4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcEnTangled(b, idxEnemy, angleAnim);
	}

	[NetworkRpcWeavedInvoker(64, 7, 7)]
	[Preserve]
	protected unsafe static void RpcReleaseEnTangled_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcReleaseEnTangled();
	}

	[NetworkRpcWeavedInvoker(65, 2, 7)]
	[Preserve]
	protected unsafe static void RpcStartSprint_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStartSprint(b);
	}

	[NetworkRpcWeavedInvoker(66, 2, 7)]
	[Preserve]
	protected unsafe static void RpcStopSprint_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcStopSprint(b);
	}

	[NetworkRpcWeavedInvoker(67, 7, 7)]
	[Preserve]
	protected unsafe static void RpcShowBaloonChatUID_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		ChatType chatType = (ChatType)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short itemID = num4;
		short num5 = *(short*)(data + num);
		num += 5 & -4;
		short uIDItem = num5;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcShowBaloonChatUID(b, chatType, itemID, uIDItem);
	}

	[NetworkRpcWeavedInvoker(68, 7, 7)]
	[Preserve]
	protected unsafe static void RpcShowBaloonChat1_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		ChatType chatType = (ChatType)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short itemID = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte idxTarget = num5;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcShowBaloonChat1(b, chatType, itemID, idxTarget);
	}

	[NetworkRpcWeavedInvoker(69, 7, 7)]
	[Preserve]
	protected unsafe static void RpcShowBaloonChat2_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		ChatType chatType = (ChatType)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short itemID = num4;
		short num5 = *(short*)(data + num);
		num += 5 & -4;
		short itemID2 = num5;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcShowBaloonChat2(b, chatType, itemID, itemID2);
	}

	[NetworkRpcWeavedInvoker(70, 7, 7)]
	[Preserve]
	protected unsafe static void RpcShowBaloonChat3_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		ChatType chatType = (ChatType)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short itemID = num4;
		short num5 = *(short*)(data + num);
		num += 5 & -4;
		short itemID2 = num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short itemID3 = num6;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcShowBaloonChat3(b, chatType, itemID, itemID2, itemID3);
	}

	[NetworkRpcWeavedInvoker(71, 7, 1)]
	[Preserve]
	protected unsafe static void RpcKillAllEnemies_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcKillAllEnemies();
	}

	[NetworkRpcWeavedInvoker(72, 7, 7)]
	[Preserve]
	protected unsafe static void RpcUnlockAllDoors_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcUnlockAllDoors();
	}

	[NetworkRpcWeavedInvoker(73, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetGodMode_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool setGodMode = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetGodMode(setGodMode);
	}

	[NetworkRpcWeavedInvoker(74, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetGodMode_0040Invoker2(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetGodMode();
	}

	[NetworkRpcWeavedInvoker(75, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetGhostMode_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetGhostMode();
	}

	[NetworkRpcWeavedInvoker(76, 7, 7)]
	[Preserve]
	protected unsafe static void RpcWallThrough_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcWallThrough();
	}

	[NetworkRpcWeavedInvoker(77, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSuperStamina_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSuperStamina();
	}

	[NetworkRpcWeavedInvoker(78, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSpeedMax_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSpeedMax();
	}

	[NetworkRpcWeavedInvoker(79, 7, 7)]
	[Preserve]
	protected unsafe static void RpcShowAllItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcShowAllItem();
	}

	[NetworkRpcWeavedInvoker(80, 7, 7)]
	[Preserve]
	protected unsafe static void RpcTonicStamina_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isUnlimitedStamina = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcTonicStamina(isUnlimitedStamina);
	}

	[NetworkRpcWeavedInvoker(81, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetQuitGame_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetQuitGame();
	}

	[NetworkRpcWeavedInvoker(82, 7, 1)]
	[Preserve]
	protected unsafe static void RPCRequestSync_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCRequestSync();
	}

	[NetworkRpcWeavedInvoker(83, 7, 1)]
	[Preserve]
	protected unsafe static void RPCRequestSyncMap_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCRequestSyncMap();
	}

	[NetworkRpcWeavedInvoker(84, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetMaxInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetMaxInventory(value);
	}

	[NetworkRpcWeavedInvoker(85, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetSyncPosition_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 value = vector;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetSyncPosition(value);
	}

	[NetworkRpcWeavedInvoker(86, 7, 1)]
	[Preserve]
	protected unsafe static void RPCVoteMission_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte missionDataMissionID = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCVoteMission(missionDataMissionID);
	}

	[NetworkRpcWeavedInvoker(87, 7, 1)]
	[Preserve]
	protected unsafe static void RPCDialogueOnboardingNPCShowed_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCDialogueOnboardingNPCShowed();
	}

	[NetworkRpcWeavedInvoker(88, 7, 1)]
	[Preserve]
	protected unsafe static void RPCSetPlayerDeviceID_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash(data + num, out var result) + 3) & -4) + num;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSetPlayerDeviceID(result);
	}

	[NetworkRpcWeavedInvoker(89, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SetBonusLootMaterial_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).Rpc_SetBonusLootMaterial(value);
	}

	[NetworkRpcWeavedInvoker(90, 7, 7)]
	[Preserve]
	protected unsafe static void Rpc_SetDiscountCraft_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).Rpc_SetDiscountCraft(value);
	}

	[NetworkRpcWeavedInvoker(91, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetLife_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte newLife = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetLife(newLife);
	}

	[NetworkRpcWeavedInvoker(92, 7, 7)]
	[Preserve]
	protected unsafe static void RPCEquipWeaponInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCEquipWeaponInventory(idxInventory);
	}

	[NetworkRpcWeavedInvoker(93, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSyncAmmoWeapon_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte ammo = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSyncAmmoWeapon(ammo);
	}

	[NetworkRpcWeavedInvoker(94, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSyncAmmoWeaponInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte ammo = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSyncAmmoWeaponInventory(idxInventory, ammo);
	}

	[NetworkRpcWeavedInvoker(95, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSubtractAmmoWeapon_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSubtractAmmoWeapon();
	}

	[NetworkRpcWeavedInvoker(96, 1, 7)]
	[Preserve]
	protected unsafe static void RpcSyncLobbyObjectPickedUp_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte itemUid = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncLobbyObjectPickedUp(itemUid);
	}

	[NetworkRpcWeavedInvoker(97, 7, 1)]
	[Preserve]
	protected unsafe static void RPCSyncAmountInventory_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxInventory = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSyncAmountInventory(idxInventory, amount);
	}

	[NetworkRpcWeavedInvoker(98, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncEventTrigger_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isCollided = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSyncEventTrigger(uniqueID, isCollided);
	}

	[NetworkRpcWeavedInvoker(99, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetDisconnectedOnLobby_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetDisconnectedOnLobby(value);
	}

	[NetworkRpcWeavedInvoker(100, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetHealingValue_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte value = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idxBar = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetHealingValue(value, idxBar);
	}

	[NetworkRpcWeavedInvoker(101, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetInteractingPuzzle_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool value = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetInteractingPuzzle(value);
	}

	[NetworkRpcWeavedInvoker(102, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetSteamID_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		long num2 = *(long*)(data + num);
		num += 8;
		ulong steamID = (ulong)num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetSteamID(steamID);
	}

	[NetworkRpcWeavedInvoker(103, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSyncDurability_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int idxInventory = num2;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float durability = num3;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSyncDurability(idxInventory, durability);
	}

	[NetworkRpcWeavedInvoker(104, 7, 1)]
	[Preserve]
	protected unsafe static void RpcSetFriendPass_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool v = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RpcSetFriendPass(v);
	}

	[NetworkRpcWeavedInvoker(105, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSetAdditionalSpeed_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float speed = num2;
		behaviour.InvokeRpc = true;
		((PlayerPhotonNetwork)behaviour).RPCSetAdditionalSpeed(speed);
	}
}
