namespace Fusion.Sockets;

internal enum NetPacketType : byte
{
	Command = 1,
	UnreliableData = 2,
	NotifyData = 3,
	NotifyAcks = 4,
	Unconnected = 5,
	MtuDiscoveryReq = 6,
	MtuDiscoveryRep = 7,
	NotifyReliableData = 8
}
