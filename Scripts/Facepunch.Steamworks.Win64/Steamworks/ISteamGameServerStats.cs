using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamGameServerStats : SteamInterface
{
	public const string Version = "SteamGameServerStats001";

	internal ISteamGameServerStats(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamGameServerStats_v001();

	public override IntPtr GetServerInterfacePointer()
	{
		return SteamAPI_SteamGameServerStats_v001();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_RequestUserStats")]
	private static extern SteamAPICall_t _RequestUserStats(IntPtr self, SteamId steamIDUser);

	internal CallResult<GSStatsReceived_t> RequestUserStats(SteamId steamIDUser)
	{
		return new CallResult<GSStatsReceived_t>(_RequestUserStats(Self, steamIDUser), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_GetUserStatInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, ref int pData);

	internal bool GetUserStat(SteamId steamIDUser, string pchName, ref int pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_GetUserStatFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, ref float pData);

	internal bool GetUserStat(SteamId steamIDUser, string pchName, ref float pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_GetUserAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserAchievement(IntPtr self, SteamId steamIDUser, IntPtr pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

	internal bool GetUserAchievement(SteamId steamIDUser, string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserAchievement(Self, steamIDUser, utf8StringToNative.Pointer, ref pbAchieved);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_SetUserStatInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, int nData);

	internal bool SetUserStat(SteamId steamIDUser, string pchName, int nData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, nData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_SetUserStatFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, float fData);

	internal bool SetUserStat(SteamId steamIDUser, string pchName, float fData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, fData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _UpdateUserAvgRateStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, float flCountThisSession, double dSessionLength);

	internal bool UpdateUserAvgRateStat(SteamId steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _UpdateUserAvgRateStat(Self, steamIDUser, utf8StringToNative.Pointer, flCountThisSession, dSessionLength);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_SetUserAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetUserAchievement(IntPtr self, SteamId steamIDUser, IntPtr pchName);

	internal bool SetUserAchievement(SteamId steamIDUser, string pchName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetUserAchievement(Self, steamIDUser, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_ClearUserAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _ClearUserAchievement(IntPtr self, SteamId steamIDUser, IntPtr pchName);

	internal bool ClearUserAchievement(SteamId steamIDUser, string pchName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _ClearUserAchievement(Self, steamIDUser, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServerStats_StoreUserStats")]
	private static extern SteamAPICall_t _StoreUserStats(IntPtr self, SteamId steamIDUser);

	internal CallResult<GSStatsStored_t> StoreUserStats(SteamId steamIDUser)
	{
		return new CallResult<GSStatsStored_t>(_StoreUserStats(Self, steamIDUser), base.IsServer);
	}
}
