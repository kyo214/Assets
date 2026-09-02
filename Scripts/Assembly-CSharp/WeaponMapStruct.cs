using System.Runtime.InteropServices;
using Fusion;

[StructLayout(LayoutKind.Explicit, Size = 8)]
[NetworkStructWeaved(2)]
public struct WeaponMapStruct : INetworkStruct
{
	[FieldOffset(0)]
	public WeaponTypeEnum WeaponType;

	[FieldOffset(4)]
	public ItemTypeEnum Weapon;
}
