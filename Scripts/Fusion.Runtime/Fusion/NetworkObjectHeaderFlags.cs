using System;

namespace Fusion;

[Flags]
public enum NetworkObjectHeaderFlags
{
	GlobalObjectInterest = 1,
	DestroyWhenStateAuthorityLeaves = 2,
	SpawnedByClient = 4,
	HasDefaultInterestGroups = 8,
	AllowStateAuthorityOverride = 0x10,
	NoPrefab = 0x20
}
