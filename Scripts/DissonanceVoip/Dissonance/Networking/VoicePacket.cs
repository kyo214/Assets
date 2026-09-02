using System;
using System.Collections.Generic;
using Dissonance.Audio.Playback;
using JetBrains.Annotations;

namespace Dissonance.Networking;

public readonly struct VoicePacket
{
	public readonly string SenderPlayerId;

	public readonly ArraySegment<byte> EncodedAudioFrame;

	public readonly uint SequenceNumber;

	[CanBeNull]
	public readonly List<RemoteChannel> Channels;

	[Obsolete("Use `PlaybackOptions.IsPositional` property")]
	public bool Positional => PlaybackOptions.IsPositional;

	[Obsolete("Use `PlaybackOptions.Priority` property")]
	public ChannelPriority Priority => PlaybackOptions.Priority;

	[Obsolete("Use `PlaybackOptions.AmplitudeMultiplier` property")]
	public float AmplitudeMultiplier => PlaybackOptions.AmplitudeMultiplier;

	public PlaybackOptions PlaybackOptions { get; }

	public VoicePacket(string senderPlayerId, ChannelPriority priority, float ampMul, bool positional, ArraySegment<byte> encodedAudioFrame, uint sequence, [CanBeNull] List<RemoteChannel> channels = null)
	{
		PlaybackOptions = new PlaybackOptions(positional, ampMul, priority);
		SenderPlayerId = senderPlayerId;
		EncodedAudioFrame = encodedAudioFrame;
		SequenceNumber = sequence;
		Channels = channels;
	}
}
