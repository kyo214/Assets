using System.Runtime.InteropServices;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
internal struct SimulationGlobalState
{
	public const int WORDS = 32;

	public const int SIZE = 128;

	[FieldOffset(0)]
	private unsafe fixed int _size[32];

	[FieldOffset(0)]
	public SceneRef Scene;

	[FieldOffset(4)]
	public SimulationModes ServerMode;

	[FieldOffset(8)]
	public int MaxPlayers;

	[FieldOffset(16)]
	public unsafe fixed ulong PlayersActive[4];
}
