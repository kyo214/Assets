using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Audio/MMAudioAnalyzer")]
public class MMAudioAnalyzer : MonoBehaviour
{
	public enum Modes
	{
		Global = 0,
		AudioSource = 1,
		Microphone = 2
	}

	[Header("Source")]
	[MMInformation("This component lets you pick an audio source (either global : the whole scene's audio, a unique source, or the microphone), and will cut it into chunks that you can then use to emit beat events, that other objects can consume and act upon. The sample interval is the frequency at which sound will be analyzed, the amount of spectrum samples will determine the accuracy of the sampling, the window defines the method used to reduce leakage, and the number of bands will determine in how many bands you want to cut the sound. The more bands, the more levers you'll have to play with afterwards.In general, for all of these settings, higher values mean better quality and lower performance. The buffer speed determines how fast buffered band levels readjust.", MMInformationAttribute.InformationType.Info, false)]
	[MMReadOnlyWhenPlaying]
	public Modes Mode;

	[MMEnumCondition("Mode", new int[] { 1 })]
	[MMReadOnlyWhenPlaying]
	public AudioSource TargetAudioSource;

	[MMEnumCondition("Mode", new int[] { 2 })]
	public int MicrophoneID;

	[Header("Sampling")]
	[MMReadOnlyWhenPlaying]
	public float SampleInterval = 0.02f;

	[MMDropdown(new object[]
	{
		2, 4, 8, 16, 32, 64, 128, 256, 512, 1024,
		2048, 4096, 8192
	})]
	[MMReadOnlyWhenPlaying]
	public int SpectrumSamples = 1024;

	[MMReadOnlyWhenPlaying]
	public FFTWindow Window;

	[Range(1f, 64f)]
	[MMReadOnlyWhenPlaying]
	public int NumberOfBands = 8;

	public float BufferSpeed = 2f;

	[Header("Beat Events")]
	public Beat[] Beats;

	[HideInInspector]
	public float[] RawSpectrum;

	[HideInInspector]
	public float[] BandLevels;

	[HideInInspector]
	public float[] BufferedBandLevels;

	[HideInInspector]
	public float[] BandPeaks;

	[HideInInspector]
	public float[] LastPeaksAt;

	[HideInInspector]
	public float[] NormalizedBandLevels;

	[HideInInspector]
	public float[] NormalizedBufferedBandLevels;

	[HideInInspector]
	public float Amplitude;

	[HideInInspector]
	public float NormalizedAmplitude;

	[HideInInspector]
	public float BufferedAmplitude;

	[HideInInspector]
	public float NormalizedBufferedAmplitude;

	[HideInInspector]
	public bool Active;

	[HideInInspector]
	public bool PeaksPasted;

	protected const int _microphoneDuration = 5;

	protected string _microphone;

	protected float _microphoneStartedAt;

	protected const float _microphoneDelay = 0.03f;

	protected const float _microphoneFrequency = 24000f;

	protected WaitForSeconds _sampleIntervalWaitForSeconds;

	protected int _cachedNumberOfBands;

	public virtual void FindPeaks()
	{
		float num = 0f;
		while (num < TargetAudioSource.clip.length)
		{
			TargetAudioSource.time = num;
			TargetAudioSource.GetSpectrumData(RawSpectrum, 0, Window);
			num += SampleInterval;
			ComputeBandLevels();
			PeaksSaver.Peaks = BandPeaks;
		}
	}

	public virtual void PastePeaks()
	{
		BandPeaks = PeaksSaver.Peaks;
		PeaksSaver.Peaks = null;
		PeaksPasted = true;
	}

	public virtual void ClearPeaks()
	{
		BandPeaks = null;
		PeaksSaver.Peaks = null;
		PeaksPasted = false;
	}

