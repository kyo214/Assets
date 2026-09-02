#define DEBUG
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion.Photon.Realtime.Extension;
using Fusion.Protocol;
using UnityEngine;

namespace Fusion.Photon.Realtime;

internal class FusionRelayClient : LoadBalancingClient, IInRoomCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
{
	private const int KeepAliveInBackground = 60000;

	private const int _delayBetweenPings = 100;

	private volatile bool _ackSent = false;

	private volatile byte _fallbackThreadId = byte.MaxValue;

	public const string FUSION_PLUGIN_NAME = "FusionPlugin";

	private const string SERVER_HOST_CN = "ns.photonengine.cn";

	private const string REGION_CN_ID = "cn";

	private readonly RaiseEventOptions _raiseEventOptions;

	private readonly SendOptions _optionsUnreliable;

	private readonly SendOptions _optionsReliable;

	private readonly ICommunicator _communicator;

	private GameObject _loggerGO;

	public bool IsReadyAndInRoom => base.IsConnectedAndReady && base.InRoom;

	public bool UseDefaultPorts { get; set; }

	public int DisconnectTimeout
	{
		get
		{
			return base.LoadBalancingPeer.DisconnectTimeout;
		}
		set
		{
			base.LoadBalancingPeer.DisconnectTimeout = value;
		}
	}

	public event Action OnRoomChanged;

	public void StartFallbackSendAck()
	{
		if (_fallbackThreadId >= byte.MaxValue)
		{
			_fallbackThreadId = SupportClass.StartBackgroundCalls(BackgroundAcksTask, 50);
			_ackSent = false;
			Log.InfoRealtime("Starting background connection handler");
		}
	}

	public void StopFallbackSendAck()
	{
		if (_fallbackThreadId < byte.MaxValue)
		{
			if (SupportClass.StopBackgroundCalls(_fallbackThreadId))
			{
				Log.InfoRealtime("Background connection handler stopped");
			}
			_fallbackThreadId = byte.MaxValue;
		}
	}

	private bool BackgroundAcksTask()
	{
		if (!base.IsConnected)
		{
			Log.InfoRealtime("Stopping Photon background connection handler by disconnect");
			return false;
		}
		int num = base.LoadBalancingPeer.ConnectionTime - base.LoadBalancingPeer.LastSendOutgoingTime;
		if (_ackSent && num > 60000)
		{
			Log.InfoRealtime("Stopping Photon background connection handler by timeout");
			_fallbackThreadId = byte.MaxValue;
			return false;
		}
		if (num > 100)
		{
			base.LoadBalancingPeer.SendAcksOnly();
			_ackSent = true;
		}
		else
		{
			_ackSent = false;
		}
		return true;
	}

	private void OnEventHandler(EventData evt)
	{
		int sender = evt.Sender;
		byte code = evt.Code;
		object customData = evt.CustomData;
		_communicator.PushPackage(sender, code, customData);
	}

	public unsafe bool SendEvent(int target, byte eventCode, byte* buffer, int bufferLength, bool reliable)
	{
		if (!IsReadyAndInRoom)
		{
			return false;
		}
		ByteArraySlice byteArraySlice = base.LoadBalancingPeer.ByteArraySlicePool.Acquire(bufferLength);
		if (buffer != null)
		{
			fixed (byte* buffer2 = byteArraySlice.Buffer)
			{
				Native.MemCpy(buffer2, buffer, bufferLength);
			}
		}
		byteArraySlice.Count = bufferLength;
		_raiseEventOptions.TargetActors[0] = target;
		bool result = OpRaiseEvent(eventCode, byteArraySlice, _raiseEventOptions, reliable ? _optionsReliable : _optionsUnreliable);
		if (base.LoadBalancingPeer.SendOutgoingCommands())
		{
			base.LoadBalancingPeer.SendOutgoingCommands();
		}
		return result;
	}

	public void ExtractData(object dataObj, byte[] buffer, ref int bufferLength)
	{
		if (dataObj is ByteArraySlice byteArraySlice)
		{
			Assert.Always(byteArraySlice.Count <= bufferLength, "Array slice to large for the buffer", bufferLength, byteArraySlice.Count);
			bufferLength = byteArraySlice.Count;
			Array.Copy(byteArraySlice.Buffer, buffer, bufferLength);
			byteArraySlice.Release();
		}
		else
		{
			bufferLength = -1;
		}
	}

