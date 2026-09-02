using System;

namespace Dissonance.Audio.Codecs.Identity;

internal class IdentityEncoder : IVoiceEncoder, IDisposable
{
	public float PacketLoss
	{
		set
		{
		}
	}

	public int FrameSize { get; }

	public int SampleRate { get; }

	public IdentityEncoder(int sampleRate, int frameSize)
	{
		SampleRate = sampleRate;
		FrameSize = frameSize;
	}

	public ArraySegment<byte> Encode(ArraySegment<float> samples, ArraySegment<byte> array)
	{
		float[] src = samples.Array ?? throw new ArgumentNullException("samples");
		byte[] dst = array.Array ?? throw new ArgumentNullException("array");
		int num = samples.Count * 4;
		if (num > array.Count)
		{
			throw new ArgumentException("output buffer is too small");
		}
		Buffer.BlockCopy(src, samples.Offset, dst, array.Offset, num);
		return new ArraySegment<byte>(array.Array, array.Offset, num);
	}

	public void Reset()
	{
	}

	public void Dispose()
	{
	}
}
