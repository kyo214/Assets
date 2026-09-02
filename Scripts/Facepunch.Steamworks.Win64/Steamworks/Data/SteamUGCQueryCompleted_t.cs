using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamUGCQueryCompleted_t : ICallbackData
{
	internal ulong Handle;

	internal Result Result;

	internal uint NumResultsReturned;

	internal uint TotalMatchingResults;

	[MarshalAs(UnmanagedType.I1)]
	internal bool CachedData;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	internal byte[] NextCursor;

	public static int _datasize = Marshal.SizeOf(typeof(SteamUGCQueryCompleted_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.SteamUGCQueryCompleted;

	internal string NextCursorUTF8()
	{
		return Utility.Utf8NoBom.GetString(NextCursor, 0, Array.IndexOf(NextCursor, (byte)0));
	}
}
