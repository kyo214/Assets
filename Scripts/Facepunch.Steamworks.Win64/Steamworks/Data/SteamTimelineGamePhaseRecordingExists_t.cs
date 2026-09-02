using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamTimelineGamePhaseRecordingExists_t : ICallbackData
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	internal byte[] PhaseID;

	internal ulong RecordingMS;

	internal ulong LongestClipMS;

	internal uint ClipCount;

	internal uint ScreenshotCount;

	public static int _datasize = Marshal.SizeOf(typeof(SteamTimelineGamePhaseRecordingExists_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.SteamTimelineGamePhaseRecordingExists;

	internal string PhaseIDUTF8()
	{
		return Utility.Utf8NoBom.GetString(PhaseID, 0, Array.IndexOf(PhaseID, (byte)0));
	}
}
