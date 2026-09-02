using System;

namespace Mono.Security.Interface;

[Flags]
public enum TlsProtocols
{
	Zero = 0,
	Tls10Client = 0x80,
	Tls10Server = 0x40,
	Tls10 = Tls10Client | Tls10Server,
	Tls11Client = 0x200,
	Tls11Server = 0x100,
	Tls11 = Tls11Client | Tls11Server,
	Tls12Client = 0x800,
	Tls12Server = 0x400,
	Tls12 = Tls12Client | Tls12Server,
	ClientMask = Tls10Client | Tls11Client | Tls12Client,
	ServerMask = Tls10Server | Tls11Server | Tls12Server
}
