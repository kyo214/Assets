using System;
using Chronos;
using Dissonance.Audio.Playback;
using UnityEngine;

namespace _Modules.VoiceChat;

public class SlowMotionEffectSubscriber : MonoBehaviour, IAudioOutputSubscriber
{
	[Header("Master")]
	[SerializeField]
	private bool _active;

	[Header("Pitch Shifter")]
	[Range(0.5f, 1.5f)]
	[Tooltip("Recommendation value 0.75 - 0.85")]
	[SerializeField]
	private float _basePitch = 0.8f;

	[Range(0.0001f, 0.05f)]
	[SerializeField]
	private float _pitchSampleSmooth = 0.005f;

	[Header("Low Pass Filter")]
	[SerializeField]
	private bool _lowPassEnabled = true;

	[Range(300f, 4000f)]
	[Tooltip("Recommendation value 800 - 1200 Hz")]
	[SerializeField]
	private float _cutoffHz = 1000f;

	[Range(0.5f, 2f)]
	[SerializeField]
	private float _q = 0.707f;

	[Range(0.0001f, 0.05f)]
	[SerializeField]
	private float _lpfSampleSmooth = 0.005f;

	[Header("Echo")]
	[SerializeField]
	private bool _echoEnabled = true;

	[Range(20f, 150f)]
	[Tooltip("Recommendation value 60 – 100 ms")]
	[SerializeField]
	private float _echoDelayMs = 80f;

	[Range(0f, 0.5f)]
	[Tooltip("Recommendation value 0.15 – 0.25")]
	[SerializeField]
	private float _echoDecay = 0.2f;

	[Range(0.0001f, 0.05f)]
	[SerializeField]
	private float _echoSampleSmooth = 0.005f;

	[Header("Time Sync")]
	[SerializeField]
	private bool _timeScaleSyncEnabled;

	[SerializeField]
	private bool _useCustomTimeScale;

	[SerializeField]
	private TimelineChild _timelineChild;

	[Range(0.1f, 1f)]
	[SerializeField]
	private float _minTimeScale = 0.25f;

	private const int SAMPLE_RATE = 48000;

	private float _readIndex;

	private float[] _copyBuffer;

	private float[] _prevBuffer;

	private float _targetPitch;

	private float _currentPitch;

	private float _targetCutoff;

	private float _currentCutoff;

	private float _targetEchoDecay;

	private float _currentEchoDecay;

	private bool _pitchInitialized;

	private float a0;

	private float a1;

	private float a2;

	private float b1;

	private float b2;

	private float z1;

	private float z2;

	private float[] _echoBuffer;

	private int _echoIndex;

	private void Awake()
	{
		InitState();
		RebuildLPF();
		InitEcho();
	}

	private void OnValidate()
	{
		InitState();
		RebuildLPF();
		InitEcho();
	}

	private void InitState()
	{
		_targetPitch = _basePitch;
		_currentPitch = _basePitch;
		_targetCutoff = _cutoffHz;
		_currentCutoff = _cutoffHz;
		_targetEchoDecay = _echoDecay;
		_currentEchoDecay = _echoDecay;
		_pitchInitialized = false;
	}

	private void Update()
	{
		if (!_active)
		{
			return;
		}
		if (!_timeScaleSyncEnabled)
		{
			if (!_pitchInitialized)
			{
				_currentPitch = _basePitch;
				_currentCutoff = _cutoffHz;
				_currentEchoDecay = _echoDecay;
				_pitchInitialized = true;
			}
		}
		else
		{
			float num = Mathf.Max(_minTimeScale, GetTimeScale());
			_targetPitch = Mathf.Lerp(1f, _basePitch, 1f - num);
			_targetCutoff = _cutoffHz * num;
			_targetEchoDecay = _echoDecay * num;
		}
		float GetTimeScale()
		{
			if (_useCustomTimeScale && (bool)_timelineChild.parent)
			{
				return _timelineChild.parent.timeScale;
			}
			return Time.timeScale;
		}
	}

	public void SetActive(bool value)
	{
		_active = value;
		if (_active)
		{
			_pitchInitialized = false;
		}
	}

	public void OnAudioPlayback(ArraySegment<float> samples, bool isComplete)
	{
		if (!_active)
		{
			return;
		}
		int count = samples.Count;
		float[] array = samples.Array;
		if (array == null || count <= 0)
		{
			return;
		}
		EnsureBuffers(count);
		Array.Copy(array, samples.Offset, _copyBuffer, 0, count);
		if (_lowPassEnabled)
		{
			_currentCutoff = Mathf.Lerp(_currentCutoff, _targetCutoff, _lpfSampleSmooth);
		}
		if (_lowPassEnabled)
		{
			RebuildLPF(_currentCutoff);
		}
		_currentEchoDecay = Mathf.Lerp(_currentEchoDecay, _targetEchoDecay, _echoSampleSmooth);
		float num = _readIndex;
		for (int i = 0; i < count; i++)
		{
			_currentPitch = Mathf.Clamp(Mathf.Lerp(_currentPitch, _targetPitch, _pitchSampleSmooth), 0.1f, 2f);
			float num2 = 1f / _currentPitch;
			float num3 = Cubic(_copyBuffer, num);
			if (_lowPassEnabled)
			{
				num3 = ApplyLowPass(num3);
			}
			float num4 = (_echoEnabled ? ApplyEcho(num3) : 0f);
			float num5 = ((_prevBuffer != null && _prevBuffer.Length > i) ? _prevBuffer[i] : 0f);
			array[samples.Offset + i] = (num5 + num3 + num4) * 0.5f;
			num += num2;
			if (num >= (float)_copyBuffer.Length)
			{
				num -= (float)_copyBuffer.Length;
			}
		}
		_readIndex = num;
		Array.Copy(_copyBuffer, _prevBuffer, count);
		if (isComplete)
		{
			_readIndex = 0f;
		}
	}

