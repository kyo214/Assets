using System;
using Dissonance.Extensions;
using NAudio.Wave;

namespace Dissonance.Audio.Capture;

internal class SampleToFrameProvider : IFrameProvider
{
	private readonly ISampleProvider _source;

	private int _samplesInFrame;

	private readonly float[] _frame;

	public WaveFormat WaveFormat => _source.WaveFormat;

	public uint FrameSize { get; }

	public SampleToFrameProvider(ISampleProvider source, uint frameSize)
	{
		_source = source;
		FrameSize = frameSize;
		_frame = new float[frameSize];
	}

	public bool Read(ArraySegment<float> outBuffer)
	{
		if (outBuffer.Count < FrameSize)
		{
			throw new ArgumentException($"Supplied buffer is smaller than frame size. {outBuffer.Count} < {FrameSize}", "outBuffer");
		}
		_samplesInFrame += _source.Read(_frame, _samplesInFrame, checked((int)(FrameSize - _samplesInFrame)));
		if (_samplesInFrame == FrameSize)
		{
			outBuffer.CopyFrom(_frame);
			_samplesInFrame = 0;
			return true;
		}
		return false;
	}

	public void Reset()
	{
		_samplesInFrame = 0;
	}
}
