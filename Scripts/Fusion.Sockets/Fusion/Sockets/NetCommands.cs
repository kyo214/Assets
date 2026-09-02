namespace Fusion.Sockets;

internal enum NetCommands : byte
{
	Connect = 1,
	Accepted = 2,
	Refused = 3,
	Disconnect = 4,
	Ping = 5
}
