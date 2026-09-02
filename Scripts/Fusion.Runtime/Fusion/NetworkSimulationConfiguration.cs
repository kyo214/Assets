using System;
using Fusion.Sockets;
using UnityEngine;

namespace Fusion;

[Serializable]
public class NetworkSimulationConfiguration
{
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public bool Enabled;

	[InlineHelp]
	[DrawIf("Enabled")]
	[MultiPropertyDrawersFix]
	public NetConfigSimulationOscillator.WaveShape DelayShape = NetConfigSimulationOscillator.WaveShape.Sine;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 0.5)]
	[MultiPropertyDrawersFix]
	public double DelayMin = 0.01;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 0.5)]
	[MultiPropertyDrawersFix]
	public double DelayMax = 0.1;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 10.0)]
	[MultiPropertyDrawersFix]
	public double DelayPeriod = 3.0;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 1.0)]
	[MultiPropertyDrawersFix]
	public double DelayThreshold = 0.5;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 0.10000000149011612)]
	[MultiPropertyDrawersFix]
	public double AdditionalJitter = 0.01;

	[InlineHelp]
	[DrawIf("Enabled")]
	[MultiPropertyDrawersFix]
	[Space]
	public NetConfigSimulationOscillator.WaveShape LossChanceShape = NetConfigSimulationOscillator.WaveShape.Sine;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.NormalizedPercentage, 0.0, 1.0)]
	[MultiPropertyDrawersFix]
	public double LossChanceMin = 0.0;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.NormalizedPercentage, 0.0, 1.0)]
	[MultiPropertyDrawersFix]
	public double LossChanceMax = 0.02;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.NormalizedPercentage, 0.0, 1.0)]
	[MultiPropertyDrawersFix]
	public double LossChanceThreshold = 0.9;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.Seconds, 0.0, 10.0)]
	[MultiPropertyDrawersFix]
	public double LossChancePeriod = 3.0;

	[InlineHelp]
	[DrawIf("Enabled")]
	[Unit(Units.NormalizedPercentage, 0.0, 1.0)]
	[MultiPropertyDrawersFix]
	public double AdditionalLoss = 0.005;

	public NetworkSimulationConfiguration Clone()
	{
		return (NetworkSimulationConfiguration)MemberwiseClone();
	}

	public NetConfigSimulation Create()
	{
		NetConfigSimulation defaults = NetConfigSimulation.Defaults;
		if (Enabled)
		{
			if (DelayMin == 0.0 && DelayMax == 0.0)
			{
				defaults.DelayOscillator.Min = 0.0;
				defaults.DelayOscillator.Max = 0.0;
			}
			else if (DelayMin > DelayMax)
			{
				defaults.DelayOscillator.Min = Math.Max(9.999999747378752E-05, DelayMax * 0.5);
				defaults.DelayOscillator.Max = Math.Max(9.999999747378752E-05, DelayMin * 0.5);
			}
			else
			{
				defaults.DelayOscillator.Min = Math.Max(9.999999747378752E-05, DelayMin * 0.5);
				defaults.DelayOscillator.Max = Math.Max(9.999999747378752E-05, DelayMax * 0.5);
			}
			defaults.DelayOscillator.Period = DelayPeriod;
			defaults.DelayOscillator.Shape = DelayShape;
			defaults.DelayOscillator.Threshold = DelayThreshold;
			defaults.DelayOscillator.Additional = AdditionalJitter * 0.5;
			if (LossChanceMin == 0.0 && LossChanceMax == 0.0)
			{
				defaults.LossOscillator.Min = 0.0;
				defaults.LossOscillator.Max = 0.0;
			}
			else if (LossChanceMin > LossChanceMax)
			{
				defaults.LossOscillator.Min = Math.Max(9.999999747378752E-05, LossChanceMax * 0.5);
				defaults.LossOscillator.Max = Math.Max(9.999999747378752E-05, LossChanceMin * 0.5);
			}
			else
			{
				defaults.LossOscillator.Min = Math.Max(9.999999747378752E-05, LossChanceMin * 0.5);
				defaults.LossOscillator.Max = Math.Max(9.999999747378752E-05, LossChanceMax * 0.5);
			}
			defaults.LossOscillator.Period = LossChancePeriod;
			defaults.LossOscillator.Shape = LossChanceShape;
			defaults.LossOscillator.Threshold = LossChanceThreshold;
			defaults.LossOscillator.Additional = AdditionalLoss;
		}
		return defaults;
	}
}
