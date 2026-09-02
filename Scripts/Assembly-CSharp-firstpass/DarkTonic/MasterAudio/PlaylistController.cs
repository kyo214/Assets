using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio;

[AudioScriptOrder(-80)]
[RequireComponent(typeof(AudioSource))]
public class PlaylistController : MonoBehaviour
{
	public enum FadeStatus
	{
		NotFading = 0,
		FadingIn = 1,
		FadeingOut = 2
	}

	public enum AudioPlayType
	{
		PlayNow = 0,
		Schedule = 1,
		AlreadyScheduled = 2
	}

	public enum PlaylistStates
	{
		NotInScene = 0,
		Stopped = 1,
		Playing = 2,
		Paused = 3,
		Crossfading = 4
	}

	public enum FadeMode
	{
		None = 0,
		GradualFade = 1
	}

	public enum AudioDuckingMode
	{
		NotDucking = 0,
		SetToDuck = 1,
		Ducked = 2,
		Unducking = 3
	}

	public delegate void SongChangedEventHandler(string newSongName, MusicSetting song);

	public delegate void SongEndedEventHandler(string songName);

	public delegate void SongLoopedEventHandler(string songName);

	public delegate void PlaylistEndedEventHandler();

	public const int FramesEarlyToTrigger = 2;

	public const int FramesEarlyToBeSyncable = 10;

	private const double UniversalAudioReactionTime = 0.3;

	private const int NextScheduleTimeRecalcConsecutiveFrameCount = 5;

	private const string NotReadyMessage = "Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.";

	private const float MinSongLength = 0.5f;

	private const float SlowestFrameTimeForCalc = 0.3f;

	public bool startPlaylistOnAwake = true;

	public bool isShuffle;

	public bool isAutoAdvance = true;

	public bool loopPlaylist = true;

	public float _playlistVolume = 1f;

	public bool isMuted;

	public string startPlaylistName = string.Empty;

	public int syncGroupNum = -1;

	public bool ignoreListenerPause;

	public AudioMixerGroup mixerChannel;

	public MasterAudio.ItemSpatialBlendType spatialBlendType;

	public float spatialBlend;

	public bool initializedEventExpanded;

	public string initializedCustomEvent = string.Empty;

	public bool crossfadeStartedExpanded;

	public string crossfadeStartedCustomEvent = string.Empty;

	public bool songChangedEventExpanded;

	public string songChangedCustomEvent = string.Empty;

	public bool songEndedEventExpanded;

	public string songEndedCustomEvent = string.Empty;

	public bool songLoopedEventExpanded;

	public string songLoopedCustomEvent = string.Empty;

	public bool playlistStartedEventExpanded;

	public string playlistStartedCustomEvent = string.Empty;

	public bool playlistEndedEventExpanded;

	public string playlistEndedCustomEvent = string.Empty;

	private AudioSource _activeAudio;

	private AudioSource _transitioningAudio;

	private float _activeAudioEndVolume;

	private float _transitioningAudioStartVolume;

	private float _crossFadeStartTime;

	private readonly List<int> _clipsRemaining = new List<int>(10);

	private int _currentSequentialClipIndex;

	private AudioDuckingMode _duckingMode;

	private float _timeToStartUnducking;

	private float _timeToFinishUnducking;

	private float _originalMusicVolume;

	private float _initialDuckVolume;

	private float _duckRange;

	private SoundGroupVariationUpdater _actorUpdater;

	private float _unduckTime;

	private MusicSetting _currentSong;

	private GameObject _go;

	private string _name;

	private FadeMode _curFadeMode;

	private float _slowFadeStartTime;

	private float _slowFadeCompletionTime;

	private float _slowFadeStartVolume;

	private float _slowFadeTargetVolume;

	private MasterAudio.Playlist _currentPlaylist;

	private float _lastTimeMissingPlaylistLogged = -5f;

	private Action _fadeCompleteCallback;

	private readonly List<MusicSetting> _queuedSongs = new List<MusicSetting>(5);

	private bool _lostFocus;

	private bool _autoStartedPlaylist;

	private bool _isLoopSectionSchedule;

	private double? _loopSectionNextStartTime;

	private AudioSource _audioClip;

	private AudioSource _transClip;

	private MusicSetting _newSongSetting;

	private bool _nextSongRequested;

	private bool _nextSongScheduled;

	private int _lastRandomClipIndex = -1;

	private float _lastTimeSongRequested = -1f;

	private float _currentDuckVolCut;

	private int? _lastSongPosition;

	private double? _currentSchedSongDspStartTime;

	private double? _currentSchedSongDspEndTime;

	private int _lastFrameSongPosition = -1;

	private int _nextScheduleTimeRecalcDifferentFirstFrameNum;

	private double? _nextScheduledTimeRecalcStart;

	private readonly Dictionary<AudioSource, double> _scheduledSongOffsetByAudioSource = new Dictionary<AudioSource, double>(2);

	private readonly Dictionary<AudioSource, double> _scheduledSongStartTimeByAudioSource = new Dictionary<AudioSource, double>(2);

	private readonly Dictionary<AudioSource, double> _scheduledSongEndTimeByAudioSource = new Dictionary<AudioSource, double>(2);

	private readonly Dictionary<AudioSource, AssetReference> _loadedAddressablesByAudioSource = new Dictionary<AudioSource, AssetReference>(2);

	public int _frames;

	private static List<PlaylistController> _instances;

	private Coroutine _resourceCoroutine;

	private Coroutine _addressableCoroutine;

	private int _songsPlayedFromPlaylist;

	private AudioSource _audio1;

	private AudioSource _audio2;

	private string _activeSongAlias;

	private bool _isPlayingQueuedSong;

	private bool _isSongReplacingScheduledTrack;

	private Transform _trans;

	private bool _willPersist;

	private double? _songPauseTime;

	private int framesOfSongPlayed;

	private bool WillSyncToOtherClip
	{
		get
		{
			if (syncGroupNum > 0)
			{
				return _currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips;
			}
			return false;
		}
	}

	public bool CurrentSongIsPlaying
	{
		get
		{
			if (!_scheduledSongOffsetByAudioSource.ContainsKey(ActiveAudioSource))
			{
				return false;
			}
			return _scheduledSongOffsetByAudioSource[ActiveAudioSource] < AudioSettings.dspTime;
		}
	}

	private bool SongIsNonAdvancible
	{
		get
		{
			if (CurrentPlaylist != null && CurrentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips)
			{
				return CrossFadeTime > 0f;
			}
			return false;
		}
	}

	public bool ControllerIsReady { get; private set; }

	public FadeStatus CurrentFadeStatus
	{
		get
		{
			if (_curFadeMode == FadeMode.GradualFade)
			{
				if (!(_slowFadeStartVolume < _slowFadeTargetVolume))
				{
					return FadeStatus.FadeingOut;
				}
				return FadeStatus.FadingIn;
			}
			return FadeStatus.NotFading;
		}
	}

	public PlaylistStates PlaylistState
	{
		get
		{
			if (_activeAudio == null || _transitioningAudio == null)
			{
				return PlaylistStates.NotInScene;
			}
			if (_songPauseTime.HasValue)
			{
				return PlaylistStates.Paused;
			}
			if (!IsPlayingScheduledLoopSection && !ActiveAudioSource.isPlaying)
			{
				return PlaylistStates.Stopped;
			}
			if (IsCrossFading)
			{
				return PlaylistStates.Crossfading;
			}
			return PlaylistStates.Playing;
		}
	}

	public AudioSource ActiveAudioSource
	{
		get
		{
			if (_activeAudio != null && _activeAudio.clip == null)
			{
				return _transitioningAudio;
			}
			return _activeAudio;
		}
	}

