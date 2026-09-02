using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/AudioSource")]
[FeedbackHelp("This feedback lets you play a target audio source, with some elements at random.")]
public class MMF_AudioSource : MMF_Feedback
{
	public enum Modes
	{
		Play = 0,
		Pause = 1,
		UnPause = 2,
		Stop = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Audiosource", true, 28, true, false)]
	[Tooltip("the target audio source to play")]
	public AudioSource TargetAudioSource;

	[Tooltip("whether we should play the audio source or stop it or pause it")]
	public Modes Mode;

	[Header("Random Sound")]
	[Tooltip("an array to pick a random sfx from")]
	public AudioClip[] RandomSfx;

	[MMFInspectorGroup("Audio Settings", true, 29, false, false)]
	[Header("Volume")]
	[Tooltip("the minimum volume to play the sound at")]
	public float MinVolume = 1f;

	[Tooltip("the maximum volume to play the sound at")]
	public float MaxVolume = 1f;

	[Header("Pitch")]
	[Tooltip("the minimum pitch to play the sound at")]
	public float MinPitch = 1f;

	[Tooltip("the maximum pitch to play the sound at")]
	public float MaxPitch = 1f;

	[Header("Mixer")]
	[Tooltip("the audiomixer to play the sound with (optional)")]
	public AudioMixerGroup SfxAudioMixerGroup;

	protected AudioClip _randomClip;

	protected float _duration;

	public override float FeedbackDuration
	{
		get
		{
			return _duration;
		}
		set
		{
			_duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		switch (Mode)
		{
		case Modes.Play:
		{
			if (RandomSfx.Length != 0)
			{
				_randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
				TargetAudioSource.clip = _randomClip;
			}
			float volume = Random.Range(MinVolume, MaxVolume) * num;
			float pitch = Random.Range(MinPitch, MaxPitch);
			_duration = TargetAudioSource.clip.length;
			PlayAudioSource(TargetAudioSource, volume, pitch);
			break;
		}
		case Modes.Pause:
			_duration = 0.1f;
			TargetAudioSource.Pause();
			break;
		case Modes.UnPause:
			_duration = 0.1f;
			TargetAudioSource.UnPause();
			break;
		case Modes.Stop:
			_duration = 0.1f;
			TargetAudioSource.Stop();
			break;
		}
	}

	protected virtual void PlayAudioSource(AudioSource audioSource, float volume, float pitch)
	{
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.timeSamples = 0;
		if (!NormalPlayDirection)
		{
			audioSource.pitch = -1f;
			audioSource.timeSamples = audioSource.clip.samples - 1;
		}
		audioSource.Play();
	}

	public override void Stop(Vector3 position, float feedbacksIntensity = 1f)
	{
		base.Stop(position, feedbacksIntensity);
		if (TargetAudioSource != null)
		{
			TargetAudioSource?.Stop();
		}
	}
}
