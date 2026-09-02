using System.Collections.Generic;
using Dissonance.Networking;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Audio.Playback;

public abstract class BaseVoicePlayback : MonoBehaviour, IVoicePlaybackInternal, IRemoteChannelProvider, IVoicePlayback, IVolumeProvider
{
	private IPriorityManager _priorityManager;

	private IVolumeProvider _volumeProvider;

	private readonly SpeechSessionStream _sessions;

	private FrameFormat _frameFormat;

	private CodecSettings _codecSettings;

	private Transform _transformCache;

	protected PlaybackOptions? LatestPlaybackOptions => TryGetActiveSession()?.PlaybackOptions;

	private Transform Transform
	{
		get
		{
			if (_transformCache == null)
			{
				_transformCache = base.transform;
			}
			return _transformCache;
		}
	}

	public bool IsActive => base.isActiveAndEnabled;

	public bool AllowPositionalPlayback { get; set; }

	public bool IsMuted { get; set; }

	public float PlaybackVolume { get; set; }

	public string PlayerName
	{
		get
		{
			return _sessions.PlayerName;
		}
		set
		{
			_sessions.PlayerName = value;
		}
	}

	ChannelPriority IVoicePlayback.Priority
	{
		get
		{
			PlaybackOptions? latestPlaybackOptions = LatestPlaybackOptions;
			if (!latestPlaybackOptions.HasValue)
			{
				return ChannelPriority.None;
			}
			return latestPlaybackOptions.Value.Priority;
		}
	}

	float IVolumeProvider.TargetVolume
	{
		get
		{
			if (((IVoicePlaybackInternal)this).IsMuted)
			{
				return 0f;
			}
			IPriorityManager priorityManager = _priorityManager;
			if (priorityManager != null && priorityManager.TopPriority > ((IVoicePlayback)this).Priority)
			{
				return 0f;
			}
			float num = _volumeProvider?.TargetVolume ?? 1f;
			return ((IVoicePlaybackInternal)this).PlaybackVolume * num;
		}
	}

	public float Jitter => ((IJitterEstimator)_sessions).Jitter;

	public float? PacketLoss => TryGetActiveSession()?.PacketLoss;

	public bool IsSpeaking => TryGetActiveSession().HasValue;

	CodecSettings IVoicePlaybackInternal.CodecSettings
	{
		get
		{
			return _codecSettings;
		}
		set
		{
			_codecSettings = value;
			if (_frameFormat.Codec != _codecSettings.Codec || _frameFormat.FrameSize != _codecSettings.FrameSize || _frameFormat.WaveFormat == null || _frameFormat.WaveFormat.SampleRate != _codecSettings.SampleRate)
			{
				_frameFormat = new FrameFormat(_codecSettings.Codec, new WaveFormat(_codecSettings.SampleRate, 1), _codecSettings.FrameSize);
			}
		}
	}

	public abstract float Amplitude { get; }

	protected BaseVoicePlayback()
	{
		_sessions = new SpeechSessionStream(this);
		PlaybackVolume = 1f;
	}

	public virtual void Setup(IPriorityManager priority, IVolumeProvider volume)
	{
		_priorityManager = priority;
		_volumeProvider = volume;
	}

	protected virtual void Start()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
		_sessions.StopSession(logNoSessionError: false);
	}

	protected virtual void Update()
	{
	}

	public void GetRemoteChannels(List<RemoteChannel> output)
	{
		output.Clear();
		TryGetActiveSession()?.Channels.GetRemoteChannels(output);
	}

	void IVoicePlaybackInternal.Reset()
	{
		((IVoicePlaybackInternal)this).IsMuted = false;
		((IVoicePlaybackInternal)this).PlaybackVolume = 1f;
	}

	void IVoicePlaybackInternal.SetTransform(Vector3 pos, Quaternion rot)
	{
		SetTransform(pos, rot);
	}

	protected virtual void SetTransform(Vector3 pos, Quaternion rot)
	{
		Transform obj = Transform;
		obj.position = pos;
		obj.rotation = rot;
	}

	void IVoicePlaybackInternal.StartPlayback()
	{
		_sessions.StartSession(_frameFormat);
	}

	void IVoicePlaybackInternal.StopPlayback()
	{
		_sessions.StopSession();
	}

	void IVoicePlaybackInternal.ReceiveAudioPacket(VoicePacket packet)
	{
		_sessions.ReceiveFrame(packet);
	}

	void IVoicePlaybackInternal.ForceReset()
	{
		_sessions.ForceReset();
	}

	protected SpeechSession? TryDequeueSession(int? outputRate = null)
	{
		_sessions.SetFixedOutputRate(outputRate);
		return _sessions.TryDequeueSession();
	}

	protected abstract SpeechSession? TryGetActiveSession();
}
