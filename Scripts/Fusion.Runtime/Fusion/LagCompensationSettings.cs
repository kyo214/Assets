using System;
using UnityEngine;

namespace Fusion;

[Serializable]
public class LagCompensationSettings
{
	[InlineHelp]
	[Unit(Units.Ticks, 2.0, 40.0, ClampMin = true, ClampMax = false)]
	[WarnIf(MsgProvider = "_bufferWarnTextProvider", MsgTypeProvider = "_bufferMsgTypeProvider")]
	[MultiPropertyDrawersFix]
	public int HitboxBufferSize = 12;

	[InlineHelp]
	[Unit(Units.Count, 16.0, 1024.0, ClampMin = true, ClampMax = false, UseSlider = false)]
	[MultiPropertyDrawersFix]
	public int HitboxCapacity = 512;

	[Unit(Units.NormalizedPercentage)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public float ExpansionFactor = 0.2f;

	[InlineHelp]
	public bool Optimize = false;

	[InlineHelp]
	public bool DebugBroadphase = false;

	[InlineHelp]
	public bool DebugHistory = false;

	[InlineHelp]
	public Color DebugColor = new Color(0f, 1f, 0f, 0.5f);

	[InlineHelp]
	public Color ClientDebugColor = new Color(0f, 0f, 1f, 0.5f);

	[InlineHelp]
	public Color HistoryDebugColor = new Color(0f, 0f, 1f, 0.5f);

	private string _bufferWarnTextProvider => (HitboxBufferSize > 40) ? "HitboxBufferSize greatly exceeds recommended value." : ((HitboxBufferSize > 20) ? "HitboxBufferSize exceeds recommended value, unless a very high tick rate (100+) is intended." : null);

	private int _bufferMsgTypeProvider => (HitboxBufferSize > 40) ? 3 : ((HitboxBufferSize > 20) ? 2 : 0);
}
