using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct gameserveritem_t
{
	internal servernetadr_t NetAdr;

	internal int Ping;

	[MarshalAs(UnmanagedType.I1)]
	internal bool HadSuccessfulResponse;

	[MarshalAs(UnmanagedType.I1)]
	internal bool DoNotRefresh;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	internal byte[] GameDir;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	internal byte[] Map;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	internal byte[] GameDescription;

	internal uint AppID;

	internal int Players;

	internal int MaxPlayers;

	internal int BotPlayers;

	[MarshalAs(UnmanagedType.I1)]
	internal bool Password;

	[MarshalAs(UnmanagedType.I1)]
	internal bool Secure;

	internal uint TimeLastPlayed;

	internal int ServerVersion;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	internal byte[] ServerName;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	internal byte[] GameTags;

	internal ulong SteamID;

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_gameserveritem_t_Construct")]
	internal static extern void InternalConstruct(ref gameserveritem_t self);

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_gameserveritem_t_GetName")]
	internal static extern Utf8StringPointer InternalGetName(ref gameserveritem_t self);

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_gameserveritem_t_SetName")]
	internal static extern void InternalSetName(ref gameserveritem_t self, IntPtr pName);

	internal string GameDirUTF8()
	{
		return Utility.Utf8NoBom.GetString(GameDir, 0, Array.IndexOf(GameDir, (byte)0));
	}

	internal string MapUTF8()
	{
		return Utility.Utf8NoBom.GetString(Map, 0, Array.IndexOf(Map, (byte)0));
	}

	internal string GameDescriptionUTF8()
	{
		return Utility.Utf8NoBom.GetString(GameDescription, 0, Array.IndexOf(GameDescription, (byte)0));
	}

	internal string ServerNameUTF8()
	{
		return Utility.Utf8NoBom.GetString(ServerName, 0, Array.IndexOf(ServerName, (byte)0));
	}

	internal string GameTagsUTF8()
	{
		return Utility.Utf8NoBom.GetString(GameTags, 0, Array.IndexOf(GameTags, (byte)0));
	}
}
