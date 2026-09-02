using System.Runtime.InteropServices;

namespace Fusion.Sockets;

[StructLayout(LayoutKind.Explicit)]
internal struct NetCommandDisconnect
{
	[FieldOffset(0)]
	public NetCommandHeader Header;

	[FieldOffset(2)]
	public NetDisconnectReason Reason;

	public static NetCommandDisconnect Create(NetDisconnectReason reason)
	{
		return new NetCommandDisconnect
		{
			Header = NetCommands.Disconnect,
			Reason = reason
		};
	}
}
