using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.SqlClient;

internal static class SNINativeMethodWrapper
{
	internal enum SniSpecialErrors : uint
	{
		LocalDBErrorCode = 50u,
		MultiSubnetFailoverWithMoreThan64IPs = 47u,
		MultiSubnetFailoverWithInstanceSpecified = 48u,
		MultiSubnetFailoverWithNonTcpProtocol = 49u,
		MaxErrorValue = 50157u
	}

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void SqlAsyncCallbackDelegate(IntPtr m_ConsKey, IntPtr pPacket, uint dwError);

	internal struct ConsumerInfo
	{
		internal int defaultBufferSize;

		internal SqlAsyncCallbackDelegate readDelegate;

		internal SqlAsyncCallbackDelegate writeDelegate;

		internal IntPtr key;
	}

	internal enum ConsumerNumber
	{
		SNI_Consumer_SNI = 0,
		SNI_Consumer_SSB = 1,
		SNI_Consumer_PacketIsReleased = 2,
		SNI_Consumer_Invalid = 3
	}

	internal enum IOType
	{
		READ = 0,
		WRITE = 1
	}

	internal enum PrefixEnum
	{
		UNKNOWN_PREFIX = 0,
		SM_PREFIX = 1,
		TCP_PREFIX = 2,
		NP_PREFIX = 3,
		VIA_PREFIX = 4,
		INVALID_PREFIX = 5
	}

	internal enum ProviderEnum
	{
		HTTP_PROV = 0,
		NP_PROV = 1,
		SESSION_PROV = 2,
		SIGN_PROV = 3,
		SM_PROV = 4,
		SMUX_PROV = 5,
		SSL_PROV = 6,
		TCP_PROV = 7,
		VIA_PROV = 8,
		MAX_PROVS = 9,
		INVALID_PROV = 10
	}

	internal enum QTypes
	{
		SNI_QUERY_CONN_INFO = 0,
		SNI_QUERY_CONN_BUFSIZE = 1,
		SNI_QUERY_CONN_KEY = 2,
		SNI_QUERY_CLIENT_ENCRYPT_POSSIBLE = 3,
		SNI_QUERY_SERVER_ENCRYPT_POSSIBLE = 4,
		SNI_QUERY_CERTIFICATE = 5,
		SNI_QUERY_LOCALDB_HMODULE = 6,
		SNI_QUERY_CONN_ENCRYPT = 7,
		SNI_QUERY_CONN_PROVIDERNUM = 8,
		SNI_QUERY_CONN_CONNID = 9,
		SNI_QUERY_CONN_PARENTCONNID = 10,
		SNI_QUERY_CONN_SECPKG = 11,
		SNI_QUERY_CONN_NETPACKETSIZE = 12,
		SNI_QUERY_CONN_NODENUM = 13,
		SNI_QUERY_CONN_PACKETSRECD = 14,
		SNI_QUERY_CONN_PACKETSSENT = 15,
		SNI_QUERY_CONN_PEERADDR = 16,
		SNI_QUERY_CONN_PEERPORT = 17,
		SNI_QUERY_CONN_LASTREADTIME = 18,
		SNI_QUERY_CONN_LASTWRITETIME = 19,
		SNI_QUERY_CONN_CONSUMER_ID = 20,
		SNI_QUERY_CONN_CONNECTTIME = 21,
		SNI_QUERY_CONN_HTTPENDPOINT = 22,
		SNI_QUERY_CONN_LOCALADDR = 23,
		SNI_QUERY_CONN_LOCALPORT = 24,
		SNI_QUERY_CONN_SSLHANDSHAKESTATE = 25,
		SNI_QUERY_CONN_SOBUFAUTOTUNING = 26,
		SNI_QUERY_CONN_SECPKGNAME = 27,
		SNI_QUERY_CONN_SECPKGMUTUALAUTH = 28,
		SNI_QUERY_CONN_CONSUMERCONNID = 29,
		SNI_QUERY_CONN_SNIUCI = 30,
		SNI_QUERY_CONN_SUPPORTS_EXTENDED_PROTECTION = 31,
		SNI_QUERY_CONN_CHANNEL_PROVIDES_AUTHENTICATION_CONTEXT = 32,
		SNI_QUERY_CONN_PEERID = 33,
		SNI_QUERY_CONN_SUPPORTS_SYNC_OVER_ASYNC = 34
	}

	internal enum TransparentNetworkResolutionMode : byte
	{
		DisabledMode = 0,
		SequentialMode = 1,
		ParallelMode = 2
	}

	private struct Sni_Consumer_Info
	{
		public int DefaultUserDataLength;

		public IntPtr ConsumerKey;

		public IntPtr fnReadComp;

		public IntPtr fnWriteComp;

		public IntPtr fnTrace;

		public IntPtr fnAcceptComp;

		public uint dwNumProts;

		public IntPtr rgListenInfo;

		public IntPtr NodeAffinity;
	}

