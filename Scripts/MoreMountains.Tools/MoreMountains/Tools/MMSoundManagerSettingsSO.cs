using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Tools;

[Serializable]
[CreateAssetMenu(menuName = "MoreMountains/Audio/MMSoundManagerSettings")]
public class MMSoundManagerSettingsSO : ScriptableObject
{
	[Header("Audio Mixer")]
	[Tooltip("the audio mixer to use when playing sounds")]
	public AudioMixer TargetAudioMixer;

	[Tooltip("the master group")]
	public AudioMixerGroup MasterAudioMixerGroup;

	[Tooltip("the group on which to play all music sounds")]
	public AudioMixerGroup MusicAudioMixerGroup;

	[Tooltip("the group on which to play all sound effects")]
	public AudioMixerGroup SfxAudioMixerGroup;

	[Tooltip("the group on which to play all UI sounds")]
	public AudioMixerGroup UIAudioMixerGroup;

	[Tooltip("the multiplier to apply when converting normalized volume values to audio mixer values")]
	public float MixerValuesMultiplier = 20f;

	[Header("Settings Unfold")]
	[Tooltip("the full settings for this MMSoundManager")]
	public MMSoundManagerSettings Settings;

	protected const string _saveFolderName = "MMSoundManager/";

	protected const string _saveFileName = "mmsound.settings";

	public virtual void SaveSoundSettings()
	{
		MMSaveLoadManager.Save(Settings, "mmsound.settings", "MMSoundManager/");
	}

	public virtual void LoadSoundSettings()
	{
		if (Settings.OverrideMixerSettings)
		{
			MMSoundManagerSettings mMSoundManagerSettings = (MMSoundManagerSettings)MMSaveLoadManager.Load(typeof(MMSoundManagerSettings), "mmsound.settings", "MMSoundManager/");
			if (mMSoundManagerSettings != null)
			{
				Settings = mMSoundManagerSettings;
				ApplyTrackVolumes();
			}
		}
	}

	public virtual void ResetSoundSettings()
	{
		MMSaveLoadManager.DeleteSave("mmsound.settings", "MMSoundManager/");
	}

	public virtual void SetTrackVolume(MMSoundManager.MMSoundManagerTracks track, float volume)
	{
		if (volume <= 0f)
		{
			volume = 0.0001f;
		}
		switch (track)
		{
		case MMSoundManager.MMSoundManagerTracks.Master:
			TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(volume));
			Settings.MasterVolume = volume;
			break;
		case MMSoundManager.MMSoundManagerTracks.Music:
			TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(volume));
			Settings.MusicVolume = volume;
			break;
		case MMSoundManager.MMSoundManagerTracks.Sfx:
			TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(volume));
			Settings.SfxVolume = volume;
			break;
		case MMSoundManager.MMSoundManagerTracks.UI:
			TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(volume));
			Settings.UIVolume = volume;
			break;
		}
		if (Settings.AutoSave)
		{
			SaveSoundSettings();
		}
	}

	public virtual float GetTrackVolume(MMSoundManager.MMSoundManagerTracks track)
	{
		float value = 1f;
		switch (track)
		{
		case MMSoundManager.MMSoundManagerTracks.Master:
			TargetAudioMixer.GetFloat(Settings.MasterVolumeParameter, out value);
			break;
		case MMSoundManager.MMSoundManagerTracks.Music:
			TargetAudioMixer.GetFloat(Settings.MusicVolumeParameter, out value);
			break;
		case MMSoundManager.MMSoundManagerTracks.Sfx:
			TargetAudioMixer.GetFloat(Settings.SfxVolumeParameter, out value);
			break;
		case MMSoundManager.MMSoundManagerTracks.UI:
			TargetAudioMixer.GetFloat(Settings.UIVolumeParameter, out value);
			break;
		}
		return MixerVolumeToNormalized(value);
	}

	public virtual void GetTrackVolumes()
	{
		Settings.MasterVolume = GetTrackVolume(MMSoundManager.MMSoundManagerTracks.Master);
		Settings.MusicVolume = GetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music);
		Settings.SfxVolume = GetTrackVolume(MMSoundManager.MMSoundManagerTracks.Sfx);
		Settings.UIVolume = GetTrackVolume(MMSoundManager.MMSoundManagerTracks.UI);
	}

	protected virtual void ApplyTrackVolumes()
	{
		if (Settings.OverrideMixerSettings)
		{
			TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(Settings.MasterVolume));
			TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(Settings.MusicVolume));
			TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(Settings.SfxVolume));
			TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(Settings.UIVolume));
			if (!Settings.MasterOn)
			{
				TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, -80f);
			}
			if (!Settings.MusicOn)
			{
				TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, -80f);
			}
			if (!Settings.SfxOn)
			{
				TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, -80f);
			}
			if (!Settings.UIOn)
			{
				TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, -80f);
			}
			if (Settings.AutoSave)
			{
				SaveSoundSettings();
			}
		}
	}

	public virtual float NormalizedToMixerVolume(float normalizedVolume)
	{
		return Mathf.Log10(normalizedVolume) * MixerValuesMultiplier;
	}

	public virtual float MixerVolumeToNormalized(float mixerVolume)
	{
		return (float)Math.Pow(10.0, mixerVolume / MixerValuesMultiplier);
	}
}
