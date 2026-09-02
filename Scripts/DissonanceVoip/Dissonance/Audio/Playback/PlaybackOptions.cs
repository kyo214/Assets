namespace Dissonance.Audio.Playback;

public readonly struct PlaybackOptions(bool isPositional, float amplitudeMultiplier, ChannelPriority priority)
{
	public bool IsPositional { get; } = isPositional;

	public float AmplitudeMultiplier { get; } = amplitudeMultiplier;

	public ChannelPriority Priority { get; } = priority;
}
