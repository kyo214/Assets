using System.Runtime.InteropServices;

namespace Fusion.Sockets;

[StructLayout(LayoutKind.Explicit)]
internal struct NetCommandHeader
{
	public const int SIZE_BYTES = 2;

	public const int SIZE_BITS = 16;

	[FieldOffset(0)]
	public NetPacketType PacketType;

	[FieldOffset(1)]
	public NetCommands Command;

	public static NetCommandHeader Create(NetCommands command)
	{
		return new NetCommandHeader
		{
			PacketType = NetPacketType.Command,
			Command = command
		};
	}

	public static implicit operator NetCommandHeader(NetCommands commands)
	{
		return Create(commands);
	}
}
