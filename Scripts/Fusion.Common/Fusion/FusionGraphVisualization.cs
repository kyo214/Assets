using System;
using System.ComponentModel;

namespace Fusion;

[Flags]
public enum FusionGraphVisualization
{
	[Description("Auto")]
	Auto = 0,
	[Description("Continuous Tick")]
	ContinuousTick = 1,
	[Description("Intermittent Tick")]
	IntermittentTick = 2,
	[Description("Intermittent Time")]
	IntermittentTime = 4,
	[Description("Value Histogram")]
	ValueHistogram = 8,
	[Description("Count Histogram")]
	CountHistogram = 0x10
}
