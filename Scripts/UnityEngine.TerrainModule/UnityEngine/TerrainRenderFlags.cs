using System;

namespace UnityEngine;

[Flags]
public enum TerrainRenderFlags
{
	[Obsolete("TerrainRenderFlags.heightmap is obsolete, use TerrainRenderFlags.Heightmap instead. (UnityUpgradable) -> Heightmap")]
	heightmap = 1,
	[Obsolete("TerrainRenderFlags.trees is obsolete, use TerrainRenderFlags.Trees instead. (UnityUpgradable) -> Trees")]
	trees = 2,
	[Obsolete("TerrainRenderFlags.details is obsolete, use TerrainRenderFlags.Details instead. (UnityUpgradable) -> Details")]
	details = 4,
	[Obsolete("TerrainRenderFlags.all is obsolete, use TerrainRenderFlags.All instead. (UnityUpgradable) -> All")]
	all = heightmap | trees | details,
	Heightmap = heightmap,
	Trees = trees,
	Details = details,
	All = all
}
