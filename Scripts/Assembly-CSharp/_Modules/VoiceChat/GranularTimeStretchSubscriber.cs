using System;
using Dissonance.Audio.Playback;
using UnityEngine;

namespace _Modules.VoiceChat;

public class GranularTimeStretchSubscriber : MonoBehaviour, IAudioOutputSubscriber
{
	public bool active;

	[Range(0.5f, 1f)]
	public float speed = 0.7f;

	private const int GrainSize = 512;

	private const int Overlap = 256;

	private float[] _grain = new float[512];

	private float[] _window = new float[512];

	private int _writePos;

	private float _readPos;

	private void Awake()
	{
		for (int i = 0; i < 512; i++)
		{
			_window[i] = 0.5f * (1f - Mathf.Cos(MathF.PI * 2f * (float)i / 511f));
		}
	}

	public void OnAudioPlayback(ArraySegment<float> samples, bool isComplete)
	{
		if (!active || speed >= 0.99f)
		{
			return;
		}
		float[] array = samples.Array;
		int offset = samples.Offset;
		int count = samples.Count;
		for (int i = 0; i < count; i++)
		{
			_grain[_writePos] = array[offset + i] * _window[_writePos];
			_writePos++;
			if (_writePos >= 512)
			{
				_writePos = 256;
			}
			int num = Mathf.FloorToInt(_readPos);
			int num2 = (num + 1) % 512;
			float t = _readPos - (float)num;
			float num3 = Mathf.Lerp(_grain[num], _grain[num2], t);
			array[offset + i] = num3;
			_readPos += speed;
			if (_readPos >= 512f)
			{
				_readPos -= 256f;
			}
		}
		if (isComplete)
		{
			_writePos = 0;
			_readPos = 0f;
		}
	}

	public void SetActive(bool active)
	{
		this.active = active;
	}
}