	private struct SNI_CLIENT_CONSUMER_INFO
	{
		public Sni_Consumer_Info ConsumerInfo;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string wszConnectionString;

		public PrefixEnum networkLibrary;

		public unsafe byte* szSPN;

		public uint cchSPN;

		public unsafe byte* szInstanceName;

		public uint cchInstanceName;

		[MarshalAs(UnmanagedType.Bool)]
		public bool fOverrideLastConnectCache;

		[MarshalAs(UnmanagedType.Bool)]
		public bool fSynchronousConnection;

		public int timeout;

		[MarshalAs(UnmanagedType.Bool)]
		public bool fParallel;

		public TransparentNetworkResolutionMode transparentNetworkResolution;

		public int totalTimeout;

		public bool isAzureSqlServerEndpoint;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SNI_Error
	{
		internal ProviderEnum provider;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		internal string errorMessage;

		internal uint nativeError;

		internal uint sniError;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string fileName;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string function;

		internal uint lineNumber;
	}

	private const string SNI = "sni.dll";

	private static int s_sniMaxComposedSpnLength = -1;

	private const int SniOpenTimeOut = -1;

	internal static int SniMaxComposedSpnLength
	{
		get
		{
			if (s_sniMaxComposedSpnLength == -1)
			{
				s_sniMaxComposedSpnLength = checked((int)GetSniMaxComposedSpnLength());
			}
			return s_sniMaxComposedSpnLength;
		}
	}

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIAddProviderWrapper")]
	internal static extern uint SNIAddProvider(SNIHandle pConn, ProviderEnum ProvNum, [In] ref uint pInfo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICheckConnectionWrapper")]
	internal static extern uint SNICheckConnection([In] SNIHandle pConn);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICloseWrapper")]
	internal static extern uint SNIClose(IntPtr pConn);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void SNIGetLastError(out SNI_Error pErrorStruct);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void SNIPacketRelease(IntPtr pPacket);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIPacketResetWrapper")]
	internal static extern void SNIPacketReset([In] SNIHandle pConn, IOType IOType, SNIPacket pPacket, ConsumerNumber ConsNum);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint SNIQueryInfo(QTypes QType, ref uint pbQInfo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint SNIQueryInfo(QTypes QType, ref IntPtr pbQInfo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIReadAsyncWrapper")]
	internal static extern uint SNIReadAsync(SNIHandle pConn, ref IntPtr ppNewPacket);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint SNIReadSyncOverAsync(SNIHandle pConn, ref IntPtr ppNewPacket, int timeout);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIRemoveProviderWrapper")]
	internal static extern uint SNIRemoveProvider(SNIHandle pConn, ProviderEnum ProvNum);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint SNISecInitPackage(ref uint pcbMaxToken);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNISetInfoWrapper")]
	internal static extern uint SNISetInfo(SNIHandle pConn, QTypes QType, [In] ref uint pbQInfo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint SNITerminate();

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIWaitForSSLHandshakeToCompleteWrapper")]
	internal static extern uint SNIWaitForSSLHandshakeToComplete([In] SNIHandle pConn, int dwMilliseconds);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint UnmanagedIsTokenRestricted([In] IntPtr token, [MarshalAs(UnmanagedType.Bool)] out bool isRestricted);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint GetSniMaxComposedSpnLength();

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIGetInfoWrapper([In] SNIHandle pConn, QTypes QType, out Guid pbQInfo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIInitialize([In] IntPtr pmo);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIOpenSyncExWrapper(ref SNI_CLIENT_CONSUMER_INFO pClientConsumerInfo, out IntPtr ppConn);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIOpenWrapper([In] ref Sni_Consumer_Info pConsumerInfo, [MarshalAs(UnmanagedType.LPStr)] string szConnect, [In] SNIHandle pConn, out IntPtr ppConn, [MarshalAs(UnmanagedType.Bool)] bool fSync);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SNIPacketAllocateWrapper([In] SafeHandle pConn, IOType IOType);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIPacketGetDataWrapper([In] IntPtr packet, [In][Out] byte[] readBuffer, uint readBufferLength, out uint dataSize);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private unsafe static extern void SNIPacketSetData(SNIPacket pPacket, [In] byte* pbBuf, uint cbBuf);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private unsafe static extern uint SNISecGenClientContextWrapper([In] SNIHandle pConn, [In][Out] byte[] pIn, uint cbIn, [In][Out] byte[] pOut, [In] ref uint pcbOut, [MarshalAs(UnmanagedType.Bool)] out bool pfDone, byte* szServerInfo, uint cbServerInfo, [MarshalAs(UnmanagedType.LPWStr)] string pwszUserName, [MarshalAs(UnmanagedType.LPWStr)] string pwszPassword);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIWriteAsyncWrapper(SNIHandle pConn, [In] SNIPacket pPacket);

	[DllImport("sni.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SNIWriteSyncOverAsync(SNIHandle pConn, [In] SNIPacket pPacket);

	internal static uint SniGetConnectionId(SNIHandle pConn, ref Guid connId)
	{
		return SNIGetInfoWrapper(pConn, QTypes.SNI_QUERY_CONN_CONNID, out connId);
	}

	internal static uint SNIInitialize()
	{
		return SNIInitialize(IntPtr.Zero);
	}

	internal static uint SNIOpenMarsSession(ConsumerInfo consumerInfo, SNIHandle parent, ref IntPtr pConn, bool fSync)
	{
		Sni_Consumer_Info native_consumerInfo = default;
		MarshalConsumerInfo(consumerInfo, ref native_consumerInfo);
		return SNIOpenWrapper(ref native_consumerInfo, "session:", parent, out pConn, fSync);
	}

	internal unsafe static uint SNIOpenSyncEx(ConsumerInfo consumerInfo, string constring, ref IntPtr pConn, byte[] spnBuffer, byte[] instanceName, bool fOverrideCache, bool fSync, int timeout, bool fParallel)
	{
		fixed (byte* szInstanceName = &instanceName[0])
		{
			SNI_CLIENT_CONSUMER_INFO pClientConsumerInfo = default;
			MarshalConsumerInfo(consumerInfo, ref pClientConsumerInfo.ConsumerInfo);
			pClientConsumerInfo.wszConnectionString = constring;
			pClientConsumerInfo.networkLibrary = PrefixEnum.UNKNOWN_PREFIX;
			pClientConsumerInfo.szInstanceName = szInstanceName;
			pClientConsumerInfo.cchInstanceName = (uint)instanceName.Length;
			pClientConsumerInfo.fOverrideLastConnectCache = fOverrideCache;
			pClientConsumerInfo.fSynchronousConnection = fSync;
			pClientConsumerInfo.timeout = timeout;
			pClientConsumerInfo.fParallel = fParallel;
			pClientConsumerInfo.transparentNetworkResolution = TransparentNetworkResolutionMode.DisabledMode;
			pClientConsumerInfo.totalTimeout = -1;
			pClientConsumerInfo.isAzureSqlServerEndpoint = ADP.IsAzureSqlServerEndpoint(constring);
			if (spnBuffer != null)
			{
				fixed (byte* szSPN = &spnBuffer[0])
				{
					pClientConsumerInfo.szSPN = szSPN;
					pClientConsumerInfo.cchSPN = (uint)spnBuffer.Length;
					return SNIOpenSyncExWrapper(ref pClientConsumerInfo, out pConn);
				}
			}
			return SNIOpenSyncExWrapper(ref pClientConsumerInfo, out pConn);
		}
	}

	internal static void SNIPacketAllocate(SafeHandle pConn, IOType IOType, ref IntPtr pPacket)
	{
		pPacket = SNIPacketAllocateWrapper(pConn, IOType);
	}

	internal static uint SNIPacketGetData(IntPtr packet, byte[] readBuffer, ref uint dataSize)
	{
		return SNIPacketGetDataWrapper(packet, readBuffer, (uint)readBuffer.Length, out dataSize);
	}

	internal unsafe static void SNIPacketSetData(SNIPacket packet, byte[] data, int length)
	{
		fixed (byte* pbBuf = &data[0])
		{
			SNIPacketSetData(packet, pbBuf, (uint)length);
		}
	}

	internal unsafe static uint SNISecGenClientContext(SNIHandle pConnectionObject, byte[] inBuff, uint receivedLength, byte[] OutBuff, ref uint sendLength, byte[] serverUserName)
	{
		fixed (byte* szServerInfo = &serverUserName[0])
		{
			bool pfDone;
			return SNISecGenClientContextWrapper(pConnectionObject, inBuff, receivedLength, OutBuff, ref sendLength, out pfDone, szServerInfo, (uint)serverUserName.Length, null, null);
		}
	}

	internal static uint SNIWritePacket(SNIHandle pConn, SNIPacket packet, bool sync)
	{
		if (sync)
		{
			return SNIWriteSyncOverAsync(pConn, packet);
		}
		return SNIWriteAsyncWrapper(pConn, packet);
	}

	private static void MarshalConsumerInfo(ConsumerInfo consumerInfo, ref Sni_Consumer_Info native_consumerInfo)
	{
		native_consumerInfo.DefaultUserDataLength = consumerInfo.defaultBufferSize;
		native_consumerInfo.fnReadComp = ((consumerInfo.readDelegate != null) ? Marshal.GetFunctionPointerForDelegate(consumerInfo.readDelegate) : IntPtr.Zero);
		native_consumerInfo.fnWriteComp = ((consumerInfo.writeDelegate != null) ? Marshal.GetFunctionPointerForDelegate(consumerInfo.writeDelegate) : IntPtr.Zero);
		native_consumerInfo.ConsumerKey = consumerInfo.key;
	}
}
