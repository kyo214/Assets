using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Dissonance.Networking.Server.Admin;

public interface IServerClientState
{
	[NotNull]
	string Name { get; }

	bool IsConnected { get; }

	ReadOnlyCollection<string> Rooms { get; }

	ReadOnlyCollection<RemoteChannel> Channels { get; }

	DateTime LastChannelUpdateUtc { get; }

	float PacketLoss { get; }

	event Action<IServerClientState, string> OnStartedListeningToRoom;

	event Action<IServerClientState, string> OnStoppedListeningToRoom;

	event Action StartedSpeaking;

	event Action StoppedSpeaking;

	event Action<VoicePacket> OnVoicePacket;

	void RemoveFromRoom(string roomName);

	void Reset();
}
public interface IServerClientState<TPeer> : IServerClientState
{
	ClientInfo<TPeer> Peer { get; }
}
