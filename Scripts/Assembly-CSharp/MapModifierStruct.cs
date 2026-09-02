using System;
using System.Runtime.InteropServices;
using Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 8)]
[NetworkStructWeaved(2)]
public struct MapModifierStruct : INetworkStruct
{
	[FieldOffset(0)]
	public byte idMissionModifier;

	[FieldOffset(4)]
	public short value;
}
