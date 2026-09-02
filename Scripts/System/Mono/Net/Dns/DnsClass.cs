namespace Mono.Net.Dns;

internal enum DnsClass : ushort
{
	Internet = 1,
	IN = Internet,
	CSNET = 2,
	CS = CSNET,
	CHAOS = 3,
	CH = CHAOS,
	Hesiod = 4,
	HS = Hesiod
}
