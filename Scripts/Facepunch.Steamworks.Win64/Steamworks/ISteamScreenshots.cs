using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamScreenshots : SteamInterface
{
	public const string Version = "STEAMSCREENSHOTS_INTERFACE_VERSION003";

	internal ISteamScreenshots(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamScreenshots_v003();

	public override IntPtr GetUserInterfacePointer()
	{
		return SteamAPI_SteamScreenshots_v003();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_WriteScreenshot")]
	private static extern ScreenshotHandle _WriteScreenshot(IntPtr self, IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight);

	internal ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
	{
		return _WriteScreenshot(Self, pubRGB, cubRGB, nWidth, nHeight);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_AddScreenshotToLibrary")]
	private static extern ScreenshotHandle _AddScreenshotToLibrary(IntPtr self, IntPtr pchFilename, IntPtr pchThumbnailFilename, int nWidth, int nHeight);

	internal ScreenshotHandle AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchFilename);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchThumbnailFilename);
		return _AddScreenshotToLibrary(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, nWidth, nHeight);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TriggerScreenshot")]
	private static extern void _TriggerScreenshot(IntPtr self);

	internal void TriggerScreenshot()
	{
		_TriggerScreenshot(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_HookScreenshots")]
	private static extern void _HookScreenshots(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bHook);

	internal void HookScreenshots([MarshalAs(UnmanagedType.U1)] bool bHook)
	{
		_HookScreenshots(Self, bHook);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_SetLocation")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetLocation(IntPtr self, ScreenshotHandle hScreenshot, IntPtr pchLocation);

	internal bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchLocation);
		return _SetLocation(Self, hScreenshot, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TagUser")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _TagUser(IntPtr self, ScreenshotHandle hScreenshot, SteamId steamID);

	internal bool TagUser(ScreenshotHandle hScreenshot, SteamId steamID)
	{
		return _TagUser(Self, hScreenshot, steamID);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TagPublishedFile")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _TagPublishedFile(IntPtr self, ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID);

	internal bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID)
	{
		return _TagPublishedFile(Self, hScreenshot, unPublishedFileID);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_IsScreenshotsHooked")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _IsScreenshotsHooked(IntPtr self);

	internal bool IsScreenshotsHooked()
	{
		return _IsScreenshotsHooked(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary")]
	private static extern ScreenshotHandle _AddVRScreenshotToLibrary(IntPtr self, VRScreenshotType eType, IntPtr pchFilename, IntPtr pchVRFilename);

	internal ScreenshotHandle AddVRScreenshotToLibrary(VRScreenshotType eType, string pchFilename, string pchVRFilename)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchFilename);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchVRFilename);
		return _AddVRScreenshotToLibrary(Self, eType, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}
}
