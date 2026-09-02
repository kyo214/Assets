using System;
using System.Runtime.InteropServices;

namespace Steamworks;

internal static class SteamInternal
{
	internal static class Native
	{
		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		public static extern SteamAPIInitResult SteamInternal_GameServer_Init_V2(uint unIP, ushort usGamePort, ushort usQueryPort, int eServerMode, IntPtr pchVersionString, IntPtr pszInternalCheckInterfaceVersions, IntPtr pOutErrMsg);
	}

	internal static SteamAPIInitResult GameServer_Init(uint unIP, ushort usGamePort, ushort usQueryPort, int eServerMode, string pchVersionString, string pszInternalCheckInterfaceVersions, out string pOutErrMsg)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchVersionString);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pszInternalCheckInterfaceVersions);
		using Helpers.Memory memory = Helpers.Memory.Take();
		SteamAPIInitResult result = Native.SteamInternal_GameServer_Init_V2(unIP, usGamePort, usQueryPort, eServerMode, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, memory.Ptr);
		pOutErrMsg = Helpers.MemoryToString(memory.Ptr);
		return result;
	}
}
