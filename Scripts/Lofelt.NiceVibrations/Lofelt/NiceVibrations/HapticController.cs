using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace Lofelt.NiceVibrations;

public static class HapticController
{
	private static bool lofeltHapticsInitalized = false;

	private static System.Timers.Timer playbackFinishedTimer = new System.Timers.Timer();

	private static float clipLoadedDurationSecs = 0f;

	private static bool clipLoaded = false;

	private static float lastSeekTime = 0f;

	private static bool deviceMeetsAdvancedRequirements = false;

	private static bool isLoopingEnabledByUser = false;

	private static bool isPlaybackLooping = false;

	private static HapticPatterns.PresetType _fallbackPreset = HapticPatterns.PresetType.None;

	internal static bool _hapticsEnabled = true;

	internal static float _outputLevel = 1f;

	internal static float _clipLevel = 1f;

	public static Action LoadedClipChanged;

	public static Action PlaybackStarted;

	public static Action PlaybackStopped;

	public static HapticPatterns.PresetType fallbackPreset
	{
		get
		{
			return _fallbackPreset;
		}
		set
		{
			_fallbackPreset = value;
		}
	}

	public static bool hapticsEnabled
	{
		get
		{
			return _hapticsEnabled;
		}
		set
		{
			if (_hapticsEnabled)
			{
				Stop();
			}
			_hapticsEnabled = value;
		}
	}

	[DefaultValue(1f)]
	public static float outputLevel
	{
		get
		{
			return _outputLevel;
		}
		set
		{
			_outputLevel = value;
			if (Init())
			{
				LofeltHaptics.SetAmplitudeMultiplication(_outputLevel * _clipLevel);
			}
			GamepadRumbler.lowFrequencyMotorSpeedMultiplication = _outputLevel * _clipLevel;
			GamepadRumbler.highFrequencyMotorSpeedMultiplication = _outputLevel * _clipLevel;
		}
	}

	[DefaultValue(1f)]
	public static float clipLevel
	{
		get
		{
			return _clipLevel;
		}
		set
		{
			_clipLevel = value;
			if (Init())
			{
				LofeltHaptics.SetAmplitudeMultiplication(_outputLevel * _clipLevel);
			}
			GamepadRumbler.lowFrequencyMotorSpeedMultiplication = _outputLevel * _clipLevel;
			GamepadRumbler.highFrequencyMotorSpeedMultiplication = _outputLevel * _clipLevel;
		}
	}

	[DefaultValue(0f)]
	public static float clipFrequencyShift
	{
		set
		{
			if (Init())
			{
				LofeltHaptics.SetFrequencyShift(value);
			}
		}
	}

	public static bool Init()
	{
		if (!lofeltHapticsInitalized)
		{
			lofeltHapticsInitalized = true;
			SynchronizationContext syncContext = SynchronizationContext.Current;
			playbackFinishedTimer.Elapsed += (object obj, ElapsedEventArgs args) =>
			{
				syncContext.Post((object _) =>
				{
					HandleFinishedPlayback();
				}, null);
			};
			if (DeviceCapabilities.isVersionSupported)
			{
				LofeltHaptics.Initialize();
				DeviceCapabilities.Init();
				deviceMeetsAdvancedRequirements = DeviceCapabilities.meetsAdvancedRequirements;
			}
			GamepadRumbler.Init();
		}
		return deviceMeetsAdvancedRequirements;
	}

	public static void Load(byte[] data)
	{
		GamepadRumbler.Unload();
		lastSeekTime = 0f;
		clipLoaded = true;
		clipLoadedDurationSecs = 0f;
		if (Init())
		{
			LofeltHaptics.Load(data);
		}
		clipLevel = 1f;
		LoadedClipChanged?.Invoke();
	}

	public static void Load(HapticClip clip)
	{
		Load(clip.json, clip.gamepadRumble);
	}

	public static void Load(byte[] json, GamepadRumble rumble)
	{
		Load(json);
		GamepadRumbler.Load(rumble);
		if (clipLoadedDurationSecs == 0f && rumble.IsValid())
		{
			clipLoadedDurationSecs = (float)rumble.totalDurationMs / 1000f;
		}
	}

	private static void HandleFinishedPlayback()
	{
		lastSeekTime = 0f;
		isPlaybackLooping = false;
		playbackFinishedTimer.Enabled = false;
		PlaybackStopped?.Invoke();
	}

	public static void Play()
	{
		if (_hapticsEnabled)
		{
			float num = 0f;
			bool flag = false;
			if (GamepadRumbler.CanPlay())
			{
				num = clipLoadedDurationSecs;
				GamepadRumbler.Play();
			}
			else if (Init())
			{
				num = Mathf.Max(clipLoadedDurationSecs - lastSeekTime, 0f);
				flag = DeviceCapabilities.canLoop;
				LofeltHaptics.Play();
			}
			else if (DeviceCapabilities.isVersionSupported)
			{
				num = HapticPatterns.GetPresetDuration(fallbackPreset);
				HapticPatterns.PlayPreset(fallbackPreset);
			}
			isPlaybackLooping = isLoopingEnabledByUser & flag;
			PlaybackStarted?.Invoke();
			if (num > 0f)
			{
				playbackFinishedTimer.Interval = num * 1000f;
				playbackFinishedTimer.AutoReset = false;
				playbackFinishedTimer.Enabled = !isPlaybackLooping;
			}
			else
			{
				HandleFinishedPlayback();
			}
		}
	}

	public static void Play(HapticClip clip)
	{
		Load(clip);
		Play();
	}

	public static void Stop()
	{
		if (Init())
		{
			LofeltHaptics.Stop();
		}
		else
		{
			LofeltHaptics.StopPattern();
		}
		GamepadRumbler.Stop();
		HandleFinishedPlayback();
	}

	public static void Seek(float time)
	{
		if (Init())
		{
			LofeltHaptics.Stop();
			LofeltHaptics.Seek(time);
		}
		GamepadRumbler.Stop();
		lastSeekTime = time;
	}

	public static void Loop(bool enabled)
	{
		if (Init())
		{
			LofeltHaptics.Loop(enabled);
		}
		isLoopingEnabledByUser = enabled;
	}

	public static bool IsPlaying()
	{
		if (playbackFinishedTimer.Enabled)
		{
			return true;
		}
		return isPlaybackLooping;
	}

	public static void Reset()
	{
		if (clipLoaded)
		{
			Seek(0f);
			Stop();
			clipLevel = 1f;
			clipFrequencyShift = 0f;
			Loop(enabled: false);
		}
		fallbackPreset = HapticPatterns.PresetType.None;
	}

	public static void ProcessApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			Stop();
		}
	}
}
