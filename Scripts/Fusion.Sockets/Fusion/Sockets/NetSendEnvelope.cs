namespace Fusion.Sockets;

public struct NetSendEnvelope
{
	public unsafe void* UserData;

	public ushort Sequence;

	public double SendTime;

	internal NetPacketType PacketType;
}
