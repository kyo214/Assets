using System;

namespace Fusion;

[Flags]
public enum OnChangedTargets
{
	StateAuthority = 1,
	InputAuthority = 2,
	Proxies = 4,
	All = StateAuthority | InputAuthority | Proxies
}
