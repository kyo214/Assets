using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct GameRichPresenceJoinRequested_t : ICallbackData
{
	internal ulong SteamIDFriend;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	internal byte[] Connect;

	public static int _datasize = Marshal.SizeOf(typeof(GameRichPresenceJoinRequested_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.GameRichPresenceJoinRequested;

	internal string ConnectUTF8()
	{
		return Utility.Utf8NoBom.GetString(Connect, 0, Array.IndexOf(Connect, (byte)0));
	}
}
