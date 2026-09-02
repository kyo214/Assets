using System;
using Dissonance.Audio.Capture;
using Dissonance.Datastructures;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance;

public abstract class BaseMicrophoneSubscriber : MonoBehaviour, IMicrophoneSubscriber
{
	private WaveFormat _format;

	private readonly TransferBuffer<float> _transfer = new TransferBuffer<float>();

	private bool _resetPending;

	private int _lostSamples;

	private readonly float[] _temporary = new float[800];

	void IMicrophoneSubscriber.ReceiveMicrophoneData(ArraySegment<float> buffer, WaveFormat format)
	{
		if (_format == null)
		{
			_format = format;
			_resetPending = true;
		}
		if (!_format.Equals(format))
		{
			_format = format;
			_resetPending = true;
			_transfer.Clear();
			_lostSamples = 0;
		}
		else
		{
			int num = _transfer.WriteSome(buffer);
			_lostSamples += buffer.Count - num;
		}
	}

	void IMicrophoneSubscriber.Reset()
	{
		_transfer.Clear();
		_resetPending = true;
	}

	public virtual void Update()
	{
		if (_resetPending)
		{
			if (_format == null)
			{
				return;
			}
			_resetPending = false;
			ResetAudioStream(_format);
		}
		bool flag = true;
		while (flag)
		{
			Array.Clear(_temporary, 0, _temporary.Length);
			int num = Math.Min(_temporary.Length / 2, _lostSamples);
			ArraySegment<float> data = new ArraySegment<float>(_temporary, 0, _temporary.Length - num);
			if (_transfer.Read(data))
			{
				_lostSamples -= num;
				ProcessAudio(new ArraySegment<float>(_temporary));
			}
			else
			{
				flag = false;
			}
		}
	}

	protected abstract void ProcessAudio(ArraySegment<float> data);

	protected abstract void ResetAudioStream(WaveFormat waveFormat);
}
