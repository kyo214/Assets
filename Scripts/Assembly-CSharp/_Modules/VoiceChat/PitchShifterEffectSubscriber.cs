using System;
using Dissonance.Audio.Playback;
using UnityEngine;

namespace _Modules.VoiceChat;

public class PitchShifterEffectSubscriber : MonoBehaviour, IAudioOutputSubscriber
{
	[Header("Effect Settings")]
	[SerializeField]
	private bool _active;

	[Range(0.5f, 2f)]
	[SerializeField]
	private float _pitch = 0.5f;

	private float _readIndex;

	private float[] _prevBuffer;

	public void OnAudioPlayback(ArraySegment<float> samples, bool isComplete)
	{
		if (!_active || Mathf.Approximately(_pitch, 1f))
		{
			return;
		}
		int count = samples.Count;
		float[] array = samples.Array;
		if (array == null || count <= 0)
		{
			return;
		}
		float[] array2 = new float[count];
		Array.Copy(array, samples.Offset, array2, 0, count);
		if (_prevBuffer == null || _prevBuffer.Length != count)
		{
			_prevBuffer = new float[count];
		}
		float num = _readIndex;
		for (int i = 0; i < count; i++)
		{
			float num2 = ((!(num >= (float)(count - 3))) ? CubicInterpolation(array2, num) : array2[count - 1]);
			array[samples.Offset + i] = ((_prevBuffer != null) ? ((_prevBuffer[i] + num2) * 0.5f) : num2);
			num += 1f / _pitch;
			if (num >= (float)count)
			{
				break;
			}
		}
		_readIndex = num - (float)count;
		Array.Copy(array2, _prevBuffer, count);
		if (isComplete)
		{
			_readIndex = 0f;
		}
	}

	private float CubicInterpolation(float[] data, float pos)
	{
		int num = data.Length;
		int num2 = Mathf.FloorToInt(pos);
		float num3 = pos - (float)num2;
		int num4 = Mathf.Clamp(num2 - 1, 0, num - 1);
		int num5 = Mathf.Clamp(num2, 0, num - 1);
		int num6 = Mathf.Clamp(num2 + 1, 0, num - 1);
		int num7 = Mathf.Clamp(num2 + 2, 0, num - 1);
		float num8 = data[num4];
		float num9 = data[num5];
		float num10 = data[num6];
		float num11 = data[num7];
		float num12 = -0.5f * num8 + 1.5f * num9 - 1.5f * num10 + 0.5f * num11;
		float num13 = num8 - 2.5f * num9 + 2f * num10 - 0.5f * num11;
		float num14 = -0.5f * num8 + 0.5f * num10;
		float num15 = num9;
		return ((num12 * num3 + num13) * num3 + num14) * num3 + num15;
	}
}
