using System;
using System.Runtime.InteropServices;
using Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 24)]
[NetworkStructWeaved(6)]
public struct ScoreDataNetwork : INetworkStruct
{
	[FieldOffset(0)]
	public short KillZombieCount;

	[FieldOffset(4)]
	public byte KillEliteCount;

	[FieldOffset(8)]
	public byte PuzzleSolved;

	[FieldOffset(12)]
	public byte DeathCount;

	[FieldOffset(16)]
	public byte Life;

	[FieldOffset(20)]
	public byte ReviveOtherPlayer;
}
