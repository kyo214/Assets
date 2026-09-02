using System;

namespace Fusion;

[Flags]
public enum NetworkObjectFlags
{
	None = 0,
	MaskVersion = 0xFF,
	V1 = 1,
	MaskType = 0xF00,
	TypePrefab = 0x900,
	TypeSceneObject = 0xA00,
	TypeSpawnedPrefab = 0xB00,
	TypePrefabChild = 0xC00,
	TypeSpawnedPrefabChild = 0xD00,
	Ignore = 0x10000,
	ActivatedByUser = 0x20000,
	AttachOptionLocalSpawn = 0x100000,
	PredictedSpawn = 0x400000,
	Spawned = 0x800000,
	RuntimeFlagsMask = 0xFF00000
}
