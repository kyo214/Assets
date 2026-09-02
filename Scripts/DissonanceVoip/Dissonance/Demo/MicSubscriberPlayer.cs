using System;
using Dissonance.Audio.Capture;
using Dissonance.Audio.Playback;
using JetBrains.Annotations;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Demo;

public class MicSubscriberPlayer : MonoBehaviour, IMicrophoneSubscriber
{
	private class SourceWrapper : ISampleSource
	{
		private readonly BufferedSampleProvider _provider;

		public WaveFormat WaveFormat => _provider.WaveFormat;

		public SourceWrapper(BufferedSampleProvider provider)
		{
			_provider = provider;
		}

		public void Prepare(SessionContext context)
		{
		}

		public bool Read(ArraySegment<float> samples)
		{
			Array.Clear(samples.Array, samples.Offset, samples.Count);
			_provider.Read(samples.Array, samples.Offset, samples.Count);
			return false;
		}

		public void Reset()
		{
			_provider.Reset();
		}
	}

	private class ConstantRate : IRateProvider
	{
		public float PlaybackRate => 1f;
	}

	private BufferedSampleProvider _inputBuffer;

	private Dissonance.Audio.Playback.Resampler _output;

	private bool _playing;

	private void OnAudioFilterRead([NotNull] float[] data, int channels)
	{
		Array.Clear(data, 0, data.Length);
		BufferedSampleProvider inputBuffer = _inputBuffer;
		if (inputBuffer == null)
		{
			return;
		}
		bool flag = false;
		if (!_playing && inputBuffer.Count > 1000)
		{
			_playing = true;
			flag = true;
		}
		if (!_playing)
		{
			return;
		}
		int num = data.Length / channels;
		float[] array = new float[num];
		_output.Read(new ArraySegment<float>(array, 0, num));
		if (flag)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] *= (float)i / (float)array.Length;
			}
		}
		int num2 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			for (int k = 0; k < channels; k++)
			{
				data[num2++] = array[j];
			}
		}
	}

	public void ReceiveMicrophoneData(ArraySegment<float> buffer, WaveFormat format)
	{
		if (_inputBuffer != null && _inputBuffer.WaveFormat.Equals(format))
		{
			_inputBuffer.Write(buffer);
		}
	}

	void IMicrophoneSubscriber.Reset()
	{
		_playing = false;
	}

	public void SetFormat(WaveFormat format)
	{
		_playing = false;
		_inputBuffer = new BufferedSampleProvider(format, 12000);
		_output = new Dissonance.Audio.Playback.Resampler(new SourceWrapper(_inputBuffer), new ConstantRate());
		_playing = false;
		_output.Prepare(new SessionContext("name", 0u));
	}
}
