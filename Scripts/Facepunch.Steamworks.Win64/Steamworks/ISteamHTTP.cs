using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamHTTP : SteamInterface
{
	public const string Version = "STEAMHTTP_INTERFACE_VERSION003";

	internal ISteamHTTP(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamHTTP_v003();

	public override IntPtr GetUserInterfacePointer()
	{
		return SteamAPI_SteamHTTP_v003();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamGameServerHTTP_v003();

	public override IntPtr GetServerInterfacePointer()
	{
		return SteamAPI_SteamGameServerHTTP_v003();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_CreateHTTPRequest")]
	private static extern HTTPRequestHandle _CreateHTTPRequest(IntPtr self, HTTPMethod eHTTPRequestMethod, IntPtr pchAbsoluteURL);

	internal HTTPRequestHandle CreateHTTPRequest(HTTPMethod eHTTPRequestMethod, string pchAbsoluteURL)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchAbsoluteURL);
		return _CreateHTTPRequest(Self, eHTTPRequestMethod, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestContextValue")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestContextValue(IntPtr self, HTTPRequestHandle hRequest, ulong ulContextValue);

	internal bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
	{
		return _SetHTTPRequestContextValue(Self, hRequest, ulContextValue);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestNetworkActivityTimeout(IntPtr self, HTTPRequestHandle hRequest, uint unTimeoutSeconds);

	internal bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
	{
		return _SetHTTPRequestNetworkActivityTimeout(Self, hRequest, unTimeoutSeconds);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestHeaderValue(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchHeaderName, IntPtr pchHeaderValue);

	internal bool SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchHeaderName);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchHeaderValue);
		return _SetHTTPRequestHeaderValue(Self, hRequest, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestGetOrPostParameter(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchParamName, IntPtr pchParamValue);

	internal bool SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchParamName);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchParamValue);
		return _SetHTTPRequestGetOrPostParameter(Self, hRequest, utf8StringToNative.Pointer, utf8StringToNative2.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SendHTTPRequest")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SendHTTPRequest(IntPtr self, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle);

	internal bool SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
	{
		return _SendHTTPRequest(Self, hRequest, ref pCallHandle);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SendHTTPRequestAndStreamResponse(IntPtr self, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle);

	internal bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
	{
		return _SendHTTPRequestAndStreamResponse(Self, hRequest, ref pCallHandle);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_DeferHTTPRequest")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _DeferHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

	internal bool DeferHTTPRequest(HTTPRequestHandle hRequest)
	{
		return _DeferHTTPRequest(Self, hRequest);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_PrioritizeHTTPRequest")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _PrioritizeHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

	internal bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
	{
		return _PrioritizeHTTPRequest(Self, hRequest);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPResponseHeaderSize(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchHeaderName, ref uint unResponseHeaderSize);

	internal bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchHeaderName);
		return _GetHTTPResponseHeaderSize(Self, hRequest, utf8StringToNative.Pointer, ref unResponseHeaderSize);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPResponseHeaderValue(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchHeaderName, ref byte pHeaderValueBuffer, uint unBufferSize);

	internal bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, ref byte pHeaderValueBuffer, uint unBufferSize)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchHeaderName);
		return _GetHTTPResponseHeaderValue(Self, hRequest, utf8StringToNative.Pointer, ref pHeaderValueBuffer, unBufferSize);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseBodySize")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPResponseBodySize(IntPtr self, HTTPRequestHandle hRequest, ref uint unBodySize);

	internal bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, ref uint unBodySize)
	{
		return _GetHTTPResponseBodySize(Self, hRequest, ref unBodySize);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseBodyData")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPResponseBodyData(IntPtr self, HTTPRequestHandle hRequest, ref byte pBodyDataBuffer, uint unBufferSize);

	internal bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, ref byte pBodyDataBuffer, uint unBufferSize)
	{
		return _GetHTTPResponseBodyData(Self, hRequest, ref pBodyDataBuffer, unBufferSize);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPStreamingResponseBodyData(IntPtr self, HTTPRequestHandle hRequest, uint cOffset, ref byte pBodyDataBuffer, uint unBufferSize);

	internal bool GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, ref byte pBodyDataBuffer, uint unBufferSize)
	{
		return _GetHTTPStreamingResponseBodyData(Self, hRequest, cOffset, ref pBodyDataBuffer, unBufferSize);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_ReleaseHTTPRequest")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _ReleaseHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

	internal bool ReleaseHTTPRequest(HTTPRequestHandle hRequest)
	{
		return _ReleaseHTTPRequest(Self, hRequest);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPDownloadProgressPct(IntPtr self, HTTPRequestHandle hRequest, ref float pflPercentOut);

	internal bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, ref float pflPercentOut)
	{
		return _GetHTTPDownloadProgressPct(Self, hRequest, ref pflPercentOut);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestRawPostBody(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchContentType, [In][Out] byte[] pubBody, uint unBodyLen);

	internal bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, [In][Out] byte[] pubBody, uint unBodyLen)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchContentType);
		return _SetHTTPRequestRawPostBody(Self, hRequest, utf8StringToNative.Pointer, pubBody, unBodyLen);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_CreateCookieContainer")]
	private static extern HTTPCookieContainerHandle _CreateCookieContainer(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAllowResponsesToModify);

	internal HTTPCookieContainerHandle CreateCookieContainer([MarshalAs(UnmanagedType.U1)] bool bAllowResponsesToModify)
	{
		return _CreateCookieContainer(Self, bAllowResponsesToModify);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_ReleaseCookieContainer")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _ReleaseCookieContainer(IntPtr self, HTTPCookieContainerHandle hCookieContainer);

	internal bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
	{
		return _ReleaseCookieContainer(Self, hCookieContainer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetCookie")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetCookie(IntPtr self, HTTPCookieContainerHandle hCookieContainer, IntPtr pchHost, IntPtr pchUrl, IntPtr pchCookie);

	internal bool SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchHost);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchUrl);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchCookie);
		return _SetCookie(Self, hCookieContainer, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestCookieContainer(IntPtr self, HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer);

	internal bool SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
	{
		return _SetHTTPRequestCookieContainer(Self, hRequest, hCookieContainer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestUserAgentInfo(IntPtr self, HTTPRequestHandle hRequest, IntPtr pchUserAgentInfo);

	internal bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchUserAgentInfo);
		return _SetHTTPRequestUserAgentInfo(Self, hRequest, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestRequiresVerifiedCertificate(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] bool bRequireVerifiedCertificate);

	internal bool SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] bool bRequireVerifiedCertificate)
	{
		return _SetHTTPRequestRequiresVerifiedCertificate(Self, hRequest, bRequireVerifiedCertificate);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _SetHTTPRequestAbsoluteTimeoutMS(IntPtr self, HTTPRequestHandle hRequest, uint unMilliseconds);

	internal bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
	{
		return _SetHTTPRequestAbsoluteTimeoutMS(Self, hRequest, unMilliseconds);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool _GetHTTPRequestWasTimedOut(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] ref bool pbWasTimedOut);

	internal bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] ref bool pbWasTimedOut)
	{
		return _GetHTTPRequestWasTimedOut(Self, hRequest, ref pbWasTimedOut);
	}
}
