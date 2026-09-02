using System.ComponentModel;

namespace Fusion;

public enum Units
{
	[Description("")]
	None = 0,
	[Description("ticks")]
	Ticks = 1,
	[Description("seconds - secs")]
	Seconds = 2,
	[Description("millisecs - ms")]
	MilliSecs = 3,
	[Description("kilobytes - kB")]
	Kilobytes = 4,
	[Description("megabytes - MB")]
	Megabytes = 5,
	[Description("normalized - norm")]
	Normalized = 6,
	[Description("multiplier - mult")]
	Multiplier = 7,
	[Description("%")]
	Percentage = 8,
	[Description("normalized % - n%")]
	NormalizedPercentage = 9,
	[Description("degrees - °")]
	Degrees = 10,
	[Description("per sec - /sec")]
	PerSecond = 11,
	[Description("° / sec - °/sec")]
	DegreesPerSecond = 12,
	[Description("radians - rad")]
	Radians = 13,
	[Description("radian / sec - rad/s")]
	RadiansPerSecond = 14,
	[Description("ticks / sec - tck/s")]
	TicksPerSecond = 15,
	[Description("units - units")]
	Units = 16,
	[Description("bytes - bytes")]
	Bytes = 17,
	[Description("count - count")]
	Count = 18
}
