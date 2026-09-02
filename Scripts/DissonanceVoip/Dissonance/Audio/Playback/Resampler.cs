using System;
using NAudio.Dsp;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Audio.Playback;

internal class Resampler : ISampleSource
{
	private static readonly Log Log = Logs.Create(LogCategory.Playback, "Resampler");

	private readonly ISampleSource _source;

	private readonly IRateProvider _rate;

	private volatile WaveFormat _outputFormat;

	private readonly WdlResampler _resampler;

	private bool _fixedRateEnabled;

	public WaveFormat WaveFormat => _outputFormat;

	public Resampler(ISampleSource source, IRateProvider rate)
	{
		_source = source;
		_rate = rate;
		AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
		_resampler = new WdlResampler();
		_resampler.SetMode(interp: true, 2, sinc: false);
		_resampler.SetFilterParms();
		_resampler.SetFeedMode(wantInputDriven: false);
	}

	public void Prepare(SessionContext context)
	{
		OnAudioConfigurationChanged(deviceWasChanged: false);
		_source.Prepare(context);
	}

	public bool Read(ArraySegment<float> samples)
	{
		WaveFormat waveFormat = _source.WaveFormat;
		WaveFormat outputFormat = _outputFormat;
		double num = outputFormat.SampleRate;
		if (Mathf.Abs(_rate.PlaybackRate - 1f) > 0.01f)
		{
			num = (float)outputFormat.SampleRate * (1f / _rate.PlaybackRate);
		}
		if (num != _resampler.OutputSampleRate)
		{
			_resampler.SetRates(waveFormat.SampleRate, num);
		}
		int channels = waveFormat.Channels;
		int num2 = samples.Count / channels;
		int num3 = _resampler.ResamplePrepare(num2, channels, out var inbuffer, out var inbufferOffset);
		ArraySegment<float> samples2 = new ArraySegment<float>(inbuffer, inbufferOffset, num3 * channels);
		bool result = _source.Read(samples2);
		_resampler.ResampleOut(samples.Array, samples.Offset, num3, num2, channels);
		return result;
	}

	public void Reset()
	{
		_resampler?.Reset();
		_source.Reset();
	}

	private void OnAudioConfigurationChanged(bool deviceWasChanged)
	{
		if (!_fixedRateEnabled)
		{
			_outputFormat = new WaveFormat(AudioSettings.outputSampleRate, _source.WaveFormat.Channels);
		}
	}

	public void SetOutputRate(int? rate)
	{
		if (rate.HasValue)
		{
			_fixedRateEnabled = true;
			if (_outputFormat == null || _outputFormat.SampleRate != rate.Value)
			{
				_outputFormat = new WaveFormat(rate.Value, _source.WaveFormat.Channels);
			}
		}
		else
		{
			_fixedRateEnabled = false;
			OnAudioConfigurationChanged(deviceWasChanged: false);
		}
	}
}
