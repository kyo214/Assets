using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion;

[Serializable]
public class SimulationConfig
{
	public enum Topologies
	{
		ClientServer = 0,
		Shared = 1
	}

	public enum StateReplicationModes
	{
		DeltaSnapshots = 0,
		EventualConsistency = 1
	}

	[HideInInspector]
	[InlineHelp]
	public int InputDataWordCount;

	[Unit(Units.Ticks, 1.0, 128.0)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int TickRate = 60;

	[Unit(Units.Ticks, 1.0, 128.0)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int MaxPrediction = 60;

	[FormerlySerializedAs("Players")]
	[Unit(Units.None, 1.0, 255.0)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int DefaultPlayers = 10;

	[InlineHelp]
	public StateReplicationModes ReplicationMode;

	[NonSerialized]
	public Topologies Topology;

	[NonSerialized]
	public bool HostMigration;

	[InlineHelp]
	[FormerlySerializedAs("UseAreaOfInterest")]
	[FormerlySerializedAs("InterestManagement")]
	[DrawIf("ReplicationMode")]
	[MultiPropertyDrawersFix]
	public bool ObjectInterest;

	[Unit(Units.Ticks, 1.0, 8.0)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int ServerPacketInterval = 1;

	[Unit(Units.Ticks, 1.0, 8.0)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int ClientPacketInterval = 1;

	internal bool EnableHalfNetworkTick = true;

	public int InputTotalWordCount => InputDataWordCount + 4;

	internal int ServerTickMultiplier => 1;

	public double DeltaTime => 1.0 / (double)TickRate;

	public double ServerDeltaTime => DeltaTime * (double)ServerTickMultiplier;

	public double ServerPacketDeltaTime => (double)ServerPacketInterval * ServerDeltaTime;

	public double ClientPacketDeltaTime => (double)ClientPacketInterval * DeltaTime;

	internal SimulationConfig Init(int? playerCountOverride, int? inputWordCount)
	{
		SimulationConfig simulationConfig = Copy();
		if (playerCountOverride.HasValue)
		{
			simulationConfig.DefaultPlayers = playerCountOverride.Value;
		}
		simulationConfig.DefaultPlayers = Maths.Clamp(simulationConfig.DefaultPlayers, 1, 2048);
		if (inputWordCount.HasValue)
		{
			simulationConfig.InputDataWordCount = inputWordCount.Value;
		}
		return simulationConfig;
	}

	internal SimulationConfig Copy()
	{
		return (SimulationConfig)MemberwiseClone();
	}
}
