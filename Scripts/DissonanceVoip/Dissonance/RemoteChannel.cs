using System;
using Dissonance.Audio.Playback;
using JetBrains.Annotations;

namespace Dissonance;

public readonly struct RemoteChannel
{
	public ChannelType Type { get; }

	public PlaybackOptions Options { get; }

	public string TargetName { get; }

	public RemoteChannel([NotNull] string targetName, ChannelType type, PlaybackOptions options)
	{
		TargetName = targetName ?? throw new ArgumentNullException("targetName");
		Type = type;
		Options = options;
	}
}
