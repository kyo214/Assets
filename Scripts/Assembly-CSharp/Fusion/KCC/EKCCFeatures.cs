using System;

namespace Fusion.KCC;

[Flags]
public enum EKCCFeatures
{
	None = 0,
	StepUp = 2,
	SnapToGround = 4,
	PredictionCorrection = 8,
	AntiJitter = 0x10,
	CCD = 0x20,
	All = -1
}
