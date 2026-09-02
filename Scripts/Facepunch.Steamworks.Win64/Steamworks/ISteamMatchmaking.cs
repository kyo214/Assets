using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamMatchmaking : SteamInterface
{
	public const string Version = "SteamMatchMaking009";

	internal ISteamMatchmaking(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamMatchmaking_v009();

	public override IntPtr GetUserInterfacePointer()
	{
		return SteamAPI_SteamMatchmaking_v009();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetFavoriteGameCount")]
	private static extern int _GetFavoriteGameCount(IntPtr self);

	internal int GetFavoriteGameCount()
	{
		return _GetFavoriteGameCount(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetFavoriteGame")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetFavoriteGame(IntPtr self, int iGame, ref AppId pnAppID, ref uint pnIP, ref ushort pnConnPort, ref ushort pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer);

	internal bool GetFavoriteGame(int iGame, ref AppId pnAppID, ref uint pnIP, ref ushort pnConnPort, ref ushort pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer)
	{
		return _GetFavoriteGame(Self, iGame, ref pnAppID, ref pnIP, ref pnConnPort, ref pnQueryPort, ref punFlags, ref pRTime32LastPlayedOnServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddFavoriteGame")]
	private static extern int _AddFavoriteGame(IntPtr self, AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer);

	internal int AddFavoriteGame(AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
	{
		return _AddFavoriteGame(Self, nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_RemoveFavoriteGame")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _RemoveFavoriteGame(IntPtr self, AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags);

	internal bool RemoveFavoriteGame(AppId nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags)
	{
		return _RemoveFavoriteGame(Self, nAppID, nIP, nConnPort, nQueryPort, unFlags);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_RequestLobbyList")]
	private static extern SteamAPICall_t _RequestLobbyList(IntPtr self);

	internal CallResult<LobbyMatchList_t> RequestLobbyList()
	{
		return new CallResult<LobbyMatchList_t>(_RequestLobbyList(Self), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter")]
	private static extern void _AddRequestLobbyListStringFilter(IntPtr self, IntPtr pchKeyToMatch, IntPtr pchValueToMatch, LobbyComparison eComparisonType);

	internal void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, LobbyComparison eComparisonType)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKeyToMatch);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchValueToMatch);
		_AddRequestLobbyListStringFilter(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, eComparisonType);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter")]
	private static extern void _AddRequestLobbyListNumericalFilter(IntPtr self, IntPtr pchKeyToMatch, int nValueToMatch, LobbyComparison eComparisonType);

	internal void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, LobbyComparison eComparisonType)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKeyToMatch);
		_AddRequestLobbyListNumericalFilter(Self, utf8StringToNative.Pointer, nValueToMatch, eComparisonType);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter")]
	private static extern void _AddRequestLobbyListNearValueFilter(IntPtr self, IntPtr pchKeyToMatch, int nValueToBeCloseTo);

	internal void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKeyToMatch);
		_AddRequestLobbyListNearValueFilter(Self, utf8StringToNative.Pointer, nValueToBeCloseTo);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable")]
	private static extern void _AddRequestLobbyListFilterSlotsAvailable(IntPtr self, int nSlotsAvailable);

	internal void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
	{
		_AddRequestLobbyListFilterSlotsAvailable(Self, nSlotsAvailable);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter")]
	private static extern void _AddRequestLobbyListDistanceFilter(IntPtr self, LobbyDistanceFilter eLobbyDistanceFilter);

	internal void AddRequestLobbyListDistanceFilter(LobbyDistanceFilter eLobbyDistanceFilter)
	{
		_AddRequestLobbyListDistanceFilter(Self, eLobbyDistanceFilter);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter")]
	private static extern void _AddRequestLobbyListResultCountFilter(IntPtr self, int cMaxResults);

	internal void AddRequestLobbyListResultCountFilter(int cMaxResults)
	{
		_AddRequestLobbyListResultCountFilter(Self, cMaxResults);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter")]
	private static extern void _AddRequestLobbyListCompatibleMembersFilter(IntPtr self, SteamId steamIDLobby);

	internal void AddRequestLobbyListCompatibleMembersFilter(SteamId steamIDLobby)
	{
		_AddRequestLobbyListCompatibleMembersFilter(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyByIndex")]
	private static extern SteamId _GetLobbyByIndex(IntPtr self, int iLobby);

	internal SteamId GetLobbyByIndex(int iLobby)
	{
		return _GetLobbyByIndex(Self, iLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_CreateLobby")]
	private static extern SteamAPICall_t _CreateLobby(IntPtr self, LobbyType eLobbyType, int cMaxMembers);

	internal CallResult<LobbyCreated_t> CreateLobby(LobbyType eLobbyType, int cMaxMembers)
	{
		return new CallResult<LobbyCreated_t>(_CreateLobby(Self, eLobbyType, cMaxMembers), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_JoinLobby")]
	private static extern SteamAPICall_t _JoinLobby(IntPtr self, SteamId steamIDLobby);

	internal CallResult<LobbyEnter_t> JoinLobby(SteamId steamIDLobby)
	{
		return new CallResult<LobbyEnter_t>(_JoinLobby(Self, steamIDLobby), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_LeaveLobby")]
	private static extern void _LeaveLobby(IntPtr self, SteamId steamIDLobby);

	internal void LeaveLobby(SteamId steamIDLobby)
	{
		_LeaveLobby(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_InviteUserToLobby")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _InviteUserToLobby(IntPtr self, SteamId steamIDLobby, SteamId steamIDInvitee);

	internal bool InviteUserToLobby(SteamId steamIDLobby, SteamId steamIDInvitee)
	{
		return _InviteUserToLobby(Self, steamIDLobby, steamIDInvitee);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetNumLobbyMembers")]
	private static extern int _GetNumLobbyMembers(IntPtr self, SteamId steamIDLobby);

	internal int GetNumLobbyMembers(SteamId steamIDLobby)
	{
		return _GetNumLobbyMembers(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex")]
	private static extern SteamId _GetLobbyMemberByIndex(IntPtr self, SteamId steamIDLobby, int iMember);

	internal SteamId GetLobbyMemberByIndex(SteamId steamIDLobby, int iMember)
	{
		return _GetLobbyMemberByIndex(Self, steamIDLobby, iMember);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyData")]
	private static extern Utf8StringPointer _GetLobbyData(IntPtr self, SteamId steamIDLobby, IntPtr pchKey);

	internal string GetLobbyData(SteamId steamIDLobby, string pchKey)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKey);
		return _GetLobbyData(Self, steamIDLobby, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyData")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLobbyData(IntPtr self, SteamId steamIDLobby, IntPtr pchKey, IntPtr pchValue);

	internal bool SetLobbyData(SteamId steamIDLobby, string pchKey, string pchValue)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKey);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchValue);
		return _SetLobbyData(Self, steamIDLobby, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyDataCount")]
	private static extern int _GetLobbyDataCount(IntPtr self, SteamId steamIDLobby);

	internal int GetLobbyDataCount(SteamId steamIDLobby)
	{
		return _GetLobbyDataCount(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetLobbyDataByIndex(IntPtr self, SteamId steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize);

	internal bool GetLobbyDataByIndex(SteamId steamIDLobby, int iLobbyData, out string pchKey, out string pchValue)
	{
		using Helpers.Memory m = Helpers.TakeMemory();
		using Helpers.Memory m2 = Helpers.TakeMemory();
		bool result = _GetLobbyDataByIndex(Self, steamIDLobby, iLobbyData, m, 32768, m2, 32768);
		pchKey = Helpers.MemoryToString(m);
		pchValue = Helpers.MemoryToString(m2);
		return result;
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_DeleteLobbyData")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _DeleteLobbyData(IntPtr self, SteamId steamIDLobby, IntPtr pchKey);

	internal bool DeleteLobbyData(SteamId steamIDLobby, string pchKey)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKey);
		return _DeleteLobbyData(Self, steamIDLobby, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyMemberData")]
	private static extern Utf8StringPointer _GetLobbyMemberData(IntPtr self, SteamId steamIDLobby, SteamId steamIDUser, IntPtr pchKey);

	internal string GetLobbyMemberData(SteamId steamIDLobby, SteamId steamIDUser, string pchKey)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKey);
		return _GetLobbyMemberData(Self, steamIDLobby, steamIDUser, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyMemberData")]
	private static extern void _SetLobbyMemberData(IntPtr self, SteamId steamIDLobby, IntPtr pchKey, IntPtr pchValue);

	internal void SetLobbyMemberData(SteamId steamIDLobby, string pchKey, string pchValue)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchKey);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchValue);
		_SetLobbyMemberData(Self, steamIDLobby, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SendLobbyChatMsg")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SendLobbyChatMsg(IntPtr self, SteamId steamIDLobby, IntPtr pvMsgBody, int cubMsgBody);

	internal bool SendLobbyChatMsg(SteamId steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
	{
		return _SendLobbyChatMsg(Self, steamIDLobby, pvMsgBody, cubMsgBody);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyChatEntry")]
	private static extern int _GetLobbyChatEntry(IntPtr self, SteamId steamIDLobby, int iChatID, ref SteamId pSteamIDUser, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType);

	internal int GetLobbyChatEntry(SteamId steamIDLobby, int iChatID, ref SteamId pSteamIDUser, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType)
	{
		return _GetLobbyChatEntry(Self, steamIDLobby, iChatID, ref pSteamIDUser, pvData, cubData, ref peChatEntryType);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_RequestLobbyData")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _RequestLobbyData(IntPtr self, SteamId steamIDLobby);

	internal bool RequestLobbyData(SteamId steamIDLobby)
	{
		return _RequestLobbyData(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyGameServer")]
	private static extern void _SetLobbyGameServer(IntPtr self, SteamId steamIDLobby, uint unGameServerIP, ushort unGameServerPort, SteamId steamIDGameServer);

	internal void SetLobbyGameServer(SteamId steamIDLobby, uint unGameServerIP, ushort unGameServerPort, SteamId steamIDGameServer)
	{
		_SetLobbyGameServer(Self, steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyGameServer")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetLobbyGameServer(IntPtr self, SteamId steamIDLobby, ref uint punGameServerIP, ref ushort punGameServerPort, ref SteamId psteamIDGameServer);

	internal bool GetLobbyGameServer(SteamId steamIDLobby, ref uint punGameServerIP, ref ushort punGameServerPort, ref SteamId psteamIDGameServer)
	{
		return _GetLobbyGameServer(Self, steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLobbyMemberLimit(IntPtr self, SteamId steamIDLobby, int cMaxMembers);

	internal bool SetLobbyMemberLimit(SteamId steamIDLobby, int cMaxMembers)
	{
		return _SetLobbyMemberLimit(Self, steamIDLobby, cMaxMembers);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit")]
	private static extern int _GetLobbyMemberLimit(IntPtr self, SteamId steamIDLobby);

	internal int GetLobbyMemberLimit(SteamId steamIDLobby)
	{
		return _GetLobbyMemberLimit(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyType")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLobbyType(IntPtr self, SteamId steamIDLobby, LobbyType eLobbyType);

	internal bool SetLobbyType(SteamId steamIDLobby, LobbyType eLobbyType)
	{
		return _SetLobbyType(Self, steamIDLobby, eLobbyType);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyJoinable")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLobbyJoinable(IntPtr self, SteamId steamIDLobby, [MarshalAs(UnmanagedType.U1)] bool bLobbyJoinable);

	internal bool SetLobbyJoinable(SteamId steamIDLobby, [MarshalAs(UnmanagedType.U1)] bool bLobbyJoinable)
	{
		return _SetLobbyJoinable(Self, steamIDLobby, bLobbyJoinable);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_GetLobbyOwner")]
	private static extern SteamId _GetLobbyOwner(IntPtr self, SteamId steamIDLobby);

	internal SteamId GetLobbyOwner(SteamId steamIDLobby)
	{
		return _GetLobbyOwner(Self, steamIDLobby);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLobbyOwner")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLobbyOwner(IntPtr self, SteamId steamIDLobby, SteamId steamIDNewOwner);

	internal bool SetLobbyOwner(SteamId steamIDLobby, SteamId steamIDNewOwner)
	{
		return _SetLobbyOwner(Self, steamIDLobby, steamIDNewOwner);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmaking_SetLinkedLobby")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLinkedLobby(IntPtr self, SteamId steamIDLobby, SteamId steamIDLobbyDependent);

	internal bool SetLinkedLobby(SteamId steamIDLobby, SteamId steamIDLobbyDependent)
	{
		return _SetLinkedLobby(Self, steamIDLobby, steamIDLobbyDependent);
	}
}
