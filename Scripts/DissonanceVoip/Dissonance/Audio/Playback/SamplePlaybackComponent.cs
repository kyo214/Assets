using System;
using System.Threading;
using Dissonance.Config;
using JetBrains.Annotations;
using UnityEngine;

namespace Dissonance.Audio.Playback;

public class SamplePlaybackComponent : MonoBehaviour
{
	private static readonly Log Log = Logs.Create(LogCategory.Playback, "SamplePlaybackComponent");

	private float[] _temp;

	[CanBeNull]
	private AudioFileWriter _diagnosticOutput;

	private SessionContext _lastPlayedSessionContext;

	private readonly ReaderWriterLockSlim _sessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

	private volatile float _arv;

	private string _playerName;

	private string _metricBufferSize;

	private IAudioOutputSubscriber[] _subscribers;

	public bool HasActiveSession => Session.HasValue;

	public SpeechSession? Session { get; private set; }

	public float ARV => _arv;

	public void Play(SpeechSession session)
	{
		if (Session.HasValue)
		{
			throw Log.CreatePossibleBugException("Attempted to play a session when one is already playing", "C4F19272-994D-4025-AAEF-37BB62685C2E");
		}
		if (_playerName != session.Context.PlayerName)
		{
			_metricBufferSize = Metrics.MetricName("PlaybackDequeueBufferSize", session.Context.PlayerName);
			_playerName = session.Context.PlayerName;
		}
		if (DebugSettings.Instance.EnablePlaybackDiagnostics && DebugSettings.Instance.RecordFinalAudio)
		{
			string filename = $"Dissonance_Diagnostics/Output_{session.Context.PlayerName}_{session.Context.Id}_{DateTime.UtcNow.ToFileTime()}";
			Interlocked.Exchange(ref _diagnosticOutput, new AudioFileWriter(filename, session.OutputWaveFormat));
		}
		_sessionLock.EnterWriteLock();
		try
		{
			ApplyReset();
			Session = session;
		}
		finally
		{
			_sessionLock.ExitWriteLock();
		}
	}

	public void Start()
	{
		_temp = new float[AudioSettings.outputSampleRate];
		_subscribers = GetComponentsInChildren<IAudioOutputSubscriber>();
	}

	public void OnEnable()
	{
		Session = null;
		ApplyReset();
	}

	public void OnDisable()
	{
		Session = null;
		ApplyReset();
	}

	public void OnAudioFilterRead([NotNull] float[] data, int channels)
	{
		if (!Session.HasValue)
		{
			Array.Clear(data, 0, data.Length);
			return;
		}
		_sessionLock.EnterUpgradeableReadLock();
		try
		{
			SpeechSession? session = Session;
			if (!session.HasValue)
			{
				Array.Clear(data, 0, data.Length);
				return;
			}
			SpeechSession value = session.Value;
			if (!value.Context.Equals(_lastPlayedSessionContext))
			{
				_lastPlayedSessionContext = session.Value.Context;
				ApplyReset();
			}
			Metrics.Sample(_metricBufferSize, value.BufferCount);
			bool num = Filter(value, data, channels, _temp, _diagnosticOutput, _subscribers, out var arv);
			_arv = arv;
			if (num)
			{
				_sessionLock.EnterWriteLock();
				try
				{
					Session = null;
				}
				finally
				{
					_sessionLock.ExitWriteLock();
				}
				ApplyReset();
				_diagnosticOutput?.Dispose();
				_diagnosticOutput = null;
			}
		}
		finally
		{
			_sessionLock.ExitUpgradeableReadLock();
		}
	}

	private void ApplyReset()
	{
		_arv = 0f;
	}

	internal static bool Filter(SpeechSession session, [NotNull] float[] output, int channels, [NotNull] float[] temp, [CanBeNull] AudioFileWriter diagnosticOutput, IAudioOutputSubscriber[] subscribers, out float arv)
	{
		int count = output.Length / channels;
		ArraySegment<float> arraySegment = new ArraySegment<float>(temp, 0, count);
		bool flag = session.Read(arraySegment);
		if (subscribers != null)
		{
			for (int i = 0; i < subscribers.Length; i++)
			{
				subscribers[i].OnAudioPlayback(arraySegment, flag);
			}
		}
		diagnosticOutput?.WriteSamples(new ArraySegment<float>(temp, 0, count));
		float num = 0f;
		int num2 = 0;
		for (int j = 0; j < output.Length; j += channels)
		{
			float num3 = temp[num2++];
			num += Mathf.Abs(num3);
			for (int k = 0; k < channels; k++)
			{
				output[j + k] *= num3;
			}
		}
		arv = num / (float)output.Length;
		return flag;
	}
}
