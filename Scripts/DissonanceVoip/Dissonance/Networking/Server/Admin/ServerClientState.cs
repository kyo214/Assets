using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dissonance.Datastructures;
using Dissonance.Networking.Client;
using JetBrains.Annotations;

namespace Dissonance.Networking.Server.Admin;

internal class ServerClientState<TServer, TClient, TPeer> : IServerClientState<TPeer>, IServerClientState where TServer : BaseServer<TServer, TClient, TPeer> where TClient : BaseClient<TServer, TClient, TPeer> where TPeer : struct, IEquatable<TPeer>
{
	private class VoiceEventHandler : IVoiceEventQueue
	{
		private readonly ServerClientState<TServer, TClient, TPeer> _parent;

		private readonly ConcurrentPool<byte[]> _bytesPool = new ConcurrentPool<byte[]>(4, () => new byte[1024]);

		public VoiceEventHandler(ServerClientState<TServer, TClient, TPeer> parent)
		{
			_parent = parent;
		}

		public void EnqueueStoppedSpeaking(string name)
		{
			_parent.StoppedSpeaking?.Invoke();
		}

		public void EnqueueStartedSpeaking(string name)
		{
			_parent.StartedSpeaking?.Invoke();
		}

		public void EnqueueVoiceData(VoicePacket voicePacket)
		{
			_parent.OnVoicePacket?.Invoke(voicePacket);
			ArraySegment<byte> encodedAudioFrame = voicePacket.EncodedAudioFrame;
			byte[] array = encodedAudioFrame.Array;
			if (array != null)
			{
				_bytesPool.Put(array);
			}
		}

		public byte[] GetEventBuffer()
		{
			return _bytesPool.Get();
		}
	}

	private static readonly Log Log = new Log(2, typeof(ServerClientState<TServer, TClient, TPeer>).Name);

	private readonly TServer _server;

	private byte _currentVoiceSession;

	private uint _previousSequenceNumber;

	private readonly PacketLossCalculator _packetLoss = new PacketLossCalculator(128u);

	private readonly PeerVoiceReceiver _voiceReceiver;

	private readonly List<string> _rooms;

	private readonly List<RemoteChannel> _channels;

	public ClientInfo<TPeer> Peer { get; }

	public string Name => Peer.PlayerName;

	public bool IsConnected => Peer.IsConnected;

	public float PacketLoss => _packetLoss.PacketLoss;

	public ReadOnlyCollection<string> Rooms { get; }

	public ReadOnlyCollection<RemoteChannel> Channels { get; }

	public DateTime LastChannelUpdateUtc { get; private set; }

	public event Action<IServerClientState, string> OnStartedListeningToRoom;

	public event Action<IServerClientState, string> OnStoppedListeningToRoom;

	public event Action StartedSpeaking;

	public event Action StoppedSpeaking;

	public event Action<VoicePacket> OnVoicePacket;

	public ServerClientState(TServer server, ClientInfo<TPeer> peer)
	{
		_server = server;
		Peer = peer;
		_rooms = new List<string>();
		Rooms = new ReadOnlyCollection<string>(_rooms);
		_channels = new List<RemoteChannel>();
		Channels = new ReadOnlyCollection<RemoteChannel>(_channels);
		_voiceReceiver = new PeerVoiceReceiver(peer.PlayerName, peer.PlayerId, "572a03f5a51c41f8b2a9b8d3b498dc33", new VoiceEventHandler(this), new Rooms(), new ConcurrentPool<List<RemoteChannel>>(0, () => new List<RemoteChannel>()))
		{
			ReceiveAllVoicePackets = true
		};
	}

	public void RemoveFromRoom([NotNull] string roomName)
	{
		if (roomName == null)
		{
			throw new ArgumentNullException("roomName");
		}
		PacketWriter packetWriter = new PacketWriter(new byte[10 + roomName.Length * 4]);
		packetWriter.WriteDeltaChannelState(_server.SessionId, joined: false, Peer.PlayerId, roomName);
		_server.NetworkReceivedPacket(Peer.Connection, packetWriter.Written);
	}

	public void Reset()
	{
		PacketWriter packetWriter = new PacketWriter(new byte[7]);
		packetWriter.WriteErrorWrongSession(_server.SessionId + 1);
		_server.SendUnreliable(new List<TPeer> { Peer.Connection }, packetWriter.Written);
	}

	public void InvokeOnEnteredRoom(string name)
	{
		if (!_rooms.Contains(name))
		{
			_rooms.Add(name);
		}
		Action<IServerClientState, string> action = OnStartedListeningToRoom;
		if (action != null)
		{
			try
			{
				action(this, name);
			}
			catch (Exception p)
			{
				Log.Error("Exception encountered invoking `PlayerJoined` event handler: {0}", p);
			}
		}
	}

	public void InvokeOnExitedRoom(string name)
	{
		_rooms.Remove(name);
		Action<IServerClientState, string> action = OnStoppedListeningToRoom;
		if (action != null)
		{
			try
			{
				action(this, name);
			}
			catch (Exception p)
			{
				Log.Error("Exception encountered invoking `PlayerJoined` event handler: {0}", p);
			}
		}
	}

	public void UpdateChannels([NotNull] List<RemoteChannel> channels)
	{
		_channels.Clear();
		_channels.AddRange(channels);
		LastChannelUpdateUtc = DateTime.UtcNow;
	}

	public void InvokeOnVoicePacket(PacketReader reader)
	{
		reader.ReadVoicePacketHeader1(out var senderId);
		if (senderId == Peer.PlayerId)
		{
			PacketReader packetReader = reader;
			packetReader.ReadVoicePacketHeader2(out var options, out var sequenceNumber, out var _);
			if (options.ChannelSession != _currentVoiceSession)
			{
				_previousSequenceNumber = sequenceNumber;
				_currentVoiceSession = options.ChannelSession;
			}
			else
			{
				bool flag = sequenceNumber != _previousSequenceNumber + 1;
				_packetLoss.Update(!flag);
				_previousSequenceNumber = sequenceNumber;
			}
			_voiceReceiver.ReceivePacket(ref reader, DateTime.UtcNow);
		}
	}
}
