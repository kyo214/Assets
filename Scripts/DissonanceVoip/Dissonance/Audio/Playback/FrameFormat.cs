using System;
using Dissonance.Audio.Codecs;
using NAudio.Wave;

namespace Dissonance.Audio.Playback;

internal readonly struct FrameFormat(Codec codec, WaveFormat waveFormat, uint frameSize) : IEquatable<FrameFormat>
{
	public readonly Codec Codec = codec;

	public readonly WaveFormat WaveFormat = waveFormat;

	public readonly uint FrameSize = frameSize;

	public override int GetHashCode()
	{
		return (((103577 + (int)(Codec + 17)) * 101117 + WaveFormat.GetHashCode()) * 101117 + (int)FrameSize) * 101117;
	}

	public bool Equals(FrameFormat other)
	{
		if (Codec != other.Codec)
		{
			return false;
		}
		if (FrameSize != other.FrameSize)
		{
			return false;
		}
		if (!WaveFormat.Equals(other.WaveFormat))
		{
			return false;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is FrameFormat other)
		{
			return Equals(other);
		}
		return false;
	}
}
