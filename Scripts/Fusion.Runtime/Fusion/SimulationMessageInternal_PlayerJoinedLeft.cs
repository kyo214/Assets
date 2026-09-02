using System.Runtime.InteropServices;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
internal struct SimulationMessageInternal_PlayerJoinedLeft
{
	public const int SIZE = 8;

	[FieldOffset(0)]
	public PlayerRef Player;

	[FieldOffset(4)]
	public int Joined;
}
