using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dissonance.Extensions;
using Dissonance.Networking.Client;
using JetBrains.Annotations;

namespace Dissonance.Networking;

internal readonly struct ClientInfo
{
	public string PlayerName { get; }

	public ushort PlayerId { get; }

	public CodecSettings CodecSettings { get; }

	public ClientInfo(string playerName, ushort playerId, CodecSettings codecSettings)
	{
		PlayerName = playerName;
		PlayerId = playerId;
		CodecSettings = codecSettings;
	}
}
public class ClientInfo<TPeer> : IEquatable<ClientInfo<TPeer>>
{
	private static readonly Log Log = Logs.Create(LogCategory.Network, typeof(ClientInfo<TPeer>).Name);

	private readonly List<string> _rooms = new List<string>();

	[NotNull]
	public string PlayerName { get; }

	public ushort PlayerId { get; }

	public CodecSettings CodecSettings { get; }

	[NotNull]
	internal ReadOnlyCollection<string> Rooms { get; }

	[CanBeNull]
	public TPeer Connection { get; internal set; }

	public bool IsConnected { get; internal set; }

	internal PeerVoiceReceiver VoiceReceiver { get; set; }

	public ClientInfo(string playerName, ushort playerId, CodecSettings codecSettings, [CanBeNull] TPeer connection)
	{
		Rooms = new ReadOnlyCollection<string>(_rooms);
		PlayerName = playerName;
		PlayerId = playerId;
		CodecSettings = codecSettings;
		Connection = connection;
		IsConnected = true;
	}

	public override string ToString()
	{
		return $"Client '{PlayerName}/{PlayerId}/{Connection}'";
	}

	public bool Equals(ClientInfo<TPeer> other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (string.Equals(PlayerName, other.PlayerName))
		{
			return PlayerId == other.PlayerId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((ClientInfo<TPeer>)obj);
	}

	public override int GetHashCode()
	{
		return (PlayerName.GetFnvHashCode() * 397) ^ PlayerId.GetHashCode();
	}

	public bool AddRoom([NotNull] string roomName)
	{
		if (roomName == null)
		{
			throw new ArgumentNullException("roomName");
		}
		int num = _rooms.BinarySearch(roomName);
		if (num < 0)
		{
			_rooms.Insert(~num, roomName);
			return true;
		}
		return false;
	}

	public bool RemoveRoom([NotNull] string roomName)
	{
		if (roomName == null)
		{
			throw new ArgumentNullException("roomName");
		}
		int num = _rooms.BinarySearch(roomName);
		if (num >= 0)
		{
			_rooms.RemoveAt(num);
			return true;
		}
		return false;
	}
}
