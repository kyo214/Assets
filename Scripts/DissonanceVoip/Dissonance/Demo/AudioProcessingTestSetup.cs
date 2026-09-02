using System;
using Dissonance.Audio.Capture;
using Dissonance.Config;
using Dissonance.VAD;
using JetBrains.Annotations;
using NAudio.Wave;
using UnityEngine;
using UnityEngine.UI;

namespace Dissonance.Demo;

public class AudioProcessingTestSetup : MonoBehaviour, IVoiceActivationListener
{
	public Slider InputVolumeSlider;

	public Slider OutputVolumeSlider;

	public Slider OutputCutoffSlider;

	public Button PlayPauseButton;

	public Dropdown ClipsDropdown;

	public Dropdown NoiseSuppressionDropdown;

	public Dropdown VadSensitivityDropdown;

	public Text VoiceIndicator;

	public Toggle BackgroundSoundRemoval;

	public Slider BackgroundSoundRemovalSlider;

	public AudioClip[] Clips;

	private WebRtcPreprocessingPipeline _preprocessor;

	private bool _enabled;

	private bool _reading;

	private float _pendingSamples;

	private int _readHead;

	private AudioClip _clip;

	private readonly float[] _buffer = new float[128];

	private bool _vad;

	private void OnEnable()
	{
		VoiceSettings.Preload();
		DebugSettings.Preload();
		ClipsDropdown.options.Clear();
		for (int i = 0; i < Clips.Length; i++)
		{
			ClipsDropdown.options.Add(new Dropdown.OptionData(Clips[i].name));
		}
		NoiseSuppressionDropdown.options.Clear();
		string[] names = Enum.GetNames(typeof(NoiseSuppressionLevels));
		foreach (string text in names)
		{
			NoiseSuppressionDropdown.options.Add(new Dropdown.OptionData(text));
		}
		NoiseSuppressionDropdown.value = (int)VoiceSettings.Instance.DenoiseAmount;
		VadSensitivityDropdown.options.Clear();
		names = Enum.GetNames(typeof(VadSensitivityLevels));
		foreach (string text2 in names)
		{
			VadSensitivityDropdown.options.Add(new Dropdown.OptionData(text2));
		}
		NoiseSuppressionDropdown.value = (int)VoiceSettings.Instance.VadSensitivity;
		BackgroundSoundRemoval.isOn = VoiceSettings.Instance.BackgroundSoundRemovalEnabled;
		BackgroundSoundRemovalSlider.value = VoiceSettings.Instance.BackgroundSoundRemovalAmount;
		OutputCutoffSlider.value = GetComponent<AudioLowPassFilter>().cutoffFrequency / 20000f;
		_enabled = true;
		OnAudioSelectionChanged();
		OnPlayPauseClicked();
	}

	private void Update()
	{
		VoiceIndicator.text = (_vad ? "True" : "False");
		if (!_reading || !(_clip != null))
		{
			return;
		}
		_pendingSamples += (int)((float)_clip.frequency * Time.unscaledDeltaTime);
		while (_pendingSamples >= (float)_buffer.Length)
		{
			_pendingSamples -= _buffer.Length;
			_clip.GetData(_buffer, _readHead);
			_readHead = (_readHead + _buffer.Length) % _clip.samples;
			for (int i = 0; i < _buffer.Length; i++)
			{
				_buffer[i] *= InputVolumeSlider.value;
			}
			ProcessSamples(_buffer);
		}
	}

	private void ProcessSamples([NotNull] float[] floats)
	{
		((IMicrophoneSubscriber)_preprocessor).ReceiveMicrophoneData(new ArraySegment<float>(floats), new WaveFormat(_clip.frequency, 1));
	}

	public void OnAudioSelectionChanged()
	{
		int value = ClipsDropdown.value;
		AudioClip audioClip = Clips[value];
		Debug.Log("Changed clip to: " + audioClip.name);
		ChangeAudioClip(audioClip);
	}

	public void OnVolumeChanged(float _)
	{
		GetComponent<AudioSource>().volume = OutputVolumeSlider.value;
	}

	public void OnLowPassCutoffChanged(float _)
	{
		GetComponent<AudioLowPassFilter>().cutoffFrequency = OutputCutoffSlider.value * 20000f;
	}

	public void OnPlayPauseClicked()
	{
		_reading = !_reading;
		PlayPauseButton.GetComponentInChildren<Text>().text = (_reading ? "Pause" : "Play");
	}

	public void OnNoiseSuppressionChanged()
	{
		if (_enabled)
		{
			int value = NoiseSuppressionDropdown.value;
			VoiceSettings.Instance.DenoiseAmount = (NoiseSuppressionLevels)Enum.Parse(typeof(NoiseSuppressionLevels), NoiseSuppressionDropdown.options[value].text);
		}
	}

	public void OnVadSensitivityChanged()
	{
		if (_enabled)
		{
			int value = VadSensitivityDropdown.value;
			VoiceSettings.Instance.VadSensitivity = (VadSensitivityLevels)Enum.Parse(typeof(VadSensitivityLevels), VadSensitivityDropdown.options[value].text);
		}
	}

	public void OnBackgroundSoundRemovalChanged()
	{
		if (_enabled)
		{
			VoiceSettings.Instance.BackgroundSoundRemovalEnabled = BackgroundSoundRemoval.isOn;
			VoiceSettings.Instance.BackgroundSoundRemovalAmount = BackgroundSoundRemovalSlider.value;
		}
	}

	private void ChangeAudioClip([NotNull] AudioClip clip)
	{
		if (clip.channels != 1)
		{
			Debug.LogError("Audio clip must be mono!");
			return;
		}
		_clip = clip;
		WaveFormat waveFormat = new WaveFormat(clip.frequency, 1);
		GetComponent<MicSubscriberPlayer>().SetFormat(waveFormat);
		if (_preprocessor != null)
		{
			_preprocessor.Dispose();
			_preprocessor = null;
		}
		_preprocessor = new WebRtcPreprocessingPipeline(waveFormat, mobilePlatform: false);
		_preprocessor.Subscribe(GetComponent<MicSubscriberPlayer>());
		_preprocessor.Start();
		VoiceActivationStop();
		_preprocessor.Subscribe(this);
		_pendingSamples = -clip.frequency * 5;
		_readHead = 0;
		_clip = clip;
	}

	public void OnDestroy()
	{
		_preprocessor.Dispose();
	}

	public void VoiceActivationStart()
	{
		_vad = true;
	}

	public void VoiceActivationStop()
	{
		_vad = false;
	}

	public int GetGains(float[] output)
	{
		if (_preprocessor != null)
		{
			return _preprocessor.GetBackgroundNoiseRemovalGains(output);
		}
		return 0;
	}
}
