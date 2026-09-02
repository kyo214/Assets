using System.Runtime.InteropServices;
using Fusion;

[StructLayout(LayoutKind.Explicit, Size = 8)]
[NetworkInputWeaved(2)]
public struct NetworkInputData : INetworkInput
{
	[FieldOffset(0)]
	public byte inputDataMove;

	[FieldOffset(4)]
	public short inputDataClick;
}
