using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Tools;

[Serializable]
public struct MMSoundManagerPlayOptions
{
	public MMSoundManager.MMSoundManagerTracks MmSoundManagerTrack;

	public Vector3 Location;

	public bool Loop;

	public float Volume;

	public int ID;

	public bool Fade;

	public float FadeInitialVolume;

	public float FadeDuration;

	public MMTweenType FadeTween;

	public bool Persistent;

	public AudioSource RecycleAudioSource;

	public AudioMixerGroup AudioGroup;

	public float Pitch;

	public float PlaybackTime;

	public float PanStereo;

	public float SpatialBlend;

	public bool SoloSingleTrack;

	public bool SoloAllTracks;

	public bool AutoUnSoloOnEnd;

	public bool BypassEffects;

	public bool BypassListenerEffects;

	public bool BypassReverbZones;

	public int Priority;

	public float ReverbZoneMix;

	public float DopplerLevel;

	public int Spread;

	public AudioRolloffMode RolloffMode;

	public float MinDistance;

	public float MaxDistance;

	public bool DoNotAutoRecycleIfNotDonePlaying;

	public static MMSoundManagerPlayOptions Default => new MMSoundManagerPlayOptions
	{
		MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx,
		Location = Vector3.zero,
		Loop = false,
		Volume = 1f,
		ID = 0,
		Fade = false,
		FadeInitialVolume = 0f,
		FadeDuration = 1f,
		FadeTween = null,
		Persistent = false,
		RecycleAudioSource = null,
		AudioGroup = null,
		Pitch = 1f,
		PanStereo = 0f,
		SpatialBlend = 0f,
		SoloSingleTrack = false,
		SoloAllTracks = false,
		AutoUnSoloOnEnd = false,
		BypassEffects = false,
		BypassListenerEffects = false,
		BypassReverbZones = false,
		Priority = 128,
		ReverbZoneMix = 1f,
		DopplerLevel = 1f,
		Spread = 0,
		RolloffMode = AudioRolloffMode.Logarithmic,
		MinDistance = 1f,
		MaxDistance = 500f,
		DoNotAutoRecycleIfNotDonePlaying = false
	};
}
