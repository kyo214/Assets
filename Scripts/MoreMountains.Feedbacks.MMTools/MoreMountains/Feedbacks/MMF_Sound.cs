using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[ExecuteAlways]
[AddComponentMenu("")]
[FeedbackPath("Audio/Sound")]
[FeedbackHelp("This feedback lets you play the specified AudioClip, either via event (you'll need something in your scene to catch a MMSfxEvent, for example a MMSoundManager), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play). For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness), random pitch, and an optional AudioMixerGroup.")]
public class MMF_Sound : MMF_Feedback
{
	public enum PlayMethods
	{
		Event = 0,
		Cached = 1,
		OnDemand = 2,
		Pool = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Sound", true, 14, true, false)]
	[Tooltip("the sound clip to play")]
	public AudioClip Sfx;

	[Tooltip("an array to pick a random sfx from")]
	public AudioClip[] RandomSfx;

	public MMF_Button TestPlayButton;

	public MMF_Button TestStopButton;

	[MMFInspectorGroup("Play Method", true, 27, false, false)]
	[Tooltip("the play method to use when playing the sound (event, cached or on demand)")]
	public PlayMethods PlayMethod;

	[Tooltip("the size of the pool when in Pool mode")]
	[MMFEnumCondition("PlayMethod", new int[] { 3 })]
	public int PoolSize = 10;

	[MMFInspectorGroup("Sound Properties", true, 28, false, false)]
	[Header("Volume")]
	[Tooltip("the minimum volume to play the sound at")]
	public float MinVolume = 1f;

	[Tooltip("the maximum volume to play the sound at")]
	public float MaxVolume = 1f;

	[Header("Pitch")]
	[Tooltip("the minimum pitch to play the sound at")]
	public float MinPitch = 1f;

	[Tooltip("the maximum pitch to play the sound at")]
	public float MaxPitch = 1f;

	[Header("Mixer")]
	[Tooltip("the audiomixer to play the sound with (optional)")]
	public AudioMixerGroup SfxAudioMixerGroup;

	protected AudioClip _randomClip;

	protected AudioSource _cachedAudioSource;

	protected AudioSource[] _pool;

	protected AudioSource _tempAudioSource;

	protected float _duration;

	protected AudioSource _editorAudioSource;

	public override float FeedbackDuration => GetDuration();

	public override void InitializeCustomAttributes()
	{
		TestPlayButton = new MMF_Button("Debug Play Sound", TestPlaySound);
		TestStopButton = new MMF_Button("Debug Stop Sound", TestStopSound);
	}

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (PlayMethod == PlayMethods.Cached)
		{
			_cachedAudioSource = CreateAudioSource(owner.gameObject, "CachedFeedbackAudioSource");
		}
		if (PlayMethod == PlayMethods.Pool)
		{
			_pool = new AudioSource[PoolSize];
			for (int i = 0; i < PoolSize; i++)
			{
				_pool[i] = CreateAudioSource(owner.gameObject, "PooledAudioSource" + i);
			}
		}
	}

	protected virtual AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
	{
		GameObject gameObject = new GameObject(audioSourceName);
		SceneManager.MoveGameObjectToScene(gameObject.gameObject, Owner.gameObject.scene);
		gameObject.transform.position = owner.transform.position;
		gameObject.transform.SetParent(owner.transform);
		_tempAudioSource = gameObject.AddComponent<AudioSource>();
		_tempAudioSource.playOnAwake = false;
		return _tempAudioSource;
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		float intensity = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (Sfx != null)
		{
			_duration = Sfx.length;
			PlaySound(Sfx, position, intensity);
		}
		else if (RandomSfx.Length != 0)
		{
			_randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
			if (_randomClip != null)
			{
				_duration = _randomClip.length;
				PlaySound(_randomClip, position, intensity);
			}
		}
	}

	protected virtual float GetDuration()
	{
		if (Sfx != null)
		{
			return Sfx.length;
		}
		float num = 0f;
		if (RandomSfx != null && RandomSfx.Length != 0)
		{
			AudioClip[] randomSfx = RandomSfx;
			foreach (AudioClip audioClip in randomSfx)
			{
				if (audioClip != null && audioClip.length > num)
				{
					num = audioClip.length;
				}
			}
			return num;
		}
		return 0f;
	}

	protected virtual void PlaySound(AudioClip sfx, Vector3 position, float intensity)
	{
		float num = Random.Range(MinVolume, MaxVolume);
		if (!Timing.ConstantIntensity)
		{
			num *= intensity;
		}
		float num2 = Random.Range(MinPitch, MaxPitch);
		int timeSamples = ((!NormalPlayDirection) ? (sfx.samples - 1) : 0);
		if (!NormalPlayDirection)
		{
			num2 = 0f - num2;
		}
		if (PlayMethod == PlayMethods.Event)
		{
			MMSfxEvent.Trigger(sfx, SfxAudioMixerGroup, num, num2);
			return;
		}
		if (PlayMethod == PlayMethods.OnDemand)
		{
			GameObject gameObject = new GameObject("TempAudio");
			SceneManager.MoveGameObjectToScene(gameObject.gameObject, Owner.gameObject.scene);
			gameObject.transform.position = position;
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			PlayAudioSource(audioSource, sfx, num, num2, timeSamples, SfxAudioMixerGroup);
			Owner.ProxyDestroy(gameObject, sfx.length);
		}
		if (PlayMethod == PlayMethods.Cached)
		{
			PlayAudioSource(_cachedAudioSource, sfx, num, num2, timeSamples, SfxAudioMixerGroup);
		}
		if (PlayMethod == PlayMethods.Pool)
		{
			_tempAudioSource = GetAudioSourceFromPool();
			if (_tempAudioSource != null)
			{
				PlayAudioSource(_tempAudioSource, sfx, num, num2, timeSamples, SfxAudioMixerGroup);
			}
		}
	}

	protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch, int timeSamples, AudioMixerGroup audioMixerGroup = null)
	{
		audioSource.clip = sfx;
		audioSource.timeSamples = timeSamples;
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.loop = false;
		if (audioMixerGroup != null)
		{
			audioSource.outputAudioMixerGroup = audioMixerGroup;
		}
		audioSource.Play();
	}

	protected virtual AudioSource GetAudioSourceFromPool()
	{
		for (int i = 0; i < PoolSize; i++)
		{
			if (!_pool[i].isPlaying)
			{
				return _pool[i];
			}
		}
		return null;
	}

	protected virtual async void TestPlaySound()
	{
		AudioClip audioClip = null;
		if (Sfx != null)
		{
			audioClip = Sfx;
		}
		if (RandomSfx.Length != 0)
		{
			audioClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
		}
		if (audioClip == null)
		{
			Debug.LogError(Label + " on " + Owner.gameObject.name + " can't play in editor mode, you haven't set its Sfx.");
			return;
		}
		float volume = Random.Range(MinVolume, MaxVolume);
		float pitch = Random.Range(MinPitch, MaxPitch);
		GameObject temporaryAudioHost = new GameObject("EditorTestAS_WillAutoDestroy");
		SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
		temporaryAudioHost.transform.position = Owner.transform.position;
		_editorAudioSource = temporaryAudioHost.AddComponent<AudioSource>();
		PlayAudioSource(_editorAudioSource, audioClip, volume, pitch, 0);
		await Task.Delay((int)(1000f * audioClip.length));
		Owner.ProxyDestroyImmediate(temporaryAudioHost);
	}

	protected virtual void TestStopSound()
	{
		if (_editorAudioSource != null)
		{
			_editorAudioSource.Stop();
		}
	}
}
