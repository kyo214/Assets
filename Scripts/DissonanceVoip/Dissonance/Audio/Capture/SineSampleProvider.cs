using System;
using NAudio.Wave;

namespace Dissonance.Audio.Capture;

internal class SineSampleProvider : ISampleProvider
{
	private readonly double _step;

	private const double TwoPi = Math.PI * 2.0;

	private double _index;

	public float Frequency { get; }

	public WaveFormat WaveFormat { get; }

	public SineSampleProvider(WaveFormat format, float frequency)
	{
		WaveFormat = format;
		Frequency = frequency;
		_step = Math.PI * 2.0 * (double)Frequency / (double)WaveFormat.SampleRate;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		for (int i = offset; i < count; i++)
		{
			buffer[i] = (float)Math.Sin(_index) * 0.95f;
			_index = (_index + _step) % (Math.PI * 2.0);
		}
		return count;
	}
}
