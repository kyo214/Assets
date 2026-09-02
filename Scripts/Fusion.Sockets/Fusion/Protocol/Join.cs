using System;

namespace Fusion.Protocol;

internal class Join : Message
{
	public JoinMessageType Type;

	public PluginGameMode GameMode;

	public PeerMode PeerMode;

	public JoinRequests JoinRequests;

	public byte[] UniqueId;

	public Join()
	{
	}

	public Join(JoinMessageType type, PluginGameMode mode, PeerMode peerMode, JoinRequests joinRequests = JoinRequests.None, byte[] uniqueID = null, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null)
		: base(protocolVersion, serializationVersion)
	{
		Type = type;
		GameMode = mode;
		JoinRequests = joinRequests;
		PeerMode = peerMode;
		UniqueId = uniqueID;
	}

	protected override void SerializeProtected(BitStream stream)
	{
		byte value = (byte)Type;
		byte value2 = (byte)GameMode;
		byte value3 = (byte)PeerMode;
		uint value4 = (uint)JoinRequests;
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
		stream.Serialize(ref value4);
		if ((int)ProtocolVersion >= 6)
		{
			stream.Serialize(ref UniqueId);
		}
		Type = (JoinMessageType)value;
		GameMode = (PluginGameMode)value2;
		PeerMode = (PeerMode)value3;
		JoinRequests = (JoinRequests)value4;
	}

	public override string ToString()
	{
		long num = ((UniqueId != null && UniqueId.Length == 8) ? BitConverter.ToInt64(UniqueId, 0) : 0);
		return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, {11}]", "Join", "Type", Type, "GameMode", GameMode, "PeerMode", PeerMode, "JoinRequests", JoinRequests, "UniqueId", num, base.ToString());
	}
}
