using System.Net;

namespace Steamworks;

public struct SteamServerInit(string modDir, string gameDesc)
{
	public IPAddress IpAddress = null;

	public ushort GamePort = 27015;

	public ushort QueryPort = 27016;

	public bool Secure = true;

	public string VersionString = "1.0.0.0";

	public string ModDir = modDir;

	public string GameDescription = gameDesc;

	public bool DedicatedServer = true;

	public SteamServerInit WithQueryShareGamePort()
	{
		QueryPort = ushort.MaxValue;
		return this;
	}
}