	public static List<PlaylistController> Instances
	{
		get
		{
			if (_instances != null)
			{
				return _instances;
			}
			_instances = new List<PlaylistController>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(PlaylistController));
			for (int i = 0; i < array.Length; i++)
			{
				_instances.Add(array[i] as PlaylistController);
			}
			return _instances;
		}
		set
		{
			_instances = value;
		}
	}

	public GameObject PlaylistControllerGameObject => _go;

	public AudioSource CurrentPlaylistSource => _activeAudio;

	public AudioClip CurrentPlaylistClip
	{
		get
		{
			if (_activeAudio == null)
			{
				return null;
			}
			return _activeAudio.clip;
		}
	}

	public AudioClip FadingPlaylistClip
	{
		get
		{
			if (!IsCrossFading)
			{
				return null;
			}
			if (_transitioningAudio == null)
			{
				return null;
			}
			return _transitioningAudio.clip;
		}
	}

	public AudioSource FadingSource
	{
		get
		{
			if (!IsCrossFading)
			{
				return null;
			}
			return _transitioningAudio;
		}
	}

	public bool IsCrossFading { get; private set; }

	public bool IsFading
	{
		get
		{
			if (!IsCrossFading)
			{
				return _curFadeMode != FadeMode.None;
			}
			return true;
		}
	}

	public float PlaylistVolume
	{
		get
		{
			return MasterAudio.PlaylistMasterVolume * _playlistVolume;
		}
		set
		{
			_playlistVolume = value;
			UpdateMasterVolume();
		}
	}

	public MasterAudio.Playlist CurrentPlaylist
	{
		get
		{
			if (_currentPlaylist != null || !(Time.realtimeSinceStartup - _lastTimeMissingPlaylistLogged > 2f))
			{
				return _currentPlaylist;
			}
			_lastTimeMissingPlaylistLogged = AudioUtil.Time;
			return _currentPlaylist;
		}
	}

	public bool HasPlaylist => _currentPlaylist != null;

	public string PlaylistName
	{
		get
		{
			if (CurrentPlaylist == null)
			{
				return string.Empty;
			}
			return CurrentPlaylist.playlistName;
		}
	}

	public MusicSetting CurrentSong => _currentSong;

	private bool IsMuted => isMuted;

	private bool PlaylistIsMuted
	{
		set
		{
			isMuted = value;
			if (Application.isPlaying)
			{
				if (_activeAudio != null)
				{
					_activeAudio.mute = value;
				}
				if (_transitioningAudio != null)
				{
					_transitioningAudio.mute = value;
				}
			}
			else
			{
				AudioSource[] components = GetComponents<AudioSource>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].mute = value;
				}
			}
		}
	}

	private float CrossFadeTime
	{
		get
		{
			if (_currentPlaylist != null)
			{
				if (_currentPlaylist.crossfadeMode != MasterAudio.Playlist.CrossfadeTimeMode.UseMasterSetting)
				{
					return _currentPlaylist.crossFadeTime;
				}
				return MasterAudio.Instance.MasterCrossFadeTime;
			}
			return MasterAudio.Instance.MasterCrossFadeTime;
		}
	}

	private bool IsAutoAdvance
	{
		get
		{
			if (SongIsNonAdvancible)
			{
				return false;
			}
			return isAutoAdvance;
		}
	}

	public GameObject GameObj
	{
		get
		{
			if (_go != null)
			{
				return _go;
			}
			_go = base.gameObject;
			return _go;
		}
	}

	public string ControllerName
	{
		get
		{
			if (_name != null)
			{
				return _name;
			}
			_name = GameObj.name;
			return _name;
		}
	}

	public bool CanSchedule
	{
		get
		{
			if (MasterAudio.Instance.useGaplessPlaylists)
			{
				return IsAutoAdvance;
			}
			return false;
		}
	}

	private bool IsFrameFastEnough => AudioUtil.FrameTime < 0.3f;

	private bool ShouldNotSwitchEarly
	{
		get
		{
			if (CrossFadeTime <= 0f)
			{
				return CanSchedule;
			}
			return false;
		}
	}

	private bool IsPlayingScheduledLoopSection
	{
		get
		{
			if (_loopSectionNextStartTime.HasValue && AudioSettings.dspTime >= _loopSectionNextStartTime.Value && _currentSchedSongDspEndTime.HasValue)
			{
				return AudioSettings.dspTime <= _currentSchedSongDspEndTime.Value;
			}
			return false;
		}
	}

	private bool IsCurrentSongLoopSection
	{
		get
		{
			if (_currentSong != null && _currentSong.songStartTimeMode == MasterAudio.CustomSongStartTimeMode.Section)
			{
				return _currentSong.isLoop;
			}
			return false;
		}
	}

	private Transform Trans
	{
		get
		{
			if (_trans != null)
			{
				return _trans;
			}
			_trans = base.transform;
			return _trans;
		}
	}

	public int ClipsRemainingInCurrentPlaylist => _clipsRemaining.Count;

	public event SongChangedEventHandler SongChanged;

	public event SongEndedEventHandler SongEnded;

	public event SongLoopedEventHandler SongLooped;

	public event PlaylistEndedEventHandler PlaylistEnded;

	private void Awake()
	{
		base.useGUILayout = false;
		if (ControllerIsReady)
		{
			return;
		}
		ControllerIsReady = false;
		PlaylistController[] array = (PlaylistController[])UnityEngine.Object.FindObjectsOfType(typeof(PlaylistController));
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].ControllerName == ControllerName)
			{
				num++;
			}
		}
		if (num > 1)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(MasterAudio));
			bool flag = false;
			for (int j = 0; j < array2.Length; j++)
			{
				MasterAudio masterAudio = array2[j] as MasterAudio;
				if (masterAudio.persistBetweenScenes && masterAudio.shouldLogDestroys)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Debug.Log("More than one Playlist Controller prefab exists in this Scene with the same name. Destroying the one called '" + ControllerName + "'. You may wish to set up a Bootstrapper Scene so this does not occur.");
			}
			return;
		}
		_autoStartedPlaylist = false;
		_duckingMode = AudioDuckingMode.NotDucking;
		_currentSong = null;
		_songsPlayedFromPlaylist = 0;
		AudioSource[] components = GetComponents<AudioSource>();
		if (components.Length < 2)
		{
			Debug.LogError("This prefab should have exactly two Audio Source components. Please revert it.");
			return;
		}
		MasterAudio safeInstance = MasterAudio.SafeInstance;
		_willPersist = safeInstance != null && safeInstance.persistBetweenScenes;
		_audio1 = components[0];
		_audio2 = components[1];
		_audio1.clip = null;
		_audio2.clip = null;
		if (_audio1.playOnAwake || _audio2.playOnAwake)
		{
			Debug.LogWarning("One or more 'Play on Awake' checkboxes in the Audio Sources on Playlist Controller '" + base.name + "' are checked. These are not used in Master Audio. Make sure to uncheck them before hitting Play next time. For Playlist Controllers, use the similarly named checkbox 'Start Playlist on Awake' in the Playlist Controller's Inspector.");
		}
		_activeAudio = _audio1;
		_transitioningAudio = _audio2;
		_audio1.outputAudioMixerGroup = mixerChannel;
		_audio2.outputAudioMixerGroup = mixerChannel;
		SetSpatialBlend();
		_audio1.ignoreListenerPause = ignoreListenerPause;
		_audio2.ignoreListenerPause = ignoreListenerPause;
		SpatializerHelper.TurnOnSpatializerIfEnabled(_audio1);
		SpatializerHelper.TurnOnSpatializerIfEnabled(_audio2);
		_curFadeMode = FadeMode.None;
		_fadeCompleteCallback = null;
		_lostFocus = false;
	}

	public void SetSpatialBlend()
	{
		if (MasterAudio.SafeInstance == null)
		{
			return;
		}
		switch (MasterAudio.Instance.musicSpatialBlendType)
		{
		case MasterAudio.AllMusicSpatialBlendType.ForceAllTo2D:
			SetAudioSpatialBlend(0f);
			break;
		case MasterAudio.AllMusicSpatialBlendType.ForceAllTo3D:
			SetAudioSpatialBlend(1f);
			break;
		case MasterAudio.AllMusicSpatialBlendType.ForceAllToCustom:
			SetAudioSpatialBlend(MasterAudio.Instance.musicSpatialBlend);
			break;
		case MasterAudio.AllMusicSpatialBlendType.AllowDifferentPerController:
			switch (spatialBlendType)
			{
			case MasterAudio.ItemSpatialBlendType.ForceTo2D:
				SetAudioSpatialBlend(0f);
				break;
			case MasterAudio.ItemSpatialBlendType.ForceTo3D:
				SetAudioSpatialBlend(1f);
				break;
			case MasterAudio.ItemSpatialBlendType.ForceToCustom:
				SetAudioSpatialBlend(spatialBlend);
				break;
			case MasterAudio.ItemSpatialBlendType.UseCurveFromAudioSource:
				break;
			}
			break;
		}
	}

	private void DetectAndRescheduleNextGaplessSongIfOff()
	{
		MasterAudio safeInstance = MasterAudio.SafeInstance;
		if (safeInstance == null || !safeInstance.useGaplessAutoReschedule)
		{
			return;
		}
		if (!CanSchedule || CurrentPlaylistSource.loop || !_currentSchedSongDspStartTime.HasValue || _scheduledSongOffsetByAudioSource.Count == 0)
		{
			_nextScheduledTimeRecalcStart = null;
			_nextScheduleTimeRecalcDifferentFirstFrameNum = 0;
			return;
		}
		double num = AudioSettings.dspTime + (double)_activeAudio.clip.samples / (double)_activeAudio.clip.frequency - (double)_activeAudio.timeSamples / (double)_activeAudio.clip.frequency;
		double num2 = Math.Round(num, 7);
		double num3 = Math.Round(_currentSchedSongDspStartTime.Value, 7);
		if (num2 == num3)
		{
			_nextScheduledTimeRecalcStart = null;
			_nextScheduleTimeRecalcDifferentFirstFrameNum = 0;
			return;
		}
		if (!_nextScheduledTimeRecalcStart.HasValue)
		{
			_nextScheduledTimeRecalcStart = num;
			_nextScheduleTimeRecalcDifferentFirstFrameNum = Time.frameCount;
		}
		if (Math.Round(_nextScheduledTimeRecalcStart.Value, 7) != num2)
		{
			_nextScheduledTimeRecalcStart = null;
			_nextScheduleTimeRecalcDifferentFirstFrameNum = 0;
		}
		else if (_nextScheduleTimeRecalcDifferentFirstFrameNum + 5 <= Time.frameCount)
		{
			_audioClip.Stop();
			ScheduleClipPlay(num, _transitioningAudio, calledAfterPause: false, addDspTime: false);
		}
	}

	private MusicSetting FindSongByAliasOrName(string clipName)
	{
		if (_currentPlaylist != null)
		{
			for (int i = 0; i < _currentPlaylist.MusicSettings.Count; i++)
			{
				MusicSetting musicSetting = _currentPlaylist.MusicSettings[i];
				if (musicSetting.alias == clipName)
				{
					return musicSetting;
				}
				switch (musicSetting.audLocation)
				{
				case MasterAudio.AudioLocation.Clip:
					if (musicSetting.clip != null && musicSetting.clip.CachedName() == clipName)
					{
						return musicSetting;
					}
					break;
				case MasterAudio.AudioLocation.ResourceFile:
					if (musicSetting.resourceFileName == clipName)
					{
						return musicSetting;
					}
					break;
				}
			}
		}
		return null;
	}

	private void SetAudiosIfEmpty()
	{
		AudioSource[] components = GetComponents<AudioSource>();
		_audio1 = components[0];
		_audio2 = components[1];
	}

	private void SetAudioSpatialBlend(float blend)
	{
		if (_audio1 == null)
		{
			SetAudiosIfEmpty();
		}
		_audio1.spatialBlend = blend;
		_audio2.spatialBlend = blend;
	}

	private void Start()
	{
		if (ControllerIsReady)
		{
			return;
		}
		if (MasterAudio.SafeInstance == null)
		{
			Debug.LogError("No Master Audio game object exists in the Hierarchy. Aborting Playlist Controller setup code.");
			return;
		}
		if (!string.IsNullOrEmpty(startPlaylistName) && _currentPlaylist == null)
		{
			InitializePlaylist();
		}
		ControllerIsReady = true;
		if (initializedEventExpanded && initializedCustomEvent != string.Empty && initializedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(initializedCustomEvent, Trans, logDupe: false);
		}
		AutoStartPlaylist();
		if (IsMuted)
		{
			MutePlaylist();
		}
	}

	private void AutoStartPlaylist()
	{
		if (_currentPlaylist != null && startPlaylistOnAwake && IsFrameFastEnough && !_autoStartedPlaylist)
		{
			PlayNextOrRandom(AudioPlayType.PlayNow);
			_autoStartedPlaylist = true;
		}
	}

	private void CoUpdate()
	{
		if (MasterAudio.SafeInstance == null || _curFadeMode != FadeMode.GradualFade || _activeAudio == null)
		{
			return;
		}
		float val = 1f - (_slowFadeCompletionTime - AudioUtil.Time) / (_slowFadeCompletionTime - _slowFadeStartTime);
		val = Math.Min(val, 1f);
		val = Math.Max(val, 0f);
		float val2 = _slowFadeStartVolume + (_slowFadeTargetVolume - _slowFadeStartVolume) * val;
		val2 = ((!(_slowFadeTargetVolume > _slowFadeStartVolume)) ? Math.Max(val2, _slowFadeTargetVolume) : Math.Min(val2, _slowFadeTargetVolume));
		_playlistVolume = val2;
		UpdateMasterVolume();
		if (!(AudioUtil.Time < _slowFadeCompletionTime))
		{
			if (MasterAudio.Instance.stopZeroVolumePlaylists && _slowFadeTargetVolume == 0f)
			{
				StopPlaylist();
			}
			if (_fadeCompleteCallback != null)
			{
				_fadeCompleteCallback();
				_fadeCompleteCallback = null;
			}
			_curFadeMode = FadeMode.None;
		}
	}

	private void OnEnable()
	{
		_instances = null;
		MasterAudio.TrackRuntimeAudioSources(new List<AudioSource> { _audio1, _audio2 });
	}

	private void OnDisable()
	{
		_instances = null;
		if (!(MasterAudio.SafeInstance == null) && !MasterAudio.AppIsShuttingDown)
		{
			if (ActiveAudioSource != null && ActiveAudioSource.clip != null && !_willPersist)
			{
				StopPlaylist();
			}
			MasterAudio.StopTrackingRuntimeAudioSources(new List<AudioSource> { _audio1, _audio2 });
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		_lostFocus = pauseStatus;
	}

	private void Update()
	{
		_frames++;
		CoUpdate();
		if (_lostFocus || !ControllerIsReady)
		{
			return;
		}
		AutoStartPlaylist();
		if (_activeAudio.isPlaying)
		{
			framesOfSongPlayed++;
		}
		if (IsPlayingScheduledLoopSection && !_songPauseTime.HasValue)
		{
			if (_activeAudio != _audioClip)
			{
				_activeAudio = _audioClip;
				_transitioningAudio = _transClip;
			}
			if (!_transitioningAudio.isPlaying && _queuedSongs.Count == 0)
			{
				PlaySong(_currentSong, AudioPlayType.Schedule, isLoopSectionSchedule: true);
			}
			return;
		}
		if (IsCurrentSongLoopSection && _songPauseTime.HasValue)
		{
			double num = AudioSettings.dspTime - _songPauseTime.Value;
			double num2 = _scheduledSongEndTimeByAudioSource[_activeAudio];
			double scheduledEndTime = num2 + num;
			_activeAudio.SetScheduledEndTime(scheduledEndTime);
			if (_scheduledSongStartTimeByAudioSource.ContainsKey(_transitioningAudio))
			{
				double num3 = _scheduledSongStartTimeByAudioSource[_transitioningAudio];
				num2 = _scheduledSongEndTimeByAudioSource[_transitioningAudio];
				double time = num3 + num;
				_transitioningAudio.PlayScheduled(time);
				scheduledEndTime = num2 + num;
				_transitioningAudio.SetScheduledEndTime(scheduledEndTime);
			}
		}
		if (IsCrossFading)
		{
			if (_activeAudio.volume >= _activeAudioEndVolume)
			{
				CeaseAudioSource(_transitioningAudio);
				IsCrossFading = false;
				if (CanSchedule && !_nextSongScheduled)
				{
					PlayNextOrRandom(AudioPlayType.Schedule);
				}
				SetDuckProperties();
			}
			float num4 = Math.Max(CrossFadeTime, 0.001f);
			float num5 = Mathf.Clamp01((Time.realtimeSinceStartup - _crossFadeStartTime) / num4);
			_activeAudio.volume = num5 * _activeAudioEndVolume;
			_transitioningAudio.volume = _transitioningAudioStartVolume * (1f - num5);
		}
		else if (IsCurrentSongLoopSection && _scheduledSongStartTimeByAudioSource.Count <= 1 && !_scheduledSongStartTimeByAudioSource.ContainsKey(_transitioningAudio))
		{
			PlaySong(_currentSong, AudioPlayType.Schedule, isLoopSectionSchedule: true);
		}
		if (!_activeAudio.loop && _activeAudio.clip != null)
		{
			if (_songPauseTime.HasValue)
			{
				goto IL_054e;
			}
			if (!_activeAudio.isPlaying)
			{
				if (!IsAutoAdvance)
				{
					FirePlaylistEndedEventIfAny();
					CeaseAudioSource(_activeAudio);
					return;
				}
				if ((!isShuffle) ? (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count) : (_clipsRemaining.Count == 0))
				{
					FirePlaylistEndedEventIfAny();
					CeaseAudioSource(_activeAudio);
					return;
				}
				if (IsCurrentSongLoopSection)
				{
					CeaseAudioSource(_activeAudio);
				}
			}
			bool flag = false;
			if (ShouldNotSwitchEarly)
			{
				if (_currentSchedSongDspStartTime.HasValue && AudioSettings.dspTime > _currentSchedSongDspStartTime.Value)
				{
					flag = true;
				}
			}
			else if (PlaylistState == PlaylistStates.Stopped)
			{
				flag = true;
			}
			else
			{
				bool flag2 = Application.targetFrameRate == 1;
				if ((IsFrameFastEnough | flag2) && _activeAudio.clip != null)
				{
					float num6 = _activeAudio.clip.length - _activeAudio.time - AudioUtil.AdjustEndLeadTimeForPitch(CrossFadeTime, _activeAudio);
					float num7 = AudioUtil.AdjustEndLeadTimeForPitch(AudioUtil.FrameTime * 2f, _activeAudio);
					flag = num6 <= num7;
				}
			}
			if (flag)
			{
				if (_currentPlaylist.fadeOutLastSong)
				{
					if (isShuffle)
					{
						if (_clipsRemaining.Count == 0 || !IsAutoAdvance)
						{
							FadeOutPlaylist();
							return;
						}
					}
					else if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count || _currentPlaylist.MusicSettings.Count == 1 || !IsAutoAdvance)
					{
						FadeOutPlaylist();
						return;
					}
				}
				if (IsAutoAdvance && !_nextSongRequested && _lastTimeSongRequested + 0.5f <= AudioUtil.Time)
				{
					_lastTimeSongRequested = AudioUtil.Time;
					if (CanSchedule)
					{
						_lastSongPosition = null;
						FadeInScheduledSong();
					}
					else if (!IsCurrentSongLoopSection)
					{
						_lastSongPosition = 0;
						PlayNextOrRandom(AudioPlayType.PlayNow);
					}
				}
			}
			else
			{
				DetectAndRescheduleNextGaplessSongIfOff();
			}
		}
		if ((IsCurrentSongLoopSection || _activeAudio.loop) && _activeAudio.clip != null)
		{
			if (_activeAudio.timeSamples < _lastFrameSongPosition)
			{
				string text = _activeAudio.clip.CachedName();
				if (SongLooped != null && !string.IsNullOrEmpty(text))
				{
					SongLooped(text);
				}
				if (songLoopedEventExpanded && songLoopedCustomEvent != string.Empty && songLoopedCustomEvent != "[None]")
				{
					MasterAudio.FireCustomEvent(songLoopedCustomEvent, Trans, logDupe: false);
				}
			}
			_lastFrameSongPosition = _activeAudio.timeSamples;
		}
		goto IL_054e;
		IL_054e:
		if (!IsCrossFading)
		{
			AudioDucking();
		}
	}

	public static PlaylistController InstanceByName(string playlistControllerName, bool errorIfNotFound = true)
	{
		for (int i = 0; i < Instances.Count; i++)
		{
			PlaylistController playlistController = Instances[i];
			if (playlistController != null && playlistController.ControllerName == playlistControllerName)
			{
				return playlistController;
			}
		}
		if (errorIfNotFound)
		{
			Debug.LogError("Could not find Playlist Controller '" + playlistControllerName + "'.");
		}
		return null;
	}

	public bool IsSongPlaying(string songName)
	{
		if (!HasPlaylist)
		{
			return false;
		}
		if (ActiveAudioSource == null || ActiveAudioSource.clip == null)
		{
			return false;
		}
		if (ActiveAudioSource.clip.CachedName() == songName)
		{
			return true;
		}
		return _activeSongAlias == songName;
	}

	public void ClearQueue()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else
		{
			_queuedSongs.Clear();
		}
	}

	public void ToggleMutePlaylist()
	{
		if (Application.isPlaying && !ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else if (IsMuted)
		{
			UnmutePlaylist();
		}
		else
		{
			MutePlaylist();
		}
	}

	public void MutePlaylist()
	{
		PlaylistIsMuted = true;
	}

	public void UnmutePlaylist()
	{
		PlaylistIsMuted = false;
	}

	public void PausePlaylist()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else if (!(_activeAudio == null) && !(_transitioningAudio == null))
		{
			if (_activeAudio.clip != null)
			{
				_activeAudio.Pause();
			}
			if (!_songPauseTime.HasValue)
			{
				_songPauseTime = AudioSettings.dspTime;
			}
			if (_transitioningAudio.clip != null)
			{
				_transitioningAudio.Pause();
			}
		}
	}

	public bool UnpausePlaylist()
	{
		double dspTime = AudioSettings.dspTime;
		double? songPauseTime = _songPauseTime;
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			_songPauseTime = null;
			return false;
		}
		if (PlaylistState == PlaylistStates.Playing || PlaylistState == PlaylistStates.Crossfading || PlaylistState == PlaylistStates.Stopped)
		{
			_songPauseTime = null;
			return false;
		}
		if (_activeAudio == null || _transitioningAudio == null)
		{
			_songPauseTime = null;
			return false;
		}
		if (_activeAudio.clip == null && _currentPlaylist != null)
		{
			FinishPlaylistInit();
			_songPauseTime = null;
			return true;
		}
		if (_activeAudio.clip == null)
		{
			_songPauseTime = null;
			return false;
		}
		bool flag = _scheduledSongOffsetByAudioSource.ContainsKey(_activeAudio) && _songPauseTime.HasValue && _scheduledSongOffsetByAudioSource[_activeAudio] <= _songPauseTime.Value;
		if (!_scheduledSongOffsetByAudioSource.ContainsKey(_activeAudio) | flag)
		{
			_activeAudio.Play();
			framesOfSongPlayed = 0;
			AudioUtil.ClipPlayed(_activeAudio.clip, GameObj);
			if (_scheduledSongEndTimeByAudioSource.ContainsKey(_activeAudio))
			{
				double num = _scheduledSongEndTimeByAudioSource[_activeAudio];
				double num2 = dspTime - _songPauseTime.Value;
				double value = num + num2;
				_scheduledSongEndTimeByAudioSource[_activeAudio] = value;
			}
			_songPauseTime = null;
		}
		else if (_songPauseTime.HasValue && _currentSchedSongDspStartTime.HasValue)
		{
			double num3 = dspTime - _songPauseTime.Value;
			double scheduledPlayTimeOffset = _currentSchedSongDspStartTime.Value - AudioSettings.dspTime + num3;
			_songPauseTime = null;
			_activeAudio.Stop();
			ScheduleClipPlay(scheduledPlayTimeOffset, _activeAudio, calledAfterPause: true);
		}
		if (!_scheduledSongOffsetByAudioSource.ContainsKey(_transitioningAudio))
		{
			_songPauseTime = null;
			if (_transitioningAudio.clip == null)
			{
				_transitioningAudio.Play();
				AudioUtil.ClipPlayed(_transitioningAudio.clip, GameObj);
			}
		}
		else if (songPauseTime.HasValue && _currentSchedSongDspStartTime.HasValue && _transitioningAudio.clip != null)
		{
			double num4 = dspTime - songPauseTime.Value;
			double scheduledPlayTimeOffset2 = _currentSchedSongDspStartTime.Value - AudioSettings.dspTime + num4;
			_songPauseTime = null;
			_transitioningAudio.Stop();
			_scheduledSongEndTimeByAudioSource.Remove(_transitioningAudio);
			ScheduleClipPlay(scheduledPlayTimeOffset2, _transitioningAudio, calledAfterPause: true);
		}
		return true;
	}

	public void StopPlaylist(bool onlyFadingClip = false)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else
		{
			if (!Application.isPlaying)
			{
				return;
			}
			_currentSchedSongDspStartTime = null;
			_currentSchedSongDspEndTime = null;
			_currentSong = null;
			PlaylistStates playlistState = PlaylistState;
			if ((uint)playlistState > 1u)
			{
				if (!onlyFadingClip)
				{
					CeaseAudioSource(_activeAudio);
				}
				CeaseAudioSource(_transitioningAudio);
				if (!onlyFadingClip && _clipsRemaining.Count == 0 && PlaylistEnded != null)
				{
					PlaylistEnded();
				}
			}
		}
	}

	public void FadeToVolume(float targetVolume, float fadeTime, Action callback = null)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return;
		}
		if (fadeTime <= 0.1f)
		{
			_playlistVolume = targetVolume;
			UpdateMasterVolume();
			_curFadeMode = FadeMode.None;
			return;
		}
		_curFadeMode = FadeMode.GradualFade;
		if (_duckingMode == AudioDuckingMode.NotDucking)
		{
			_slowFadeStartVolume = _playlistVolume;
		}
		else
		{
			_slowFadeStartVolume = _activeAudio.volume;
		}
		_slowFadeTargetVolume = targetVolume;
		_slowFadeStartTime = AudioUtil.Time;
		_slowFadeCompletionTime = AudioUtil.Time + fadeTime;
		_fadeCompleteCallback = callback;
	}

	public void PlayRandomSong()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else
		{
			PlayARandomSong(AudioPlayType.PlayNow);
		}
	}

	public void PlayARandomSong(AudioPlayType playType)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else if (_clipsRemaining.Count == 0)
		{
			Debug.LogWarning("There are no clips left in this Playlist. Turn on Loop Playlist if you want to loop the entire song selection.");
		}
		else
		{
			if (IsCrossFading && playType == AudioPlayType.Schedule)
			{
				return;
			}
			if (framesOfSongPlayed > 0)
			{
				_nextSongScheduled = false;
			}
			int num = UnityEngine.Random.Range(0, _clipsRemaining.Count);
			int index = _clipsRemaining[num];
			switch (playType)
			{
			case AudioPlayType.PlayNow:
				RemoveRandomClip(num);
				break;
			case AudioPlayType.Schedule:
				_lastRandomClipIndex = num;
				break;
			case AudioPlayType.AlreadyScheduled:
				if (_lastRandomClipIndex >= 0)
				{
					RemoveRandomClip(_lastRandomClipIndex);
				}
				break;
			}
			PlaySong(_currentPlaylist.MusicSettings[index], playType, isLoopSectionSchedule: false);
		}
	}

	private void RemoveRandomClip(int randIndex)
	{
		_clipsRemaining.RemoveAt(randIndex);
		if (loopPlaylist && _clipsRemaining.Count == 0)
		{
			FillClips();
		}
	}

	private void PlayFirstQueuedSong(AudioPlayType playType)
	{
		if (_queuedSongs.Count == 0)
		{
			Debug.LogWarning("There are zero queued songs in PlaylistController '" + ControllerName + "'. Cannot play first queued song.");
			return;
		}
		MusicSetting musicSetting = _queuedSongs[0];
		_queuedSongs.RemoveAt(0);
		_currentSequentialClipIndex = musicSetting.songIndex;
		PlaySong(musicSetting, playType, isLoopSectionSchedule: false, isQueuedSong: true);
	}

	public void PlayNextSong()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else
		{
			PlayTheNextSong(AudioPlayType.PlayNow);
		}
	}

	public void PlayTheNextSong(AudioPlayType playType)
	{
		if (_currentPlaylist == null || (IsCrossFading && playType == AudioPlayType.Schedule))
		{
			return;
		}
		if (playType != AudioPlayType.AlreadyScheduled && _songsPlayedFromPlaylist > 0 && (!_nextSongScheduled || IsCurrentSongLoopSection))
		{
			AdvanceSongCounter();
		}
		if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count)
		{
			Debug.LogWarning("There are no clips left in this Playlist. Turn on Loop Playlist if you want to loop the entire song selection.");
			return;
		}
		if (framesOfSongPlayed > 0)
		{
			_nextSongScheduled = false;
			_lastSongPosition = ActiveAudioSource.timeSamples;
		}
		PlaySong(_currentPlaylist.MusicSettings[_currentSequentialClipIndex], playType, isLoopSectionSchedule: false);
	}

	private void AdvanceSongCounter()
	{
		_currentSequentialClipIndex++;
		if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count && loopPlaylist)
		{
			_currentSequentialClipIndex = 0;
		}
	}

	public void StopPlaylistAfterCurrentSong()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return;
		}
		if (_currentPlaylist == null)
		{
			MasterAudio.LogNoPlaylist(ControllerName, "StopPlaylistAfterCurrentSong");
			return;
		}
		if (!_activeAudio.isPlaying)
		{
			Debug.Log("No song is currently playing.");
			return;
		}
		_activeAudio.loop = false;
		_queuedSongs.Clear();
		isAutoAdvance = false;
		if (_scheduledSongOffsetByAudioSource.ContainsKey(_activeAudio))
		{
			CeaseAudioSource(_activeAudio);
		}
		if (_scheduledSongOffsetByAudioSource.ContainsKey(_transitioningAudio))
		{
			CeaseAudioSource(_transitioningAudio);
		}
	}

	public void StopLoopingCurrentSong()
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return;
		}
		if (_currentPlaylist == null)
		{
			MasterAudio.LogNoPlaylist(ControllerName, "StopLoopingCurrentSong");
			return;
		}
		if (!_activeAudio.isPlaying)
		{
			Debug.Log("No song is currently playing.");
			return;
		}
		_activeAudio.loop = false;
		if (CanSchedule && _queuedSongs.Count == 0)
		{
			ScheduleNextSong();
		}
	}

	public void QueuePlaylistClip(string clipName, bool scheduleNow = true)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return;
		}
		if (_currentPlaylist == null)
		{
			MasterAudio.LogNoPlaylist(ControllerName, "QueuePlaylistClip");
			return;
		}
		if (!_activeAudio.isPlaying)
		{
			TriggerPlaylistClip(clipName);
			return;
		}
		MusicSetting musicSetting = FindSongByAliasOrName(clipName);
		if (musicSetting == null)
		{
			Debug.LogWarning("Could not find clip '" + clipName + "' in current Playlist in '" + ControllerName + "'. If you are using Addressables, try assigning an alias to the song and use the alias when specifying the song to play.");
			return;
		}
		_activeAudio.loop = false;
		if (IsCurrentSongLoopSection && !IsPlayingScheduledLoopSection)
		{
			CeaseAudioSource(_transitioningAudio);
			_loopSectionNextStartTime = null;
		}
		_queuedSongs.Add(musicSetting);
		if (CanSchedule & scheduleNow)
		{
			PlayNextOrRandom(AudioPlayType.Schedule);
		}
	}

	public bool TriggerPlaylistClip(string clipName)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return false;
		}
		if (_currentPlaylist == null)
		{
			MasterAudio.LogNoPlaylist(ControllerName, "TriggerPlaylistClip");
			return false;
		}
		MusicSetting musicSetting = FindSongByAliasOrName(clipName);
		if (musicSetting == null)
		{
			Debug.LogWarning("Could not find clip '" + clipName + "' in current Playlist in '" + ControllerName + "'.");
			return false;
		}
		_nextSongScheduled = false;
		_currentSequentialClipIndex = musicSetting.songIndex;
		PlaySong(musicSetting, AudioPlayType.PlayNow, isLoopSectionSchedule: false);
		return true;
	}

	public void EndDucking(SoundGroupVariationUpdater actorUpdater)
	{
		if (!(_actorUpdater != actorUpdater) && _duckingMode == AudioDuckingMode.Ducked)
		{
			_timeToStartUnducking = AudioUtil.Time;
			float timeToFinishUnducking = _timeToStartUnducking + _unduckTime;
			_timeToFinishUnducking = timeToFinishUnducking;
		}
	}

	public void DuckMusicForTime(SoundGroupVariationUpdater actorUpdater, float duckLength, float unduckTime, float pitch, float duckedTimePercentage, float duckedVolCut)
	{
		if (MasterAudio.IsWarming)
		{
			return;
		}
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else if (!IsCrossFading)
		{
			_actorUpdater = actorUpdater;
			float num = AudioUtil.AdjustAudioClipDurationForPitch(duckLength, pitch);
			_unduckTime = unduckTime;
			_currentDuckVolCut = duckedVolCut;
			float num2 = (_initialDuckVolume = AudioUtil.GetFloatVolumeFromDb(AudioUtil.GetDbFromFloatVolume(_originalMusicVolume) + duckedVolCut));
			_duckRange = _originalMusicVolume - num2;
			_duckingMode = AudioDuckingMode.SetToDuck;
			_timeToStartUnducking = AudioUtil.Time + num * duckedTimePercentage;
			float num3 = _timeToStartUnducking + unduckTime;
			if (num3 > AudioUtil.Time + num)
			{
				num3 = AudioUtil.Time + num;
			}
			_timeToFinishUnducking = num3;
		}
	}

	private void InitControllerIfNot()
	{
		if (!ControllerIsReady)
		{
			Awake();
			Start();
		}
	}

	public void UpdateMasterVolume()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		InitControllerIfNot();
		if (_currentSong != null)
		{
			float num = _currentSong.volume * PlaylistVolume;
			if (!IsCrossFading)
			{
				if (_activeAudio != null)
				{
					_activeAudio.volume = num;
				}
				if (_transitioningAudio != null)
				{
					_transitioningAudio.volume = num;
				}
			}
			_activeAudioEndVolume = num;
		}
		SetDuckProperties();
	}

	public void StartPlaylist(string playlistName, string clipName = null)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else if (_currentPlaylist != null && _currentPlaylist.playlistName == playlistName)
		{
			RestartPlaylist(clipName);
		}
		else
		{
			ChangePlaylist(playlistName, playFirstClip: true, clipName);
		}
	}

	public void ChangePlaylist(string playlistName, bool playFirstClip = true, string clipName = null)
	{
		InitControllerIfNot();
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
			return;
		}
		if (_currentPlaylist != null && _currentPlaylist.playlistName == playlistName)
		{
			Debug.LogWarning("The Playlist '" + playlistName + "' is already loaded. Ignoring Change Playlist request.");
			return;
		}
		startPlaylistName = playlistName;
		FinishPlaylistInit(playFirstClip, clipName);
	}

	private void FinishPlaylistInit(bool playFirstClip = true, string clipName = null)
	{
		if (IsCrossFading)
		{
			StopPlaylist(onlyFadingClip: true);
		}
		InitializePlaylist();
		if (!Application.isPlaying)
		{
			return;
		}
		_queuedSongs.Clear();
		if (!string.IsNullOrEmpty(clipName))
		{
			MusicSetting musicSetting = FindSongByAliasOrName(clipName);
			if (musicSetting != null)
			{
				_queuedSongs.Add(musicSetting);
			}
		}
		if (playFirstClip)
		{
			PlayNextOrRandom(AudioPlayType.PlayNow);
		}
	}

	public void RestartPlaylist(string clipName = null)
	{
		if (!ControllerIsReady)
		{
			Debug.LogError("Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity). Awake event is still not guaranteed to work, so use Start where possible.");
		}
		else
		{
			FinishPlaylistInit(playFirstClip: true, clipName);
		}
	}

	private void CheckIfPlaylistStarted()
	{
		if (_songsPlayedFromPlaylist <= 0 && playlistStartedEventExpanded && playlistStartedCustomEvent != string.Empty && playlistStartedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(playlistStartedCustomEvent, Trans);
		}
	}

	private PlaylistController FindOtherControllerInSameSyncGroup()
	{
		if (!WillSyncToOtherClip)
		{
			return null;
		}
		for (int i = 0; i < Instances.Count; i++)
		{
			PlaylistController playlistController = Instances[i];
			if (playlistController != this && playlistController.syncGroupNum == syncGroupNum && playlistController.ActiveAudioSource != null && playlistController.ActiveAudioSource.isPlaying && playlistController.CurrentSongIsPlaying)
			{
				return playlistController;
			}
		}
		return null;
	}

	private void FadeOutPlaylist()
	{
		if (_curFadeMode != FadeMode.GradualFade)
		{
			float volumeBeforeFade = _playlistVolume;
			FadeToVolume(0f, CrossFadeTime, () =>
			{
				StopPlaylist();
				_playlistVolume = volumeBeforeFade;
			});
		}
	}

	private void InitializePlaylist()
	{
		FillClips();
		_songsPlayedFromPlaylist = 0;
		_currentSequentialClipIndex = 0;
		_nextSongScheduled = false;
		_lastRandomClipIndex = -1;
	}

	private void PlayNextOrRandom(AudioPlayType playType)
	{
		_nextSongRequested = true;
		if (_queuedSongs.Count > 0)
		{
			PlayFirstQueuedSong(playType);
		}
		else if (!isShuffle)
		{
			PlayTheNextSong(playType);
		}
		else
		{
			PlayARandomSong(playType);
		}
	}

	private void FirePlaylistEndedEventIfAny()
	{
		if (playlistEndedEventExpanded && playlistEndedCustomEvent != string.Empty && playlistStartedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(playlistEndedCustomEvent, Trans);
		}
	}

	private void FillClips()
	{
		_clipsRemaining.Clear();
		if (startPlaylistName == "[No Playlist]")
		{
			return;
		}
		_currentPlaylist = MasterAudio.GrabPlaylist(startPlaylistName);
		if (_currentPlaylist == null)
		{
			return;
		}
		for (int i = 0; i < _currentPlaylist.MusicSettings.Count; i++)
		{
			MusicSetting musicSetting = _currentPlaylist.MusicSettings[i];
			musicSetting.songIndex = i;
			switch (musicSetting.audLocation)
			{
			case MasterAudio.AudioLocation.Clip:
				if (musicSetting.clip == null)
				{
					continue;
				}
				break;
			case MasterAudio.AudioLocation.ResourceFile:
				if (string.IsNullOrEmpty(musicSetting.resourceFileName))
				{
					continue;
				}
				break;
			case MasterAudio.AudioLocation.Addressable:
				if (!AudioAddressableOptimizer.IsAddressableValid(musicSetting.audioClipAddressable))
				{
					continue;
				}
				break;
			}
			_clipsRemaining.Add(i);
		}
	}

	private void PlaySong(MusicSetting setting, AudioPlayType playType, bool isLoopSectionSchedule, bool isQueuedSong = false)
	{
		_isPlayingQueuedSong = isQueuedSong;
		_isSongReplacingScheduledTrack = _currentSchedSongDspStartTime.HasValue && _currentSchedSongDspStartTime.Value > AudioSettings.dspTime;
		_newSongSetting = setting;
		_isLoopSectionSchedule = isLoopSectionSchedule;
		if (_activeAudio == null)
		{
			Debug.LogError("PlaylistController prefab is not in your scene. Cannot play a song.");
			return;
		}
		AudioClip audioClip = null;
		int num;
		if (playType != AudioPlayType.PlayNow)
		{
			num = ((playType == AudioPlayType.AlreadyScheduled) ? 1 : 0);
			if (num == 0)
			{
				goto IL_006d;
			}
		}
		else
		{
			num = 1;
		}
		_lastFrameSongPosition = -1;
		goto IL_006d;
		IL_006d:
		if (num != 0 && _currentSong != null && !CanSchedule && _currentSong.songChangedEventExpanded && _currentSong.songChangedCustomEvent != string.Empty && _currentSong.songChangedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(_currentSong.songChangedCustomEvent, Trans);
		}
		if (playType != AudioPlayType.AlreadyScheduled)
		{
			if (_activeAudio.clip != null)
			{
				string value = string.Empty;
				switch (setting.audLocation)
				{
				case MasterAudio.AudioLocation.Clip:
					if (setting.clip != null)
					{
						value = setting.clip.CachedName();
					}
					break;
				case MasterAudio.AudioLocation.ResourceFile:
					value = setting.resourceFileName;
					break;
				case MasterAudio.AudioLocation.Addressable:
					if (AudioAddressableOptimizer.IsAddressableValid(setting.audioClipAddressable))
					{
						value = (string.IsNullOrEmpty(setting.alias) ? "~Empty Alias Song~" : setting.alias);
					}
					break;
				}
				if (string.IsNullOrEmpty(value))
				{
					Debug.LogWarning("The next song has no clip or Resource file assigned. Please fix. Ignoring song change request.");
					return;
				}
			}
			if (_activeAudio.clip == null)
			{
				_audioClip = _activeAudio;
				_transClip = _transitioningAudio;
			}
			else if (_transitioningAudio.clip == null)
			{
				_audioClip = _transitioningAudio;
				_transClip = _activeAudio;
			}
			else
			{
				_audioClip = _transitioningAudio;
				_transClip = _activeAudio;
			}
			_audioClip.loop = SongShouldLoop(setting);
			switch (setting.audLocation)
			{
			case MasterAudio.AudioLocation.Clip:
				if (setting.clip == null)
				{
					MasterAudio.LogWarning("MasterAudio will not play empty Playlist clip for PlaylistController '" + ControllerName + "'.");
					return;
				}
				audioClip = setting.clip;
				break;
			case MasterAudio.AudioLocation.ResourceFile:
				if (_resourceCoroutine != null)
				{
					StopCoroutine(_resourceCoroutine);
				}
				_resourceCoroutine = StartCoroutine(AudioResourceOptimizer.PopulateResourceSongToPlaylistControllerAsync(setting, setting.resourceFileName, CurrentPlaylist.playlistName, this, playType));
				break;
			case MasterAudio.AudioLocation.Addressable:
				if (_addressableCoroutine != null)
				{
					StopCoroutine(_addressableCoroutine);
				}
				_addressableCoroutine = StartCoroutine(AudioAddressableOptimizer.PopulateAddressableSongToPlaylistControllerAsync(setting, setting.audioClipAddressable, this, playType));
				break;
			}
		}
		else
		{
			FinishLoadingNewSong(setting, null, AudioPlayType.AlreadyScheduled);
		}
		if (audioClip != null)
		{
			FinishLoadingNewSong(setting, audioClip, playType);
		}
	}

	public double? ScheduledGaplessNextSongStartTime()
	{
		if (!_scheduledSongOffsetByAudioSource.ContainsKey(_audioClip))
		{
			return null;
		}
		return _scheduledSongOffsetByAudioSource[_audioClip];
	}

	public void FinishLoadingNewSong(MusicSetting songSetting, AudioClip clipToPlay, AudioPlayType playType)
	{
		_nextSongRequested = false;
		bool flag = playType == AudioPlayType.Schedule;
		bool num = (playType == AudioPlayType.PlayNow) | flag;
		bool flag2 = playType == AudioPlayType.PlayNow || playType == AudioPlayType.AlreadyScheduled;
		double? num2 = null;
		if (num)
		{
			if (flag && CanSchedule)
			{
				num2 = CalculateNextTrackStartTimeOffset();
			}
			_audioClip.clip = clipToPlay;
			_audioClip.pitch = _newSongSetting.pitch;
			if (songSetting.audLocation == MasterAudio.AudioLocation.Addressable)
			{
				_loadedAddressablesByAudioSource[_audioClip] = songSetting.audioClipAddressable;
				AudioAddressableOptimizer.AddAddressablePlayingClip(songSetting.audioClipAddressable, _audioClip);
			}
			_activeSongAlias = songSetting.alias;
		}
		if (_currentSong != null)
		{
			_currentSong.lastKnownTimePoint = _activeAudio.timeSamples;
			_currentSong.wasLastKnownTimePointSet = true;
		}
		if (flag2)
		{
			if (CrossFadeTime == 0f || _transClip.clip == null)
			{
				CeaseAudioSource(_transClip);
				_audioClip.volume = _newSongSetting.volume * PlaylistVolume;
				if (!ActiveAudioSource.isPlaying && _currentPlaylist != null && _currentPlaylist.fadeInFirstSong && CrossFadeTime > 0f)
				{
					CrossFadeNow(_audioClip);
				}
			}
			else
			{
				CrossFadeNow(_audioClip);
			}
			SetDuckProperties();
		}
		bool flag3 = false;
		switch (playType)
		{
		case AudioPlayType.AlreadyScheduled:
			_nextSongScheduled = false;
			RemoveScheduledClip();
			break;
		case AudioPlayType.PlayNow:
		{
			if (_audioClip.clip != null && _audioClip.timeSamples >= _audioClip.clip.samples - 1)
			{
				_audioClip.timeSamples = 0;
			}
			PlaylistController playlistController = FindOtherControllerInSameSyncGroup();
			framesOfSongPlayed = 0;
			AudioUtil.ClipPlayed(_activeAudio.clip, GameObj);
			CheckIfPlaylistStarted();
			_songsPlayedFromPlaylist++;
			double scheduledPlayTimeOffset;
			if (playlistController != null)
			{
				int timeSamples = playlistController._activeAudio.timeSamples;
				float time = playlistController._audioClip.time;
				bool flag4 = Math.Abs(_audioClip.clip.length - time) >= AudioUtil.FrameTime * 10f;
				if ((_audioClip.clip != null && timeSamples < _audioClip.clip.samples) & flag4)
				{
					int num3 = timeSamples + (int)(0.3 * (double)_audioClip.clip.frequency);
					if (num3 >= _audioClip.clip.samples && _audioClip.loop)
					{
						num3 -= _audioClip.clip.samples;
					}
					_audioClip.timeSamples = num3;
					flag3 = true;
				}
				scheduledPlayTimeOffset = AudioSettings.dspTime + 0.3;
			}
			else
			{
				scheduledPlayTimeOffset = AudioSettings.dspTime;
			}
			ScheduleClipPlay(scheduledPlayTimeOffset, _audioClip, calledAfterPause: false, addDspTime: false);
			break;
		}
		case AudioPlayType.Schedule:
			if (!_songPauseTime.HasValue)
			{
				if (!num2.HasValue)
				{
					num2 = CalculateNextTrackStartTimeOffset();
				}
				ScheduleClipPlay(num2.Value, _audioClip, calledAfterPause: false);
				_nextSongScheduled = true;
				CheckIfPlaylistStarted();
				_songsPlayedFromPlaylist++;
			}
			break;
		}
		if (_currentPlaylist != null)
		{
			if (_songsPlayedFromPlaylist <= 1 && !flag3)
			{
				_audioClip.timeSamples = 0;
			}
			else
			{
				switch (_currentPlaylist.songTransitionType)
				{
				case MasterAudio.SongFadeInPosition.SynchronizeClips:
					if (flag3)
					{
						break;
					}
					if (playType == AudioPlayType.PlayNow)
					{
						int num4 = (_lastSongPosition.HasValue ? _lastSongPosition.Value : _activeAudio.timeSamples);
						if (_transitioningAudio.clip != null && num4 >= _transitioningAudio.clip.samples - 1)
						{
							num4 = 0;
						}
						_lastSongPosition = null;
						_transitioningAudio.timeSamples = num4;
					}
					else if (!ShouldNotSwitchEarly)
					{
						_transitioningAudio.timeSamples = 0;
					}
					break;
				case MasterAudio.SongFadeInPosition.NewClipFromLastKnownPosition:
				{
					MusicSetting musicSetting = null;
					for (int i = 0; i < _currentPlaylist.MusicSettings.Count; i++)
					{
						MusicSetting musicSetting2 = _currentPlaylist.MusicSettings[i];
						if (musicSetting2 == _newSongSetting)
						{
							musicSetting = musicSetting2;
							break;
						}
					}
					if (musicSetting != null)
					{
						int num5 = musicSetting.lastKnownTimePoint;
						if (_transitioningAudio.clip != null && num5 >= _transitioningAudio.clip.samples - 1)
						{
							num5 = 0;
						}
						_transitioningAudio.timeSamples = num5;
					}
					break;
				}
				case MasterAudio.SongFadeInPosition.NewClipFromBeginning:
					if (!ShouldNotSwitchEarly)
					{
						_audioClip.timeSamples = 0;
					}
					break;
				}
			}
			bool flag5 = _currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.NewClipFromLastKnownPosition && !_newSongSetting.wasLastKnownTimePointSet;
			if ((_currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.NewClipFromBeginning) | flag5)
			{
				float songStartTime = _newSongSetting.SongStartTime;
				if (songStartTime > 0f)
				{
					_audioClip.timeSamples = (int)(songStartTime * (float)_audioClip.clip.frequency);
				}
			}
		}
		if (flag)
		{
			UpdateMasterVolume();
		}
		if (flag2)
		{
			_activeAudio = _audioClip;
			_transitioningAudio = _transClip;
			if (songChangedCustomEvent != string.Empty && songChangedEventExpanded && songChangedCustomEvent != "[None]")
			{
				MasterAudio.FireCustomEvent(songChangedCustomEvent, Trans);
			}
			if (SongChanged != null)
			{
				string newSongName = string.Empty;
				if (_audioClip != null)
				{
					newSongName = _audioClip.clip.CachedName();
				}
				SongChanged(newSongName, _newSongSetting);
			}
		}
		_activeAudioEndVolume = _newSongSetting.volume * PlaylistVolume;
		float volume = _transitioningAudio.volume;
		if (_currentSong != null)
		{
			volume = _currentSong.volume;
		}
		_transitioningAudioStartVolume = volume * PlaylistVolume;
		_currentSong = _newSongSetting;
		if (flag2 && _currentSong.songStartedEventExpanded && _currentSong.songStartedCustomEvent != string.Empty && _currentSong.songStartedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(_currentSong.songStartedCustomEvent, Trans);
		}
		bool flag6 = playType != AudioPlayType.Schedule;
		if (flag6 && CanSchedule && !_currentSong.isLoop)
		{
			ScheduleNextSong();
		}
		else if (!IsCrossFading && (!_isLoopSectionSchedule & flag6) && IsCurrentSongLoopSection)
		{
			PlaySong(_currentSong, AudioPlayType.Schedule, isLoopSectionSchedule: true);
		}
	}

	private void RemoveScheduledClip()
	{
		if (_audioClip != null)
		{
			_scheduledSongOffsetByAudioSource.Remove(_audioClip);
		}
	}

	private void ScheduleNextSong()
	{
		PlayNextOrRandom(AudioPlayType.Schedule);
	}

	private void FadeInScheduledSong()
	{
		PlayNextOrRandom(AudioPlayType.AlreadyScheduled);
	}

	private double CalculateNextTrackStartTimeOffset()
	{
		PlaylistController playlistController = FindOtherControllerInSameSyncGroup();
		if (playlistController != null)
		{
			double? num = playlistController.ScheduledGaplessNextSongStartTime();
			if (num.HasValue)
			{
				return num.Value;
			}
		}
		return GetClipDuration(_activeAudio);
	}

	private double GetClipDuration(AudioSource src)
	{
		float num = src.time;
		float num2 = src.clip.length;
		float num3 = CrossFadeTime;
		switch (_newSongSetting.songStartTimeMode)
		{
		case MasterAudio.CustomSongStartTimeMode.Section:
			num2 = _newSongSetting.sectionEndTime;
			num = _newSongSetting.sectionStartTime;
			num3 = 0f;
			break;
		case MasterAudio.CustomSongStartTimeMode.SpecificTime:
			num = _newSongSetting.customStartTime;
			break;
		}
		return AudioUtil.AdjustAudioClipDurationForPitch(num2 - num, src) - num3;
	}

	private double? GetLatestEndTime(AudioSource src)
	{
		double? result = null;
		foreach (AudioSource key in _scheduledSongEndTimeByAudioSource.Keys)
		{
			double num = _scheduledSongEndTimeByAudioSource[key];
			if (!result.HasValue || result.Value < num)
			{
				result = num;
			}
		}
		return result;
	}

	private void ScheduleClipPlay(double scheduledPlayTimeOffset, AudioSource source, bool calledAfterPause, bool addDspTime = true)
	{
		double? num = null;
		if (_newSongSetting.songStartTimeMode == MasterAudio.CustomSongStartTimeMode.Section)
		{
			double? latestEndTime = GetLatestEndTime(source);
			if (latestEndTime.HasValue)
			{
				num = latestEndTime.Value;
			}
		}
		if (!num.HasValue)
		{
			num = (addDspTime ? (AudioSettings.dspTime + scheduledPlayTimeOffset) : scheduledPlayTimeOffset);
		}
		if (ShouldNotSwitchEarly && _currentSchedSongDspEndTime.HasValue && !calledAfterPause)
		{
			num = ((!_isSongReplacingScheduledTrack) ? new double?(_currentSchedSongDspEndTime.Value) : ((!_isPlayingQueuedSong) ? new double?(AudioSettings.dspTime) : new double?(_currentSchedSongDspStartTime.Value)));
			scheduledPlayTimeOffset = num.Value - AudioSettings.dspTime;
		}
		if (num.Value < AudioSettings.dspTime)
		{
			num = AudioSettings.dspTime - (double)Time.deltaTime;
		}
		source.PlayScheduled(num.Value);
		if (_isLoopSectionSchedule)
		{
			_loopSectionNextStartTime = num;
		}
		else
		{
			_loopSectionNextStartTime = null;
		}
		_currentSchedSongDspStartTime = num;
		_currentSchedSongDspEndTime = num + GetClipDuration(source);
		if (_newSongSetting.songStartTimeMode == MasterAudio.CustomSongStartTimeMode.Section)
		{
			_scheduledSongStartTimeByAudioSource[source] = num.Value;
			_scheduledSongEndTimeByAudioSource[source] = _currentSchedSongDspEndTime.Value;
			source.SetScheduledEndTime(_currentSchedSongDspEndTime.Value);
		}
		RemoveScheduledClip();
		_scheduledSongOffsetByAudioSource.Add(source, scheduledPlayTimeOffset);
		_nextScheduledTimeRecalcStart = null;
		_nextScheduleTimeRecalcDifferentFirstFrameNum = 0;
	}

	private void CrossFadeNow(AudioSource audioClip)
	{
		audioClip.volume = 0f;
		IsCrossFading = true;
		ResetDuckingState();
		_crossFadeStartTime = AudioUtil.Time;
		if (crossfadeStartedExpanded && crossfadeStartedCustomEvent != string.Empty && crossfadeStartedCustomEvent != "[None]")
		{
			MasterAudio.FireCustomEvent(crossfadeStartedCustomEvent, Trans, logDupe: false);
		}
	}

	private void CeaseAudioSource(AudioSource source)
	{
		if (source == null)
		{
			return;
		}
		if (source == _activeAudio)
		{
			framesOfSongPlayed = 0;
			_activeSongAlias = null;
		}
		bool num = source.clip != null;
		source.Stop();
		source.timeSamples = 0;
		if (_transClip == null || _transClip.clip != source.clip)
		{
			AudioUtil.UnloadNonPreloadedAudioData(source.clip, GameObj);
		}
		_scheduledSongEndTimeByAudioSource.Remove(source);
		_scheduledSongStartTimeByAudioSource.Remove(source);
		AudioResourceOptimizer.UnloadPlaylistSongIfUnused(ControllerName, source.clip);
		string text = ((source.clip == null) ? string.Empty : source.clip.CachedName());
		source.clip = null;
		if (_loadedAddressablesByAudioSource.ContainsKey(source))
		{
			AssetReference addressable = _loadedAddressablesByAudioSource[source];
			_loadedAddressablesByAudioSource.Remove(source);
			AudioAddressableOptimizer.RemoveAddressablePlayingClip(addressable, source);
		}
		RemoveScheduledClip();
		if (!num)
		{
			return;
		}
		bool flag = songEndedEventExpanded && songEndedCustomEvent != string.Empty && songEndedCustomEvent != "[None]";
		bool flag2 = SongEnded != null;
		if ((flag || flag2) && !string.IsNullOrEmpty(text))
		{
			if (flag)
			{
				MasterAudio.FireCustomEvent(songEndedCustomEvent, Trans, logDupe: false);
			}
			if (flag2)
			{
				SongEnded(text);
			}
		}
	}

	private void SetDuckProperties()
	{
		_originalMusicVolume = ((_activeAudio == null) ? 1f : _activeAudio.volume);
		if (_currentSong != null)
		{
			_originalMusicVolume = _currentSong.volume * PlaylistVolume;
		}
		float floatVolumeFromDb = AudioUtil.GetFloatVolumeFromDb(AudioUtil.GetDbFromFloatVolume(_originalMusicVolume) - _currentDuckVolCut);
		_duckRange = _originalMusicVolume - floatVolumeFromDb;
		_initialDuckVolume = floatVolumeFromDb;
		ResetDuckingState();
	}

	private void AudioDucking()
	{
		switch (_duckingMode)
		{
		case AudioDuckingMode.SetToDuck:
			_activeAudio.volume = _initialDuckVolume;
			_duckingMode = AudioDuckingMode.Ducked;
			break;
		case AudioDuckingMode.Ducked:
			if (Time.realtimeSinceStartup >= _timeToStartUnducking)
			{
				_duckingMode = AudioDuckingMode.Unducking;
			}
			else if (Time.realtimeSinceStartup >= _timeToFinishUnducking)
			{
				_activeAudio.volume = _originalMusicVolume;
				ResetDuckingState();
			}
			break;
		case AudioDuckingMode.Unducking:
			_activeAudio.volume = _initialDuckVolume + (Time.realtimeSinceStartup - _timeToStartUnducking) / (_timeToFinishUnducking - _timeToStartUnducking) * _duckRange;
			if (Time.realtimeSinceStartup >= _timeToFinishUnducking)
			{
				_activeAudio.volume = _originalMusicVolume;
				ResetDuckingState();
			}
			break;
		case AudioDuckingMode.NotDucking:
			break;
		}
	}

	private void ResetDuckingState()
	{
		_duckingMode = AudioDuckingMode.NotDucking;
		_actorUpdater = null;
	}

	private bool SongShouldLoop(MusicSetting setting)
	{
		if (_queuedSongs.Count > 0)
		{
			return false;
		}
		if (SongIsNonAdvancible)
		{
			return true;
		}
		if (setting.songStartTimeMode == MasterAudio.CustomSongStartTimeMode.Section)
		{
			return false;
		}
		return setting.isLoop;
	}

	public void RouteToMixerChannel(AudioMixerGroup group)
	{
		_activeAudio.outputAudioMixerGroup = group;
		_transitioningAudio.outputAudioMixerGroup = group;
	}
}