	public FusionRelayClient(ICommunicator communicator)
	{
		base.ClientType = ClientAppType.Fusion;
		base.SerializationProtocol = SerializationProtocol.GpBinaryV18;
		base.LoadBalancingPeer.TimePingInterval = 200;
		base.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true;
		base.LoadBalancingPeer.ReuseEventInstance = true;
		base.LoadBalancingPeer.QuickResendAttempts = 4;
		base.LoadBalancingPeer.SentCountAllowance *= 10;
		base.LoadBalancingPeer.DisconnectTimeout = 15000;
		base.LoadBalancingPeer.SendWindowSize /= 3;
		base.LoadBalancingPeer.OnDisconnectMessage += OnDisconnectMessage;
		UseDefaultPorts = false;
		base.EventReceived += OnEventHandler;
		AddCallbackTarget(this);
		_raiseEventOptions = new RaiseEventOptions
		{
			TargetActors = new int[1]
		};
		_optionsUnreliable = new SendOptions
		{
			Channel = 0,
			DeliveryMode = DeliveryMode.UnreliableUnsequenced
		};
		_optionsReliable = new SendOptions
		{
			Channel = 1,
			DeliveryMode = DeliveryMode.Reliable
		};
		_communicator = communicator;
	}

	public void Reset()
	{
		UnityEngine.Object.Destroy(_loggerGO);
	}

	public override bool ConnectUsingSettings(AppSettings appSettings)
	{
		AppSettings copy = appSettings.GetCopy();
		ServerPortOverrides = default;
		if (RuntimeUnityFlagsSetup.IsUNITY_WEBGL)
		{
			copy.Protocol = ConnectionProtocol.WebSocketSecure;
			Log.InfoRealtime("Changing Photon Cloud Communication Protocol to WebSocketSecure");
		}
		if (copy.Protocol == ConnectionProtocol.Udp && !UseDefaultPorts)
		{
			ServerPortOverrides = PhotonPortDefinition.AlternativeUdpPorts;
			Log.InfoRealtime("Changing Photon Cloud Communication Ports to [" + string.Format("{0}={1},", "NameServerPort", ServerPortOverrides.NameServerPort) + string.Format("{0}={1},", "MasterServerPort", ServerPortOverrides.MasterServerPort) + string.Format("{0}={1}", "GameServerPort", ServerPortOverrides.GameServerPort) + "]");
		}
		if (copy.FixedRegion.ToLower().Equals("cn") && string.IsNullOrEmpty(copy.Server?.Trim()))
		{
			copy.Server = "ns.photonengine.cn";
		}
		return base.ConnectUsingSettings(copy);
	}

