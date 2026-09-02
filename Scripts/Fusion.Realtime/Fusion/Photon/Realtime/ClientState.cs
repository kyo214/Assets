using System;

namespace Fusion.Photon.Realtime;

internal enum ClientState
{
	PeerCreated = 0,
	Authenticating = 1,
	Authenticated = 2,
	JoiningLobby = 3,
	JoinedLobby = 4,
	DisconnectingFromMasterServer = 5,
	[Obsolete("Renamed to DisconnectingFromMasterServer")]
	DisconnectingFromMasterserver = DisconnectingFromMasterServer,
	ConnectingToGameServer = 6,
	[Obsolete("Renamed to ConnectingToGameServer")]
	ConnectingToGameserver = ConnectingToGameServer,
	ConnectedToGameServer = 7,
	[Obsolete("Renamed to ConnectedToGameServer")]
	ConnectedToGameserver = ConnectedToGameServer,
	Joining = 8,
	Joined = 9,
	Leaving = 10,
	DisconnectingFromGameServer = 11,
	[Obsolete("Renamed to DisconnectingFromGameServer")]
	DisconnectingFromGameserver = DisconnectingFromGameServer,
	ConnectingToMasterServer = 12,
	[Obsolete("Renamed to ConnectingToMasterServer.")]
	ConnectingToMasterserver = ConnectingToMasterServer,
	Disconnecting = 13,
	Disconnected = 14,
	ConnectedToMasterServer = 15,
	[Obsolete("Renamed to ConnectedToMasterServer.")]
	ConnectedToMasterserver = ConnectedToMasterServer,
	[Obsolete("Renamed to ConnectedToMasterServer.")]
	ConnectedToMaster = ConnectedToMasterServer,
	ConnectingToNameServer = 16,
	ConnectedToNameServer = 17,
	DisconnectingFromNameServer = 18,
	ConnectWithFallbackProtocol = 19
}
