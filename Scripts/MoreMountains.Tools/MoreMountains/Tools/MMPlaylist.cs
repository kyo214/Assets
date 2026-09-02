using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Audio/MMPlaylist")]
public class MMPlaylist : MonoBehaviour
{
	public enum PlaylistStates
	{
		Idle = 0,
		Playing = 1,
		Paused = 2
	}

	[Header("Playlist Songs")]
	[Tooltip("the channel on which to broadcast orders for this playlist")]
	public int Channel;

	[Tooltip("the songs that this playlist will play")]
	public List<MMPlaylistSong> Songs;

	[Header("Settings")]
	[Tooltip("whether this should play in random order or not")]
	public bool RandomOrder;

	[Tooltip("if this is true, random seed will be randomized by the system clock")]
	[MMCondition("RandomOrder", true)]
	public bool RandomizeOrderSeed = true;

	[Tooltip("whether this playlist should play and loop as a whole forever or not")]
	public bool Endless = true;

	[Tooltip("whether this playlist should auto play on start or not")]
	public bool PlayOnStart = true;

	[Tooltip("a global volume multiplier to apply when playing a song")]
	public float VolumeMultiplier = 1f;

	[Header("Status")]
	[Tooltip("the index we're currently playing")]
	[MMReadOnly]
	public int CurrentlyPlayingIndex = -1;

	[Tooltip("the name of the song that is currently playing")]
	[MMReadOnly]
	public string CurrentSongName;

	[MMReadOnly]
	public MMStateMachine<PlaylistStates> PlaylistState;

	[Header("Test")]
	[MMInspectorButton("Play")]
	public bool PlayButton;

	[MMInspectorButton("Pause")]
	public bool PauseButton;

	[MMInspectorButton("Stop")]
	public bool StopButton;

	[MMInspectorButton("PlayNextSong")]
	public bool NextButton;

	[Tooltip("the index of the song to play when pressing the PlayTargetSong button")]
	public int TargetSongIndex;

	[MMInspectorButton("PlayTargetSong")]
	public bool TargetSongButton;

	protected int _songsPlayedSoFar;

	protected int _songsPlayedThisCycle;

	protected Coroutine _coroutine;

	protected bool _firstDeserialization = true;