	protected virtual void Awake()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		_cachedNumberOfBands = NumberOfBands;
		RawSpectrum = new float[SpectrumSamples];
		BandLevels = new float[_cachedNumberOfBands];
		BufferedBandLevels = new float[_cachedNumberOfBands];
		if (BandPeaks == null || BandPeaks.Length == 0)
		{
			BandPeaks = new float[_cachedNumberOfBands];
			PeaksPasted = false;
		}
		if (BandPeaks.Length != BandLevels.Length)
		{
			BandPeaks = new float[_cachedNumberOfBands];
			PeaksPasted = false;
		}
		LastPeaksAt = new float[_cachedNumberOfBands];
		NormalizedBandLevels = new float[_cachedNumberOfBands];
		NormalizedBufferedBandLevels = new float[_cachedNumberOfBands];
		if (Mode == Modes.AudioSource && TargetAudioSource == null)
		{
			Debug.LogError(base.name + " : this MMAudioAnalyzer needs a target audio source to operate.");
			return;
		}
		if (Mode == Modes.Microphone)
		{
			GameObject gameObject = new GameObject("Microphone");
			SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
			gameObject.transform.SetParent(base.gameObject.transform);
			TargetAudioSource = gameObject.AddComponent<AudioSource>();
			_microphoneStartedAt = Time.time;
		}
		Active = true;
		_sampleIntervalWaitForSeconds = new WaitForSeconds(SampleInterval);
		StartCoroutine(Analyze());
	}

	protected virtual void Update()
	{
		HandleBuffer();
		ComputeAmplitudes();
		HandleBeats();
	}

	protected virtual IEnumerator Analyze()
	{
		while (true)
		{
			switch (Mode)
			{
			case Modes.AudioSource:
				TargetAudioSource.GetSpectrumData(RawSpectrum, 0, Window);
				break;
			case Modes.Global:
				AudioListener.GetSpectrumData(RawSpectrum, 0, Window);
				break;
			case Modes.Microphone:
			{
				int num = 0;
				if ((float)num / 24000f > 0.03f)
				{
					if (!TargetAudioSource.isPlaying)
					{
						TargetAudioSource.timeSamples = (int)((float)num - 720f);
						TargetAudioSource.Play();
					}
					_microphoneStartedAt = Time.time;
				}
				AudioListener.GetSpectrumData(RawSpectrum, 0, Window);
				break;
			}
			}
			ComputeBandLevels();
			yield return _sampleIntervalWaitForSeconds;
		}
	}

	protected virtual void HandleBuffer()
	{
		for (int i = 0; i < BandLevels.Length; i++)
		{
			BufferedBandLevels[i] = Mathf.Max(BufferedBandLevels[i] * Mathf.Exp((0f - BufferSpeed) * Time.deltaTime), BandLevels[i]);
			NormalizedBandLevels[i] = BandLevels[i] / BandPeaks[i];
			NormalizedBufferedBandLevels[i] = BufferedBandLevels[i] / BandPeaks[i];
		}
	}

	protected virtual void ComputeBandLevels()
	{
		float num = Mathf.Log(RawSpectrum.Length);
		int i = 0;
		for (int j = 0; j < BandLevels.Length; j++)
		{
			float num2 = 0f;
			float num3 = Mathf.Exp(num / (float)BandLevels.Length * (float)(j + 1));
			float num4 = 1f / (num3 - (float)i);
			float num5 = 0f;
			for (; (float)i < num3; i++)
			{
				num5 += RawSpectrum[i];
				num2 = num5;
			}
			BandLevels[j] = Mathf.Sqrt(num4 * num2);
			if (BandLevels[j] > BandPeaks[j])
			{
				BandPeaks[j] = BandLevels[j];
				LastPeaksAt[j] = Time.time;
			}
		}
	}

	protected virtual void ComputeAmplitudes()
	{
		Amplitude = 0f;
		BufferedAmplitude = 0f;
		NormalizedAmplitude = 0f;
		NormalizedBufferedAmplitude = 0f;
		for (int i = 0; i < _cachedNumberOfBands; i++)
		{
			Amplitude += BandLevels[i];
			BufferedAmplitude += BufferedBandLevels[i];
			NormalizedAmplitude += NormalizedBandLevels[i];
			NormalizedBufferedAmplitude += NormalizedBufferedBandLevels[i];
		}
		Amplitude /= _cachedNumberOfBands;
		BufferedAmplitude /= _cachedNumberOfBands;
		NormalizedAmplitude /= _cachedNumberOfBands;
		NormalizedBufferedAmplitude /= _cachedNumberOfBands;
	}

	protected virtual void HandleBeats()
	{
		if (Beats.Length == 0)
		{
			return;
		}
		Beat[] beats = Beats;
		foreach (Beat beat in beats)
		{
			float num = 0f;
			beat.BeatThisFrame = false;
			switch (beat.Mode)
			{
			case Beat.Modes.Amplitude:
				num = Amplitude;
				break;
			case Beat.Modes.AmplitudeBuffered:
				num = BufferedAmplitude;
				break;
			case Beat.Modes.BufferedNormalized:
				num = NormalizedBufferedBandLevels[beat.BandID];
				break;
			case Beat.Modes.BufferedRaw:
				num = BufferedBandLevels[beat.BandID];
				break;
			case Beat.Modes.Normalized:
				num = NormalizedBandLevels[beat.BandID];
				break;
			case Beat.Modes.NormalizedAmplitude:
				num = NormalizedAmplitude;
				break;
			case Beat.Modes.NormalizedAmplitudeBuffered:
				num = NormalizedBufferedAmplitude;
				break;
			case Beat.Modes.Raw:
				num = BandLevels[beat.BandID];
				break;
			}
			if (beat.BeatValueMode == Beat.BeatValueModes.Live)
			{
				beat.CurrentValue = num;
				continue;
			}
			if (beat._previousValue > beat.Threshold && num <= beat.Threshold && Time.time - beat._lastBeatAt > beat.MinimumTimeBetweenBeats)
			{
				OnBeat(beat, num);
			}
			if (beat._previousValue <= beat.Threshold && num > beat.Threshold && Time.time - beat._lastBeatAt > beat.MinimumTimeBetweenBeats)
			{
				OnBeat(beat, num);
			}
			beat._previousValue = num;
		}
	}

	protected virtual void OnBeat(Beat beat, float rawValue)
	{
		beat._lastBeatAt = Time.time;
		beat.BeatThisFrame = true;
		if (beat.OnBeat != null)
		{
			beat.OnBeat.Invoke();
		}
		MMBeatEvent.Trigger(beat.Name, beat.CurrentValue);
		StartCoroutine(RemapBeat(beat));
	}

	protected virtual IEnumerator RemapBeat(Beat beat)
	{
		float remapStartedAt = Time.time;
		while (Time.time - remapStartedAt < beat.RemappedAttack + beat.RemappedDecay)
		{
			if (Time.time - remapStartedAt < beat.RemappedAttack)
			{
				beat.CurrentValue = Mathf.Lerp(0f, 1f, (Time.time - remapStartedAt) / beat.RemappedAttack);
			}
			if (Time.time - remapStartedAt > beat.RemappedAttack)
			{
				beat.CurrentValue = Mathf.Lerp(1f, 0f, (Time.time - remapStartedAt - beat.RemappedAttack) / beat.RemappedDecay);
			}
			yield return null;
		}
		beat.CurrentValue = 0f;
	}

	protected virtual void OnValidate()
	{
		if (Beats == null || Beats.Length == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < Beats.Length; i++)
		{
			if (num >= _cachedNumberOfBands)
			{
				num = 0;
			}
			Beats[i].InitializeIfNeeded(i, num);
			num++;
		}
	}
}
