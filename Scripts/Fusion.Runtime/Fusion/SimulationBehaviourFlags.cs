using System;

namespace Fusion;

[Flags]
internal enum SimulationBehaviourFlags
{
	SkipNextUpdate = 1,
	PendingRemoval = 2
}
