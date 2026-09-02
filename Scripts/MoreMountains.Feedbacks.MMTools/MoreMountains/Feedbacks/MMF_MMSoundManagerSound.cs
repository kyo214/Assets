using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[ExecuteAlways]
[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Sound")]
[FeedbackHelp("This feedback will let you play a sound via the MMSoundManager. You will need a game object in your scene with a MMSoundManager object on it for this to work.")]
public class MMF_MMSoundManagerSound : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Sound", true, 14, true, false)]
	[Tooltip("the sound clip to play")]
	public AudioClip Sfx;

	[MMFInspectorGroup("Random Sound", true, 34, true, false)]
	[Tooltip("an array to pick a random sfx from")]
	public AudioClip[] RandomSfx;

	[Tooltip("if this is true, random sfx audio clips will be played in sequential order instead of at random")]
	public bool SequentialOrder;

	[Tooltip("if we're in sequential order, determines whether or not to hold at the last index, until either a cooldown is met, or the ResetSequentialIndex method is called")]
	[MMFCondition("SequentialOrder", true)]
	public bool SequentialOrderHoldLast;

	[Tooltip("if we're in sequential order hold last mode, index will reset to 0 automatically after this duration, unless it's 0, in which case it'll be ignored")]
	[MMFCondition("SequentialOrderHoldLast", true)]
	public float SequentialOrderHoldCooldownDuration = 2f;

	[MMFInspectorGroup("Debug", true, 31, false, false)]
	public MMF_Button TestPlayButton;

	public MMF_Button TestStopButton;

	public MMF_Button ResetSequentialIndexButton;

	[MMFInspectorGroup("Sound Properties", true, 24, false, false)]
	[Header("Volume")]
	[Tooltip("the minimum volume to play the sound at")]
	[Range(0f, 2f)]
	public float MinVolume = 1f;

	[Tooltip("the maximum volume to play the sound at")]
	[Range(0f, 2f)]
	public float MaxVolume = 1f;

	[Header("Pitch")]
	[Tooltip("the minimum pitch to play the sound at")]
	[Range(-3f, 3f)]
	public float MinPitch = 1f;

	[Tooltip("the maximum pitch to play the sound at")]
	[Range(-3f, 3f)]
	public float MaxPitch = 1f;

	[Header("Time")]
	[Tooltip("the minimum and maximum time stamps at which to play the sound")]
	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 PlaybackTime = new Vector2(0f, 0f);

	[MMFInspectorGroup("SoundManager Options", true, 28, false, false)]
	[Tooltip("the track on which to play the sound. Pick the one that matches the nature of your sound")]
	public MMSoundManager.MMSoundManagerTracks MmSoundManagerTrack;

	[Tooltip("the ID of the sound. This is useful if you plan on using sound control feedbacks on it afterwards.")]
	public int ID;

	[Tooltip("the AudioGroup on which to play the sound. If you're already targeting a preset track, you can leave it blank, otherwise the group you specify here will override it.")]
	public AudioMixerGroup AudioGroup;

	[Tooltip("if (for some reason) you've already got an audiosource and wouldn't like to use the built-in pool system, you can specify it here")]
	public AudioSource RecycleAudioSource;

	[Tooltip("whether or not this sound should loop")]
	public bool Loop;

	[Tooltip("whether or not this sound should continue playing when transitioning to another scene")]
	public bool Persistent;

	[Tooltip("whether or not this sound should play if the same sound clip is already playing")]
	public bool DoNotPlayIfClipAlreadyPlaying;

	[Tooltip("if this is true, this sound will stop playing when stopping the feedback")]
	public bool StopSoundOnFeedbackStop;

	[MMFInspectorGroup("Fade", true, 30, false, false)]
	[Tooltip("whether or not to fade this sound in when playing it")]
	public bool Fade;

	[Tooltip("if fading, the volume at which to start the fade")]
	[MMCondition("Fade", true)]
	public float FadeInitialVolume;

	[Tooltip("if fading, the duration of the fade, in seconds")]
	[MMCondition("Fade", true)]
	public float FadeDuration = 1f;

	[Tooltip("if fading, the tween over which to fade the sound ")]
	[MMCondition("Fade", true)]
	public MMTweenType FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic);

	[MMFInspectorGroup("Solo", true, 32, false, false)]
	[Tooltip("whether or not this sound should play in solo mode over its destination track. If yes, all other sounds on that track will be muted when this sound starts playing")]
	public bool SoloSingleTrack;

	[Tooltip("whether or not this sound should play in solo mode over all other tracks. If yes, all other tracks will be muted when this sound starts playing")]
	public bool SoloAllTracks;

	[Tooltip("if in any of the above solo modes, AutoUnSoloOnEnd will unmute the track(s) automatically once that sound stops playing")]
	public bool AutoUnSoloOnEnd;

	[MMFInspectorGroup("Spatial Settings", true, 33, false, false)]
	[Tooltip("Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.")]
	[Range(-1f, 1f)]
	public float PanStereo;

	[Tooltip("Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.")]
	[Range(0f, 1f)]
	public float SpatialBlend;

	[MMFInspectorGroup("Effects", true, 36, false, false)]
	[Tooltip("Bypass effects (Applied from filter components or global listener filters).")]
	public bool BypassEffects;

	[Tooltip("When set global effects on the AudioListener will not be applied to the audio signal generated by the AudioSource. Does not apply if the AudioSource is playing into a mixer group.")]
	public bool BypassListenerEffects;

	[Tooltip("When set doesn't route the signal from an AudioSource into the global reverb associated with reverb zones.")]
	public bool BypassReverbZones;

	[Tooltip("Sets the priority of the AudioSource.")]
	[Range(0f, 256f)]
	public int Priority = 128;

	[Tooltip("The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.")]
	[Range(0f, 1.1f)]
	public float ReverbZoneMix = 1f;

	[MMFInspectorGroup("3D Sound Settings", true, 37, false, false)]
	[Tooltip("Sets the Doppler scale for this AudioSource.")]
	[Range(0f, 5f)]
	public float DopplerLevel = 1f;

	[Tooltip("Sets the spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space.")]
	[Range(0f, 360f)]
	public int Spread;

	[Tooltip("Sets/Gets how the AudioSource attenuates over distance.")]
	public AudioRolloffMode RolloffMode;

	[Tooltip("Within the Min distance the AudioSource will cease to grow louder in volume.")]
	public float MinDistance = 1f;

	[Tooltip("(Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.")]
	public float MaxDistance = 500f;

	protected AudioClip _randomClip;

	protected AudioSource _editorAudioSource;

	protected MMSoundManagerPlayOptions _options;

	protected AudioSource _playedAudioSource;

	protected float _randomPlaybackTime;

	protected int _currentIndex;

	public override float FeedbackDuration => GetDuration();

	public override void InitializeCustomAttributes()
	{
		TestPlayButton = new MMF_Button("Debug Play Sound", TestPlaySound);
		TestStopButton = new MMF_Button("Debug Stop Sound", TestStopSound);
		ResetSequentialIndexButton = new MMF_Button("Reset Sequential Index", ResetSequentialIndex);
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		float intensity = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (Sfx != null)
		{
			PlaySound(Sfx, position, intensity);
		}
		else if (RandomSfx.Length != 0)
		{
			_randomClip = PickRandomClip();
			if (_randomClip != null)
			{
				PlaySound(_randomClip, position, intensity);
			}
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && StopSoundOnFeedbackStop && _playedAudioSource != null)
		{
			_playedAudioSource.Stop();
			MMPersistentSingleton<MMSoundManager>.Instance.FreeSound(_playedAudioSource);
		}
	}

	protected virtual void PlaySound(AudioClip sfx, Vector3 position, float intensity)
	{
		if (!DoNotPlayIfClipAlreadyPlaying || !(_playedAudioSource != null) || !_playedAudioSource.isPlaying)
		{
			float num = Random.Range(MinVolume, MaxVolume);
			if (!Timing.ConstantIntensity)
			{
				num *= intensity;
			}
			float pitch = Random.Range(MinPitch, MaxPitch);
			_randomPlaybackTime = Random.Range(PlaybackTime.x, PlaybackTime.y);
			if (!NormalPlayDirection)
			{
				_ = sfx.samples;
			}
			_options.MmSoundManagerTrack = MmSoundManagerTrack;
			_options.Location = position;
			_options.Loop = Loop;
			_options.Volume = num;
			_options.ID = ID;
			_options.Fade = Fade;
			_options.FadeInitialVolume = FadeInitialVolume;
			_options.FadeDuration = FadeDuration;
			_options.FadeTween = FadeTween;
			_options.Persistent = Persistent;
			_options.RecycleAudioSource = RecycleAudioSource;
			_options.AudioGroup = AudioGroup;
			_options.Pitch = pitch;
			_options.PlaybackTime = _randomPlaybackTime;
			_options.PanStereo = PanStereo;
			_options.SpatialBlend = SpatialBlend;
			_options.SoloSingleTrack = SoloSingleTrack;
			_options.SoloAllTracks = SoloAllTracks;
			_options.AutoUnSoloOnEnd = AutoUnSoloOnEnd;
			_options.BypassEffects = BypassEffects;
			_options.BypassListenerEffects = BypassListenerEffects;
			_options.BypassReverbZones = BypassReverbZones;
			_options.Priority = Priority;
			_options.ReverbZoneMix = ReverbZoneMix;
			_options.DopplerLevel = DopplerLevel;
			_options.Spread = Spread;
			_options.RolloffMode = RolloffMode;
			_options.MinDistance = MinDistance;
			_options.MaxDistance = MaxDistance;
			_playedAudioSource = MMSoundManagerSoundPlayEvent.Trigger(sfx, _options);
			_lastPlayTimestamp = FeedbackTime;
		}
	}

	protected virtual float GetDuration()
	{
		if (Sfx != null)
		{
			return Sfx.length - _randomPlaybackTime;
		}
		float num = 0f;
		if (RandomSfx != null && RandomSfx.Length != 0)
		{
			AudioClip[] randomSfx = RandomSfx;
			foreach (AudioClip audioClip in randomSfx)
			{
				if (audioClip != null && audioClip.length > num)
				{
					num = audioClip.length;
				}
			}
			return num - _randomPlaybackTime;
		}
		return 0f;
	}

	protected virtual async void TestPlaySound()
	{
		AudioClip audioClip = null;
		if (Sfx != null)
		{
			audioClip = Sfx;
		}
		if (RandomSfx != null && RandomSfx.Length != 0)
		{
			audioClip = PickRandomClip();
		}
		if (audioClip == null)
		{
			Debug.LogError(Label + " on " + Owner.gameObject.name + " can't play in editor mode, you haven't set its Sfx.");
			return;
		}
		float volume = Random.Range(MinVolume, MaxVolume);
		float num = Random.Range(MinPitch, MaxPitch);
		_randomPlaybackTime = Random.Range(PlaybackTime.x, PlaybackTime.y);
		GameObject temporaryAudioHost = new GameObject("EditorTestAS_WillAutoDestroy");
		SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
		temporaryAudioHost.transform.position = Owner.transform.position;
		_editorAudioSource = temporaryAudioHost.AddComponent<AudioSource>();
		PlayAudioSource(_editorAudioSource, audioClip, volume, num, _randomPlaybackTime);
		_lastPlayTimestamp = FeedbackTime;
		Debug.Log("time : " + _lastPlayTimestamp);
		await Task.Delay((int)(1000f * audioClip.length / Mathf.Abs(num)));
		Object.DestroyImmediate(temporaryAudioHost);
	}

	protected virtual void TestStopSound()
	{
		if (_editorAudioSource != null)
		{
			_editorAudioSource.Stop();
		}
	}

	protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch, float time)
	{
		audioSource.clip = sfx;
		audioSource.time = time;
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.loop = false;
		audioSource.Play();
	}

	protected virtual AudioClip PickRandomClip()
	{
		int num = 0;
		if (!SequentialOrder)
		{
			num = Random.Range(0, RandomSfx.Length);
		}
		else
		{
			num = _currentIndex;
			if (num >= RandomSfx.Length)
			{
				if (SequentialOrderHoldLast)
				{
					num--;
					if (SequentialOrderHoldCooldownDuration > 0f && FeedbackTime - _lastPlayTimestamp > SequentialOrderHoldCooldownDuration)
					{
						num = 0;
					}
				}
				else
				{
					num = 0;
				}
			}
			_currentIndex = num + 1;
		}
		return RandomSfx[num];
	}

	public virtual void ResetSequentialIndex()
	{
		_currentIndex = 0;
	}

	public virtual void SetSequentialIndex(int newIndex)
	{
		_currentIndex = newIndex;
	}
}
