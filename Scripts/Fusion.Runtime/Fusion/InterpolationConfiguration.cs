using System;

namespace Fusion;

[Serializable]
public class InterpolationConfiguration
{
	[InlineHelp]
	[Unit(Units.Percentage, 1.0, 10.0)]
	[MultiPropertyDrawersFix]
	public int DeltaAdjustment = 1;

	[InlineHelp]
	[Unit(Units.Percentage, 25.0, 100.0)]
	[MultiPropertyDrawersFix]
	public int AllowedJitter = 25;

	[InlineHelp]
	[Unit(Units.Percentage, 100.0, 1000.0)]
	[MultiPropertyDrawersFix]
	public int SnapLimit = 200;

	[InlineHelp]
	[Unit(Units.Multiplier, 1.0, 10.0)]
	[MultiPropertyDrawersFix]
	public double MultiplierMin = 1.25;

	[InlineHelp]
	[Unit(Units.Multiplier, 1.0, 10.0)]
	[MultiPropertyDrawersFix]
	public double MultiplierMax = 3.0;

	internal double TimeAdjust => (double)DeltaAdjustment / 100.0;

	internal double SmoothAdjustRange => (double)AllowedJitter / 100.0;

	internal double SnapAdjustRange => (double)SnapLimit / 100.0;
}
