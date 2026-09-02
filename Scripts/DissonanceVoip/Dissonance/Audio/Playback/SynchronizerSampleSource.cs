using System;
using System.Diagnostics;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Audio.Playback;

internal class SynchronizerSampleSource : ISampleSource, IRateProvider
{
	private static readonly Log Log = Logs.Create(LogCategory.Playback, "SynchronizerSampleSource");

	private static readonly float[] DesyncFixBuffer = new float[1024];

	private readonly ISampleSource _upstream;

	private readonly TimeSpan _resetDesyncTime;

	private readonly Stopwatch _timer = new Stopwatch();

	private bool _enabled;

	private TimeSpan _aheadWarningLastSent = TimeSpan.MinValue;

	private long _totalSamplesRead;

	private DesyncCalculator _desync;

	public TimeSpan IdealPlaybackPosition => _timer.Elapsed;

	public TimeSpan PlaybackPosition => TimeSpan.FromSeconds((double)_totalSamplesRead / (double)WaveFormat.SampleRate);

	public TimeSpan Desync => TimeSpan.FromMilliseconds(_desync.DesyncMilliseconds);

	public WaveFormat WaveFormat => _upstream.WaveFormat;

	public float PlaybackRate { get; private set; }

	public SyncState State => new SyncState(PlaybackPosition, IdealPlaybackPosition, Desync, PlaybackRate, _enabled);

	public SynchronizerSampleSource(ISampleSource upstream, TimeSpan resetDesyncTime)
	{
		_upstream = upstream;
		_resetDesyncTime = resetDesyncTime;
	}

	public void Prepare(SessionContext context)
	{
		_timer.Reset();
		_desync = default;
		PlaybackRate = 1f;
		_totalSamplesRead = 0L;
		_aheadWarningLastSent = TimeSpan.FromSeconds(0.0);
		_upstream.Prepare(context);
	}

	public void Enable()
	{
		_enabled = true;
	}

	public void Reset()
	{
		_timer.Reset();
		_enabled = false;
		_upstream.Reset();
	}

	public bool Read(ArraySegment<float> samples)
	{
		if (!_enabled)
		{
			PlaybackRate = 1f;
			return _upstream.Read(samples);
		}
		if (!_timer.IsRunning)
		{
			_timer.Reset();
			_timer.Start();
		}
		_totalSamplesRead += samples.Count;
		_desync.Update(IdealPlaybackPosition, PlaybackPosition);
		float correctedPlaybackSpeed = _desync.CorrectedPlaybackSpeed;
		PlaybackRate = ((correctedPlaybackSpeed < PlaybackRate) ? correctedPlaybackSpeed : Mathf.LerpUnclamped(PlaybackRate, _desync.CorrectedPlaybackSpeed, 0.25f));
		bool num = Skip(_desync.DesyncMilliseconds, out var deltaSamples, out var deltaDesyncMilliseconds);
		if (deltaSamples > 0)
		{
			_totalSamplesRead += deltaSamples;
			_desync.Skip(deltaDesyncMilliseconds);
		}
		if (num)
		{
			Array.Clear(samples.Array, samples.Offset, samples.Count);
			return true;
		}
		return _upstream.Read(samples);
	}

	private bool Skip(int desyncMilliseconds, out int deltaSamples, out int deltaDesyncMilliseconds)
	{
		double num = desyncMilliseconds;
		TimeSpan resetDesyncTime = _resetDesyncTime;
		if (num > resetDesyncTime.TotalMilliseconds)
		{
			Log.Warn("Playback desync ({0}ms) beyond recoverable threshold; resetting stream to current time", desyncMilliseconds);
			deltaSamples = desyncMilliseconds * WaveFormat.SampleRate / 1000;
			deltaDesyncMilliseconds = -desyncMilliseconds;
			PlaybackRate = 1f;
			int num2 = deltaSamples;
			while (num2 > 0)
			{
				int num3 = Math.Min(num2, DesyncFixBuffer.Length);
				if (_upstream.Read(new ArraySegment<float>(DesyncFixBuffer, 0, num3)))
				{
					return true;
				}
				num2 -= num3;
			}
			return false;
		}
		double num4 = desyncMilliseconds;
		resetDesyncTime = _resetDesyncTime;
		if (num4 < 0.0 - resetDesyncTime.TotalMilliseconds && PlaybackPosition - _aheadWarningLastSent > TimeSpan.FromSeconds(1.0))
		{
			_aheadWarningLastSent = PlaybackPosition;
			Log.Error("Playback desync ({0}ms) AHEAD beyond recoverable threshold", desyncMilliseconds);
		}
		deltaSamples = 0;
		deltaDesyncMilliseconds = 0;
		return false;
	}
}
