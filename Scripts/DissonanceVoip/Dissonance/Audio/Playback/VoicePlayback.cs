using UnityEngine;

namespace Dissonance.Audio.Playback;

public class VoicePlayback : BaseVoicePlayback
{
	private static readonly Log Log = Logs.Create(LogCategory.Playback, "Voice Playback Component");

	private SamplePlaybackComponent _player;

	private float? _savedSpatialBlend;

	public AudioSource AudioSource { get; private set; }

	public override float Amplitude
	{
		get
		{
			if (!(_player == null))
			{
				return _player.ARV;
			}
			return 0f;
		}
	}

	public void Awake()
	{
		AudioSource = GetComponent<AudioSource>();
		_player = GetComponent<SamplePlaybackComponent>();
		((IVoicePlaybackInternal)this).Reset();
	}

	public override void Setup(IPriorityManager priority, IVolumeProvider volume)
	{
		base.Setup(priority, volume);
		AudioSource audioSource = base.gameObject.GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.bypassReverbZones = true;
		}
		audioSource.loop = true;
		audioSource.pitch = 1f;
		audioSource.clip = null;
		audioSource.playOnAwake = false;
		audioSource.ignoreListenerPause = true;
		audioSource.Stop();
		if (base.gameObject.GetComponent<SamplePlaybackComponent>() == null)
		{
			base.gameObject.AddComponent<SamplePlaybackComponent>();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		AudioSource.Stop();
		if (AudioSource.spatialize)
		{
			AudioSource.spatialize = false;
		}
		AudioSource.clip = AudioClip.Create("Flatline", 4096, 1, AudioSettings.outputSampleRate, stream: false, (float[] buf) =>
		{
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = 1f;
			}
		});
		AudioSource.loop = true;
		AudioSource.pitch = 1f;
		AudioSource.dopplerLevel = 0f;
		AudioSource.mute = false;
		AudioSource.priority = 0;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (AudioSource != null && AudioSource.clip != null)
		{
			AudioClip clip = AudioSource.clip;
			AudioSource.clip = null;
			Object.Destroy(clip);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!_player.HasActiveSession)
		{
			SpeechSession? speechSession = TryDequeueSession();
			if (speechSession.HasValue)
			{
				_player.Play(speechSession.Value);
				AudioSource.Play();
			}
			else if (AudioSource.isPlaying)
			{
				AudioSource.Stop();
			}
		}
		if (AudioSource.mute)
		{
			Log.Warn("Voice AudioSource was muted, unmuting source. To mute a specific Dissonance player see: https://placeholder-software.co.uk/dissonance/docs/Reference/Other/VoicePlayerState.html#islocallymuted-bool");
			AudioSource.mute = false;
		}
		UpdatePositionalPlayback();
	}

	private void UpdatePositionalPlayback()
	{
		if (!_player.Session.HasValue)
		{
			return;
		}
		bool flag = base.LatestPlaybackOptions?.IsPositional ?? false;
		if (((IVoicePlaybackInternal)this).AllowPositionalPlayback & flag)
		{
			if (_savedSpatialBlend.HasValue)
			{
				AudioSource.spatialBlend = _savedSpatialBlend.Value;
				_savedSpatialBlend = null;
			}
		}
		else if (!_savedSpatialBlend.HasValue)
		{
			_savedSpatialBlend = AudioSource.spatialBlend;
			AudioSource.spatialBlend = 0f;
		}
	}

	protected override SpeechSession? TryGetActiveSession()
	{
		if (!(_player == null))
		{
			return _player.Session;
		}
		return null;
	}
}