	protected int _listCount;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (RandomOrder && RandomizeOrderSeed)
		{
			UnityEngine.Random.InitState(Environment.TickCount);
		}
		_songsPlayedSoFar = 0;
		PlaylistState = new MMStateMachine<PlaylistStates>(base.gameObject, triggerEvents: true);
		PlaylistState.ChangeState(PlaylistStates.Idle);
		if (Songs.Count != 0 && PlayOnStart)
		{
			PlayFirstSong();
		}
	}

	protected virtual void PlayFirstSong()
	{
		_songsPlayedThisCycle = 0;
		CurrentlyPlayingIndex = -1;
		int index = PickNextIndex();
		_coroutine = StartCoroutine(PlaySong(index));
	}

	protected virtual IEnumerator PlaySong(int index)
	{
		if (Songs.Count == 0 || (!Endless && _songsPlayedThisCycle > Songs.Count))
		{
			yield break;
		}
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		if (PlaylistState.CurrentState == PlaylistStates.Playing && index >= 0 && index < Songs.Count)
		{
			StartCoroutine(Fade(CurrentlyPlayingIndex, UnityEngine.Random.Range(Songs[index].CrossFadeDuration.x, Songs[index].CrossFadeDuration.y), Songs[CurrentlyPlayingIndex].Volume.y * VolumeMultiplier, Songs[CurrentlyPlayingIndex].Volume.x * VolumeMultiplier, stopAtTheEnd: true));
		}
		if (CurrentlyPlayingIndex >= 0 && Songs.Count > CurrentlyPlayingIndex)
		{
			foreach (MMPlaylistSong song in Songs)
			{
				if (song != Songs[CurrentlyPlayingIndex])
				{
					song.Fading = false;
				}
			}
		}
		if (index < 0 || index >= Songs.Count)
		{
			yield break;
		}
		yield return MMCoroutine.WaitFor(UnityEngine.Random.Range(Songs[index].InitialDelay.x, Songs[index].InitialDelay.y));
		if (Songs[index].TargetAudioSource == null)
		{
			Debug.LogError(base.name + " : the playlist song you're trying to play is null");
			yield break;
		}
		Songs[index].TargetAudioSource.pitch = UnityEngine.Random.Range(Songs[index].Pitch.x, Songs[index].Pitch.y);
		Songs[index].TargetAudioSource.panStereo = Songs[index].StereoPan;
		Songs[index].TargetAudioSource.spatialBlend = Songs[index].SpatialBlend;
		Songs[index].TargetAudioSource.loop = Songs[index].Loop;
		StartCoroutine(Fade(index, UnityEngine.Random.Range(Songs[index].CrossFadeDuration.x, Songs[index].CrossFadeDuration.y), Songs[index].Volume.x * VolumeMultiplier, Songs[index].Volume.y * VolumeMultiplier, stopAtTheEnd: false));
		Songs[index].TargetAudioSource.Play();
		CurrentSongName = Songs[index].TargetAudioSource.clip.name;
		PlaylistState.ChangeState(PlaylistStates.Playing);
		Songs[index].Playing = true;
		CurrentlyPlayingIndex = index;
		_songsPlayedSoFar++;
		_songsPlayedThisCycle++;
		while (Songs[index].TargetAudioSource.isPlaying)
		{
			yield return null;
		}
		if (PlaylistState.CurrentState == PlaylistStates.Playing)
		{
			if (_songsPlayedSoFar < Songs.Count)
			{
				_coroutine = StartCoroutine(PlaySong(PickNextIndex()));
			}
			else if (Endless)
			{
				_coroutine = StartCoroutine(PlaySong(PickNextIndex()));
			}
			else
			{
				PlaylistState.ChangeState(PlaylistStates.Idle);
			}
		}
	}

	protected virtual IEnumerator Fade(int index, float duration, float initialVolume, float endVolume, bool stopAtTheEnd)
	{
		if (index >= 0 && index < Songs.Count)
		{
			float startTimestamp = Time.time;
			Songs[index].Fading = true;
			while (Time.time - startTimestamp < duration && Songs[index].Fading)
			{
				float t = MMMaths.Remap(Time.time - startTimestamp, 0f, duration, 0f, 1f);
				Songs[index].TargetAudioSource.volume = Mathf.Lerp(initialVolume, endVolume, t);
				yield return null;
			}
			Songs[index].TargetAudioSource.volume = endVolume;
			if (stopAtTheEnd)
			{
				Songs[index].TargetAudioSource.Stop();
				Songs[index].Playing = false;
				Songs[index].Fading = false;
			}
		}
	}

	protected virtual int PickNextIndex()
	{
		if (Songs.Count == 0)
		{
			return -1;
		}
		int num = CurrentlyPlayingIndex;
		if (RandomOrder)
		{
			while (num == CurrentlyPlayingIndex)
			{
				num = UnityEngine.Random.Range(0, Songs.Count);
			}
		}
		else
		{
			num = (CurrentlyPlayingIndex + 1) % Songs.Count;
		}
		return num;
	}

	protected virtual int PickPreviousIndex()
	{
		if (Songs.Count == 0)
		{
			return -1;
		}
		int num = CurrentlyPlayingIndex;
		if (RandomOrder)
		{
			while (num == CurrentlyPlayingIndex)
			{
				num = UnityEngine.Random.Range(0, Songs.Count);
			}
		}
		else
		{
			num = CurrentlyPlayingIndex - 1;
			if (num < 0)
			{
				num = Songs.Count - 1;
			}
		}
		return num;
	}

	public virtual void Play()
	{
		switch (PlaylistState.CurrentState)
		{
		case PlaylistStates.Idle:
			PlayFirstSong();
			break;
		case PlaylistStates.Paused:
			Songs[CurrentlyPlayingIndex].TargetAudioSource.UnPause();
			PlaylistState.ChangeState(PlaylistStates.Playing);
			break;
		case PlaylistStates.Playing:
			break;
		}
	}

	public virtual void PlayAtIndex(int songIndex)
	{
		_coroutine = StartCoroutine(PlaySong(songIndex));
	}

	public virtual void Pause()
	{
		if (PlaylistState.CurrentState == PlaylistStates.Playing)
		{
			Songs[CurrentlyPlayingIndex].TargetAudioSource.Pause();
			PlaylistState.ChangeState(PlaylistStates.Paused);
		}
	}

	public virtual void Stop()
	{
		if (PlaylistState.CurrentState == PlaylistStates.Playing)
		{
			Songs[CurrentlyPlayingIndex].TargetAudioSource.Stop();
			Songs[CurrentlyPlayingIndex].Playing = false;
			Songs[CurrentlyPlayingIndex].Fading = false;
			CurrentlyPlayingIndex = -1;
			PlaylistState.ChangeState(PlaylistStates.Idle);
		}
	}

	public virtual void PlayNextSong()
	{
		int index = PickNextIndex();
		_coroutine = StartCoroutine(PlaySong(index));
	}

	public virtual void PlayPreviousSong()
	{
		int index = PickPreviousIndex();
		_coroutine = StartCoroutine(PlaySong(index));
	}

	protected virtual void PlayTargetSong()
	{
		int songIndex = Mathf.Clamp(TargetSongIndex, 0, Songs.Count - 1);
		PlayAtIndex(songIndex);
	}

	protected virtual void OnPlayEvent(int channel)
	{
		if (channel == Channel)
		{
			Play();
		}
	}

	protected virtual void OnPauseEvent(int channel)
	{
		if (channel == Channel)
		{
			Pause();
		}
	}

	protected virtual void OnStopEvent(int channel)
	{
		if (channel == Channel)
		{
			Stop();
		}
	}

	protected virtual void OnPlayNextEvent(int channel)
	{
		if (channel == Channel)
		{
			PlayNextSong();
		}
	}

	protected virtual void OnPlayPreviousEvent(int channel)
	{
		if (channel == Channel)
		{
			PlayPreviousSong();
		}
	}

	protected virtual void OnPlayIndexEvent(int channel, int index)
	{
		if (channel == Channel)
		{
			_coroutine = StartCoroutine(PlaySong(index));
		}
	}

	protected virtual void OnEnable()
	{
		MMPlaylistPauseEvent.Register(OnPauseEvent);
		MMPlaylistPlayEvent.Register(OnPlayEvent);
		MMPlaylistPlayNextEvent.Register(OnPlayNextEvent);
		MMPlaylistPlayPreviousEvent.Register(OnPlayPreviousEvent);
		MMPlaylistStopEvent.Register(OnStopEvent);
		MMPlaylistPlayIndexEvent.Register(OnPlayIndexEvent);
	}

	protected virtual void OnDisable()
	{
		MMPlaylistPauseEvent.Unregister(OnPauseEvent);
		MMPlaylistPlayEvent.Unregister(OnPlayEvent);
		MMPlaylistPlayNextEvent.Unregister(OnPlayNextEvent);
		MMPlaylistPlayPreviousEvent.Unregister(OnPlayPreviousEvent);
		MMPlaylistStopEvent.Unregister(OnStopEvent);
		MMPlaylistPlayIndexEvent.Unregister(OnPlayIndexEvent);
	}

	protected virtual void OnValidate()
	{
		if (_firstDeserialization)
		{
			if (Songs == null)
			{
				_listCount = 0;
				_firstDeserialization = false;
			}
			else
			{
				_listCount = Songs.Count;
				_firstDeserialization = false;
			}
		}
		else
		{
			if (Songs.Count == _listCount)
			{
				return;
			}
			if (Songs.Count > _listCount)
			{
				foreach (MMPlaylistSong song in Songs)
				{
					song.Initialization();
				}
			}
			_listCount = Songs.Count;
		}
	}
}
