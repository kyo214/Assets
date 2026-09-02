namespace Fusion.KCC;

public enum ECollisionType
{
	None = 0,
	Ground = 1,
	Slope = 2,
	Wall = 4,
	Hang = 8,
	Top = 0x10,
	Trigger = 0x20
}
