using System;
using System.Runtime.InteropServices;

namespace Steamworks;

internal class ISteamMatchmakingRulesResponse : SteamInterface
{
	internal ISteamMatchmakingRulesResponse(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesResponded")]
	private static extern void _RulesResponded(IntPtr self, IntPtr pchRule, IntPtr pchValue);

	internal void RulesResponded(string pchRule, string pchValue)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchRule);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchValue);
		_RulesResponded(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesFailedToRespond")]
	private static extern void _RulesFailedToRespond(IntPtr self);

	internal void RulesFailedToRespond()
	{
		_RulesFailedToRespond(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesRefreshComplete")]
	private static extern void _RulesRefreshComplete(IntPtr self);

	internal void RulesRefreshComplete()
	{
		_RulesRefreshComplete(Self);
	}
}