	private float ApplyLowPass(float input)
	{
		float num = a0 * input + z1;
		z1 = a1 * input - b1 * num + z2;
		z2 = a2 * input - b2 * num;
		return num;
	}

	private float ApplyEcho(float input)
	{
		float num = _echoBuffer[_echoIndex];
		float value = input + num * _currentEchoDecay;
		_echoBuffer[_echoIndex] = Mathf.Clamp(value, -1f, 1f);
		_echoIndex++;
		if (_echoIndex >= _echoBuffer.Length)
		{
			_echoIndex = 0;
		}
		return num;
	}

	private void RebuildLPF(float cutoff = -1f)
	{
		if (_lowPassEnabled)
		{
			float num = ((cutoff > 0f) ? cutoff : _cutoffHz);
			float f = MathF.PI * 2f * num / 48000f;
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f) / (2f * _q);
			float num4 = (1f - num2) * 0.5f;
			float num5 = 1f - num2;
			float num6 = (1f - num2) * 0.5f;
			float num7 = 1f + num3;
			float num8 = -2f * num2;
			float num9 = 1f - num3;
			a0 = num4 / num7;
			a1 = num5 / num7;
			a2 = num6 / num7;
			b1 = num8 / num7;
			b2 = num9 / num7;
			z1 = 0f;
			z2 = 0f;
		}
	}

	private void InitEcho()
	{
		if (_echoEnabled)
		{
			int num = Mathf.Max(1, Mathf.CeilToInt(48000f * (_echoDelayMs / 1000f)));
			if (_echoBuffer == null || _echoBuffer.Length != num)
			{
				_echoBuffer = new float[num];
			}
			_echoIndex = 0;
		}
	}

	private void EnsureBuffers(int length)
	{
		if (_copyBuffer == null || _copyBuffer.Length != length)
		{
			_copyBuffer = new float[length];
		}
		if (_prevBuffer == null || _prevBuffer.Length != length)
		{
			_prevBuffer = new float[length];
		}
	}

	private static float Cubic(float[] d, float p)
	{
		int num = d.Length;
		int num2 = Mathf.FloorToInt(p);
		float num3 = p - (float)num2;
		int num4 = Mathf.Clamp(num2 - 1, 0, num - 1);
		int num5 = Mathf.Clamp(num2, 0, num - 1);
		int num6 = Mathf.Clamp(num2 + 1, 0, num - 1);
		int num7 = Mathf.Clamp(num2 + 2, 0, num - 1);
		float num8 = d[num4];
		float num9 = d[num5];
		float num10 = d[num6];
		float num11 = d[num7];
		float num12 = -0.5f * num8 + 1.5f * num9 - 1.5f * num10 + 0.5f * num11;
		float num13 = num8 - 2.5f * num9 + 2f * num10 - 0.5f * num11;
		float num14 = -0.5f * num8 + 0.5f * num10;
		return ((num12 * num3 + num13) * num3 + num14) * num3 + num9;
	}

	public void SetPresetDefault()
	{
		_basePitch = 0.8f;
		_lowPassEnabled = true;
		_cutoffHz = 1000f;
		_q = 0.707f;
		_lpfSampleSmooth = 0.005f;
		_echoEnabled = true;
		_echoDelayMs = 80f;
		_echoDecay = 0.2f;
		_echoSampleSmooth = 0.005f;
		_timeScaleSyncEnabled = false;
		_minTimeScale = 0.25f;
		InitState();
		RebuildLPF();
		InitEcho();
	}

	public void SetPresetSlowMoExtreme()
	{
		_basePitch = 0.6f;
		_lowPassEnabled = true;
		_cutoffHz = 500f;
		_q = 0.707f;
		_lpfSampleSmooth = 0.01f;
		_echoEnabled = true;
		_echoDelayMs = 120f;
		_echoDecay = 0.25f;
		_echoSampleSmooth = 0.01f;
		_timeScaleSyncEnabled = false;
		_minTimeScale = 0.25f;
		InitState();
		RebuildLPF();
		InitEcho();
	}

	public void SetPresetMildSlowMo()
	{
		_basePitch = 0.8f;
		_lowPassEnabled = true;
		_cutoffHz = 1200f;
		_q = 0.707f;
		_lpfSampleSmooth = 0.005f;
		_echoEnabled = true;
		_echoDelayMs = 60f;
		_echoDecay = 0.15f;
		_echoSampleSmooth = 0.005f;
		_timeScaleSyncEnabled = false;
		InitState();
		RebuildLPF();
		InitEcho();
	}

	public void SetPresetExtremeEcho()
	{
		_basePitch = 1f;
		_lowPassEnabled = true;
		_cutoffHz = 2500f;
		_q = 0.707f;
		_lpfSampleSmooth = 0.01f;
		_echoEnabled = true;
		_echoDelayMs = 150f;
		_echoDecay = 0.35f;
		_echoSampleSmooth = 0.01f;
		_timeScaleSyncEnabled = false;
		InitState();
		RebuildLPF();
		InitEcho();
	}

	public void SetPresetUltraFast()
	{
		_basePitch = 1.3f;
		_lowPassEnabled = false;
		_cutoffHz = 4000f;
		_q = 0.707f;
		_lpfSampleSmooth = 0.005f;
		_echoEnabled = false;
		_timeScaleSyncEnabled = false;
		InitState();
		RebuildLPF();
		InitEcho();
	}
}
