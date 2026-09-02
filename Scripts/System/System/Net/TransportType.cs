namespace System.Net;

public enum TransportType
{
	Udp = 1,
	Connectionless = Udp,
	Tcp = 2,
	ConnectionOriented = Tcp,
	All = 3
}