	public bool UpdateRoomProperties(Dictionary<string, SessionProperty> customProperties)
	{
		if (customProperties == null)
		{
			return false;
		}
		if (customProperties.Count == 0)
		{
			return false;
		}
		if (base.CurrentRoom.IsOffline)
		{
			return false;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Merge(base.CurrentRoom.CustomProperties);
		bool flag = false;
		foreach (string key in customProperties.Keys)
		{
			if (hashtable.ContainsKey(key))
			{
				if (!hashtable[key].Equals(customProperties[key].PropertyValue))
				{
					hashtable[key] = customProperties[key].PropertyValue;
					flag = true;
				}
			}
			else
			{
				Log.WarnRealtime("Invalid custom property key [" + key + "], ignore. Only existing custom properties can be updated.");
			}
		}
		if (hashtable.Count > 10)
		{
			Log.WarnRealtime("Max number of Custom Session Properties reached, only 10 properties are allowed.");
			return false;
		}
		int num = hashtable.CalculateTotalSize();
		if (num > 500)
		{
			Log.WarnRealtime($"Max size of Custom Session Properties reached, current size of {num} bytes, max 500 bytes are allowed.");
			return false;
		}
		return flag && base.CurrentRoom.SetCustomProperties(hashtable);
	}

	public bool UpdateRoomIsVisible(bool value)
	{
		if (base.CurrentRoom.IsOffline)
		{
			return false;
		}
		if (base.CurrentRoom.IsVisible == value)
		{
			return false;
		}
		base.CurrentRoom.IsVisible = value;
		return true;
	}

	public bool UpdateRoomIsOpen(bool value)
	{
		if (base.CurrentRoom.IsOffline)
		{
			return false;
		}
		if (base.CurrentRoom.IsOpen == value)
		{
			return false;
		}
		base.CurrentRoom.IsOpen = value;
		return true;
	}

	public void Update()
	{
		if (_loggerGO == null && (int)base.LoadBalancingPeer.DebugOut >= 2)
		{
			SupportLogger supportLogger = UnityEngine.Object.FindObjectOfType<SupportLogger>();
			if (supportLogger == null)
			{
				_loggerGO = new GameObject
				{
					name = "RealtimeLogger",
					hideFlags = HideFlags.NotEditable
				};
				UnityEngine.Object.DontDestroyOnLoad(_loggerGO);
				supportLogger = _loggerGO.AddComponent<SupportLogger>();
			}
			supportLogger.Client = this;
			_loggerGO = supportLogger.gameObject;
		}
		try
		{
			Service();
		}
		catch (Exception msg)
		{
			Log.ErrorRealtime(msg);
		}
	}

	private void OnDisconnectMessage(DisconnectMessage obj)
	{
		Log.ErrorRealtime($"DisconnectMessage. Code: {obj.Code} Msg: \"{obj.DebugMessage}\". Debug Info: {obj.Parameters.ToStringFull()}");
	}

	public static EnterRoomParams BuildEnterRoomParams(TypedLobby typedLobby, string roomName, int maxPlayers, Dictionary<string, SessionProperty> customProperties = null, bool isOpen = true, bool isVisible = true, bool extendedTtl = false)
	{
		BuildSessionCustomPropertyHolders(customProperties, out var sessionCustomProperties, out var publicSessionProperties);
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.Lobby = typedLobby;
		enterRoomParams.RoomOptions = new RoomOptions
		{
			MaxPlayers = maxPlayers,
			IsOpen = isOpen,
			IsVisible = isVisible,
			DeleteNullProperties = true,
			PlayerTtl = (extendedTtl ? 15000 : 0),
			EmptyRoomTtl = 0,
			Plugins = new string[1] { "FusionPlugin" },
			SuppressRoomEvents = false,
			SuppressPlayerInfo = false,
			PublishUserId = true,
			CustomRoomProperties = sessionCustomProperties,
			CustomRoomPropertiesForLobby = publicSessionProperties
		};
		return enterRoomParams;
	}

	public static OpJoinRandomRoomParams BuildJoinParams(TypedLobby typedLobby, Dictionary<string, SessionProperty> customProperties = null, MatchmakingMode matchmakingMode = MatchmakingMode.FillRoom)
	{
		BuildSessionCustomPropertyHolders(customProperties, out var sessionCustomProperties, out var _);
		return new OpJoinRandomRoomParams
		{
			MatchingType = matchmakingMode,
			TypedLobby = typedLobby,
			ExpectedCustomRoomProperties = sessionCustomProperties
		};
	}

	private static void BuildSessionCustomPropertyHolders(Dictionary<string, SessionProperty> customProperties, out Hashtable sessionCustomProperties, out string[] publicSessionProperties)
	{
		sessionCustomProperties = null;
		publicSessionProperties = null;
		if (customProperties != null && customProperties.Count > 0)
		{
			sessionCustomProperties = customProperties.ConvertToHashtable();
			publicSessionProperties = new List<string>(customProperties.Keys).ToArray();
		}
	}

	public void OnJoinedRoom()
	{
		StartFallbackSendAck();
	}

	public void OnLeftRoom()
	{
		StopFallbackSendAck();
	}

	public void OnCreatedRoom()
	{
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
	}

	public void OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
	}

	public void OnJoinedLobby()
	{
		StartFallbackSendAck();
	}

	public void OnLeftLobby()
	{
		StopFallbackSendAck();
	}

	public void OnRoomListUpdate(List<RoomInfo> roomList)
	{
	}

	public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
	{
	}

	public void OnMasterClientSwitched(Player newMasterClient)
	{
		OnRoomChanged?.Invoke();
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		OnRoomChanged?.Invoke();
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		OnRoomChanged?.Invoke();
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		OnRoomChanged?.Invoke();
	}
}
