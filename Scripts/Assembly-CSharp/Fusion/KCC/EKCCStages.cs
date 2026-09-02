using System;

namespace Fusion.KCC;

[Flags]
public enum EKCCStages
{
	None = 0,
	SetInputProperties = 2,
	SetDynamicVelocity = 4,
	SetKinematicDirection = 8,
	SetKinematicTangent = 0x10,
	SetKinematicSpeed = 0x20,
	SetKinematicVelocity = 0x40,
	ProcessPhysicsQuery = 0x80,
	OnStay = 0x100,
	OnInterpolate = 0x200,
	ProcessUserLogic = 0x400,
	All = -1
}
