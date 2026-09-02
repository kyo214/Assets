namespace Fusion.Sockets;

public enum NetConnectFailedReason : byte
{
	Timeout = 1,
	ServerFull = 2,
	ServerRefused = 3
}
