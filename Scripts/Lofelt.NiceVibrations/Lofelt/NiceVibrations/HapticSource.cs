using System;
using System.ComponentModel;
using UnityEngine;

namespace Lofelt.NiceVibrations;

[AddComponentMenu("Nice Vibrations/Haptic Source")]
public class HapticSource : MonoBehaviour
{
	private const int DEFAULT_PRIORITY = 128;

	public HapticClip clip;

	public int priority = 128;

	private float seekTime;

	[SerializeField]
	private HapticPatterns.PresetType _fallbackPreset = HapticPatterns.PresetType.None;

	[SerializeField]
	private bool _loop;

	[SerializeField]
	private float _level = 1f;

	[SerializeField]
	private float _frequencyShift;

	private static HapticSource loadedHapticSource;

	private static HapticSource lastPlayedHapticSource;

	[DefaultValue(HapticPatterns.PresetType.None)]
	public HapticPatterns.PresetType fallbackPreset
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

	[DefaultValue(false)]
	public bool loop
	{
		get
		{
			return _loop;
		}
		set
		{
			_loop = value;
		}
	}

	[DefaultValue(1.0)]
	public float level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
			if (IsLoaded())
			{
				HapticController.clipLevel = _level;
			}
		}
	}

	[DefaultValue(0.0)]
	public float frequencyShift
	{
		get
		{
			return _frequencyShift;
		}
		set
		{
			_frequencyShift = value;
			if (IsLoaded())
			{
				HapticController.clipFrequencyShift = _frequencyShift;
			}
		}
	}

	static HapticSource()
	{
		HapticController.LoadedClipChanged = (Action)Delegate.Combine(HapticController.LoadedClipChanged, (Action)(() =>
		{
			loadedHapticSource = null;
		}));
		HapticController.PlaybackStarted = (Action)Delegate.Combine(HapticController.PlaybackStarted, (Action)(() =>
		{
			lastPlayedHapticSource = null;
		}));
	}

	public void Play()
	{
		if (CanPlay())
		{
			HapticController.Load(clip);
			loadedHapticSource = this;
			HapticController.Loop(loop);
			HapticController.clipLevel = level;
			HapticController.clipFrequencyShift = frequencyShift;
			if (seekTime != 0f && !loop)
			{
				HapticController.Seek(seekTime);
			}
			HapticController.fallbackPreset = fallbackPreset;
			HapticController.Play();
			lastPlayedHapticSource = this;
		}
	}

	private bool CanPlay()
	{
		if (HapticController.IsPlaying())
		{
			if (lastPlayedHapticSource != null)
			{
				return priority <= lastPlayedHapticSource.priority;
			}
			return false;
		}
		return true;
	}

	private bool IsLoaded()
	{
		return (object)this == loadedHapticSource;
	}

	public void Stop()
	{
		if (IsLoaded())
		{
			HapticController.Stop();
		}
	}

	public void Seek(float time)
	{
		seekTime = time;
	}

	public void OnDisable()
	{
		if (HapticController.IsPlaying() && IsLoaded())
		{
			Stop();
		}
	}
}
