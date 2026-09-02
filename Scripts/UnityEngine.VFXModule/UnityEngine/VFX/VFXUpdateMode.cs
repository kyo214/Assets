using System;

namespace UnityEngine.VFX;

[Flags]
internal enum VFXUpdateMode
{
	FixedDeltaTime = 0,
	DeltaTime = 1,
	IgnoreTimeScale = 2,
	ExactFixedTimeStep = 4,
	DeltaTimeAndIgnoreTimeScale = DeltaTime | IgnoreTimeScale,
	FixedDeltaAndExactTime = ExactFixedTimeStep,
	FixedDeltaAndExactTimeAndIgnoreTimeScale = IgnoreTimeScale | ExactFixedTimeStep
}
