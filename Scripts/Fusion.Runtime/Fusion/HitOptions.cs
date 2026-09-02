using System;

namespace Fusion;

[Flags]
public enum HitOptions
{
	None = 0,
	IncludePhysX = 1,
	SubtickAccuracy = 2,
	[Obsolete("Detailed hit is now always computed.")]
	DetailedHit = 4,
	IgnoreInputAuthority = 8
}
