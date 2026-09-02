using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamUserStats : SteamInterface
{
	public const string Version = "STEAMUSERSTATS_INTERFACE_VERSION013";

	internal ISteamUserStats(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamUserStats_v013();

	public override IntPtr GetUserInterfacePointer()
	{
		return SteamAPI_SteamUserStats_v013();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetStatInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetStat(IntPtr self, IntPtr pchName, ref int pData);

	internal bool GetStat(string pchName, ref int pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetStat(Self, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetStatFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetStat(IntPtr self, IntPtr pchName, ref float pData);

	internal bool GetStat(string pchName, ref float pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetStat(Self, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetStatInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetStat(IntPtr self, IntPtr pchName, int nData);

	internal bool SetStat(string pchName, int nData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetStat(Self, utf8StringToNative.Pointer, nData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetStatFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetStat(IntPtr self, IntPtr pchName, float fData);

	internal bool SetStat(string pchName, float fData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetStat(Self, utf8StringToNative.Pointer, fData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_UpdateAvgRateStat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _UpdateAvgRateStat(IntPtr self, IntPtr pchName, float flCountThisSession, double dSessionLength);

	internal bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _UpdateAvgRateStat(Self, utf8StringToNative.Pointer, flCountThisSession, dSessionLength);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetAchievement(IntPtr self, IntPtr pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

	internal bool GetAchievement(string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievement(Self, utf8StringToNative.Pointer, ref pbAchieved);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetAchievement(IntPtr self, IntPtr pchName);

	internal bool SetAchievement(string pchName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _SetAchievement(Self, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_ClearAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _ClearAchievement(IntPtr self, IntPtr pchName);

	internal bool ClearAchievement(string pchName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _ClearAchievement(Self, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetAchievementAndUnlockTime(IntPtr self, IntPtr pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime);

	internal bool GetAchievementAndUnlockTime(string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievementAndUnlockTime(Self, utf8StringToNative.Pointer, ref pbAchieved, ref punUnlockTime);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_StoreStats")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _StoreStats(IntPtr self);

	internal bool StoreStats()
	{
		return _StoreStats(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementIcon")]
	private static extern int _GetAchievementIcon(IntPtr self, IntPtr pchName);

	internal int GetAchievementIcon(string pchName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievementIcon(Self, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute")]
	private static extern Utf8StringPointer _GetAchievementDisplayAttribute(IntPtr self, IntPtr pchName, IntPtr pchKey);

	internal string GetAchievementDisplayAttribute(string pchName, string pchKey)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchKey);
		return _GetAchievementDisplayAttribute(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_IndicateAchievementProgress")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _IndicateAchievementProgress(IntPtr self, IntPtr pchName, uint nCurProgress, uint nMaxProgress);

	internal bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _IndicateAchievementProgress(Self, utf8StringToNative.Pointer, nCurProgress, nMaxProgress);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNumAchievements")]
	private static extern uint _GetNumAchievements(IntPtr self);

	internal uint GetNumAchievements()
	{
		return _GetNumAchievements(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementName")]
	private static extern Utf8StringPointer _GetAchievementName(IntPtr self, uint iAchievement);

	internal string GetAchievementName(uint iAchievement)
	{
		return _GetAchievementName(Self, iAchievement);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestUserStats")]
	private static extern SteamAPICall_t _RequestUserStats(IntPtr self, SteamId steamIDUser);

	internal CallResult<UserStatsReceived_t> RequestUserStats(SteamId steamIDUser)
	{
		return new CallResult<UserStatsReceived_t>(_RequestUserStats(Self, steamIDUser), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserStatInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, ref int pData);

	internal bool GetUserStat(SteamId steamIDUser, string pchName, ref int pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserStatFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, IntPtr pchName, ref float pData);

	internal bool GetUserStat(SteamId steamIDUser, string pchName, ref float pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserStat(Self, steamIDUser, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserAchievement")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserAchievement(IntPtr self, SteamId steamIDUser, IntPtr pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

	internal bool GetUserAchievement(SteamId steamIDUser, string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserAchievement(Self, steamIDUser, utf8StringToNative.Pointer, ref pbAchieved);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetUserAchievementAndUnlockTime(IntPtr self, SteamId steamIDUser, IntPtr pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime);

	internal bool GetUserAchievementAndUnlockTime(SteamId steamIDUser, string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetUserAchievementAndUnlockTime(Self, steamIDUser, utf8StringToNative.Pointer, ref pbAchieved, ref punUnlockTime);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_ResetAllStats")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _ResetAllStats(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAchievementsToo);

	internal bool ResetAllStats([MarshalAs(UnmanagedType.U1)] bool bAchievementsToo)
	{
		return _ResetAllStats(Self, bAchievementsToo);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_FindOrCreateLeaderboard")]
	private static extern SteamAPICall_t _FindOrCreateLeaderboard(IntPtr self, IntPtr pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType);

	internal CallResult<LeaderboardFindResult_t> FindOrCreateLeaderboard(string pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchLeaderboardName);
		return new CallResult<LeaderboardFindResult_t>(_FindOrCreateLeaderboard(Self, utf8StringToNative.Pointer, eLeaderboardSortMethod, eLeaderboardDisplayType), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_FindLeaderboard")]
	private static extern SteamAPICall_t _FindLeaderboard(IntPtr self, IntPtr pchLeaderboardName);

	internal CallResult<LeaderboardFindResult_t> FindLeaderboard(string pchLeaderboardName)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchLeaderboardName);
		return new CallResult<LeaderboardFindResult_t>(_FindLeaderboard(Self, utf8StringToNative.Pointer), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardName")]
	private static extern Utf8StringPointer _GetLeaderboardName(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

	internal string GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
	{
		return _GetLeaderboardName(Self, hSteamLeaderboard);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardEntryCount")]
	private static extern int _GetLeaderboardEntryCount(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

	internal int GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
	{
		return _GetLeaderboardEntryCount(Self, hSteamLeaderboard);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardSortMethod")]
	private static extern LeaderboardSort _GetLeaderboardSortMethod(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

	internal LeaderboardSort GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
	{
		return _GetLeaderboardSortMethod(Self, hSteamLeaderboard);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardDisplayType")]
	private static extern LeaderboardDisplay _GetLeaderboardDisplayType(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

	internal LeaderboardDisplay GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
	{
		return _GetLeaderboardDisplayType(Self, hSteamLeaderboard);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_DownloadLeaderboardEntries")]
	private static extern SteamAPICall_t _DownloadLeaderboardEntries(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd);

	internal CallResult<LeaderboardScoresDownloaded_t> DownloadLeaderboardEntries(SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
	{
		return new CallResult<LeaderboardScoresDownloaded_t>(_DownloadLeaderboardEntries(Self, hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers")]
	private static extern SteamAPICall_t _DownloadLeaderboardEntriesForUsers(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers);

	internal CallResult<LeaderboardScoresDownloaded_t> DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers)
	{
		return new CallResult<LeaderboardScoresDownloaded_t>(_DownloadLeaderboardEntriesForUsers(Self, hSteamLeaderboard, prgUsers, cUsers), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetDownloadedLeaderboardEntry(IntPtr self, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax);

	internal bool GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax)
	{
		return _GetDownloadedLeaderboardEntry(Self, hSteamLeaderboardEntries, index, ref pLeaderboardEntry, pDetails, cDetailsMax);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_UploadLeaderboardScore")]
	private static extern SteamAPICall_t _UploadLeaderboardScore(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount);

	internal CallResult<LeaderboardScoreUploaded_t> UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount)
	{
		return new CallResult<LeaderboardScoreUploaded_t>(_UploadLeaderboardScore(Self, hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_AttachLeaderboardUGC")]
	private static extern SteamAPICall_t _AttachLeaderboardUGC(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC);

	internal CallResult<LeaderboardUGCSet_t> AttachLeaderboardUGC(SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC)
	{
		return new CallResult<LeaderboardUGCSet_t>(_AttachLeaderboardUGC(Self, hSteamLeaderboard, hUGC), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers")]
	private static extern SteamAPICall_t _GetNumberOfCurrentPlayers(IntPtr self);

	internal CallResult<NumberOfCurrentPlayers_t> GetNumberOfCurrentPlayers()
	{
		return new CallResult<NumberOfCurrentPlayers_t>(_GetNumberOfCurrentPlayers(Self), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages")]
	private static extern SteamAPICall_t _RequestGlobalAchievementPercentages(IntPtr self);

	internal CallResult<GlobalAchievementPercentagesReady_t> RequestGlobalAchievementPercentages()
	{
		return new CallResult<GlobalAchievementPercentagesReady_t>(_RequestGlobalAchievementPercentages(Self), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo")]
	private static extern int _GetMostAchievedAchievementInfo(IntPtr self, IntPtr pchName, uint unNameBufLen, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

	internal int GetMostAchievedAchievementInfo(out string pchName, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
	{
		using Helpers.Memory m = Helpers.TakeMemory();
		int result = _GetMostAchievedAchievementInfo(Self, m, 32768u, ref pflPercent, ref pbAchieved);
		pchName = Helpers.MemoryToString(m);
		return result;
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo")]
	private static extern int _GetNextMostAchievedAchievementInfo(IntPtr self, int iIteratorPrevious, IntPtr pchName, uint unNameBufLen, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

	internal int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, out string pchName, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
	{
		using Helpers.Memory m = Helpers.TakeMemory();
		int result = _GetNextMostAchievedAchievementInfo(Self, iIteratorPrevious, m, 32768u, ref pflPercent, ref pbAchieved);
		pchName = Helpers.MemoryToString(m);
		return result;
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementAchievedPercent")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetAchievementAchievedPercent(IntPtr self, IntPtr pchName, ref float pflPercent);

	internal bool GetAchievementAchievedPercent(string pchName, ref float pflPercent)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievementAchievedPercent(Self, utf8StringToNative.Pointer, ref pflPercent);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestGlobalStats")]
	private static extern SteamAPICall_t _RequestGlobalStats(IntPtr self, int nHistoryDays);

	internal CallResult<GlobalStatsReceived_t> RequestGlobalStats(int nHistoryDays)
	{
		return new CallResult<GlobalStatsReceived_t>(_RequestGlobalStats(Self, nHistoryDays), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatInt64")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetGlobalStat(IntPtr self, IntPtr pchStatName, ref long pData);

	internal bool GetGlobalStat(string pchStatName, ref long pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchStatName);
		return _GetGlobalStat(Self, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatDouble")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetGlobalStat(IntPtr self, IntPtr pchStatName, ref double pData);

	internal bool GetGlobalStat(string pchStatName, ref double pData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchStatName);
		return _GetGlobalStat(Self, utf8StringToNative.Pointer, ref pData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatHistoryInt64")]
	private static extern int _GetGlobalStatHistory(IntPtr self, IntPtr pchStatName, [In][Out] long[] pData, uint cubData);

	internal int GetGlobalStatHistory(string pchStatName, [In][Out] long[] pData, uint cubData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchStatName);
		return _GetGlobalStatHistory(Self, utf8StringToNative.Pointer, pData, cubData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatHistoryDouble")]
	private static extern int _GetGlobalStatHistory(IntPtr self, IntPtr pchStatName, [In][Out] double[] pData, uint cubData);

	internal int GetGlobalStatHistory(string pchStatName, [In][Out] double[] pData, uint cubData)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchStatName);
		return _GetGlobalStatHistory(Self, utf8StringToNative.Pointer, pData, cubData);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementProgressLimitsInt32")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetAchievementProgressLimits(IntPtr self, IntPtr pchName, ref int pnMinProgress, ref int pnMaxProgress);

	internal bool GetAchievementProgressLimits(string pchName, ref int pnMinProgress, ref int pnMaxProgress)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievementProgressLimits(Self, utf8StringToNative.Pointer, ref pnMinProgress, ref pnMaxProgress);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementProgressLimitsFloat")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetAchievementProgressLimits(IntPtr self, IntPtr pchName, ref float pfMinProgress, ref float pfMaxProgress);

	internal bool GetAchievementProgressLimits(string pchName, ref float pfMinProgress, ref float pfMaxProgress)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchName);
		return _GetAchievementProgressLimits(Self, utf8StringToNative.Pointer, ref pfMinProgress, ref pfMaxProgress);
	}
}
