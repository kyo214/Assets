using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[Serializable]
public class Beat
{
	public enum Modes
	{
		Raw = 0,
		Normalized = 1,
		BufferedRaw = 2,
		BufferedNormalized = 3,
		Amplitude = 4,
		NormalizedAmplitude = 5,
		AmplitudeBuffered = 6,
		NormalizedAmplitudeBuffered = 7
	}

	public enum BeatValueModes
	{
		Remapped = 0,
		Live = 1
	}

	public string Name = "Beat";

	public Modes Mode = Modes.BufferedNormalized;

	public BeatValueModes BeatValueMode;

	[MMEnumCondition("Mode", new int[] { 0, 1, 2, 3 })]
	public Color BeatColor = Color.cyan;

	public int BandID;

	public float Threshold = 0.5f;

	public float MinimumTimeBetweenBeats = 0.25f;

	[MMEnumCondition("BeatValueMode", new int[] { 0 })]
	public float RemappedAttack = 0.05f;

	[MMEnumCondition("BeatValueMode", new int[] { 0 })]
	public float RemappedDecay = 0.2f;

	[MMReadOnly]
	public bool BeatThisFrame;

	[MMReadOnly]
	public float CurrentValue;

	[HideInInspector]
	public float _previousValue;

	[HideInInspector]
	public float _lastBeatAt;

	[HideInInspector]
	public float _lastBeatValue;

	[HideInInspector]
	public bool _initialized;

	public UnityEvent OnBeat;

	public void InitializeIfNeeded(int id, int bandID)
	{
		if (!_initialized)
		{
			Mode = Modes.Normalized;
			BeatValueMode = BeatValueModes.Remapped;
			Name = "Beat " + id;
			BeatColor = MMColors.RandomColor();
			BandID = bandID;
			Threshold = 0.3f + (float)id * 0.02f;
			if (Threshold > 0.6f)
			{
				Threshold -= 0.5f;
			}
			Threshold %= 1f;
			MinimumTimeBetweenBeats = 0.25f + (float)id * 0.02f;
			RemappedAttack = 0.05f + (float)id * 0.01f;
			RemappedDecay = 0.2f + (float)id * 0.01f;
			_initialized = true;
		}
	}
}
