using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio;

[AudioScriptOrder(-50)]
public class MasterAudio : MonoBehaviour
{
	public enum JukeBoxDisplayMode
	{
		DisplayAll = 0,
		DisplayActive = 1
	}

	public enum BusVoiceLimitExceededMode
	{
		DoNotPlayNewSound = 0,
		StopOldestSound = 1,
		StopFarthestSound = 2,
		StopLeastImportantSound = 3
	}

	public enum AmbientSoundExitMode
	{
		StopSound = 0,
		FadeSound = 1
	}

	public enum AmbientSoundReEnterMode
	{
		StopExistingSound = 0,
		FadeInSameSound = 1
	}

	public enum VariationFollowerType
	{
		LateUpdate = 0,
		FixedUpdate = 1
	}

	public enum LinkedGroupSelectionType
	{
		All = 0,
		OneAtRandom = 1
	}

	public enum OcclusionSelectionType
	{
		AllGroups = 0,
		TurnOnPerBusOrGroup = 1
	}

	public enum RaycastMode
	{
		Physics3D = 0,
		Physics2D = 1
	}

	public enum AllMusicSpatialBlendType
	{
		ForceAllTo2D = 0,
		ForceAllTo3D = 1,
		ForceAllToCustom = 2,
		AllowDifferentPerController = 3
	}

	public enum AllMixerSpatialBlendType
	{
		ForceAllTo2D = 0,
		ForceAllTo3D = 1,
		ForceAllToCustom = 2,
		AllowDifferentPerGroup = 3
	}

	public enum ItemSpatialBlendType
	{
		ForceTo2D = 0,
		ForceTo3D = 1,
		ForceToCustom = 2,
		UseCurveFromAudioSource = 3
	}

	public enum GroupPlayType
	{
		Always = 0,
		WhenActorInAudibleRange = 1,
		AllowDifferentPerGroup = 2
	}

	public enum DefaultGroupPlayType
	{
		Always = 0,
		WhenActorInAudibleRange = 1
	}

	public enum MixerWidthMode
	{
		Narrow = 0,
		Normal = 1,
		Wide = 2
	}

	public enum CustomEventReceiveMode
	{
		Always = 0,
		WhenDistanceLessThan = 1,
		WhenDistanceMoreThan = 2,
		Never = 3,
		OnSameGameObject = 4,
		OnChildGameObject = 5,
		OnParentGameObject = 6,
		OnSameOrChildGameObject = 7,
		OnSameOrParentGameObject = 8
	}

	public enum EventReceiveFilter
	{
		All = 0,
		Closest = 1,
		Random = 2
	}

	public enum VariationLoadStatus
	{
		None = 0,
		Loading = 1,
		Loaded = 2,
		LoadFailed = 3
	}

	public enum AudioLocation
	{
		Clip = 0,
		ResourceFile = 1,
		Addressable = 2
	}

	public enum CustomSongStartTimeMode
	{
		Beginning = 0,
		SpecificTime = 1,
		RandomTime = 2,
		Section = 3
	}

	public enum BusCommand
	{
		None = 0,
		FadeToVolume = 1,
		Mute = 2,
		Pause = 3,
		Solo = 4,
		Unmute = 5,
		Unpause = 6,
		Unsolo = 7,
		Stop = 8,
		ChangePitch = 9,
		ToggleMute = 10,
		StopBusOfTransform = 11,
		PauseBusOfTransform = 12,
		UnpauseBusOfTransform = 13,
		GlideByPitch = 14,
		StopOldBusVoices = 15,
		FadeOutOldBusVoices = 16
	}

	public enum DragGroupMode
	{
		OneGroupPerClip = 0,
		OneGroupWithVariations = 1
	}

	public enum EventSoundFunctionType
	{
		PlaySound = 0,
		GroupControl = 1,
		BusControl = 2,
		PlaylistControl = 3,
		CustomEventControl = 4,
		GlobalControl = 5,
		UnityMixerControl = 6,
		PersistentSettingsControl = 7
	}

	public enum LanguageMode
	{
		UseDeviceSetting = 0,
		SpecificLanguage = 1,
		DynamicallySet = 2
	}

	public enum UnityMixerCommand
	{
		None = 0,
		TransitionToSnapshot = 1,
		TransitionToSnapshotBlend = 2
	}

	public enum PlaylistCommand
	{
		None = 0,
		ChangePlaylist = 1,
		FadeToVolume = 2,
		PlaySong = 3,
		PlayRandomSong = 4,
		PlayNextSong = 5,
		Pause = 6,
		Resume = 7,
		Stop = 8,
		Mute = 9,
		Unmute = 10,
		ToggleMute = 11,
		Restart = 12,
		Start = 13,
		StopLoopingCurrentSong = 14,
		StopPlaylistAfterCurrentSong = 15,
		AddSongToQueue = 16
	}

	public enum CustomEventCommand
	{
		None = 0,
		FireEvent = 1
	}

	public enum GlobalCommand
	{
		None = 0,
		PauseMixer = 1,
		UnpauseMixer = 2,
		StopMixer = 3,
		StopEverything = 4,
		PauseEverything = 5,
		UnpauseEverything = 6,
		MuteEverything = 7,
		UnmuteEverything = 8,
		SetMasterMixerVolume = 9,
		SetMasterPlaylistVolume = 10,
		PauseAudioListener = 11,
		UnpauseAudioListener = 12
	}

	public enum SoundGroupCommand
	{
		None = 0,
		FadeToVolume = 1,
		FadeOutAllOfSound = 2,
		Mute = 3,
		Pause = 4,
		Solo = 5,
		StopAllOfSound = 6,
		Unmute = 7,
		Unpause = 8,
		Unsolo = 9,
		StopAllSoundsOfTransform = 10,
		PauseAllSoundsOfTransform = 11,
		UnpauseAllSoundsOfTransform = 12,
		StopSoundGroupOfTransform = 13,
		PauseSoundGroupOfTransform = 14,
		UnpauseSoundGroupOfTransform = 15,
		FadeOutSoundGroupOfTransform = 16,
		RefillSoundGroupPool = 17,
		RouteToBus = 18,
		GlideByPitch = 19,
		ToggleSoundGroup = 20,
		ToggleSoundGroupOfTransform = 21,
		FadeOutAllSoundsOfTransform = 22,
		StopOldSoundGroupVoices = 23,
		FadeOutOldSoundGroupVoices = 24
	}

	public enum PersistentSettingsCommand
	{
		None = 0,
		SetBusVolume = 1,
		SetGroupVolume = 2,
		SetMixerVolume = 3,
		SetMusicVolume = 4,
		MixerMuteToggle = 5,
		MusicMuteToggle = 6
	}

	public enum SongFadeInPosition
	{
		NewClipFromBeginning = 1,
		NewClipFromLastKnownPosition = 3,
		SynchronizeClips = 5
	}

	public enum SoundSpawnLocationMode
	{
		MasterAudioLocation = 0,
		CallerLocation = 1,
		AttachToCaller = 2
	}

	public enum VariationCommand
	{
		None = 0,
		Stop = 1,
		Pause = 2,
		Unpause = 3
	}

	[Serializable]
	public struct CustomEventCandidate(float distance, ICustomEventReceiver rec, Transform trans, int randomId)
	{
		public float DistanceAway = distance;

		public ICustomEventReceiver Receiver = rec;

		public Transform Trans = trans;

		public int RandomId = randomId;
	}

	[Serializable]
	public class AudioGroupInfo
	{
		public List<AudioInfo> Sources;

		public int LastFramePlayed;

		public float LastTimePlayed;

		public MasterAudioGroup Group;

		public bool PlayedForWarming;

		public AudioGroupInfo(List<AudioInfo> sources, MasterAudioGroup groupScript)
		{
			Sources = sources;
			LastFramePlayed = -50;
			LastTimePlayed = -50f;
			Group = groupScript;
			PlayedForWarming = false;
		}
	}

	[Serializable]
	public class AudioInfo
	{
		public AudioSource Source;

		public float OriginalVolume;

		public float LastPercentageVolume;

		public float LastRandomVolume;

		public SoundGroupVariation Variation;

		public AudioInfo(SoundGroupVariation variation, AudioSource source, float origVol)
		{
			Variation = variation;
			Source = source;
			OriginalVolume = origVol;
			LastPercentageVolume = 1f;
			LastRandomVolume = 0f;
		}
	}

	[Serializable]
	public class Playlist
	{
		public enum CrossfadeTimeMode
		{
			UseMasterSetting = 0,
			Override = 1
		}

		public bool isExpanded = true;

		public string playlistName = "new playlist";

		public SongFadeInPosition songTransitionType = SongFadeInPosition.NewClipFromBeginning;

		public List<MusicSetting> MusicSettings;

		public AudioLocation bulkLocationMode;

		public CrossfadeTimeMode crossfadeMode;

		public float crossFadeTime = 1f;

		public bool fadeInFirstSong;

		public bool fadeOutLastSong;

		public bool bulkEditMode;

		public bool isTemporary;

		public bool showMetadata;

		public List<SongMetadataProperty> songMetadataProps = new List<SongMetadataProperty>();

		public string newMetadataPropName = "PropertyName";

		public SongMetadataProperty.MetadataPropertyType newMetadataPropType = SongMetadataProperty.MetadataPropertyType.String;

		public bool newMetadataPropRequired = true;

		public bool newMetadataPropCanHaveMult;

		private readonly List<int> _actorInstanceIds = new List<int>();

		public bool HasLiveActors => _actorInstanceIds.Count > 0;

		public Playlist()
		{
			MusicSettings = new List<MusicSetting>();
		}

		public void AddActorInstanceId(int instanceId)
		{
			if (!_actorInstanceIds.Contains(instanceId))
			{
				_actorInstanceIds.Add(instanceId);
			}
		}

		public void RemoveActorInstanceId(int instanceId)
		{
			_actorInstanceIds.Remove(instanceId);
		}
	}

	[Serializable]
	public class SoundGroupRefillInfo
	{
		public float LastTimePlayed;

		public float InactivePeriodSeconds;

		public SoundGroupRefillInfo(float lastTimePlayed, float inactivePeriodSeconds)
		{
			LastTimePlayed = lastTimePlayed;
			InactivePeriodSeconds = inactivePeriodSeconds;
		}
	}

	public const string VideoPlayersSoundGroupSelectedError = "Can't use specially named Sound Group for Video Players. Please select another.";

	public const string VideoPlayerSoundGroupName = "_VideoPlayers";

	public const string VideoPlayerBusName = "_VideoPlayers";

	public const string MasterAudioDefaultFolder = "Assets/Plugins/DarkTonic/MasterAudio";

	public const string PreviewText = "Random delay, custom fading & start/end position settings are ignored by preview in edit mode.";

	public const string LoopDisabledLoopedChain = "Loop Clip is always OFF for Looped Chain Groups";

	public const string LoopDisabledCustomEnd = "Loop Clip is always OFF when using Custom End Position";

	public const string DragAudioTip = "Drag Audio clips or a folder containing some here";

	public const string NoCategory = "[Uncategorized]";

	public const float SemiTonePitchFactor = 1.05946f;

	public const float SpatialBlend_2DValue = 0f;

	public const float SpatialBlend_3DValue = 1f;

	public const float MaxCrossFadeTimeSeconds = 120f;

	public const float DefaultDuckVolCut = -6f;

	public const int ERROR_MA_LAYER_COLLISIONS_DISABLED = 1;

	public const int PHYSICS_DISABLED = 2;

	public const string StoredLanguageNameKey = "~MA_Language_Key~";

	public static readonly YieldInstruction EndOfFrameDelay = new WaitForEndOfFrame();

	public static readonly List<string> ExemptChildNames = new List<string> { "_Followers" };

	public static readonly HashSet<int> ErrorNumbersLogged = new HashSet<int>();

	public static List<string> ImportanceChoices = new List<string>
	{
		"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"10"
	};

	public static List<string> AddressableDeadIds = new List<string>();

	public static Action NumberOfAudioSourcesChanged;

	public const int HardCodedBusOptions = 2;

	public const string AllBusesName = "[All]";

	public const string NoGroupName = "[None]";

	public const string DynamicGroupName = "[Type In]";

	public const string NoPlaylistName = "[No Playlist]";

	public const string NoVoiceLimitName = "[NO LMT]";

	public const string OnlyPlaylistControllerName = "~only~";

	public const float InnerLoopCheckInterval = 0.1f;

	private const int MaxComponents = 20;

	public AudioLocation bulkLocationMode;

	public string groupTemplateName = "Default Single";

	public string audioSourceTemplateName = "Max Distance 500";

	public bool showGroupCreation = true;

	public bool useGroupTemplates;

	public DragGroupMode curDragGroupMode;

	public List<GameObject> groupTemplates = new List<GameObject>(10);

	public List<GameObject> audioSourceTemplates = new List<GameObject>(10);

	public bool mixerMuted;

	public bool playlistsMuted;

	public LanguageMode langMode;

	public SystemLanguage testLanguage = SystemLanguage.English;

	public SystemLanguage defaultLanguage = SystemLanguage.English;

	public List<SystemLanguage> supportedLanguages = new List<SystemLanguage> { SystemLanguage.English };

	public string busFilter = string.Empty;

	public bool useTextGroupFilter;

	public string textGroupFilter = string.Empty;

	public bool resourceClipsPauseDoNotUnload;

	public Transform playlistControllerPrefab;

	public bool persistBetweenScenes;

	public bool shouldLogDestroys;

	public bool showBusColors;

	public bool showGroupImportance;

	public bool areGroupsExpanded = true;

	public Transform soundGroupTemplate;

	public Transform soundGroupVariationTemplate;

	public List<GroupBus> groupBuses = new List<GroupBus>();

	public bool groupByBus = true;

	public bool sortAlpha = true;

	public bool showRangeSoundGizmos = true;

	public bool showSelectedRangeSoundGizmos = true;

	public Color rangeGizmoColor = Color.green;

	public Color selectedRangeGizmoColor = Color.cyan;

	public bool showAdvancedSettings = true;

	public bool showLocalization = true;

	public bool showVideoPlayerSettings;

	public bool useTextPlaylistFilter;

	public string textPlaylistFilter = string.Empty;

	public bool playListExpanded = true;

	public bool playlistsExpanded = true;

	public AllMusicSpatialBlendType musicSpatialBlendType;

	public float musicSpatialBlend;

	public AllMixerSpatialBlendType mixerSpatialBlendType = AllMixerSpatialBlendType.ForceAllTo3D;

	public float mixerSpatialBlend = 1f;

	public GroupPlayType groupPlayType;

	public DefaultGroupPlayType defaultGroupPlayType;

	public ItemSpatialBlendType newGroupSpatialType = ItemSpatialBlendType.ForceTo3D;

	public float newGroupSpatialBlend = 1f;

	public List<Playlist> musicPlaylists = new List<Playlist>
	{
		new Playlist()
	};

	public float _masterAudioVolume = 1f;

	public bool vrSettingsExpanded;

	public bool useSpatializer;

	public bool useSpatializerPostFX;

	public bool addOculusAudioSources;

	public bool addResonanceAudioSources;

	public bool ignoreTimeScale;

	public bool useGaplessPlaylists;

	public bool useGaplessAutoReschedule;

	public bool saveRuntimeChanges;

	public bool prioritizeOnDistance;

	public int rePrioritizeEverySecIndex = 1;

	public bool useOcclusion;

	public float occlusionMaxCutoffFreq;

	public float occlusionMinCutoffFreq = 22000f;

	public float occlusionFreqChangeSeconds;

	public OcclusionSelectionType occlusionSelectType;

	public int occlusionMaxRayCastsPerFrame = 4;

	public float occlusionRayCastOffset;

	public bool occlusionUseLayerMask;

	public LayerMask occlusionLayerMask;

	public bool occlusionShowRaycasts = true;

	public bool occlusionShowCategories;

	public RaycastMode occlusionRaycastMode;

	public bool occlusionIncludeStartRaycast2DCollider = true;

	public bool occlusionRaycastsHitTriggers = true;

	public bool ambientAdvancedExpanded;

	public int ambientMaxRecalcsPerFrame = 4;

	public bool visualAdvancedExpanded = true;

	public bool logAdvancedExpanded = true;

	public bool listenerAdvancedExpanded;

	public bool listenerFollowerHasRigidBody = true;

	public bool deletePreviewerAudioSourceWhenPlaying = true;

	public VariationFollowerType variationFollowerType;

	public bool showFadingSettings;

	public bool stopZeroVolumeGroups;

	public bool stopZeroVolumeBuses;

	public bool stopZeroVolumePlaylists;

	public float stopOldestBusFadeTime = 0.3f;

	public bool resourceAdvancedExpanded = true;

	public bool useClipAgePriority;

	public bool logOutOfVoices = true;

	public bool LogSounds;

	public bool logCustomEvents;

	public bool disableLogging;

	public bool showMusicDucking;

	public bool enableMusicDucking = true;

	public List<DuckGroupInfo> musicDuckingSounds = new List<DuckGroupInfo>();

	public float defaultRiseVolStart = 0.5f;

	public float defaultUnduckTime = 1f;

	public float defaultDuckedVolumeCut = -6f;

	public float crossFadeTime = 1f;

	public float _masterPlaylistVolume = 1f;

	public bool showGroupSelect;

	public bool hideGroupsWithNoActiveVars;

	public JukeBoxDisplayMode jukeBoxDisplayMode;

	public bool logPerfExpanded = true;

	public bool willWarm = true;

	public bool mixerSettingsExpanded;

	public AudioMixerUpdateMode mixerUpdateMode = AudioMixerUpdateMode.UnscaledTime;

	public string newEventName = "my event";

	public bool showCustomEvents = true;

	public string newCustomEventCategoryName = "New Category";

	public string addToCustomEventCategoryName = "New Category";

	public List<CustomEvent> customEvents = new List<CustomEvent>();

	public List<CustomEventCategory> customEventCategories = new List<CustomEventCategory>
	{
		new CustomEventCategory()
	};

	public Dictionary<string, DuckGroupInfo> duckingBySoundType = new Dictionary<string, DuckGroupInfo>(StringComparer.OrdinalIgnoreCase);

	public int frames;

	public bool showUnityMixerGroupAssignment = true;

	public static readonly PlaySoundResult AndForgetSuccessResult = new PlaySoundResult
	{
		SoundPlayed = true
	};

	private static readonly PlaySoundResult failedResultDuringInit = new PlaySoundResult
	{
		SoundPlayed = false
	};

	private readonly Dictionary<string, AudioGroupInfo> AudioSourcesBySoundType = new Dictionary<string, AudioGroupInfo>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, List<int>> _randomizer = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, List<int>> _randomizerOrigin = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, List<int>> _randomizerLeftovers = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, List<int>> _nonRandomChoices = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<string, List<int>> _clipsPlayedBySoundTypeOldestFirst = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

	private readonly List<SoundGroupVariationUpdater> ActiveVariationUpdaters = new List<SoundGroupVariationUpdater>(32);

	private readonly List<SoundGroupVariationUpdater> ActiveUpdatersToRemove = new List<SoundGroupVariationUpdater>();

	private readonly List<ICustomEventReceiver> ValidReceivers = new List<ICustomEventReceiver>();

	private readonly List<CustomEventCandidate> ValidReceiverCandidates = new List<CustomEventCandidate>(10);

	private readonly List<MasterAudioGroup> SoloedGroups = new List<MasterAudioGroup>();

	private readonly List<AmbientSoundToTriggerInfo> AmbientsToDelayedTrigger = new List<AmbientSoundToTriggerInfo>();

	private readonly Queue<CustomEventToFireInfo> CustomEventsToFire = new Queue<CustomEventToFireInfo>(32);

	private readonly Queue<TransformFollower> TransFollowerColliderPositionRecalcs = new Queue<TransformFollower>(32);

	private readonly List<TransformFollower> ProcessedColliderPositionRecalcs = new List<TransformFollower>(32);

	private readonly List<BusFadeInfo> BusFades = new List<BusFadeInfo>(2);

	private readonly List<GroupFadeInfo> GroupFades = new List<GroupFadeInfo>();

	private readonly List<GroupPitchGlideInfo> GroupPitchGlides = new List<GroupPitchGlideInfo>();

	private readonly List<BusPitchGlideInfo> BusPitchGlides = new List<BusPitchGlideInfo>();

	private readonly List<OcclusionFreqChangeInfo> VariationOcclusionFreqChanges = new List<OcclusionFreqChangeInfo>();

	private readonly List<AudioSource> AllAudioSources = new List<AudioSource>();

	private readonly Dictionary<string, Dictionary<ICustomEventReceiver, Transform>> ReceiversByEventName = new Dictionary<string, Dictionary<ICustomEventReceiver, Transform>>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, PlaylistController> PlaylistControllersByName = new Dictionary<string, PlaylistController>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, SoundGroupRefillInfo> LastTimeSoundGroupPlayed = new Dictionary<string, SoundGroupRefillInfo>(StringComparer.OrdinalIgnoreCase);

	private readonly List<GameObject> OcclusionSourcesInRange = new List<GameObject>(32);

	private readonly List<GameObject> OcclusionSourcesOutOfRange = new List<GameObject>(32);

	private readonly List<GameObject> OcclusionSourcesBlocked = new List<GameObject>(32);

	private readonly Queue<SoundGroupVariationUpdater> QueuedOcclusionRays = new Queue<SoundGroupVariationUpdater>(32);

	private readonly List<AddressableDelayedRelease> AddressablesToReleaseLater = new List<AddressableDelayedRelease>();

	private readonly List<string> AllSoundGroupNames = new List<string>(32);

	private readonly List<string> AllBusNames = new List<string>(32);

	private readonly List<AudioInfo> GroupsToDelete = new List<AudioInfo>();

	private readonly List<SoundGroupVariation> VariationsStartedDuringMultiStop = new List<SoundGroupVariation>(16);

	private readonly List<PlaylistController> ControllersToPause = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToUnpause = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToMute = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToUnmute = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToToggleMute = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToStop = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToFade = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToTrigNext = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToTrigRandom = new List<PlaylistController>();

	private readonly List<PlaylistController> ControllersToStart = new List<PlaylistController>();

	private readonly List<AmbientSoundToTriggerInfo> AmbientsToTriggerNow = new List<AmbientSoundToTriggerInfo>();

	private bool _isStoppingMultiple;

	private float _repriTime = -1f;

	private List<string> _groupsToRemove;

	private bool _mustRescanGroups;

	private Transform _trans;

	private bool _soundsLoaded;

	private bool _warming;

	private static MasterAudio _instance;

	private static string _prospectiveMAFolder = string.Empty;

	private static Transform _listenerTrans;

	public static readonly List<SoundGroupCommand> GroupCommandsWithNoGroupSelector = new List<SoundGroupCommand>
	{
		SoundGroupCommand.None,
		SoundGroupCommand.PauseAllSoundsOfTransform,
		SoundGroupCommand.StopAllSoundsOfTransform,
		SoundGroupCommand.UnpauseAllSoundsOfTransform,
		SoundGroupCommand.FadeOutAllSoundsOfTransform
	};

	public static readonly List<SoundGroupCommand> GroupCommandsWithNoAllGroupSelector = new List<SoundGroupCommand>
	{
		SoundGroupCommand.None,
		SoundGroupCommand.FadeOutSoundGroupOfTransform,
		SoundGroupCommand.PauseSoundGroupOfTransform,
		SoundGroupCommand.UnpauseSoundGroupOfTransform,
		SoundGroupCommand.StopSoundGroupOfTransform,
		SoundGroupCommand.ToggleSoundGroupOfTransform,
		SoundGroupCommand.ToggleSoundGroup,
		SoundGroupCommand.FadeOutAllSoundsOfTransform
	};

	public static float PlaylistMasterVolume
	{
		get
		{
			return Instance._masterPlaylistVolume;
		}
		set
		{
			Instance._masterPlaylistVolume = value;
			List<PlaylistController> instances = PlaylistController.Instances;
			for (int i = 0; i < instances.Count; i++)
			{
				instances[i].UpdateMasterVolume();
			}
		}
	}

	public static bool LogSoundsEnabled
	{
		get
		{
			return Instance.LogSounds;
		}
		set
		{
			Instance.LogSounds = value;
		}
	}

	public static bool LogOutOfVoices
	{
		get
		{
			return Instance.logOutOfVoices;
		}
		set
		{
			Instance.logOutOfVoices = value;
		}
	}

	public static Transform VideoPlayerSoundGroupTransform => Instance.transform.Find("_VideoPlayers");

	public static List<AudioSource> MasterAudioSources => Instance.AllAudioSources;

	public static Transform ListenerTrans
	{
		get
		{
			if (_listenerTrans == null || !DTMonoHelper.IsActive(_listenerTrans.gameObject))
			{
				_listenerTrans = null;
				AudioListener[] array = UnityEngine.Object.FindObjectsOfType<AudioListener>();
				foreach (AudioListener audioListener in array)
				{
					if (DTMonoHelper.IsActive(audioListener.gameObject))
					{
						_listenerTrans = audioListener.transform;
					}
				}
			}
			return _listenerTrans;
		}
	}

	public static PlaylistController OnlyPlaylistController
	{
		get
		{
			List<PlaylistController> instances = PlaylistController.Instances;
			if (instances.Count != 0)
			{
				return instances[0];
			}
			Debug.LogError("There are no Playlist Controller in this Scene.");
			return null;
		}
	}

	public static bool IsWarming
	{
		get
		{
			if (SafeInstance != null)
			{
				return Instance._warming;
			}
			return false;
		}
	}

	public static bool MixerMuted
	{
		get
		{
			return Instance.mixerMuted;
		}
		set
		{
			Instance.mixerMuted = value;
			if (value)
			{
				foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
				{
					MuteGroup(Instance.AudioSourcesBySoundType[allSoundGroupName].Group.GameObjectName, shouldCheckMuteStatus: false);
				}
			}
			else
			{
				foreach (string allSoundGroupName2 in Instance.AllSoundGroupNames)
				{
					UnmuteGroup(Instance.AudioSourcesBySoundType[allSoundGroupName2].Group.GameObjectName, shouldCheckMuteStatus: false);
				}
			}
			if (Application.isPlaying)
			{
				SilenceOrUnsilenceGroupsFromSoloChange();
			}
		}
	}

	public static bool PlaylistsMuted
	{
		get
		{
			return Instance.playlistsMuted;
		}
		set
		{
			Instance.playlistsMuted = value;
			List<PlaylistController> instances = PlaylistController.Instances;
			for (int i = 0; i < instances.Count; i++)
			{
				if (value)
				{
					instances[i].MutePlaylist();
				}
				else
				{
					instances[i].UnmutePlaylist();
				}
			}
		}
	}

	public bool EnableMusicDucking
	{
		get
		{
			return enableMusicDucking;
		}
		set
		{
			enableMusicDucking = value;
		}
	}

	public float MasterCrossFadeTime => crossFadeTime;

	public static List<Playlist> MusicPlaylists => Instance.musicPlaylists;

	public static List<GroupBus> GroupBuses => Instance.groupBuses;

	public static List<string> RuntimeSoundGroupNames => Instance.AllSoundGroupNames;

	public static List<string> RuntimeBusNames
	{
		get
		{
			Instance.AllBusNames.Clear();
			if (!Application.isPlaying)
			{
				return Instance.AllBusNames;
			}
			for (int i = 0; i < Instance.groupBuses.Count; i++)
			{
				Instance.AllBusNames.Add(Instance.groupBuses[i].busName);
			}
			return Instance.AllBusNames;
		}
	}

	public static MasterAudio SafeInstance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			_instance = (MasterAudio)UnityEngine.Object.FindObjectOfType(typeof(MasterAudio));
			return _instance;
		}
	}

	public static MasterAudio Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			_instance = (MasterAudio)UnityEngine.Object.FindObjectOfType(typeof(MasterAudio));
			if (_instance == null && Application.isPlaying)
			{
				Debug.LogError("There is no Master Audio prefab in this Scene. Subsequent method calls will fail.");
			}
			return _instance;
		}
		set
		{
			_instance = null;
		}
	}

	public static bool SoundsReady
	{
		get
		{
			if (Instance != null)
			{
				return Instance._soundsLoaded;
			}
			return false;
		}
	}

	public static bool AppIsShuttingDown { get; set; }

	public List<string> GroupNames
	{
		get
		{
			List<string> soundGroupHardCodedNames = SoundGroupHardCodedNames;
			List<string> list = new List<string>(Trans.childCount);
			for (int i = 0; i < Trans.childCount; i++)
			{
				string item = Trans.GetChild(i).name;
				if (!ArrayListUtil.IsExcludedChildName(item))
				{
					list.Add(item);
				}
			}
			DynamicSoundGroupCreator[] array = UnityEngine.Object.FindObjectsOfType(typeof(DynamicSoundGroupCreator)) as DynamicSoundGroupCreator[];
			for (int j = 0; j < array.Length; j++)
			{
				Transform transform = array[j].transform;
				for (int k = 0; k < transform.childCount; k++)
				{
					DynamicSoundGroup component = transform.GetChild(k).GetComponent<DynamicSoundGroup>();
					if (!(component == null) && !list.Contains(component.name))
					{
						list.Add(component.name);
					}
				}
			}
			list.Sort();
			soundGroupHardCodedNames.AddRange(list);
			return soundGroupHardCodedNames;
		}
	}

	public static List<string> SoundGroupHardCodedNames => new List<string> { "[Type In]", "[None]" };

	public List<string> BusNames
	{
		get
		{
			List<string> list = new List<string> { "[Type In]", "[None]" };
			for (int i = 0; i < groupBuses.Count; i++)
			{
				list.Add(groupBuses[i].busName);
			}
			return list;
		}
	}

	public List<string> PlaylistNames
	{
		get
		{
			List<string> list = new List<string> { "[Type In]", "[No Playlist]" };
			for (int i = 0; i < musicPlaylists.Count; i++)
			{
				list.Add(musicPlaylists[i].playlistName);
			}
			return list;
		}
	}

	public List<string> PlaylistNamesOnly
	{
		get
		{
			List<string> list = new List<string>(musicPlaylists.Count);
			for (int i = 0; i < musicPlaylists.Count; i++)
			{
				list.Add(musicPlaylists[i].playlistName);
			}
			return list;
		}
	}

	public Transform Trans
	{
		get
		{
			if (_trans != null)
			{
				return _trans;
			}
			_trans = GetComponent<Transform>();
			return _trans;
		}
	}

	public bool ShouldShowUnityAudioMixerGroupAssignments => showUnityMixerGroupAssignment;

	public List<string> CustomEventNames
	{
		get
		{
			List<string> customEventHardCodedNames = CustomEventHardCodedNames;
			List<CustomEvent> list = Instance.customEvents;
			for (int i = 0; i < list.Count; i++)
			{
				customEventHardCodedNames.Add(list[i].EventName);
			}
			return customEventHardCodedNames;
		}
	}

	public List<string> CustomEventNamesOnly
	{
		get
		{
			List<string> list = new List<string>(customEvents.Count);
			List<CustomEvent> list2 = Instance.customEvents;
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].EventName);
			}
			return list;
		}
	}

	public static List<string> CustomEventHardCodedNames => new List<string> { "[Type In]", "[None]" };

	public static float MasterVolumeLevel
	{
		get
		{
			return Instance._masterAudioVolume;
		}
		set
		{
			Instance._masterAudioVolume = value;
			if (Application.isPlaying)
			{
				for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
				{
					string key = RuntimeSoundGroupNames[i];
					MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
					SetGroupVolume(masterAudioGroup.GameObjectName, masterAudioGroup.groupMasterVolume);
				}
			}
		}
	}

	private static bool SceneHasMasterAudio => Instance != null;

	public static bool IgnoreTimeScale => Instance.ignoreTimeScale;

	public static SystemLanguage DynamicLanguage
	{
		get
		{
			if (!PlayerPrefs.HasKey("~MA_Language_Key~") || string.IsNullOrEmpty(PlayerPrefs.GetString("~MA_Language_Key~")))
			{
				PlayerPrefs.SetString("~MA_Language_Key~", SystemLanguage.Unknown.ToString());
			}
			return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString("~MA_Language_Key~"));
		}
		set
		{
			PlayerPrefs.SetString("~MA_Language_Key~", value.ToString());
			AudioResourceOptimizer.ClearSupportLanguageFolder();
		}
	}

	public static float ReprioritizeTime
	{
		get
		{
			if (Instance._repriTime < 0f)
			{
				Instance._repriTime = (float)(Instance.rePrioritizeEverySecIndex + 1) * 0.1f;
			}
			return Instance._repriTime;
		}
	}

	public static bool ShouldRescanGroups
	{
		get
		{
			if (SafeInstance == null)
			{
				return false;
			}
			return Instance._mustRescanGroups;
		}
	}

	public static string ProspectiveMAPath
	{
		get
		{
			return _prospectiveMAFolder;
		}
		set
		{
			_prospectiveMAFolder = value;
		}
	}

	private void Awake()
	{
		bool flag = false;
		bool flag2 = false;
		if (MasterAudioReferenceHolder.MasterAudio == null)
		{
			MasterAudioReferenceHolder.MasterAudio = this;
		}
		else
		{
			flag = true;
			MasterAudio masterAudio = MasterAudioReferenceHolder.MasterAudio;
			if (masterAudio.persistBetweenScenes && masterAudio.shouldLogDestroys)
			{
				flag2 = true;
			}
		}
		if (flag)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			if (flag2)
			{
				Debug.Log("More than one Master Audio prefab exists in this Scene. Destroying the newer one called '" + base.name + "'. You may wish to set up a Bootstrapper Scene so this does not occur.");
			}
			return;
		}
		AudioListener.pause = false;
		base.useGUILayout = false;
		_soundsLoaded = false;
		_mustRescanGroups = false;
		Transform listenerTrans = ListenerTrans;
		if (listenerTrans != null && deletePreviewerAudioSourceWhenPlaying)
		{
			AudioSource component = listenerTrans.GetComponent<AudioSource>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
		AmbientUtil.InitFollowerHolder();
		AudioSourcesBySoundType.Clear();
		AllBusNames.Clear();
		AllSoundGroupNames.Clear();
		GroupsToDelete.Clear();
		ValidReceivers.Clear();
		ValidReceiverCandidates.Clear();
		ControllersToPause.Clear();
		ControllersToUnpause.Clear();
		ControllersToMute.Clear();
		ControllersToUnmute.Clear();
		ControllersToToggleMute.Clear();
		ControllersToStop.Clear();
		ControllersToFade.Clear();
		ControllersToTrigNext.Clear();
		ControllersToTrigRandom.Clear();
		ControllersToStart.Clear();
		PlaylistControllersByName.Clear();
		LastTimeSoundGroupPlayed.Clear();
		ErrorNumbersLogged.Clear();
		AmbientsToTriggerNow.Clear();
		AllAudioSources.Clear();
		OcclusionSourcesInRange.Clear();
		OcclusionSourcesOutOfRange.Clear();
		OcclusionSourcesBlocked.Clear();
		QueuedOcclusionRays.Clear();
		TransFollowerColliderPositionRecalcs.Clear();
		CustomEventsToFire.Clear();
		AmbientsToDelayedTrigger.Clear();
		ProcessedColliderPositionRecalcs.Clear();
		ActiveVariationUpdaters.Clear();
		ActiveUpdatersToRemove.Clear();
		List<string> list = new List<string>();
		AudioResourceOptimizer.ClearAudioClips();
		PlaylistController.Instances = null;
		List<PlaylistController> instances = PlaylistController.Instances;
		for (int i = 0; i < instances.Count; i++)
		{
			PlaylistController playlistController = instances[i];
			if (list.Contains(playlistController.ControllerName))
			{
				Debug.LogError("You have more than 1 Playlist Controller with the name '" + playlistController.ControllerName + "'. You must name them all uniquely or the same-named ones will be deleted once they awake.");
				continue;
			}
			list.Add(playlistController.ControllerName);
			PlaylistControllersByName.Add(playlistController.ControllerName, playlistController);
			if (persistBetweenScenes)
			{
				UnityEngine.Object.DontDestroyOnLoad(playlistController);
			}
		}
		if (persistBetweenScenes)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		List<int> list2 = new List<int>();
		_randomizer = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
		_randomizerOrigin = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
		_randomizerLeftovers = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
		_nonRandomChoices = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
		_clipsPlayedBySoundTypeOldestFirst = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
		List<SoundGroupVariation> list3 = new List<SoundGroupVariation>();
		_groupsToRemove = new List<string>(Trans.childCount);
		List<string> list4 = new List<string>();
		for (int j = 0; j < Trans.childCount; j++)
		{
			Transform child = Trans.GetChild(j);
			List<AudioInfo> list5 = new List<AudioInfo>();
			MasterAudioGroup component2 = child.GetComponent<MasterAudioGroup>();
			if (component2 == null)
			{
				if (!ArrayListUtil.IsExcludedChildName(child.name))
				{
					Debug.LogError("MasterAudio could not find 'MasterAudioGroup' script for group '" + child.name + "'. Skipping this group.");
				}
				continue;
			}
			string gameObjectName = component2.GameObjectName;
			List<Transform> list6 = new List<Transform>();
			List<int> list7 = new List<int>();
			for (int k = 0; k < child.childCount; k++)
			{
				Transform child2 = child.GetChild(k);
				SoundGroupVariation component3 = child2.GetComponent<SoundGroupVariation>();
				AudioSource component4 = child2.GetComponent<AudioSource>();
				int weight = component3.weight;
				for (int l = 0; l < weight; l++)
				{
					if (l > 0)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(child2.gameObject, child.transform.position, Quaternion.identity);
						gameObject.transform.name = child2.gameObject.name;
						SoundGroupVariation component5 = gameObject.GetComponent<SoundGroupVariation>();
						component5.weight = 1;
						list6.Add(gameObject.transform);
						component4 = gameObject.GetComponent<AudioSource>();
						list5.Add(new AudioInfo(component5, component4, component4.volume));
						list3.Add(component5);
						if (component5.audLocation == AudioLocation.ResourceFile)
						{
							AudioResourceOptimizer.AddTargetForClip(component5.resourceFileName, component4);
						}
					}
					else
					{
						list5.Add(new AudioInfo(component3, component4, component4.volume));
						list3.Add(component3);
						if (component3.audLocation == AudioLocation.ResourceFile)
						{
							AudioResourceOptimizer.AddTargetForClip(AudioResourceOptimizer.GetLocalizedFileName(component3.useLocalization, component3.resourceFileName), component4);
						}
					}
				}
			}
			for (int m = 0; m < list6.Count; m++)
			{
				list6[m].parent = child;
			}
			AudioGroupInfo audioGroupInfo = new AudioGroupInfo(list5, component2);
			if (component2.isSoloed)
			{
				SoloedGroups.Add(component2);
			}
			if (component2.isMuted)
			{
				if (list4.Contains(component2.GameObjectName))
				{
					continue;
				}
				list4.Add(component2.GameObjectName);
			}
			if (AudioSourcesBySoundType.ContainsKey(gameObjectName))
			{
				Debug.LogError("You have more than one SoundGroup named '" + gameObjectName + "'. Ignoring the 2nd one. Please rename it.");
				continue;
			}
			audioGroupInfo.Group.OriginalVolume = audioGroupInfo.Group.groupMasterVolume;
			float? groupVolume = PersistentAudioSettings.GetGroupVolume(gameObjectName);
			if (groupVolume.HasValue)
			{
				audioGroupInfo.Group.groupMasterVolume = groupVolume.Value;
			}
			AddRuntimeGroupInfo(gameObjectName, audioGroupInfo);
			for (int n = 0; n < list5.Count; n++)
			{
				list2.Add(n);
			}
			if (audioGroupInfo.Group.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
			{
				ArrayListUtil.SortIntArray(ref list2);
			}
			_randomizer.Add(gameObjectName, list2);
			list7.Clear();
			list7.AddRange(list2);
			_randomizerOrigin.Add(gameObjectName, list7);
			_randomizerLeftovers.Add(gameObjectName, new List<int>(list2.Count));
			_randomizerLeftovers[gameObjectName].AddRange(list2);
			_clipsPlayedBySoundTypeOldestFirst.Add(gameObjectName, new List<int>());
			_nonRandomChoices.Add(gameObjectName, new List<int>());
			list2 = new List<int>();
		}
		GroupFades.Clear();
		BusFades.Clear();
		GroupPitchGlides.Clear();
		BusPitchGlides.Clear();
		VariationOcclusionFreqChanges.Clear();
		for (int num = 0; num < groupBuses.Count; num++)
		{
			GroupBus groupBus = groupBuses[num];
			groupBus.OriginalVolume = groupBus.volume;
			string busName = groupBus.busName;
			float? busVolume = PersistentAudioSettings.GetBusVolume(busName);
			if (busVolume.HasValue)
			{
				SetBusVolumeByName(busName, busVolume.Value);
			}
		}
		duckingBySoundType.Clear();
		for (int num2 = 0; num2 < musicDuckingSounds.Count; num2++)
		{
			DuckGroupInfo duckGroupInfo = musicDuckingSounds[num2];
			if (duckingBySoundType.ContainsKey(duckGroupInfo.soundType))
			{
				Debug.LogWarning("You have more than one Duck Group set up with the Sound Group '" + duckGroupInfo.soundType + "'. Please delete the duplicates before running again.");
			}
			else if (duckGroupInfo.soundType == "_VideoPlayers")
			{
				Debug.LogError("The specially named Sound Group for Video Players '_VideoPlayers' cannot be used as a Music Ducking Group. Please remove it.");
			}
			else
			{
				duckingBySoundType.Add(duckGroupInfo.soundType, duckGroupInfo);
			}
		}
		_soundsLoaded = true;
		if (willWarm)
		{
			_warming = true;
			string text = SoundGroupForWarming();
			if (!string.IsNullOrEmpty(text))
			{
				PlaySoundResult playSoundResult = PlaySound3DFollowTransform(text, Trans, 0f);
				if (playSoundResult != null && playSoundResult.SoundPlayed)
				{
					playSoundResult.ActingVariation.Stop();
				}
			}
			FireCustomEvent("FakeEvent", _trans);
			for (int num3 = 0; num3 < customEvents.Count; num3++)
			{
				customEvents[num3].frameLastFired = -1;
			}
			frames = 0;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(EventSounds));
			if (array.Length != 0)
			{
				EventSounds obj = array[0] as EventSounds;
				obj.PlaySounds(obj.particleCollisionSound, EventSounds.EventType.UserDefinedEvent);
			}
			for (int num4 = 0; num4 < list4.Count; num4++)
			{
				MuteGroup(list4[num4], shouldCheckMuteStatus: false);
			}
			_warming = false;
		}
		for (int num5 = 0; num5 < list3.Count; num5++)
		{
			list3[num5].DisableUpdater();
		}
		AmbientUtil.InitListenerFollower();
		PersistentAudioSettings.RestoreMasterSettings();
	}

	private void Start()
	{
		if (musicPlaylists.Count > 0 && musicPlaylists[0].MusicSettings != null && musicPlaylists[0].MusicSettings.Count > 0 && musicPlaylists[0].MusicSettings[0].clip != null && PlaylistControllersByName.Count == 0)
		{
			Debug.Log("No Playlist Controllers exist in the Scene. Music will not play.");
		}
	}

	private void OnDestroy()
	{
		if (MasterAudioReferenceHolder.MasterAudio == this)
		{
			MasterAudioReferenceHolder.MasterAudio = null;
		}
	}

	private void OnDisable()
	{
		StopTrackingRuntimeAudioSources(GetComponentsInChildren<AudioSource>().ToList());
	}

	private void Update()
	{
		frames++;
		PerformOcclusionFrequencyChanges();
		PerformBusFades();
		PerformBusPitchGlides();
		PerformGroupFades();
		PerformGroupPitchGlides();
		PerformDelayedAmbientTriggers();
		RefillInactiveGroupPools();
		FireCustomEventsWaiting();
		CheckAddressablesForDelayedRelease();
	}

	private void LateUpdate()
	{
		if (variationFollowerType == VariationFollowerType.LateUpdate)
		{
			ManualUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (variationFollowerType == VariationFollowerType.FixedUpdate)
		{
			ManualUpdate();
		}
	}

	private void ManualUpdate()
	{
		RecalcClosestColliderPositions();
		AmbientUtil.ManualUpdate();
		UpdateActiveVariations();
	}

	public string SoundGroupForWarming()
	{
		string text = null;
		for (int i = 0; i < Trans.childCount; i++)
		{
			Transform child = Trans.GetChild(i);
			if (child.name == "_Followers")
			{
				continue;
			}
			if (text == null)
			{
				text = child.name;
			}
			for (int j = 0; j < child.childCount; j++)
			{
				SoundGroupVariation component = child.GetChild(j).GetComponent<SoundGroupVariation>();
				if (!(component == null) && component.audLocation == AudioLocation.Clip)
				{
					return child.name;
				}
			}
		}
		return text;
	}

	public static void RegisterUpdaterForUpdates(SoundGroupVariationUpdater updater)
	{
		if (!Instance.ActiveVariationUpdaters.Contains(updater))
		{
			Instance.ActiveVariationUpdaters.Add(updater);
		}
	}

	public static void UnregisterUpdaterForUpdates(SoundGroupVariationUpdater updater)
	{
		Instance.ActiveVariationUpdaters.Remove(updater);
	}

	private void UpdateActiveVariations()
	{
		ActiveUpdatersToRemove.Clear();
		for (int i = 0; i < ActiveVariationUpdaters.Count; i++)
		{
			SoundGroupVariationUpdater soundGroupVariationUpdater = ActiveVariationUpdaters[i];
			if (soundGroupVariationUpdater == null || !soundGroupVariationUpdater.enabled)
			{
				ActiveUpdatersToRemove.Add(soundGroupVariationUpdater);
			}
			else
			{
				soundGroupVariationUpdater.ManualUpdate();
			}
		}
		for (int j = 0; j < ActiveUpdatersToRemove.Count; j++)
		{
			ActiveVariationUpdaters.Remove(ActiveUpdatersToRemove[j]);
		}
	}

	private static void UpdateRefillTime(string sType, float inactivePeriodSeconds)
	{
		if (!Instance.LastTimeSoundGroupPlayed.ContainsKey(sType))
		{
			Instance.LastTimeSoundGroupPlayed.Add(sType, new SoundGroupRefillInfo(Time.realtimeSinceStartup, inactivePeriodSeconds));
		}
		else
		{
			Instance.LastTimeSoundGroupPlayed[sType].LastTimePlayed = AudioUtil.Time;
		}
	}

	private static void RecalcClosestColliderPositions()
	{
		if (!AmbientUtil.HasListenerFollower)
		{
			AmbientUtil.InitListenerFollower();
		}
		Instance.ProcessedColliderPositionRecalcs.Clear();
		int count = Instance.TransFollowerColliderPositionRecalcs.Count;
		if (count > Instance.ambientMaxRecalcsPerFrame)
		{
			count = Instance.ambientMaxRecalcsPerFrame;
		}
		int num = 0;
		while (num < count && Instance.TransFollowerColliderPositionRecalcs.Count != 0)
		{
			TransformFollower transformFollower = Instance.TransFollowerColliderPositionRecalcs.Dequeue();
			if (!(transformFollower == null) && transformFollower.enabled)
			{
				bool num2 = transformFollower.RecalcClosestColliderPosition();
				Instance.ProcessedColliderPositionRecalcs.Add(transformFollower);
				if (num2)
				{
					num++;
				}
			}
		}
		for (int i = 0; i < Instance.ProcessedColliderPositionRecalcs.Count; i++)
		{
			Instance.TransFollowerColliderPositionRecalcs.Enqueue(Instance.ProcessedColliderPositionRecalcs[i]);
		}
	}

	private static void FireCustomEventsWaiting()
	{
		while (Instance.CustomEventsToFire.Count > 0)
		{
			CustomEventToFireInfo customEventToFireInfo = Instance.CustomEventsToFire.Dequeue();
			FireCustomEvent(customEventToFireInfo.eventName, customEventToFireInfo.eventOrigin);
		}
	}

	private static void CheckAddressablesForDelayedRelease()
	{
		if (Instance.AddressablesToReleaseLater.Count == 0)
		{
			return;
		}
		AddressableDeadIds.Clear();
		for (int i = 0; i < Instance.AddressablesToReleaseLater.Count; i++)
		{
			AddressableDelayedRelease addressableDelayedRelease = Instance.AddressablesToReleaseLater[i];
			if (Time.realtimeSinceStartup >= addressableDelayedRelease.RealtimeToRelease)
			{
				AddressableDeadIds.Add(addressableDelayedRelease.AddressableId);
			}
		}
		foreach (string addressableDeadId in AddressableDeadIds)
		{
			AudioAddressableOptimizer.MaybeReleaseAddressable(addressableDeadId, forceRelease: true);
		}
		Instance.AddressablesToReleaseLater.RemoveAll((AddressableDelayedRelease adr) => AddressableDeadIds.Contains(adr.AddressableId));
	}

	private static void RefillInactiveGroupPools()
	{
		Dictionary<string, SoundGroupRefillInfo>.Enumerator enumerator = Instance.LastTimeSoundGroupPlayed.GetEnumerator();
		if (Instance._groupsToRemove == null)
		{
			Instance._groupsToRemove = new List<string>();
		}
		Instance._groupsToRemove.Clear();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, SoundGroupRefillInfo> current = enumerator.Current;
			if (current.Value.LastTimePlayed + current.Value.InactivePeriodSeconds < AudioUtil.Time)
			{
				RefillSoundGroupPool(current.Key);
				Instance._groupsToRemove.Add(current.Key);
			}
		}
		for (int i = 0; i < Instance._groupsToRemove.Count; i++)
		{
			Instance.LastTimeSoundGroupPlayed.Remove(Instance._groupsToRemove[i]);
		}
	}

	private static void PerformOcclusionFrequencyChanges()
	{
		if (!AmbientUtil.HasListenerFollower)
		{
			AmbientUtil.InitListenerFollower();
		}
		for (int i = 0; i < Instance.VariationOcclusionFreqChanges.Count; i++)
		{
			OcclusionFreqChangeInfo occlusionFreqChangeInfo = Instance.VariationOcclusionFreqChanges[i];
			if (occlusionFreqChangeInfo.IsActive)
			{
				float val = 1f - (occlusionFreqChangeInfo.CompletionTime - AudioUtil.Time) / (occlusionFreqChangeInfo.CompletionTime - occlusionFreqChangeInfo.StartTime);
				val = Math.Min(val, 1f);
				val = Math.Max(val, 0f);
				float val2 = occlusionFreqChangeInfo.StartFrequency + (occlusionFreqChangeInfo.TargetFrequency - occlusionFreqChangeInfo.StartFrequency) * val;
				val2 = ((!(occlusionFreqChangeInfo.TargetFrequency > occlusionFreqChangeInfo.StartFrequency)) ? Math.Max(val2, occlusionFreqChangeInfo.TargetFrequency) : Math.Min(val2, occlusionFreqChangeInfo.TargetFrequency));
				occlusionFreqChangeInfo.ActingVariation.LowPassFilter.cutoffFrequency = val2;
				if (!(AudioUtil.Time < occlusionFreqChangeInfo.CompletionTime))
				{
					occlusionFreqChangeInfo.IsActive = false;
				}
			}
		}
	}

	private void PerformBusFades()
	{
		for (int i = 0; i < BusFades.Count; i++)
		{
			BusFadeInfo busFadeInfo = BusFades[i];
			if (!busFadeInfo.IsActive)
			{
				continue;
			}
			GroupBus actingBus = busFadeInfo.ActingBus;
			if (actingBus == null)
			{
				busFadeInfo.IsActive = false;
				continue;
			}
			float val = 1f - (busFadeInfo.CompletionTime - AudioUtil.Time) / (busFadeInfo.CompletionTime - busFadeInfo.StartTime);
			val = Math.Min(val, 1f);
			val = Math.Max(val, 0f);
			float val2 = busFadeInfo.StartVolume + (busFadeInfo.TargetVolume - busFadeInfo.StartVolume) * val;
			SetBusVolumeByName(newVolume: (!(busFadeInfo.TargetVolume > busFadeInfo.StartVolume)) ? Math.Max(val2, busFadeInfo.TargetVolume) : Math.Min(val2, busFadeInfo.TargetVolume), busName: actingBus.busName);
			if (!(AudioUtil.Time < busFadeInfo.CompletionTime))
			{
				busFadeInfo.IsActive = false;
				if (stopZeroVolumeBuses && busFadeInfo.TargetVolume == 0f)
				{
					StopBus(busFadeInfo.NameOfBus);
				}
				else if (busFadeInfo.WillStopGroupAfterFade)
				{
					StopBus(busFadeInfo.NameOfBus);
				}
				if (busFadeInfo.WillResetVolumeAfterFade)
				{
					SetBusVolumeByName(actingBus.busName, busFadeInfo.StartVolume);
				}
				if (busFadeInfo.completionAction != null)
				{
					busFadeInfo.completionAction();
				}
			}
		}
	}

	private void PerformGroupFades()
	{
		for (int i = 0; i < GroupFades.Count; i++)
		{
			GroupFadeInfo groupFadeInfo = GroupFades[i];
			if (!groupFadeInfo.IsActive)
			{
				continue;
			}
			MasterAudioGroup actingGroup = groupFadeInfo.ActingGroup;
			if (actingGroup == null)
			{
				groupFadeInfo.IsActive = false;
				continue;
			}
			float val = 1f - (groupFadeInfo.CompletionTime - AudioUtil.Time) / (groupFadeInfo.CompletionTime - groupFadeInfo.StartTime);
			val = Math.Min(val, 1f);
			val = Math.Max(val, 0f);
			float val2 = groupFadeInfo.StartVolume + (groupFadeInfo.TargetVolume - groupFadeInfo.StartVolume) * val;
			SetGroupVolume(volumeLevel: (!(groupFadeInfo.TargetVolume > groupFadeInfo.StartVolume)) ? Math.Max(val2, groupFadeInfo.TargetVolume) : Math.Min(val2, groupFadeInfo.TargetVolume), sType: actingGroup.GameObjectName);
			if (!(AudioUtil.Time < groupFadeInfo.CompletionTime))
			{
				groupFadeInfo.IsActive = false;
				if (groupFadeInfo.completionAction != null)
				{
					groupFadeInfo.completionAction();
				}
				if (stopZeroVolumeGroups && groupFadeInfo.TargetVolume == 0f)
				{
					StopAllOfSound(groupFadeInfo.NameOfGroup);
				}
				else if (groupFadeInfo.WillStopGroupAfterFade)
				{
					StopAllOfSound(groupFadeInfo.NameOfGroup);
				}
				if (groupFadeInfo.WillResetVolumeAfterFade)
				{
					SetGroupVolume(actingGroup.GameObjectName, groupFadeInfo.StartVolume);
				}
			}
		}
	}

	public static void PerformDelayedAmbientTriggers()
	{
		if (AppIsShuttingDown || Instance.AmbientsToDelayedTrigger.Count == 0)
		{
			return;
		}
		Instance.AmbientsToTriggerNow.Clear();
		for (int i = 0; i < Instance.AmbientsToDelayedTrigger.Count; i++)
		{
			AmbientSoundToTriggerInfo ambientSoundToTriggerInfo = Instance.AmbientsToDelayedTrigger[i];
			if (Time.frameCount >= ambientSoundToTriggerInfo.frameToTrigger)
			{
				Instance.AmbientsToTriggerNow.Add(ambientSoundToTriggerInfo);
			}
		}
		if (Instance.AmbientsToTriggerNow.Count == 0)
		{
			return;
		}
		foreach (AmbientSoundToTriggerInfo item in Instance.AmbientsToTriggerNow)
		{
			if (item.ambient != null)
			{
				item.ambient.StartTrackers();
			}
			Instance.AmbientsToDelayedTrigger.Remove(item);
		}
	}

	private void PerformGroupPitchGlides()
	{
		for (int i = 0; i < GroupPitchGlides.Count; i++)
		{
			GroupPitchGlideInfo groupPitchGlideInfo = GroupPitchGlides[i];
			if (!groupPitchGlideInfo.IsActive)
			{
				continue;
			}
			if (groupPitchGlideInfo.ActingGroup == null)
			{
				groupPitchGlideInfo.IsActive = false;
			}
			else if (!(AudioUtil.Time < groupPitchGlideInfo.CompletionTime))
			{
				groupPitchGlideInfo.IsActive = false;
				if (groupPitchGlideInfo.completionAction != null)
				{
					groupPitchGlideInfo.completionAction();
					groupPitchGlideInfo.completionAction = null;
				}
			}
		}
		GroupPitchGlides.RemoveAll((GroupPitchGlideInfo obj) => !obj.IsActive);
	}

	private void PerformBusPitchGlides()
	{
		for (int i = 0; i < BusPitchGlides.Count; i++)
		{
			BusPitchGlideInfo busPitchGlideInfo = BusPitchGlides[i];
			if (!busPitchGlideInfo.IsActive)
			{
				continue;
			}
			if (GetBusIndex(busPitchGlideInfo.NameOfBus, alertMissing: true) < 0)
			{
				busPitchGlideInfo.IsActive = false;
			}
			else if (!(AudioUtil.Time < busPitchGlideInfo.CompletionTime))
			{
				busPitchGlideInfo.IsActive = false;
				if (busPitchGlideInfo.completionAction != null)
				{
					busPitchGlideInfo.completionAction();
					busPitchGlideInfo.completionAction = null;
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		AppIsShuttingDown = true;
	}

	public static bool PlaySoundAndForget(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			return false;
		}
		return PSRAsSuccessBool(PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, null, variationName, attachToSource: false, delaySoundTime, useVector3: false, makePlaySoundResult: false, isChaining));
	}

	public static PlaySoundResult PlaySound(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
	{
		if (!SceneHasMasterAudio)
		{
			return failedResultDuringInit;
		}
		if (SoundsReady)
		{
			return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, null, variationName, attachToSource: false, delaySoundTime, useVector3: false, makePlaySoundResult: true, isChaining, isSingleSubscribedPlay);
		}
		Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
		return failedResultDuringInit;
	}

	public static bool PlaySound3DAtVector3AndForget(string sType, Vector3 sourcePosition, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			return false;
		}
		return PSRAsSuccessBool(PlaySoundAtVolume(sType, volumePercentage, sourcePosition, timeToSchedulePlay, pitch, null, variationName, attachToSource: false, delaySoundTime, useVector3: true));
	}

	public static PlaySoundResult PlaySound3DAtVector3(string sType, Vector3 sourcePosition, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null)
	{
		if (!SceneHasMasterAudio)
		{
			return failedResultDuringInit;
		}
		if (SoundsReady)
		{
			return PlaySoundAtVolume(sType, volumePercentage, sourcePosition, timeToSchedulePlay, pitch, null, variationName, attachToSource: false, delaySoundTime, useVector3: true, makePlaySoundResult: true);
		}
		Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
		return failedResultDuringInit;
	}

	public static bool PlaySound3DAtTransformAndForget(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			return false;
		}
		return PSRAsSuccessBool(PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, sourceTrans, variationName, attachToSource: false, delaySoundTime, useVector3: false, makePlaySoundResult: false, isChaining));
	}

	public static PlaySoundResult PlaySound3DAtTransform(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
	{
		if (!SceneHasMasterAudio)
		{
			return failedResultDuringInit;
		}
		if (SoundsReady)
		{
			return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, sourceTrans, variationName, attachToSource: false, delaySoundTime, useVector3: false, makePlaySoundResult: true, isChaining, isSingleSubscribedPlay);
		}
		Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
		return failedResultDuringInit;
	}

	public static bool PlaySound3DFollowTransformAndForget(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			return false;
		}
		return PSRAsSuccessBool(PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, sourceTrans, variationName, attachToSource: true, delaySoundTime, useVector3: false, makePlaySoundResult: false, isChaining));
	}

	public static PlaySoundResult PlaySound3DFollowTransform(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
	{
		if (!SceneHasMasterAudio)
		{
			return failedResultDuringInit;
		}
		if (SoundsReady)
		{
			return PlaySoundAtVolume(sType, volumePercentage, Vector3.zero, timeToSchedulePlay, pitch, sourceTrans, variationName, attachToSource: true, delaySoundTime, useVector3: false, makePlaySoundResult: true, isChaining, isSingleSubscribedPlay);
		}
		Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
		return failedResultDuringInit;
	}

	public static IEnumerator PlaySoundAndWaitUntilFinished(string sType, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, Action completedAction = null)
	{
		if (!SceneHasMasterAudio)
		{
			yield break;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			yield break;
		}
		PlaySoundResult playSoundResult = PlaySound(sType, volumePercentage, pitch, delaySoundTime, variationName, null, isChaining: false, isSingleSubscribedPlay: true);
		bool done = false;
		if (playSoundResult != null && !(playSoundResult.ActingVariation == null))
		{
			playSoundResult.ActingVariation.SoundFinished += () =>
			{
				done = true;
			};
			while (!done)
			{
				yield return EndOfFrameDelay;
			}
			completedAction?.Invoke();
		}
	}

	public static IEnumerator PlaySound3DAtTransformAndWaitUntilFinished(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, Action completedAction = null)
	{
		if (!SceneHasMasterAudio)
		{
			yield break;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			yield break;
		}
		PlaySoundResult playSoundResult = PlaySound3DAtTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay, isChaining: false, isSingleSubscribedPlay: true);
		bool done = false;
		if (playSoundResult != null && !(playSoundResult.ActingVariation == null))
		{
			playSoundResult.ActingVariation.SoundFinished += () =>
			{
				done = true;
			};
			while (!done)
			{
				yield return EndOfFrameDelay;
			}
			completedAction?.Invoke();
		}
	}

	public static IEnumerator PlaySound3DFollowTransformAndWaitUntilFinished(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, Action completedAction = null)
	{
		if (!SceneHasMasterAudio)
		{
			yield break;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot play: " + sType);
			yield break;
		}
		PlaySoundResult playSoundResult = PlaySound3DFollowTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay, isChaining: false, isSingleSubscribedPlay: true);
		bool done = false;
		if (playSoundResult != null && !(playSoundResult.ActingVariation == null))
		{
			playSoundResult.ActingVariation.SoundFinished += () =>
			{
				done = true;
			};
			while (!done)
			{
				yield return EndOfFrameDelay;
			}
			completedAction?.Invoke();
		}
	}

	public static bool PSRAsSuccessBool(PlaySoundResult psr)
	{
		if (psr != null)
		{
			if (!psr.SoundPlayed)
			{
				return psr.SoundScheduled;
			}
			return true;
		}
		return false;
	}

	private static PlaySoundResult PlaySoundAtVolume(string sType, float volumePercentage, Vector3 sourcePosition, double? timeToSchedulePlay, float? pitch = null, Transform sourceTrans = null, string variationName = null, bool attachToSource = false, float delaySoundTime = 0f, bool useVector3 = false, bool makePlaySoundResult = false, bool isChaining = false, bool isSingleSubscribedPlay = false, bool triggeredAsChildGroup = false)
	{
		if (!IsSoundGroupValidAndReady(sType, sourceTrans))
		{
			return null;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
		List<AudioInfo> sources = audioGroupInfo.Sources;
		bool flag = LoggingEnabledForGroup(masterAudioGroup);
		if (!SoundGroupHasVariations(sType, sources, flag))
		{
			return null;
		}
		audioGroupInfo.PlayedForWarming = IsWarming;
		LogIfSilentPlay(sType, flag, masterAudioGroup);
		if (IsReplayLimited(sType, masterAudioGroup, audioGroupInfo, flag))
		{
			return null;
		}
		bool flag2 = string.IsNullOrEmpty(variationName);
		AudioInfo audioInfo = null;
		if (IsGroupPolyphonyLimited(masterAudioGroup, audioGroupInfo))
		{
			audioInfo = FindRetriggerableVariationInGroup(variationName, flag2, sources, masterAudioGroup);
			if (audioInfo == null)
			{
				if (flag || LogOutOfVoices)
				{
					LogMessage("Polyphony limit of group: " + audioGroupInfo.Group.GameObjectName + " exceeded and no playing Variation is usable for Retrigger Limit Mode. Will not play this sound for this instance.");
				}
				return null;
			}
		}
		GroupBus busForGroup = audioGroupInfo.Group.BusForGroup;
		SoundGroupVariation soundGroupVariation = null;
		if (IsBusVoiceLimited(busForGroup))
		{
			if (!CanStopLimitedBusVoice(busForGroup, flag, audioGroupInfo))
			{
				return null;
			}
			soundGroupVariation = FindBusVoiceToStop(busForGroup, audioGroupInfo.Group);
			if (soundGroupVariation == null)
			{
				return null;
			}
		}
		bool isSingleVarLoop = false;
		audioInfo = UseOnlyVariationIfOnlyOne(sType, sources, flag, audioInfo, masterAudioGroup, ref isSingleVarLoop);
		List<int> choices = null;
		int? randomIndex = null;
		List<int> otherChoices = null;
		int pickedChoice = -1;
		bool canUseBusVoiceToStop = soundGroupVariation != null && Instance.stopOldestBusFadeTime == 0f;
		if (!CanFindVariationToPlay(sType, variationName, flag2, canUseBusVoiceToStop, sources, flag, soundGroupVariation, ref audioInfo, ref choices, ref randomIndex, ref pickedChoice, ref otherChoices))
		{
			return null;
		}
		if (!VariationIsUsable(audioInfo))
		{
			return null;
		}
		if (IsNoClipSilentPlay(sType, volumePercentage, pitch, sourceTrans, attachToSource, isChaining, audioInfo, flag, audioGroupInfo, flag2, randomIndex, choices, pickedChoice))
		{
			return null;
		}
		if (!VariationPassesProbabilityToPlayCheck(sType, volumePercentage, pitch, sourceTrans, attachToSource, isChaining, audioInfo, flag, audioGroupInfo, flag2, randomIndex, choices, pickedChoice))
		{
			return null;
		}
		if (IsActorTooFarAwayToPlay(sType, sourceTrans, audioGroupInfo, audioInfo, flag))
		{
			return null;
		}
		if (!CanPlayDialogBasedOnImportanceOrIsNotDialog(sType, audioGroupInfo, flag, audioInfo))
		{
			return null;
		}
		bool forgetSoundPlayedOrScheduled = false;
		bool hasRefilledPool = false;
		PlaySoundResult result = TryPlayVariationOrOtherMatches(sType, volumePercentage, sourcePosition, timeToSchedulePlay, pitch, sourceTrans, attachToSource, delaySoundTime, useVector3, makePlaySoundResult, isChaining, isSingleSubscribedPlay, audioInfo, soundGroupVariation, busForGroup, canUseBusVoiceToStop, forgetSoundPlayedOrScheduled, audioGroupInfo, flag2, randomIndex, choices, pickedChoice, flag, isSingleVarLoop, otherChoices, hasRefilledPool, sources, out var soundSuccess);
		if (!soundSuccess)
		{
			if (flag || LogOutOfVoices)
			{
				if (flag2)
				{
					LogMessage("All " + sources.Count + " children of " + sType + " were busy. Will not play this sound for this instance. If you need more voices, increase the 'Voices / Weight' field on the Variation(s) in your Sound Group.");
				}
				else
				{
					LogMessage("Child '" + audioInfo.Variation.GameObjectName + "' of " + sType + " was busy. Will not play this sound for this instance. If you need more voices, increase the 'Voices / Weight' field on the Variation(s) in your Sound Group.");
				}
			}
			return result;
		}
		SetLastPlayed(audioGroupInfo);
		if (!triggeredAsChildGroup && !IsWarming)
		{
			switch (audioGroupInfo.Group.linkedStartGroupSelectionType)
			{
			case LinkedGroupSelectionType.All:
			{
				for (int i = 0; i < audioGroupInfo.Group.childSoundGroups.Count; i++)
				{
					PlaySoundAtVolume(audioGroupInfo.Group.childSoundGroups[i], volumePercentage, sourcePosition, timeToSchedulePlay, pitch, sourceTrans, null, attachToSource, delaySoundTime, useVector3, makePlaySoundResult: false, isChaining: false, isSingleSubscribedPlay: false, triggeredAsChildGroup: true);
				}
				break;
			}
			case LinkedGroupSelectionType.OneAtRandom:
			{
				int index = UnityEngine.Random.Range(0, audioGroupInfo.Group.childSoundGroups.Count);
				PlaySoundAtVolume(audioGroupInfo.Group.childSoundGroups[index], volumePercentage, sourcePosition, timeToSchedulePlay, pitch, sourceTrans, null, attachToSource, delaySoundTime, useVector3, makePlaySoundResult: false, isChaining: false, isSingleSubscribedPlay: false, triggeredAsChildGroup: true);
				break;
			}
			}
		}
		if (audioGroupInfo.Group.soundPlayedEventActive)
		{
			FireCustomEvent(audioGroupInfo.Group.soundPlayedCustomEvent, Instance._trans);
		}
		if (!makePlaySoundResult)
		{
			return AndForgetSuccessResult;
		}
		return result;
	}

	private static PlaySoundResult TryPlayVariationOrOtherMatches(string sType, float volumePercentage, Vector3 sourcePosition, double? timeToSchedulePlay, float? pitch, Transform sourceTrans, bool attachToSource, float delaySoundTime, bool useVector3, bool makePlaySoundResult, bool isChaining, bool isSingleSubscribedPlay, AudioInfo randomSource, SoundGroupVariation busVoiceToStop, GroupBus groupBus, bool canUseBusVoiceToStop, bool forgetSoundPlayedOrScheduled, AudioGroupInfo group, bool isNonSpecific, int? randomIndex, List<int> choices, int pickedChoice, bool loggingEnabledForGrp, bool isSingleVarLoop, List<int> otherChoices, bool hasRefilledPool, List<AudioInfo> sources, out bool soundSuccess)
	{
		bool flag;
		PlaySoundResult playSoundResult;
		do
		{
			flag = false;
			playSoundResult = PlaySoundIfAvailable(randomSource, sourcePosition, volumePercentage, busVoiceToStop, groupBus, canUseBusVoiceToStop, ref forgetSoundPlayedOrScheduled, pitch, group, sourceTrans, attachToSource, delaySoundTime, useVector3, makePlaySoundResult, timeToSchedulePlay, isChaining, isSingleSubscribedPlay);
			bool flag2 = makePlaySoundResult && playSoundResult != null && (playSoundResult.SoundPlayed || playSoundResult.SoundScheduled);
			bool flag3 = !makePlaySoundResult & forgetSoundPlayedOrScheduled;
			soundSuccess = flag2 | flag3;
			if (soundSuccess)
			{
				if (!IsWarming)
				{
					RemoveClipAndRefillIfEmpty(group, isNonSpecific, randomIndex, choices, sType, pickedChoice, loggingEnabledForGrp, isSingleVarLoop);
				}
				break;
			}
			if (isNonSpecific)
			{
				if (otherChoices == null)
				{
					continue;
				}
				if (otherChoices.Count <= 0)
				{
					if (hasRefilledPool)
					{
						continue;
					}
					RefillSoundGroupPool(sType);
					hasRefilledPool = true;
					otherChoices.Clear();
					otherChoices.AddRange(choices);
				}
				randomSource = sources[otherChoices[0]];
				if (randomSource.Variation == null)
				{
					SoundGroupVariation component = randomSource.Source.GetComponent<SoundGroupVariation>();
					if (component == null)
					{
						break;
					}
					randomSource.Variation = component;
				}
				if (loggingEnabledForGrp)
				{
					LogMessage("Child was busy. Cueing child named '" + randomSource.Variation.GameObjectName + "' of " + sType);
				}
				otherChoices.RemoveAt(0);
				if (hasRefilledPool && otherChoices.Count == 0)
				{
					flag = true;
				}
			}
			else
			{
				if (loggingEnabledForGrp)
				{
					LogMessage("Child was busy. Since you requested a named Variation '" + randomSource.Variation.GameObjectName + "', no others to try. Aborting.");
				}
				if (otherChoices != null)
				{
					otherChoices.Clear();
					break;
				}
			}
		}
		while (otherChoices != null && ((otherChoices.Count > 0 || !hasRefilledPool) | flag));
		return playSoundResult;
	}

	private static bool CanPlayDialogBasedOnImportanceOrIsNotDialog(string sType, AudioGroupInfo group, bool loggingEnabledForGrp, AudioInfo randomSource)
	{
		if (group.Group.curVariationMode != MasterAudioGroup.VariationMode.Dialog)
		{
			return true;
		}
		AudioInfo audioInfo = group.Sources.Find((AudioInfo info) => info.Variation.IsPlaying);
		if (audioInfo == null)
		{
			return true;
		}
		SoundGroupVariation variation = audioInfo.Variation;
		if (variation.isUninterruptible)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage($"Already playing Child named '{randomSource.Variation.GameObjectName}' of '{sType}' is marked as Uninterruptible, so not playing.");
				return false;
			}
		}
		else if (randomSource.Variation.importance < variation.importance)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage($"Already playing Child named '{variation.GameObjectName}' of '{sType} has higher Importance ({randomSource.Variation.importance}) than Child '{randomSource.Variation.GameObjectName}', so not playing.");
				return false;
			}
		}
		else if (group.Group.useDialogFadeOut)
		{
			FadeOutAllOfSound(group.Group.GameObjectName, group.Group.dialogFadeOutTime);
		}
		else
		{
			StopAllOfSound(group.Group.GameObjectName);
		}
		return true;
	}

	private static AudioInfo UseOnlyVariationIfOnlyOne(string sType, List<AudioInfo> sources, bool loggingEnabledForGrp, AudioInfo randomSource, MasterAudioGroup maGroup, ref bool isSingleVarLoop)
	{
		if (sources.Count != 1)
		{
			return randomSource;
		}
		if (loggingEnabledForGrp)
		{
			LogMessage("Cueing only child of " + sType);
		}
		if (randomSource == null)
		{
			randomSource = sources[0];
		}
		if (maGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain)
		{
			isSingleVarLoop = true;
		}
		return randomSource;
	}

	private static bool IsActorTooFarAwayToPlay(string sType, Transform sourceTrans, AudioGroupInfo group, AudioInfo randomSource, bool loggingEnabledForGrp)
	{
		if (sourceTrans != null && ListenerTrans != null && group.Group.spatialBlend > 0f && group.Group.GroupPlayType == GroupPlayType.WhenActorInAudibleRange)
		{
			float maxDistance = randomSource.Variation.VarAudio.maxDistance;
			if ((ListenerTrans.position - sourceTrans.position).magnitude < maxDistance)
			{
				return false;
			}
			if (loggingEnabledForGrp)
			{
				LogMessage(string.Format("Child named '{0}' of '{1}' is too far away to be heard{2}, so not playing.", randomSource.Variation.GameObjectName, sType, (sourceTrans.name == null) ? "" : (" with Actor '" + sourceTrans.name + "'")));
			}
			return true;
		}
		return false;
	}

	private static bool VariationPassesProbabilityToPlayCheck(string sType, float volumePercentage, float? pitch, Transform sourceTrans, bool attachToSource, bool isChaining, AudioInfo randomSource, bool loggingEnabledForGrp, AudioGroupInfo group, bool isNonSpecific, int? randomIndex, List<int> choices, int pickedChoice)
	{
		if (randomSource.Variation.probabilityToPlay >= 100)
		{
			return true;
		}
		if (UnityEngine.Random.Range(0, 100) >= randomSource.Variation.probabilityToPlay)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage($"Child named '{randomSource.Variation.GameObjectName}' of {sType} failed its Random number check for 'Probability to Play' to it so nothing will be played this time.");
			}
			MaybeChainNextVar(isChaining, randomSource.Variation, volumePercentage, pitch, sourceTrans, attachToSource);
			return false;
		}
		return true;
	}

	private static bool IsNoClipSilentPlay(string sType, float volumePercentage, float? pitch, Transform sourceTrans, bool attachToSource, bool isChaining, AudioInfo randomSource, bool loggingEnabledForGrp, AudioGroupInfo group, bool isNonSpecific, int? randomIndex, List<int> choices, int pickedChoice)
	{
		if (randomSource.Variation.audLocation == AudioLocation.Clip && randomSource.Variation.VarAudio.clip == null)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage($"Child named '{randomSource.Variation.GameObjectName}' of {sType} has no audio assigned to it so nothing will be played.");
			}
			RemoveClipAndRefillIfEmpty(group, isNonSpecific, randomIndex, choices, sType, pickedChoice, loggingEnabledForGrp, isSingleVarLoop: false);
			MaybeChainNextVar(isChaining, randomSource.Variation, volumePercentage, pitch, sourceTrans, attachToSource);
			return true;
		}
		return false;
	}

	private static bool VariationIsUsable(AudioInfo randomSource)
	{
		if (randomSource.Variation != null)
		{
			return true;
		}
		if (AppIsShuttingDown || randomSource.Source == null)
		{
			return false;
		}
		SoundGroupVariation component = randomSource.Source.GetComponent<SoundGroupVariation>();
		if (component == null)
		{
			return false;
		}
		randomSource.Variation = component;
		return true;
	}

	private static bool CanFindVariationToPlay(string sType, string variationName, bool isNonSpecific, bool canUseBusVoiceToStop, List<AudioInfo> sources, bool loggingEnabledForGrp, SoundGroupVariation busVoiceToStop, ref AudioInfo randomSource, ref List<int> choices, ref int? randomIndex, ref int pickedChoice, ref List<int> otherChoices)
	{
		if (randomSource != null)
		{
			return true;
		}
		if (!Instance._randomizer.ContainsKey(sType))
		{
			Debug.Log("Sound Group '" + sType + "' has no active Variations.");
			return false;
		}
		if (isNonSpecific)
		{
			choices = Instance._randomizer[sType];
			randomIndex = 0;
			pickedChoice = choices[randomIndex.Value];
			randomSource = sources[pickedChoice];
			otherChoices = Instance._randomizerLeftovers[sType];
			otherChoices.Remove(pickedChoice);
			if (loggingEnabledForGrp)
			{
				LogMessage($"Cueing child {choices[randomIndex.Value]} of {sType}");
			}
			return true;
		}
		int num = 0;
		List<int> list = Instance._nonRandomChoices[sType];
		list.Clear();
		for (int i = 0; i < sources.Count; i++)
		{
			AudioInfo audioInfo = sources[i];
			if ((!string.IsNullOrEmpty(audioInfo.Variation.clipAlias) && audioInfo.Variation.clipAlias == variationName) || audioInfo.Variation.GameObjectName == variationName)
			{
				num++;
				if (audioInfo.Variation.IsAvailableToPlay || (canUseBusVoiceToStop && audioInfo.Variation == busVoiceToStop))
				{
					list.Add(i);
				}
			}
		}
		if (num == 0)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage("Can't find variation '" + variationName + "' of " + sType);
			}
			return false;
		}
		if (list.Count == 0)
		{
			if (loggingEnabledForGrp || LogOutOfVoices)
			{
				LogMessage("Can't find non-busy variation '" + variationName + "' of " + sType);
			}
			return false;
		}
		if (list.Count == 1)
		{
			randomIndex = 0;
		}
		else
		{
			randomIndex = UnityEngine.Random.Range(0, list.Count);
		}
		pickedChoice = list[randomIndex.Value];
		randomSource = sources[pickedChoice];
		list.Remove(pickedChoice);
		otherChoices = list;
		if (loggingEnabledForGrp)
		{
			LogMessage($"Cueing child named '{randomSource.Variation.GameObjectName}' of {sType}");
		}
		return true;
	}

	private static SoundGroupVariation FindBusVoiceToStop(GroupBus groupBus, MasterAudioGroup group)
	{
		switch (groupBus.busVoiceLimitExceededMode)
		{
		case BusVoiceLimitExceededMode.StopOldestSound:
			return FindOldestSoundOnBus(groupBus);
		case BusVoiceLimitExceededMode.StopFarthestSound:
			return FindFarthestSoundOnBus(groupBus);
		case BusVoiceLimitExceededMode.StopLeastImportantSound:
		{
			SoundGroupVariation soundGroupVariation = FindLeastImportantSoundOnBus(groupBus, group);
			if (soundGroupVariation == null && (group.LoggingEnabledForGroup || LogOutOfVoices))
			{
				LogMessage("Could not find a Less Important and Interruptible voice to stop on Bus '" + groupBus.busName + "', and Bus voice limit has been reached. Cannot play the sound: " + group.GameObjectName + " with Importance " + group.importance + ".");
			}
			return soundGroupVariation;
		}
		default:
			return null;
		}
	}

	private static bool CanStopLimitedBusVoice(GroupBus groupBus, bool loggingEnabledForGrp, AudioGroupInfo group)
	{
		if (groupBus.busVoiceLimitExceededMode == BusVoiceLimitExceededMode.DoNotPlayNewSound)
		{
			if (loggingEnabledForGrp || LogOutOfVoices)
			{
				LogMessage("Bus voice limit has been reached. Cannot play the sound: " + group.Group.GameObjectName + " until one voice has stopped playing. You can turn on the 'Stop Oldest Sound' or 'Stop Farthest Sound' option for the bus to change ");
			}
			return false;
		}
		return true;
	}

	private static bool IsBusVoiceLimited(GroupBus groupBus)
	{
		if (groupBus != null && groupBus.BusVoiceLimitReached)
		{
			return true;
		}
		return false;
	}

	private static AudioInfo FindRetriggerableVariationInGroup(string variationName, bool isNonSpecific, List<AudioInfo> sources, MasterAudioGroup maGroup)
	{
		AudioInfo audioInfo = null;
		audioInfo = ((!isNonSpecific) ? sources.Find((AudioInfo info) => info.Source.isPlaying && info.Variation.ClipIsLoaded && !(info.Variation.GameObjectName != variationName) && AudioUtil.GetAudioPlayedPercentage(info.Source) >= (float)maGroup.retriggerPercentage) : sources.Find((AudioInfo info) => info.Source.isPlaying && info.Variation.ClipIsLoaded && AudioUtil.GetAudioPlayedPercentage(info.Source) >= (float)maGroup.retriggerPercentage));
		if (audioInfo != null && maGroup.LoggingEnabledForGroup)
		{
			LogMessage("Cueing Retrigger of child named '" + audioInfo.Variation.GameObjectName + "' of " + maGroup.GameObjectName);
		}
		return audioInfo;
	}

	private static bool IsGroupPolyphonyLimited(MasterAudioGroup maGroup, AudioGroupInfo group)
	{
		if (maGroup.curVariationMode != MasterAudioGroup.VariationMode.Normal)
		{
			return false;
		}
		if (!group.Group.limitPolyphony)
		{
			return false;
		}
		if (group.Group.ActiveVoices < group.Group.voiceLimitCount)
		{
			return false;
		}
		return true;
	}

	private static bool IsReplayLimited(string sType, MasterAudioGroup maGroup, AudioGroupInfo group, bool loggingEnabledForGrp)
	{
		if (maGroup.curVariationMode != MasterAudioGroup.VariationMode.Normal)
		{
			return false;
		}
		switch (maGroup.limitMode)
		{
		case MasterAudioGroup.LimitMode.TimeBased:
			if (maGroup.minimumTimeBetween > 0f && Time.realtimeSinceStartup < group.LastTimePlayed + maGroup.minimumTimeBetween)
			{
				if (loggingEnabledForGrp)
				{
					LogMessage("MasterAudio skipped playing sound: " + sType + " due to Group's Min Seconds Between setting.");
				}
				return true;
			}
			break;
		case MasterAudioGroup.LimitMode.FrameBased:
			if (Time.frameCount - group.LastFramePlayed < maGroup.limitPerXFrames)
			{
				if (loggingEnabledForGrp)
				{
					LogMessage("Master Audio skipped playing sound: " + sType + " due to Group's Per Frame Limit.");
				}
				return true;
			}
			break;
		}
		return false;
	}

	private static void LogIfSilentPlay(string sType, bool loggingEnabledForGrp, MasterAudioGroup maGroup)
	{
		if (Instance.mixerMuted)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage("MasterAudio playing sound: " + sType + " silently because the Mixer is muted.");
			}
		}
		else if (maGroup.isMuted && loggingEnabledForGrp)
		{
			LogMessage("MasterAudio playing sound: " + sType + " silently because the Group is muted.");
		}
		if (Instance.SoloedGroups.Count > 0 && !Instance.SoloedGroups.Contains(maGroup) && loggingEnabledForGrp)
		{
			LogMessage("MasterAudio playing sound: " + sType + " silently because there are one or more Groups soloed. This one is not.");
		}
	}

	private static bool SoundGroupHasVariations(string sType, List<AudioInfo> sources, bool loggingEnabledForGrp)
	{
		if (sources.Count == 0)
		{
			if (loggingEnabledForGrp)
			{
				LogMessage("Sound Group '" + sType + "' has no active Variations.");
			}
			return false;
		}
		return true;
	}

	private static bool IsSoundGroupValidAndReady(string sType, Transform sourceTrans)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!SoundsReady || sType == string.Empty || sType == "[None]")
		{
			return false;
		}
		if (sType == "_VideoPlayers")
		{
			LogError("You cannot play sounds from the specially named Sound Group for Video Players.");
			return false;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			string text = "MasterAudio could not find sound: " + sType + ". If your Scene just changed, this could happen when an OnDisable or OnInvisible event sound happened to a per-scene sound, which is expected.";
			if (sourceTrans != null)
			{
				text = text + " Triggered by prefab: " + sourceTrans.name;
			}
			LogWarning(text);
			return false;
		}
		return true;
	}

	private static void MaybeChainNextVar(bool isChaining, SoundGroupVariation variation, float volumePercentage, float? pitch, Transform sourceTrans, bool attachToSource)
	{
		if (isChaining)
		{
			variation.DoNextChain(volumePercentage, pitch, sourceTrans, attachToSource);
		}
	}

	private static void SetLastPlayed(AudioGroupInfo grp)
	{
		grp.LastTimePlayed = AudioUtil.Time;
		grp.LastFramePlayed = AudioUtil.FrameCount;
	}

	private static void RemoveClipAndRefillIfEmpty(AudioGroupInfo grp, bool isNonSpecific, int? randomIndex, List<int> choices, string sType, int pickedChoice, bool loggingEnabledForGrp, bool isSingleVarLoop)
	{
		if (isSingleVarLoop)
		{
			grp.Group.ChainLoopCount++;
			return;
		}
		if (isNonSpecific && randomIndex.HasValue)
		{
			choices.RemoveAt(randomIndex.Value);
			Instance._clipsPlayedBySoundTypeOldestFirst[sType].Add(pickedChoice);
			if (choices.Count == 0)
			{
				if (loggingEnabledForGrp)
				{
					LogMessage("Refilling Variation pool: " + sType);
				}
				RefillSoundGroupPool(sType);
			}
		}
		if (grp.Group.curVariationSequence == MasterAudioGroup.VariationSequence.TopToBottom && grp.Group.useInactivePeriodPoolRefill)
		{
			UpdateRefillTime(sType, grp.Group.inactivePeriodSeconds);
		}
	}

	private static PlaySoundResult PlaySoundIfAvailable(AudioInfo info, Vector3 sourcePosition, float volumePercentage, SoundGroupVariation busVoiceToStop, GroupBus groupBus, bool canUseBusVoiceToStop, ref bool forgetSoundPlayed, float? pitch = null, AudioGroupInfo audioGroup = null, Transform sourceTrans = null, bool attachToSource = false, float delaySoundTime = 0f, bool useVector3 = false, bool makePlaySoundResult = false, double? timeToSchedulePlay = null, bool isChaining = false, bool isSingleSubscribedPlay = false)
	{
		if (info.Source == null)
		{
			return null;
		}
		if (info.Variation.LoadStatus == VariationLoadStatus.Loading)
		{
			return null;
		}
		MasterAudioGroup masterAudioGroup = audioGroup.Group;
		bool flag = false;
		if ((!canUseBusVoiceToStop || !(info.Variation == busVoiceToStop)) && masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.Normal && info.Source.isPlaying && info.Variation.ClipIsLoaded)
		{
			float audioPlayedPercentage = AudioUtil.GetAudioPlayedPercentage(info.Source);
			int retriggerPercentage = masterAudioGroup.retriggerPercentage;
			if (audioPlayedPercentage < (float)retriggerPercentage)
			{
				return null;
			}
			flag = true;
		}
		if (!IsWarming)
		{
			info.Variation.Stop(stopEndDetection: false, skipLinked: true);
		}
		info.Variation.ObjectToFollow = null;
		bool flag2 = Instance.prioritizeOnDistance && (Instance.useClipAgePriority || info.Variation.ParentGroup.useClipAgePriority);
		if (useVector3)
		{
			info.Source.transform.position = sourcePosition;
			if (Instance.prioritizeOnDistance)
			{
				AudioPrioritizer.Set3DPriority(info.Variation, flag2);
			}
		}
		else if (sourceTrans != null)
		{
			if (attachToSource)
			{
				info.Variation.ObjectToFollow = sourceTrans;
			}
			else
			{
				info.Source.transform.position = sourceTrans.position;
				info.Variation.ObjectToTriggerFrom = sourceTrans;
			}
			if (Instance.prioritizeOnDistance)
			{
				AudioPrioritizer.Set3DPriority(info.Variation, flag2);
			}
		}
		else
		{
			if (Instance.prioritizeOnDistance)
			{
				AudioPrioritizer.Set2DSoundPriority(info.Source);
			}
			info.Source.transform.localPosition = Vector3.zero;
		}
		float groupMasterVolume = masterAudioGroup.groupMasterVolume;
		float busVolume = GetBusVolume(masterAudioGroup);
		float num = info.OriginalVolume;
		float num2 = 0f;
		if (info.Variation.useRandomVolume)
		{
			num2 = UnityEngine.Random.Range(info.Variation.randomVolumeMin, info.Variation.randomVolumeMax);
			switch (info.Variation.randomVolumeMode)
			{
			case SoundGroupVariation.RandomVolumeMode.AddToClipVolume:
				num += num2;
				break;
			case SoundGroupVariation.RandomVolumeMode.IgnoreClipVolume:
				num = num2;
				break;
			}
		}
		float num3 = num * groupMasterVolume * busVolume * Instance._masterAudioVolume;
		float num4 = num3 * volumePercentage;
		info.Source.volume = num4;
		info.LastPercentageVolume = volumePercentage;
		info.LastRandomVolume = num2;
		if (!info.Variation.GameObj.activeInHierarchy)
		{
			DTMonoHelper.SetActive(info.Variation.GameObj, isActive: true);
			if (!info.Variation.GameObj.activeInHierarchy)
			{
				return null;
			}
			info.Variation.DisableUpdater();
		}
		PlaySoundResult playSoundResult = null;
		if (makePlaySoundResult)
		{
			playSoundResult = new PlaySoundResult
			{
				ActingVariation = info.Variation
			};
			if (delaySoundTime > 0f)
			{
				playSoundResult.SoundScheduled = true;
			}
			else
			{
				playSoundResult.SoundPlayed = true;
			}
		}
		else
		{
			forgetSoundPlayed = true;
		}
		string gameObjectName = masterAudioGroup.GameObjectName;
		if (masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain)
		{
			if (!isChaining)
			{
				masterAudioGroup.ChainLoopCount = 0;
			}
			Transform objectToFollow = info.Variation.ObjectToFollow;
			if (masterAudioGroup.ActiveVoices > 0 && !isChaining)
			{
				StopAllOfSound(gameObjectName);
			}
			info.Variation.ObjectToFollow = objectToFollow;
		}
		if (!flag)
		{
			FadeOldestOrFarthestBusVoice(busVoiceToStop, groupBus);
		}
		info.Variation.Play(pitch, num4, gameObjectName, volumePercentage, num3, pitch, sourceTrans, attachToSource, delaySoundTime, timeToSchedulePlay, isChaining, isSingleSubscribedPlay);
		if (Instance._isStoppingMultiple)
		{
			Instance.VariationsStartedDuringMultiStop.Add(info.Variation);
		}
		return playSoundResult;
	}

	private static void FadeOldestOrFarthestBusVoice(SoundGroupVariation busVoiceToStop, GroupBus groupBus)
	{
		if (groupBus != null && !(busVoiceToStop == null))
		{
			BusVoiceLimitExceededMode busVoiceLimitExceededMode = groupBus.busVoiceLimitExceededMode;
			if ((uint)(busVoiceLimitExceededMode - 1) <= 2u)
			{
				busVoiceToStop.FadeOutNowAndStop(Instance.stopOldestBusFadeTime);
			}
		}
	}

	public static void EndDucking(SoundGroupVariationUpdater actorUpdater)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		for (int i = 0; i < instances.Count; i++)
		{
			instances[i].EndDucking(actorUpdater);
		}
	}

	public static void DuckSoundGroup(string soundGroupName, AudioSource aSource, SoundGroupVariationUpdater actorUpdater)
	{
		MasterAudio instance = Instance;
		if (instance.EnableMusicDucking && instance.duckingBySoundType.ContainsKey(soundGroupName) && !(aSource.clip == null))
		{
			DuckGroupInfo duckGroupInfo = instance.duckingBySoundType[soundGroupName];
			float length = aSource.clip.length;
			float pitch = aSource.pitch;
			List<PlaylistController> instances = PlaylistController.Instances;
			for (int i = 0; i < instances.Count; i++)
			{
				instances[i].DuckMusicForTime(actorUpdater, length, duckGroupInfo.unduckTime, pitch, duckGroupInfo.riseVolStart, duckGroupInfo.duckedVolumeCut);
			}
		}
	}

	private static void StopPauseOrUnpauseSoundsOfTransform(Transform trans, List<AudioInfo> varList, VariationCommand varCmd)
	{
		MasterAudioGroup masterAudioGroup = null;
		for (int i = 0; i < varList.Count; i++)
		{
			SoundGroupVariation variation = varList[i].Variation;
			if (variation.WasTriggeredFromTransform(trans))
			{
				if (masterAudioGroup == null)
				{
					masterAudioGroup = GrabGroup(variation.ParentGroup.GameObjectName);
				}
				bool stopEndDetection = masterAudioGroup != null && masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain;
				switch (varCmd)
				{
				case VariationCommand.Stop:
					variation.Stop(stopEndDetection);
					break;
				case VariationCommand.Pause:
					variation.Pause();
					break;
				case VariationCommand.Unpause:
					variation.Unpause();
					break;
				}
			}
		}
	}

	public static void StopAllSoundsOfTransform(Transform sourceTrans)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		Instance.VariationsStartedDuringMultiStop.Clear();
		Instance._isStoppingMultiple = true;
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Stop);
		}
		Instance._isStoppingMultiple = false;
	}

	public static void StopSoundGroupOfTransform(Transform sourceTrans, string sType)
	{
		if (SceneHasMasterAudio && !(sourceTrans == null))
		{
			if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
			{
				Debug.LogWarning("Could not locate group '" + sType + "'.");
				return;
			}
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Stop);
		}
	}

	public static void PauseAllSoundsOfTransform(Transform sourceTrans)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Pause);
		}
	}

	public static void PauseSoundGroupOfTransform(Transform sourceTrans, string sType)
	{
		if (SceneHasMasterAudio && !(sourceTrans == null))
		{
			if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
			{
				Debug.LogWarning("Could not locate group '" + sType + "'.");
				return;
			}
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Pause);
		}
	}

	public static void UnpauseAllSoundsOfTransform(Transform sourceTrans)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Unpause);
		}
	}

	public static void UnpauseSoundGroupOfTransform(Transform sourceTrans, string sType)
	{
		if (SceneHasMasterAudio && !(sourceTrans == null))
		{
			if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
			{
				Debug.LogWarning("Could not locate group '" + sType + "'.");
				return;
			}
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			StopPauseOrUnpauseSoundsOfTransform(sourceTrans, sources, VariationCommand.Unpause);
		}
	}

	public static void FadeOutAllSoundsOfTransform(Transform sourceTrans, float fadeTime)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		List<SoundGroupVariation> allPlayingVariationsOfTransform = GetAllPlayingVariationsOfTransform(sourceTrans);
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < allPlayingVariationsOfTransform.Count; i++)
		{
			string gameObjectName = allPlayingVariationsOfTransform[i].ParentGroup.GameObjectName;
			if (!hashSet.Contains(gameObjectName))
			{
				hashSet.Add(gameObjectName);
				FadeOutSoundGroupOfTransform(sourceTrans, gameObjectName, fadeTime);
			}
		}
	}

	public static void FadeOutSoundGroupOfTransform(Transform sourceTrans, string sType, float fadeTime)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		for (int i = 0; i < sources.Count; i++)
		{
			SoundGroupVariation variation = sources[i].Variation;
			if (variation.WasTriggeredFromTransform(sourceTrans))
			{
				variation.FadeOutNowAndStop(fadeTime);
			}
		}
	}

	public static void StopAllOfSound(string sType)
	{
		if (!SceneHasMasterAudio || sType == "_VideoPlayers")
		{
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		bool stopEndDetection = masterAudioGroup != null && masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain;
		foreach (AudioInfo item in sources)
		{
			if (!(item.Variation == null) && !IsLinkedGroupPlay(item.Variation))
			{
				item.Variation.Stop(stopEndDetection);
			}
		}
	}

	public static void FadeOutAllOfSound(string sType, float fadeTime)
	{
		if (!SceneHasMasterAudio)
		{
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		foreach (AudioInfo source in Instance.AudioSourcesBySoundType[sType].Sources)
		{
			source.Variation.FadeOutNowAndStop(fadeTime);
		}
	}

	public static List<SoundGroupVariation> GetAllPlayingVariations()
	{
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				SoundGroupVariation variation = sources[i].Variation;
				if (variation.IsPlaying)
				{
					list.Add(variation);
				}
			}
		}
		return list;
	}

	public static List<SoundGroupVariation> GetAllPlayingVariationsOfTransform(Transform sourceTrans)
	{
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return list;
		}
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				SoundGroupVariation variation = sources[i].Variation;
				if (variation.WasTriggeredFromTransform(sourceTrans))
				{
					list.Add(variation);
				}
			}
		}
		return list;
	}

	public static List<SoundGroupVariation> GetAllPlayingVariationsOfTransformList(List<Transform> sourceTransList)
	{
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		if (!SceneHasMasterAudio)
		{
			return list;
		}
		HashSet<Transform> hashSet = new HashSet<Transform>();
		for (int i = 0; i < sourceTransList.Count; i++)
		{
			hashSet.Add(sourceTransList[i]);
		}
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			for (int j = 0; j < sources.Count; j++)
			{
				SoundGroupVariation variation = sources[j].Variation;
				if (variation.WasTriggeredFromAnyOfTransformMap(hashSet))
				{
					list.Add(variation);
				}
			}
		}
		return list;
	}

	public static List<SoundGroupVariation> GetAllPlayingVariationsInBus(string busName)
	{
		int busIndex = GetBusIndex(busName, alertMissing: false);
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		if (busIndex < 0)
		{
			return list;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			if (audioGroupInfo.Group.busIndex != busIndex)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if (variation.IsPlaying)
				{
					list.Add(variation);
				}
			}
		}
		return list;
	}

	public static void DeleteGroupVariation(string sType, string variationName)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot delete Variation clip yet.");
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		Instance.GroupsToDelete.Clear();
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			AudioInfo audioInfo = audioGroupInfo.Sources[i];
			if (!(audioInfo.Variation.GameObjectName != variationName))
			{
				Instance.GroupsToDelete.Add(audioInfo);
			}
		}
		if (Instance.GroupsToDelete.Count == 0)
		{
			LogWarning("Could not find Variation for '" + sType + "' Group named '" + variationName + "'.\nWill not delete any Variations.");
			return;
		}
		for (int j = 0; j < Instance.GroupsToDelete.Count; j++)
		{
			AudioInfo audioInfo2 = Instance.GroupsToDelete[j];
			SoundGroupVariation variation = audioInfo2.Variation;
			variation.Stop();
			variation.DisableUpdater();
			switch (variation.audLocation)
			{
			case AudioLocation.ResourceFile:
				AudioResourceOptimizer.DeleteAudioSourceFromList((variation.VarAudio.clip == null) ? string.Empty : variation.VarAudio.clip.CachedName(), variation.VarAudio);
				break;
			case AudioLocation.Addressable:
				if (AudioAddressableOptimizer.IsAddressableValid(variation.audioClipAddressable))
				{
					AudioAddressableOptimizer.RemoveAddressablePlayingClip(variation.audioClipAddressable, variation.VarAudio);
				}
				break;
			}
			int num = audioGroupInfo.Sources.IndexOf(audioInfo2);
			if (num >= 0)
			{
				Instance._randomizer[sType].Remove(num);
				for (int k = 0; k < Instance._randomizer[sType].Count; k++)
				{
					if (Instance._randomizer[sType][k] > num)
					{
						Instance._randomizer[sType][k]--;
					}
				}
				Instance._randomizerOrigin[sType].Remove(num);
				for (int l = 0; l < Instance._randomizerOrigin[sType].Count; l++)
				{
					if (Instance._randomizerOrigin[sType][l] > num)
					{
						Instance._randomizerOrigin[sType][l]--;
					}
				}
				Instance._randomizerLeftovers[sType].Remove(num);
				for (int m = 0; m < Instance._randomizerLeftovers[sType].Count; m++)
				{
					if (Instance._randomizerLeftovers[sType][m] > num)
					{
						Instance._randomizerLeftovers[sType][m]--;
					}
				}
				Instance._clipsPlayedBySoundTypeOldestFirst[sType].Remove(num);
				audioGroupInfo.Sources.RemoveAt(num);
			}
			Instance.OcclusionSourcesInRange.Remove(variation.GameObj);
			Instance.OcclusionSourcesOutOfRange.Remove(variation.GameObj);
			Instance.OcclusionSourcesBlocked.Remove(variation.GameObj);
			RemoveFromOcclusionFrequencyTransitioning(variation);
			Instance.AllAudioSources.Remove(variation.VarAudio);
			UnityEngine.Object.Destroy(variation.GameObj);
		}
	}

	public static void CreateGroupVariationFromClip(string sType, AudioClip clip, string variationName, float volume = 1f, float pitch = 1f)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot create change variation clip yet.");
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		bool flag = false;
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			if (!(audioGroupInfo.Sources[i].Variation.GameObjectName != variationName))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			LogWarning("You already have a Variation for this Group named '" + variationName + "'. \n\nPlease rename these Variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
		}
		GameObject obj = UnityEngine.Object.Instantiate(Instance.soundGroupVariationTemplate.gameObject, audioGroupInfo.Group.transform.position, Quaternion.identity);
		obj.transform.name = variationName;
		obj.transform.parent = audioGroupInfo.Group.transform;
		AudioSource component = obj.GetComponent<AudioSource>();
		component.clip = clip;
		component.pitch = pitch;
		Instance.AllAudioSources.Add(component);
		SoundGroupVariation component2 = obj.GetComponent<SoundGroupVariation>();
		component2.DisableUpdater();
		AudioInfo item = new AudioInfo(component2, component2.VarAudio, volume);
		audioGroupInfo.Sources.Add(item);
		if (Instance._randomizer.ContainsKey(sType))
		{
			int item2 = audioGroupInfo.Sources.Count - 1;
			Instance._randomizer[sType].Add(item2);
			Instance._randomizerOrigin[sType].Add(item2);
			Instance._randomizerLeftovers[sType].Add(audioGroupInfo.Sources.Count - 1);
		}
	}

	public static void ChangeVariationPitch(string sType, bool changeAllVariations, string variationName, float pitch)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot change variation clip yet.");
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		int num = 0;
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			AudioInfo audioInfo = audioGroupInfo.Sources[i];
			if (changeAllVariations || !(audioInfo.Variation.GameObjectName != variationName))
			{
				audioInfo.Variation.original_pitch = pitch;
				AudioSource varAudio = audioInfo.Variation.VarAudio;
				if (varAudio != null)
				{
					varAudio.pitch = pitch;
				}
				num++;
			}
		}
		if (num == 0 && !changeAllVariations)
		{
			Debug.Log("Could not find any matching variations of Sound Group '" + sType + "' to change the pitch of.");
		}
	}

	public static void ChangeVariationVolume(string sType, bool changeAllVariations, string variationName, float volume)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot change variation clip yet.");
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		int num = 0;
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			AudioInfo audioInfo = audioGroupInfo.Sources[i];
			if (changeAllVariations || !(audioInfo.Variation.GameObjectName != variationName))
			{
				audioInfo.OriginalVolume = volume;
				num++;
			}
		}
		if (num == 0 && !changeAllVariations)
		{
			Debug.Log("Could not find any matching variations of Sound Group '" + sType + "' to change the volume of.");
		}
	}

	public static void ChangeVariationClipFromResources(string sType, bool changeAllVariations, string variationName, string resourceFileName)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot create change variation clip yet.");
			return;
		}
		AudioClip audioClip = Resources.Load(resourceFileName) as AudioClip;
		if (audioClip == null)
		{
			LogWarning("Resource file '" + resourceFileName + "' could not be located.");
		}
		else
		{
			ChangeVariationClip(sType, changeAllVariations, variationName, audioClip);
		}
	}

	public static void ChangeVariationClip(string sType, bool changeAllVariations, string variationName, AudioClip clip)
	{
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot create change variation clip yet.");
			return;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			AudioInfo audioInfo = audioGroupInfo.Sources[i];
			if (changeAllVariations || audioInfo.Variation.GameObjectName == variationName)
			{
				if (audioInfo.Variation.IsPlaying)
				{
					audioInfo.Variation.Stop();
				}
				audioInfo.Source.clip = clip;
			}
		}
	}

	public static void GradualOcclusionFreqChange(SoundGroupVariation variation, float fadeTime, float newCutoffFreq)
	{
		if (IsOcclusionFrequencyTransitioning(variation))
		{
			LogWarning("Occlusion is already fading for: " + variation.GameObjectName + ". This is a bug.");
			return;
		}
		OcclusionFreqChangeInfo occlusionFreqChangeInfo = null;
		for (int i = 0; i < Instance.VariationOcclusionFreqChanges.Count; i++)
		{
			OcclusionFreqChangeInfo occlusionFreqChangeInfo2 = Instance.VariationOcclusionFreqChanges[i];
			if (!occlusionFreqChangeInfo2.IsActive)
			{
				occlusionFreqChangeInfo = occlusionFreqChangeInfo2;
				break;
			}
		}
		if (occlusionFreqChangeInfo == null)
		{
			occlusionFreqChangeInfo = new OcclusionFreqChangeInfo();
			Instance.VariationOcclusionFreqChanges.Add(occlusionFreqChangeInfo);
		}
		occlusionFreqChangeInfo.ActingVariation = variation;
		occlusionFreqChangeInfo.CompletionTime = Time.realtimeSinceStartup + fadeTime;
		occlusionFreqChangeInfo.IsActive = true;
		occlusionFreqChangeInfo.StartFrequency = variation.LowPassFilter.cutoffFrequency;
		occlusionFreqChangeInfo.StartTime = Time.realtimeSinceStartup;
		occlusionFreqChangeInfo.TargetFrequency = newCutoffFreq;
	}

	public static AudioSource GetNextVariationForSoundGroup(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType, logIfMissing: false);
		if (masterAudioGroup == null || AppIsShuttingDown)
		{
			return null;
		}
		if (masterAudioGroup.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
		{
			Debug.LogWarning("Cannot determine the next Variation of randomly sequenced Sound Group '" + sType + "'.");
			return null;
		}
		if (!Instance._randomizer.ContainsKey(sType))
		{
			Debug.Log("Sound Group '" + sType + "' has no active Variations.");
			return null;
		}
		List<int> list = Instance._randomizer[sType];
		return Instance.AudioSourcesBySoundType[sType].Sources[list[0]].Source;
	}

	public static bool IsSoundGroupPlaying(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType, logIfMissing: false);
		if (masterAudioGroup == null || AppIsShuttingDown)
		{
			return false;
		}
		return masterAudioGroup.ActiveVoices > 0;
	}

	public static bool IsTransformPlayingSoundGroup(string sType, Transform sourceTrans)
	{
		if (!SceneHasMasterAudio)
		{
			return false;
		}
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogWarning("Could not locate group '" + sType + "'.");
			return false;
		}
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		for (int i = 0; i < sources.Count; i++)
		{
			if (sources[i].Variation.WasTriggeredFromTransform(sourceTrans))
			{
				return true;
			}
		}
		return false;
	}

	public static void RouteGroupToBus(string sType, string busName)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			LogError("Could not find Sound Group '" + sType + "'");
			return;
		}
		int num = 0;
		if (busName != null)
		{
			int num2 = GroupBuses.FindIndex((GroupBus x) => x.busName == busName);
			if (num2 < 0)
			{
				LogError("Could not find bus '" + busName + "' to assign to Sound Group '" + sType + "'");
				return;
			}
			num = 2 + num2;
		}
		GroupBus busByIndex = GetBusByIndex(masterAudioGroup.busIndex);
		masterAudioGroup.busIndex = num;
		GroupBus groupBus = null;
		bool flag = false;
		if (num > 0)
		{
			groupBus = GroupBuses.Find((GroupBus x) => x.busName == busName);
			if (groupBus.isMuted)
			{
				MuteGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
				flag = true;
			}
			else if (groupBus.isSoloed)
			{
				SoloGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
				flag = true;
			}
		}
		bool flag2 = false;
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		for (int num3 = 0; num3 < sources.Count; num3++)
		{
			SoundGroupVariation variation = sources[num3].Variation;
			variation.SetMixerGroup();
			variation.SetSpatialBlend();
			if (variation.IsPlaying)
			{
				groupBus?.AddActiveAudioSourceId(variation.InstanceId);
				busByIndex?.RemoveActiveAudioSourceId(variation.InstanceId);
				flag2 = true;
			}
		}
		if (flag2)
		{
			SetBusVolume(groupBus, groupBus?.volume ?? 0f);
		}
		if (Application.isPlaying & flag)
		{
			SilenceOrUnsilenceGroupsFromSoloChange();
		}
	}

	public static float GetVariationLength(string sType, string variationName)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			return -1f;
		}
		SoundGroupVariation soundGroupVariation = null;
		foreach (SoundGroupVariation groupVariation in masterAudioGroup.groupVariations)
		{
			if (!(groupVariation.GameObjectName != variationName))
			{
				soundGroupVariation = groupVariation;
				break;
			}
		}
		if (soundGroupVariation == null)
		{
			LogError("Could not find Variation '" + variationName + "' in Sound Group '" + sType + "'.");
			return -1f;
		}
		switch (soundGroupVariation.audLocation)
		{
		case AudioLocation.ResourceFile:
			LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' length cannot be determined because it's a Resource Files.");
			return -1f;
		case AudioLocation.Addressable:
			LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' length cannot be determined because it's an Addressable.");
			return -1f;
		default:
		{
			AudioClip clip = soundGroupVariation.VarAudio.clip;
			if (clip == null)
			{
				LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' has no Audio Clip.");
				return -1f;
			}
			if (!(soundGroupVariation.VarAudio.pitch <= 0f))
			{
				return AudioUtil.AdjustAudioClipDurationForPitch(clip.length, soundGroupVariation.VarAudio);
			}
			LogError("Variation '" + variationName + "' in Sound Group '" + sType + "' has negative or zero pitch. Cannot compute length.");
			return -1f;
		}
		}
	}

	public static void RefillSoundGroupPool(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType, logIfMissing: false);
		if (masterAudioGroup == null)
		{
			return;
		}
		List<int> list = Instance._randomizer[sType];
		List<int> list2 = Instance._clipsPlayedBySoundTypeOldestFirst[sType];
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int item = list[i];
				if (!list2.Contains(item))
				{
					list2.Add(item);
				}
			}
		}
		List<int> list3 = Instance._randomizerOrigin[sType];
		if (list2.Count < list3.Count)
		{
			for (int j = 0; j < list3.Count; j++)
			{
				int item2 = list3[j];
				if (!list2.Contains(item2))
				{
					list2.Add(item2);
				}
			}
		}
		list.Clear();
		if (masterAudioGroup.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
		{
			int? num = null;
			if (masterAudioGroup.UsesNoRepeat && list2.Count > 0)
			{
				num = list2[list2.Count - 1];
			}
			ArrayListUtil.SortIntArray(ref list2);
			if (num.HasValue && num.Value == list2[0])
			{
				int item3 = list2[0];
				list2.RemoveAt(0);
				list2.Insert(UnityEngine.Random.Range(1, list2.Count), item3);
			}
		}
		list.AddRange(list2);
		Instance._randomizerLeftovers[sType].AddRange(list2);
		list2.Clear();
		if (masterAudioGroup.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain)
		{
			masterAudioGroup.ChainLoopCount++;
		}
	}

	public static bool SoundGroupExists(string sType)
	{
		return GrabGroup(sType, logIfMissing: false) != null;
	}

	public static void PauseSoundGroup(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null) && !(masterAudioGroup.GameObjectName == "_VideoPlayers"))
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				sources[i].Variation.Pause();
			}
		}
	}

	public static void SetGroupSpatialBlend(string sType)
	{
		if (!(GrabGroup(sType) == null))
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				sources[i].Variation.SetSpatialBlend();
			}
		}
	}

	public static void RouteGroupToUnityMixerGroup(string sType, AudioMixerGroup mixerGroup)
	{
		if (Application.isPlaying && !(GrabGroup(sType, logIfMissing: false) == null))
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				sources[i].Variation.VarAudio.outputAudioMixerGroup = mixerGroup;
			}
		}
	}

	public static void UnpauseSoundGroup(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null) && !(masterAudioGroup.GameObjectName == "_VideoPlayers"))
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				sources[i].Variation.Unpause();
			}
		}
	}

	public static void FadeSoundGroupToVolume(string sType, float newVolume, float fadeTime, Action completionCallback = null, bool willStopAfterFade = false, bool willResetVolumeAfterFade = false)
	{
		if (newVolume < 0f || newVolume > 1f)
		{
			Debug.LogError("Illegal volume passed to FadeSoundGroupToVolume: '" + newVolume + "'. Legal volumes are between 0 and 1");
			return;
		}
		if (fadeTime <= 0.1f)
		{
			SetGroupVolume(sType, newVolume);
			completionCallback?.Invoke();
			if (willStopAfterFade)
			{
				StopAllOfSound(sType);
			}
			return;
		}
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			return;
		}
		if (newVolume < 0f || newVolume > 1f)
		{
			Debug.Log("Cannot fade Sound Group '" + sType + "'. Invalid volume specified. Volume should be between 0 and 1.");
			return;
		}
		for (int i = 0; i < Instance.GroupFades.Count; i++)
		{
			GroupFadeInfo groupFadeInfo = Instance.GroupFades[i];
			if (groupFadeInfo.NameOfGroup == sType && groupFadeInfo.IsActive)
			{
				groupFadeInfo.IsActive = false;
				break;
			}
		}
		GroupFadeInfo groupFadeInfo2 = null;
		for (int j = 0; j < Instance.GroupFades.Count; j++)
		{
			GroupFadeInfo groupFadeInfo3 = Instance.GroupFades[j];
			if (!groupFadeInfo3.IsActive)
			{
				groupFadeInfo2 = groupFadeInfo3;
				break;
			}
		}
		if (groupFadeInfo2 == null)
		{
			groupFadeInfo2 = new GroupFadeInfo();
			Instance.GroupFades.Add(groupFadeInfo2);
		}
		groupFadeInfo2.NameOfGroup = sType;
		groupFadeInfo2.ActingGroup = masterAudioGroup;
		groupFadeInfo2.StartTime = AudioUtil.Time;
		groupFadeInfo2.CompletionTime = AudioUtil.Time + fadeTime;
		groupFadeInfo2.StartVolume = masterAudioGroup.groupMasterVolume;
		groupFadeInfo2.TargetVolume = newVolume;
		groupFadeInfo2.WillStopGroupAfterFade = willStopAfterFade;
		groupFadeInfo2.WillResetVolumeAfterFade = willResetVolumeAfterFade;
		groupFadeInfo2.IsActive = true;
		if (completionCallback != null)
		{
			groupFadeInfo2.completionAction = completionCallback;
		}
	}

	public static void FadeOutOldSoundGroupVoices(string sType, float minimumPlayTime, float fadeTime)
	{
		if (!SceneHasMasterAudio || !Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			return;
		}
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		for (int i = 0; i < sources.Count; i++)
		{
			SoundGroupVariation variation = sources[i].Variation;
			if ((variation.IsPaused || variation.IsPlaying) && !(AudioUtil.Time - variation.LastTimePlayed <= minimumPlayTime))
			{
				if (fadeTime <= 0f)
				{
					variation.Stop();
				}
				else
				{
					variation.FadeOutNowAndStop(fadeTime);
				}
			}
		}
	}

	public static void StopOldSoundGroupVoices(string sType, float minimumPlayTime)
	{
		if (!SceneHasMasterAudio || !Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			return;
		}
		List<AudioInfo> sources = Instance.AudioSourcesBySoundType[sType].Sources;
		for (int i = 0; i < sources.Count; i++)
		{
			SoundGroupVariation variation = sources[i].Variation;
			if ((variation.IsPaused || variation.IsPlaying) && !(AudioUtil.Time - variation.LastTimePlayed <= minimumPlayTime))
			{
				variation.Stop();
			}
		}
	}

	public static void GlideSoundGroupByPitch(string sType, float pitchAddition, float glideTime, Action completionCallback = null)
	{
		if (pitchAddition < -3f || pitchAddition > 3f)
		{
			Debug.LogError("Illegal pitch passed to GlideSoundGroupByPitch: '" + pitchAddition + "'. Legal pitches are between -3 and 3");
			return;
		}
		if (pitchAddition == 0f)
		{
			completionCallback?.Invoke();
			return;
		}
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			return;
		}
		for (int i = 0; i < Instance.GroupPitchGlides.Count; i++)
		{
			GroupPitchGlideInfo groupPitchGlideInfo = Instance.GroupPitchGlides[i];
			if (groupPitchGlideInfo.NameOfGroup == sType && groupPitchGlideInfo.IsActive)
			{
				groupPitchGlideInfo.IsActive = false;
				if (groupPitchGlideInfo.completionAction != null)
				{
					groupPitchGlideInfo.completionAction();
				}
				break;
			}
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		if (glideTime <= 0.1f)
		{
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				audioGroupInfo.Sources[j].Variation.GlideByPitch(pitchAddition, 0f);
			}
			completionCallback?.Invoke();
			return;
		}
		if (completionCallback == null)
		{
			completionCallback?.Invoke();
			return;
		}
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		for (int k = 0; k < audioGroupInfo.Sources.Count; k++)
		{
			SoundGroupVariation variation = audioGroupInfo.Sources[k].Variation;
			if (variation.IsPlaying)
			{
				if (variation.curPitchMode == SoundGroupVariation.PitchMode.Gliding)
				{
					variation.VariationUpdater.StopPitchGliding();
				}
				variation.GlideByPitch(pitchAddition, glideTime);
				list.Add(variation);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		GroupPitchGlideInfo groupPitchGlideInfo2 = null;
		for (int l = 0; l < Instance.GroupPitchGlides.Count; l++)
		{
			GroupPitchGlideInfo groupPitchGlideInfo3 = Instance.GroupPitchGlides[l];
			if (!groupPitchGlideInfo3.IsActive)
			{
				groupPitchGlideInfo2 = groupPitchGlideInfo3;
				break;
			}
		}
		if (groupPitchGlideInfo2 == null)
		{
			groupPitchGlideInfo2 = new GroupPitchGlideInfo();
			Instance.GroupPitchGlides.Add(groupPitchGlideInfo2);
		}
		groupPitchGlideInfo2.NameOfGroup = sType;
		groupPitchGlideInfo2.ActingGroup = masterAudioGroup;
		groupPitchGlideInfo2.CompletionTime = AudioUtil.Time + glideTime;
		groupPitchGlideInfo2.GlidingVariations.Clear();
		groupPitchGlideInfo2.GlidingVariations.AddRange(list);
		groupPitchGlideInfo2.completionAction = completionCallback;
	}

	public static void DeleteSoundGroup(string sType)
	{
		if (SafeInstance == null)
		{
			return;
		}
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			return;
		}
		StopAllOfSound(sType);
		Transform transform = masterAudioGroup.transform;
		MasterAudio instance = Instance;
		if (instance.duckingBySoundType.ContainsKey(sType))
		{
			instance.duckingBySoundType.Remove(sType);
		}
		Instance._randomizer.Remove(sType);
		Instance._randomizerLeftovers.Remove(sType);
		Instance._randomizerOrigin.Remove(sType);
		Instance._nonRandomChoices.Remove(sType);
		Instance._clipsPlayedBySoundTypeOldestFirst.Remove(sType);
		RemoveRuntimeGroupInfo(sType);
		Instance.LastTimeSoundGroupPlayed.Remove(sType);
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			AudioSource component = child.GetComponent<AudioSource>();
			SoundGroupVariation component2 = child.GetComponent<SoundGroupVariation>();
			switch (component2.audLocation)
			{
			case AudioLocation.ResourceFile:
				AudioResourceOptimizer.DeleteAudioSourceFromList(AudioResourceOptimizer.GetLocalizedFileName(component2.useLocalization, component2.resourceFileName), component);
				break;
			case AudioLocation.Addressable:
				if (!AudioAddressableOptimizer.IsAddressableValid(component2.audioClipAddressable))
				{
					AudioAddressableOptimizer.RemoveAddressablePlayingClip(component2.audioClipAddressable, component2.VarAudio);
				}
				break;
			}
		}
		transform.parent = null;
		UnityEngine.Object.Destroy(transform.gameObject);
		RescanGroupsNow();
	}

	public static Transform CreateSoundGroup(DynamicSoundGroup aGroup, int? creatorInstanceId, bool errorOnExisting = true)
	{
		if (!SceneHasMasterAudio)
		{
			return null;
		}
		if (!SoundsReady)
		{
			Debug.LogError("MasterAudio not finished initializing sounds. Cannot create new group yet.");
			return null;
		}
		string text = aGroup.transform.name;
		MasterAudio instance = Instance;
		if (Instance.AudioSourcesBySoundType.ContainsKey(text))
		{
			if (errorOnExisting)
			{
				Debug.LogError("Cannot add a new Sound Group named '" + text + "' because there is already a Sound Group of that name.");
			}
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(instance.soundGroupTemplate.gameObject, instance.Trans.position, Quaternion.identity);
		Transform transform = gameObject.transform;
		transform.name = UtilStrings.TrimSpace(text);
		transform.parent = Instance.Trans;
		transform.gameObject.layer = Instance.gameObject.layer;
		for (int i = 0; i < aGroup.groupVariations.Count; i++)
		{
			DynamicGroupVariation dynamicGroupVariation = aGroup.groupVariations[i];
			for (int j = 0; j < dynamicGroupVariation.weight; j++)
			{
				GameObject obj = UnityEngine.Object.Instantiate(dynamicGroupVariation.gameObject, transform.position, Quaternion.identity);
				obj.transform.parent = transform;
				obj.transform.gameObject.layer = transform.gameObject.layer;
				UnityEngine.Object.Destroy(obj.GetComponent<DynamicGroupVariation>());
				obj.AddComponent<SoundGroupVariation>();
				SoundGroupVariation component = obj.GetComponent<SoundGroupVariation>();
				string text2 = component.GameObjectName;
				int num = text2.IndexOf("(Clone)");
				if (num >= 0)
				{
					text2 = text2.Substring(0, num);
				}
				AudioSource component2 = dynamicGroupVariation.GetComponent<AudioSource>();
				switch (dynamicGroupVariation.audLocation)
				{
				case AudioLocation.Clip:
				{
					AudioClip clip = component2.clip;
					component.VarAudio.clip = clip;
					break;
				}
				case AudioLocation.ResourceFile:
					AudioResourceOptimizer.AddTargetForClip(AudioResourceOptimizer.GetLocalizedFileName(dynamicGroupVariation.useLocalization, dynamicGroupVariation.resourceFileName), component.VarAudio);
					component.resourceFileName = dynamicGroupVariation.resourceFileName;
					component.useLocalization = dynamicGroupVariation.useLocalization;
					break;
				case AudioLocation.Addressable:
					component.audioClipAddressable = dynamicGroupVariation.audioClipAddressable;
					break;
				}
				component.clipAlias = dynamicGroupVariation.clipAlias;
				component.audLocation = dynamicGroupVariation.audLocation;
				component.original_pitch = component2.pitch;
				component.transform.name = text2;
				component.isExpanded = dynamicGroupVariation.isExpanded;
				component.probabilityToPlay = dynamicGroupVariation.probabilityToPlay;
				component.isUninterruptible = dynamicGroupVariation.isUninterruptible;
				component.importance = dynamicGroupVariation.importance;
				component.useRandomPitch = dynamicGroupVariation.useRandomPitch;
				component.randomPitchMode = dynamicGroupVariation.randomPitchMode;
				component.randomPitchMin = dynamicGroupVariation.randomPitchMin;
				component.randomPitchMax = dynamicGroupVariation.randomPitchMax;
				component.useRandomVolume = dynamicGroupVariation.useRandomVolume;
				component.randomVolumeMode = dynamicGroupVariation.randomVolumeMode;
				component.randomVolumeMin = dynamicGroupVariation.randomVolumeMin;
				component.randomVolumeMax = dynamicGroupVariation.randomVolumeMax;
				component.useCustomLooping = dynamicGroupVariation.useCustomLooping;
				component.minCustomLoops = dynamicGroupVariation.minCustomLoops;
				component.maxCustomLoops = dynamicGroupVariation.maxCustomLoops;
				component.useFades = dynamicGroupVariation.useFades;
				component.fadeInTime = dynamicGroupVariation.fadeInTime;
				component.fadeOutTime = dynamicGroupVariation.fadeOutTime;
				component.useIntroSilence = dynamicGroupVariation.useIntroSilence;
				component.introSilenceMin = dynamicGroupVariation.introSilenceMin;
				component.introSilenceMax = dynamicGroupVariation.introSilenceMax;
				component.useRandomStartTime = dynamicGroupVariation.useRandomStartTime;
				component.randomStartMinPercent = dynamicGroupVariation.randomStartMinPercent;
				component.randomStartMaxPercent = dynamicGroupVariation.randomStartMaxPercent;
				component.randomEndPercent = dynamicGroupVariation.randomEndPercent;
				if (Instance.addResonanceAudioSources && ResonanceAudioHelper.DarkTonicResonanceAudioPackageInstalled())
				{
					ResonanceAudioHelper.AddResonanceAudioSourceToVariation(component);
				}
				else if (Instance.addOculusAudioSources && OculusAudioHelper.DarkTonicOculusAudioPackageInstalled())
				{
					OculusAudioHelper.AddOculusAudioSourceToVariation(component);
				}
				if (component.LowPassFilter != null && !component.LowPassFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.LowPassFilter);
				}
				if (component.HighPassFilter != null && !component.HighPassFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.HighPassFilter);
				}
				if (component.DistortionFilter != null && !component.DistortionFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.DistortionFilter);
				}
				if (component.ChorusFilter != null && !component.ChorusFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.ChorusFilter);
				}
				if (component.EchoFilter != null && !component.EchoFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.EchoFilter);
				}
				if (component.ReverbFilter != null && !component.ReverbFilter.enabled)
				{
					UnityEngine.Object.Destroy(component.ReverbFilter);
				}
			}
		}
		MasterAudioGroup component3 = gameObject.GetComponent<MasterAudioGroup>();
		component3.retriggerPercentage = aGroup.retriggerPercentage;
		if (creatorInstanceId.HasValue)
		{
			component3.AddActorInstanceId(creatorInstanceId.Value);
		}
		float? groupVolume = PersistentAudioSettings.GetGroupVolume(aGroup.name);
		component3.OriginalVolume = aGroup.groupMasterVolume;
		if (groupVolume.HasValue)
		{
			component3.groupMasterVolume = groupVolume.Value;
		}
		else
		{
			component3.groupMasterVolume = aGroup.groupMasterVolume;
		}
		component3.addressableUnusedSecondsLifespan = aGroup.addressableUnusedSecondsLifespan;
		component3.useClipAgePriority = aGroup.useClipAgePriority;
		component3.limitMode = aGroup.limitMode;
		component3.limitPerXFrames = aGroup.limitPerXFrames;
		component3.minimumTimeBetween = aGroup.minimumTimeBetween;
		component3.limitPolyphony = aGroup.limitPolyphony;
		component3.voiceLimitCount = aGroup.voiceLimitCount;
		component3.curVariationSequence = aGroup.curVariationSequence;
		component3.useInactivePeriodPoolRefill = aGroup.useInactivePeriodPoolRefill;
		component3.inactivePeriodSeconds = aGroup.inactivePeriodSeconds;
		component3.curVariationMode = aGroup.curVariationMode;
		component3.useNoRepeatRefill = aGroup.useNoRepeatRefill;
		component3.useDialogFadeOut = aGroup.useDialogFadeOut;
		component3.dialogFadeOutTime = aGroup.dialogFadeOutTime;
		component3.groupPlayType = aGroup.groupPlayType;
		component3.isUninterruptible = aGroup.isUninterruptible;
		component3.importance = aGroup.importance;
		component3.isUsingOcclusion = aGroup.isUsingOcclusion;
		component3.willOcclusionOverrideRaycastOffset = aGroup.willOcclusionOverrideRaycastOffset;
		component3.occlusionRayCastOffset = aGroup.occlusionRayCastOffset;
		component3.willOcclusionOverrideFrequencies = aGroup.willOcclusionOverrideFrequencies;
		component3.occlusionMaxCutoffFreq = aGroup.occlusionMaxCutoffFreq;
		component3.occlusionMinCutoffFreq = aGroup.occlusionMinCutoffFreq;
		component3.chainLoopDelayMin = aGroup.chainLoopDelayMin;
		component3.chainLoopDelayMax = aGroup.chainLoopDelayMax;
		component3.chainLoopMode = aGroup.chainLoopMode;
		component3.chainLoopNumLoops = aGroup.chainLoopNumLoops;
		component3.expandLinkedGroups = aGroup.expandLinkedGroups;
		component3.childSoundGroups = aGroup.childSoundGroups;
		component3.endLinkedGroups = aGroup.endLinkedGroups;
		component3.linkedStartGroupSelectionType = aGroup.linkedStartGroupSelectionType;
		component3.linkedStopGroupSelectionType = aGroup.linkedStopGroupSelectionType;
		component3.soundPlayedEventActive = aGroup.soundPlayedEventActive;
		component3.soundPlayedCustomEvent = aGroup.soundPlayedCustomEvent;
		component3.targetDespawnedBehavior = aGroup.targetDespawnedBehavior;
		component3.despawnFadeTime = aGroup.despawnFadeTime;
		component3.logSound = aGroup.logSound;
		component3.alwaysHighestPriority = aGroup.alwaysHighestPriority;
		component3.spatialBlendType = aGroup.spatialBlendType;
		component3.spatialBlend = aGroup.spatialBlend;
		List<AudioInfo> list = new List<AudioInfo>();
		List<int> list2 = new List<int>();
		for (int k = 0; k < gameObject.transform.childCount; k++)
		{
			list2.Add(k);
			Transform child = gameObject.transform.GetChild(k);
			AudioSource component4 = child.GetComponent<AudioSource>();
			SoundGroupVariation component = child.GetComponent<SoundGroupVariation>();
			list.Add(new AudioInfo(component, component4, component4.volume));
			component.DisableUpdater();
		}
		AddRuntimeGroupInfo(text, new AudioGroupInfo(list, component3));
		if (component3.curVariationSequence == MasterAudioGroup.VariationSequence.Randomized)
		{
			ArrayListUtil.SortIntArray(ref list2);
		}
		Instance._randomizer.Add(text, list2);
		List<int> list3 = new List<int>(list2.Count);
		list3.AddRange(list2);
		Instance._randomizerOrigin.Add(text, list3);
		Instance._randomizerLeftovers.Add(text, new List<int>(list2.Count));
		Instance._randomizerLeftovers[text].AddRange(list2);
		Instance._clipsPlayedBySoundTypeOldestFirst.Add(text, new List<int>(list2.Count));
		Instance._nonRandomChoices.Add(text, new List<int>());
		RescanGroupsNow();
		if (string.IsNullOrEmpty(aGroup.busName))
		{
			return transform;
		}
		component3.busIndex = GetBusIndex(aGroup.busName, alertMissing: true);
		if (component3.BusForGroup != null && component3.BusForGroup.isMuted)
		{
			MuteGroup(component3.GameObjectName, shouldCheckMuteStatus: false);
		}
		else if (Instance.mixerMuted)
		{
			MuteGroup(component3.GameObjectName, shouldCheckMuteStatus: false);
		}
		return transform;
	}

	public static float GetGroupVolume(string sType)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (masterAudioGroup == null)
		{
			return 0f;
		}
		return masterAudioGroup.groupMasterVolume;
	}

	public static void SetGroupVolume(string sType, float volumeLevel)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType, Application.isPlaying);
		if (masterAudioGroup == null || AppIsShuttingDown)
		{
			return;
		}
		masterAudioGroup.groupMasterVolume = volumeLevel;
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		float busVolume = GetBusVolume(masterAudioGroup);
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			AudioInfo audioInfo = audioGroupInfo.Sources[i];
			AudioSource source = audioInfo.Source;
			if (!(source == null))
			{
				float volume = ((!audioInfo.Variation.useRandomVolume || audioInfo.Variation.randomVolumeMode != SoundGroupVariation.RandomVolumeMode.AddToClipVolume) ? (audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * masterAudioGroup.groupMasterVolume * busVolume * Instance._masterAudioVolume) : (audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * masterAudioGroup.groupMasterVolume * busVolume * Instance._masterAudioVolume + audioInfo.LastRandomVolume));
				source.volume = volume;
			}
		}
	}

	public static void MuteGroup(string sType, bool shouldCheckMuteStatus = true)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null))
		{
			Instance.SoloedGroups.Remove(masterAudioGroup);
			masterAudioGroup.isSoloed = false;
			SetGroupMuteStatus(masterAudioGroup, sType, isMute: true);
			if (shouldCheckMuteStatus)
			{
				SilenceOrUnsilenceGroupsFromSoloChange();
			}
		}
	}

	public static void UnmuteGroup(string sType, bool shouldCheckMuteStatus = true)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null))
		{
			SetGroupMuteStatus(masterAudioGroup, sType, isMute: false);
			if (shouldCheckMuteStatus)
			{
				SilenceOrUnsilenceGroupsFromSoloChange();
			}
		}
	}

	private static void AddRuntimeGroupInfo(string groupName, AudioGroupInfo groupInfo)
	{
		Instance.AudioSourcesBySoundType.Add(groupName, groupInfo);
		Instance.AllSoundGroupNames.Add(groupName);
		List<AudioSource> list = new List<AudioSource>(groupInfo.Sources.Count);
		for (int i = 0; i < groupInfo.Sources.Count; i++)
		{
			list.Add(groupInfo.Sources[i].Source);
		}
		TrackRuntimeAudioSources(list);
	}

	private static void FireAudioSourcesNumberChangedEvent()
	{
		if (NumberOfAudioSourcesChanged != null)
		{
			NumberOfAudioSourcesChanged();
		}
	}

	public static void TrackRuntimeAudioSources(List<AudioSource> sources)
	{
		bool flag = false;
		for (int i = 0; i < sources.Count; i++)
		{
			AudioSource item = sources[i];
			if (!Instance.AllAudioSources.Contains(item))
			{
				Instance.AllAudioSources.Add(item);
				flag = true;
			}
		}
		if (flag)
		{
			FireAudioSourcesNumberChangedEvent();
		}
	}

	public static void StopTrackingRuntimeAudioSources(List<AudioSource> sources)
	{
		if (AppIsShuttingDown || SafeInstance == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < sources.Count; i++)
		{
			AudioSource item = sources[i];
			if (Instance.AllAudioSources.Contains(item))
			{
				Instance.AllAudioSources.Remove(item);
				flag = true;
			}
		}
		if (flag)
		{
			FireAudioSourcesNumberChangedEvent();
		}
	}

	private static void RemoveRuntimeGroupInfo(string groupName)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(groupName);
		if (masterAudioGroup != null)
		{
			List<AudioSource> list = new List<AudioSource>(masterAudioGroup.groupVariations.Count);
			for (int i = 0; i < masterAudioGroup.groupVariations.Count; i++)
			{
				list.Add(masterAudioGroup.groupVariations[i].VarAudio);
			}
			StopTrackingRuntimeAudioSources(list);
		}
		Instance.AudioSourcesBySoundType.Remove(groupName);
		Instance.AllSoundGroupNames.Remove(groupName);
	}

	private static void RescanChildren(MasterAudioGroup group)
	{
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < group.transform.childCount; i++)
		{
			Transform child = group.transform.GetChild(i);
			if (!list2.Contains(child.name))
			{
				list2.Add(child.name);
				SoundGroupVariation component = child.GetComponent<SoundGroupVariation>();
				list.Add(component);
			}
		}
		group.groupVariations = list;
	}

	private static void SetGroupMuteStatus(MasterAudioGroup aGroup, string sType, bool isMute)
	{
		aGroup.isMuted = isMute;
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		for (int i = 0; i < audioGroupInfo.Sources.Count; i++)
		{
			audioGroupInfo.Sources[i].Source.mute = isMute;
		}
	}

	public static void SoloGroup(string sType, bool shouldCheckMuteStatus = true)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null) && !Instance.SoloedGroups.Contains(masterAudioGroup))
		{
			masterAudioGroup.isMuted = false;
			masterAudioGroup.isSoloed = true;
			Instance.SoloedGroups.Add(masterAudioGroup);
			SetGroupMuteStatus(masterAudioGroup, sType, isMute: false);
			if (shouldCheckMuteStatus)
			{
				SilenceOrUnsilenceGroupsFromSoloChange();
			}
		}
	}

	public static void SilenceOrUnsilenceGroupsFromSoloChange()
	{
		if (Instance.SoloedGroups.Count > 0)
		{
			SilenceNonSoloedGroups();
		}
		else
		{
			UnsilenceNonSoloedGroups();
		}
	}

	private static void UnsilenceNonSoloedGroups()
	{
		for (int i = 0; i < Instance.AllSoundGroupNames.Count; i++)
		{
			string key = Instance.AllSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			if (!audioGroupInfo.Group.isMuted)
			{
				UnsilenceGroup(audioGroupInfo);
			}
		}
	}

	private static void UnsilenceGroup(AudioGroupInfo grp)
	{
		for (int i = 0; i < grp.Sources.Count; i++)
		{
			grp.Sources[i].Source.mute = false;
		}
	}

	private static void SilenceNonSoloedGroups()
	{
		for (int i = 0; i < Instance.AllSoundGroupNames.Count; i++)
		{
			string key = Instance.AllSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			if (!audioGroupInfo.Group.isSoloed && !audioGroupInfo.Group.isMuted)
			{
				SilenceGroup(audioGroupInfo);
			}
		}
	}

	private static void SilenceGroup(AudioGroupInfo grp)
	{
		for (int i = 0; i < grp.Sources.Count; i++)
		{
			grp.Sources[i].Source.mute = true;
		}
	}

	public static void UnsoloGroup(string sType, bool shouldCheckMuteStatus = true)
	{
		MasterAudioGroup masterAudioGroup = GrabGroup(sType);
		if (!(masterAudioGroup == null))
		{
			masterAudioGroup.isSoloed = false;
			Instance.SoloedGroups.Remove(masterAudioGroup);
			if (shouldCheckMuteStatus)
			{
				SilenceOrUnsilenceGroupsFromSoloChange();
			}
		}
	}

	public static MasterAudioGroup GrabGroup(string sType, bool logIfMissing = true)
	{
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			if (logIfMissing)
			{
				Debug.LogError("Could not grab Sound Group '" + sType + "' because it does not exist in this scene.");
			}
			return null;
		}
		AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[sType];
		if (audioGroupInfo.Group == null)
		{
			Transform childTransform = Instance.Trans.GetChildTransform(sType);
			if (!(childTransform != null))
			{
				return null;
			}
			MasterAudioGroup component = childTransform.GetComponent<MasterAudioGroup>();
			audioGroupInfo.Group = component;
		}
		MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
		if (masterAudioGroup.groupVariations.Count == 0)
		{
			RescanChildren(masterAudioGroup);
		}
		return masterAudioGroup;
	}

	public static int VoicesForGroup(string sType)
	{
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			return -1;
		}
		return Instance.AudioSourcesBySoundType[sType].Sources.Count;
	}

	public static Transform FindGroupTransform(string sType)
	{
		if (SafeInstance != null)
		{
			Transform childTransform = Instance.Trans.GetChildTransform(sType);
			if (childTransform != null)
			{
				return childTransform;
			}
		}
		DynamicSoundGroupCreator[] array = UnityEngine.Object.FindObjectsOfType<DynamicSoundGroupCreator>();
		for (int i = 0; i < array.Count(); i++)
		{
			Transform childTransform = array[i].transform.GetChildTransform(sType);
			if (childTransform != null)
			{
				return childTransform;
			}
		}
		return null;
	}

	public static List<AudioInfo> GetAllVariationsOfGroup(string sType, bool logIfMissing = true)
	{
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			if (logIfMissing)
			{
				Debug.LogError("Could not grab Sound Group '" + sType + "' because it does not exist in this scene.");
			}
			return null;
		}
		return Instance.AudioSourcesBySoundType[sType].Sources;
	}

	public static AudioGroupInfo GetGroupInfo(string sType)
	{
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			return null;
		}
		return Instance.AudioSourcesBySoundType[sType];
	}

	public static void SubscribeToLastVariationPlayed(string sType, Action finishedCallback)
	{
		if (!Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Debug.LogError("Could not grab Sound Group '" + sType + "' because it does not exist in this scene.");
		}
		else
		{
			Instance.AudioSourcesBySoundType[sType].Group.SubscribeToLastVariationFinishedPlay(finishedCallback);
		}
	}

	public static void UnsubscribeFromLastVariationPlayed(string sType)
	{
		if (Instance.AudioSourcesBySoundType.ContainsKey(sType))
		{
			Instance.AudioSourcesBySoundType[sType].Group.UnsubscribeFromLastVariationFinishedPlay();
		}
	}

	public void SetSpatialBlendForMixer()
	{
		foreach (string allSoundGroupName in AllSoundGroupNames)
		{
			SetGroupSpatialBlend(allSoundGroupName);
		}
	}

	public static void PauseMixer()
	{
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			PauseSoundGroup(Instance.AudioSourcesBySoundType[allSoundGroupName].Group.GameObjectName);
		}
	}

	public static void UnpauseMixer()
	{
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			UnpauseSoundGroup(Instance.AudioSourcesBySoundType[allSoundGroupName].Group.GameObjectName);
		}
	}

	public static void StopMixer()
	{
		Instance.VariationsStartedDuringMultiStop.Clear();
		Instance._isStoppingMultiple = true;
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			StopAllOfSound(Instance.AudioSourcesBySoundType[allSoundGroupName].Group.GameObjectName);
		}
		Instance._isStoppingMultiple = false;
	}

	public static void UnsubscribeFromAllVariations()
	{
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			List<AudioInfo> sources = Instance.AudioSourcesBySoundType[allSoundGroupName].Sources;
			for (int i = 0; i < sources.Count; i++)
			{
				sources[i].Variation.ClearSubscribers();
			}
		}
	}

	public static void StopEverything()
	{
		StopMixer();
		StopAllPlaylists();
	}

	public static void PauseEverything()
	{
		PauseMixer();
		PauseAllPlaylists();
	}

	public static void UnpauseEverything()
	{
		UnpauseMixer();
		UnpauseAllPlaylists();
	}

	public static void MuteEverything()
	{
		MixerMuted = true;
		MuteAllPlaylists();
	}

	public static void UnmuteEverything()
	{
		MixerMuted = false;
		UnmuteAllPlaylists();
	}

	public static List<string> ListOfAudioClipsInGroupsEditTime()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Instance.transform.childCount; i++)
		{
			MasterAudioGroup component = Instance.transform.GetChild(i).GetComponent<MasterAudioGroup>();
			for (int j = 0; j < component.transform.childCount; j++)
			{
				SoundGroupVariation component2 = component.transform.GetChild(j).GetComponent<SoundGroupVariation>();
				string text = string.Empty;
				switch (component2.audLocation)
				{
				case AudioLocation.Clip:
				{
					AudioClip clip = component2.VarAudio.clip;
					if (clip != null)
					{
						text = clip.CachedName();
					}
					break;
				}
				case AudioLocation.ResourceFile:
					text = component2.resourceFileName;
					break;
				case AudioLocation.Addressable:
					text = "";
					break;
				}
				if (!string.IsNullOrEmpty(text) && !list.Contains(text))
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	private static int GetBusIndex(string busName, bool alertMissing)
	{
		if (!SceneHasMasterAudio)
		{
			return -1;
		}
		for (int i = 0; i < GroupBuses.Count; i++)
		{
			if (GroupBuses[i].busName == busName)
			{
				return i + 2;
			}
		}
		if (alertMissing)
		{
			LogWarning("Could not find bus '" + busName + "'.");
		}
		return -1;
	}

	private static GroupBus GetBusByIndex(int busIndex)
	{
		if (busIndex < 2)
		{
			return null;
		}
		return GroupBuses[busIndex - 2];
	}

	public static void ChangeBusPitch(string busName, float pitch)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				ChangeVariationPitch(masterAudioGroup.GameObjectName, changeAllVariations: true, string.Empty, pitch);
			}
		}
	}

	public static void MuteBus(string busName)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		GroupBus groupBus = GrabBusByName(busName);
		groupBus.isMuted = true;
		if (groupBus.isSoloed)
		{
			UnsoloBus(busName);
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				MuteGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
			}
		}
		if (Application.isPlaying)
		{
			SilenceOrUnsilenceGroupsFromSoloChange();
		}
	}

	public static void UnmuteBus(string busName, bool shouldCheckMuteStatus = true)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		GrabBusByName(busName).isMuted = false;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				UnmuteGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
			}
		}
		if (shouldCheckMuteStatus)
		{
			SilenceOrUnsilenceGroupsFromSoloChange();
		}
	}

	public static void ToggleMuteBus(string busName)
	{
		if (GetBusIndex(busName, alertMissing: true) >= 0)
		{
			if (GrabBusByName(busName).isMuted)
			{
				UnmuteBus(busName);
			}
			else
			{
				MuteBus(busName);
			}
		}
	}

	public static void PauseBus(string busName)
	{
		if (!SceneHasMasterAudio)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				PauseSoundGroup(masterAudioGroup.GameObjectName);
			}
		}
	}

	public static void SoloBus(string busName)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		GroupBus groupBus = GrabBusByName(busName);
		groupBus.isSoloed = true;
		if (groupBus.isMuted)
		{
			UnmuteBus(busName);
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				SoloGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
			}
		}
		if (Application.isPlaying)
		{
			SilenceOrUnsilenceGroupsFromSoloChange();
		}
	}

	public static void UnsoloBus(string busName, bool shouldCheckMuteStatus = true)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		GrabBusByName(busName).isSoloed = false;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				UnsoloGroup(masterAudioGroup.GameObjectName, shouldCheckMuteStatus: false);
			}
		}
		if (shouldCheckMuteStatus)
		{
			SilenceOrUnsilenceGroupsFromSoloChange();
		}
	}

	public static void RouteBusToUnityMixerGroup(string busName, AudioMixerGroup mixerGroup)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				RouteGroupToUnityMixerGroup(masterAudioGroup.GameObjectName, mixerGroup);
			}
		}
	}

	private static SoundGroupVariation FindLeastImportantSoundOnBus(GroupBus bus, MasterAudioGroup group)
	{
		int busIndex = GetBusIndex(bus.busName, alertMissing: true);
		if (busIndex < 0)
		{
			return null;
		}
		SoundGroupVariation soundGroupVariation = null;
		float num = -1f;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
			if (masterAudioGroup.busIndex != busIndex || masterAudioGroup.ActiveVoices == 0)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if (!variation.PlaySoundParm.IsPlaying)
				{
					continue;
				}
				if (variation.curFadeMode == SoundGroupVariation.FadeMode.FadeOutEarly)
				{
					variation.Stop();
				}
				else if (!variation.ParentGroup.isUninterruptible)
				{
					if (soundGroupVariation == null)
					{
						soundGroupVariation = variation;
						num = variation.ParentGroup.importance;
					}
					else if ((float)variation.ParentGroup.importance < num)
					{
						soundGroupVariation = variation;
						num = variation.ParentGroup.importance;
					}
				}
			}
		}
		if (num > (float)group.importance)
		{
			return null;
		}
		return soundGroupVariation;
	}

	private static SoundGroupVariation FindFarthestSoundOnBus(GroupBus bus)
	{
		int busIndex = GetBusIndex(bus.busName, alertMissing: true);
		if (busIndex < 0)
		{
			return null;
		}
		SoundGroupVariation soundGroupVariation = null;
		float num = -1f;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
			if (masterAudioGroup.busIndex != busIndex || masterAudioGroup.ActiveVoices == 0)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if (!variation.PlaySoundParm.IsPlaying)
				{
					continue;
				}
				if (variation.curFadeMode == SoundGroupVariation.FadeMode.FadeOutEarly)
				{
					variation.Stop();
					continue;
				}
				float num2 = 0f;
				Transform objectToFollow = variation.ObjectToFollow;
				if (objectToFollow != null)
				{
					num2 = (ListenerTrans.position - objectToFollow.position).sqrMagnitude;
				}
				if (soundGroupVariation == null)
				{
					soundGroupVariation = variation;
					num = num2;
				}
				else if (num2 > num)
				{
					soundGroupVariation = variation;
					num = num2;
				}
			}
		}
		return soundGroupVariation;
	}

	private static SoundGroupVariation FindOldestSoundOnBus(GroupBus bus)
	{
		int busIndex = GetBusIndex(bus.busName, alertMissing: true);
		if (busIndex < 0)
		{
			return null;
		}
		SoundGroupVariation soundGroupVariation = null;
		float num = -1f;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
			if (masterAudioGroup.busIndex != busIndex || masterAudioGroup.ActiveVoices == 0)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if (variation.PlaySoundParm.IsPlaying)
				{
					if (variation.curFadeMode == SoundGroupVariation.FadeMode.FadeOutEarly)
					{
						variation.Stop();
					}
					else if (soundGroupVariation == null)
					{
						soundGroupVariation = variation;
						num = variation.LastTimePlayed;
					}
					else if (variation.LastTimePlayed < num)
					{
						soundGroupVariation = variation;
						num = variation.LastTimePlayed;
					}
				}
			}
		}
		return soundGroupVariation;
	}

	public static void StopBus(string busName)
	{
		if (busName == "_VideoPlayers")
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		Instance.VariationsStartedDuringMultiStop.Clear();
		Instance._isStoppingMultiple = true;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				StopAllOfSound(masterAudioGroup.GameObjectName);
			}
		}
		Instance._isStoppingMultiple = false;
	}

	public static void UnpauseBus(string busName)
	{
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				UnpauseSoundGroup(masterAudioGroup.GameObjectName);
			}
		}
	}

	public static bool CreateBus(string busName, int? actorInstanceId, bool errorOnExisting = true, bool isTemporary = false)
	{
		if (GroupBuses.FindAll((GroupBus obj) => obj.busName == busName).Count > 0)
		{
			if (errorOnExisting)
			{
				LogError("You already have a bus named '" + busName + "'. Not creating a second one.");
			}
			return false;
		}
		GroupBus groupBus = new GroupBus
		{
			busName = busName,
			isTemporary = isTemporary
		};
		float? busVolume = PersistentAudioSettings.GetBusVolume(busName);
		GroupBuses.Add(groupBus);
		if (busVolume.HasValue)
		{
			SetBusVolumeByName(busName, busVolume.Value);
		}
		if (actorInstanceId.HasValue)
		{
			groupBus.AddActorInstanceId(actorInstanceId.Value);
		}
		return true;
	}

	public static void DeleteBusByName(string busName)
	{
		int busIndex = GetBusIndex(busName, alertMissing: false);
		if (busIndex > 0)
		{
			DeleteBusByIndex(busIndex);
		}
	}

	public static void DeleteBusByIndex(int busIndex)
	{
		int index = busIndex - 2;
		if (Application.isPlaying)
		{
			GroupBus groupBus = GroupBuses[index];
			if (groupBus.isSoloed)
			{
				UnsoloBus(groupBus.busName, shouldCheckMuteStatus: false);
			}
			else if (groupBus.isMuted)
			{
				UnmuteBus(groupBus.busName, shouldCheckMuteStatus: false);
			}
		}
		GroupBuses.RemoveAt(index);
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			MasterAudioGroup masterAudioGroup = audioGroupInfo.Group;
			if (masterAudioGroup.busIndex == -1)
			{
				continue;
			}
			if (masterAudioGroup.busIndex == busIndex)
			{
				masterAudioGroup.busIndex = -1;
				RouteGroupToUnityMixerGroup(masterAudioGroup.GameObjectName, null);
				for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
				{
					audioGroupInfo.Sources[j].Variation.SetSpatialBlend();
				}
				RecalculateGroupVolumes(audioGroupInfo, null);
			}
			else if (masterAudioGroup.busIndex > busIndex)
			{
				masterAudioGroup.busIndex--;
			}
		}
	}

	public static float GetBusVolume(MasterAudioGroup maGroup)
	{
		float result = 1f;
		if (maGroup.busIndex >= 2)
		{
			result = GroupBuses[maGroup.busIndex - 2].volume;
		}
		return result;
	}

	public static void FadeBusToVolume(string busName, float newVolume, float fadeTime, Action completionCallback = null, bool willStopAfterFade = false, bool willResetVolumeAfterFade = false)
	{
		if (newVolume < 0f || newVolume > 1f)
		{
			Debug.LogError("Illegal volume passed to FadeBusToVolume: '" + newVolume + "'. Legal volumes are between 0 and 1");
			return;
		}
		if (fadeTime <= 0.1f)
		{
			SetBusVolumeByName(busName, newVolume);
			completionCallback?.Invoke();
			if (willStopAfterFade)
			{
				StopBus(busName);
			}
			return;
		}
		GroupBus groupBus = GrabBusByName(busName);
		if (groupBus == null)
		{
			Debug.Log("Could not find bus '" + busName + "' to fade it.");
			return;
		}
		for (int i = 0; i < Instance.BusFades.Count; i++)
		{
			BusFadeInfo busFadeInfo = Instance.BusFades[i];
			if (busFadeInfo.IsActive && busFadeInfo.NameOfBus == busName)
			{
				busFadeInfo.IsActive = false;
			}
		}
		BusFadeInfo busFadeInfo2 = null;
		for (int j = 0; j < Instance.BusFades.Count; j++)
		{
			BusFadeInfo busFadeInfo3 = Instance.BusFades[j];
			if (!busFadeInfo3.IsActive)
			{
				busFadeInfo2 = busFadeInfo3;
				break;
			}
		}
		if (busFadeInfo2 == null)
		{
			busFadeInfo2 = new BusFadeInfo();
			Instance.BusFades.Add(busFadeInfo2);
		}
		busFadeInfo2.NameOfBus = busName;
		busFadeInfo2.ActingBus = groupBus;
		busFadeInfo2.StartVolume = groupBus.volume;
		busFadeInfo2.TargetVolume = newVolume;
		busFadeInfo2.StartTime = AudioUtil.Time;
		busFadeInfo2.CompletionTime = AudioUtil.Time + fadeTime;
		busFadeInfo2.WillStopGroupAfterFade = willStopAfterFade;
		busFadeInfo2.WillResetVolumeAfterFade = willResetVolumeAfterFade;
		busFadeInfo2.IsActive = true;
		if (completionCallback != null)
		{
			busFadeInfo2.completionAction = completionCallback;
		}
	}

	public static void FadeOutOldBusVoices(string busName, float minimumPlayTime, float fadeTime)
	{
		if (!SceneHasMasterAudio)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			if (audioGroupInfo.Group.busIndex != busIndex)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if ((variation.IsPaused || variation.IsPlaying) && !(AudioUtil.Time - variation.LastTimePlayed <= minimumPlayTime))
				{
					if (fadeTime <= 0f)
					{
						variation.Stop();
					}
					else
					{
						variation.FadeOutNowAndStop(fadeTime);
					}
				}
			}
		}
	}

	public static void StopOldBusVoices(string busName, float minimumPlayTime)
	{
		if (!SceneHasMasterAudio)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
			if (audioGroupInfo.Group.busIndex != busIndex)
			{
				continue;
			}
			for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
			{
				SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
				if ((variation.IsPaused || variation.IsPlaying) && !(AudioUtil.Time - variation.LastTimePlayed <= minimumPlayTime))
				{
					variation.Stop();
				}
			}
		}
	}

	public static void GlideBusByPitch(string busName, float pitchAddition, float glideTime, Action completionCallback = null)
	{
		if (pitchAddition < -3f || pitchAddition > 3f)
		{
			Debug.LogError("Illegal pitch passed to GlideBusByPitch: '" + pitchAddition + "'. Legal pitches are between -3 and 3");
			return;
		}
		if (pitchAddition == 0f)
		{
			completionCallback?.Invoke();
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		if (glideTime <= 0.1f)
		{
			for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
			{
				string key = RuntimeSoundGroupNames[i];
				AudioGroupInfo audioGroupInfo = Instance.AudioSourcesBySoundType[key];
				if (audioGroupInfo.Group.busIndex != busIndex)
				{
					continue;
				}
				for (int j = 0; j < audioGroupInfo.Sources.Count; j++)
				{
					SoundGroupVariation variation = audioGroupInfo.Sources[j].Variation;
					if (variation.IsPlaying)
					{
						if (variation.curPitchMode == SoundGroupVariation.PitchMode.Gliding)
						{
							variation.VariationUpdater.StopPitchGliding();
						}
						variation.GlideByPitch(pitchAddition, 0f);
					}
				}
			}
			completionCallback?.Invoke();
			return;
		}
		for (int k = 0; k < Instance.BusPitchGlides.Count; k++)
		{
			BusPitchGlideInfo busPitchGlideInfo = Instance.BusPitchGlides[k];
			if (busPitchGlideInfo.NameOfBus == busName && busPitchGlideInfo.IsActive)
			{
				busPitchGlideInfo.IsActive = false;
				if (busPitchGlideInfo.completionAction != null)
				{
					busPitchGlideInfo.completionAction();
					busPitchGlideInfo.completionAction = null;
				}
				break;
			}
		}
		BusPitchGlideInfo busPitchGlideInfo2 = null;
		for (int l = 0; l < Instance.BusPitchGlides.Count; l++)
		{
			BusPitchGlideInfo busPitchGlideInfo3 = Instance.BusPitchGlides[l];
			if (!busPitchGlideInfo3.IsActive)
			{
				busPitchGlideInfo2 = busPitchGlideInfo3;
				break;
			}
		}
		bool flag = false;
		if (busPitchGlideInfo2 == null)
		{
			busPitchGlideInfo2 = new BusPitchGlideInfo();
			flag = true;
		}
		List<SoundGroupVariation> list = new List<SoundGroupVariation>();
		for (int m = 0; m < RuntimeSoundGroupNames.Count; m++)
		{
			string key2 = RuntimeSoundGroupNames[m];
			AudioGroupInfo audioGroupInfo2 = Instance.AudioSourcesBySoundType[key2];
			if (audioGroupInfo2.Group.busIndex != busIndex)
			{
				continue;
			}
			for (int n = 0; n < audioGroupInfo2.Sources.Count; n++)
			{
				SoundGroupVariation variation2 = audioGroupInfo2.Sources[n].Variation;
				if (variation2.IsPlaying)
				{
					if (variation2.curPitchMode == SoundGroupVariation.PitchMode.Gliding)
					{
						variation2.VariationUpdater.StopPitchGliding();
					}
					variation2.GlideByPitch(pitchAddition, glideTime);
					list.Add(variation2);
				}
			}
		}
		if (list.Count == 0)
		{
			completionCallback?.Invoke();
			return;
		}
		busPitchGlideInfo2.NameOfBus = busName;
		busPitchGlideInfo2.CompletionTime = AudioUtil.Time + glideTime;
		busPitchGlideInfo2.GlidingVariations = list;
		busPitchGlideInfo2.IsActive = true;
		if (completionCallback != null)
		{
			busPitchGlideInfo2.completionAction = completionCallback;
		}
		if (flag)
		{
			Instance.BusPitchGlides.Add(busPitchGlideInfo2);
		}
	}

	public static void SetBusVolumeByName(string busName, float newVolume)
	{
		GroupBus groupBus = GrabBusByName(busName);
		if (groupBus == null)
		{
			Debug.LogError("bus '" + busName + "' not found!");
		}
		else
		{
			SetBusVolume(groupBus, newVolume);
		}
	}

	private static void RecalculateGroupVolumes(AudioGroupInfo aGroup, GroupBus bus)
	{
		GroupBus busByIndex = GetBusByIndex(aGroup.Group.busIndex);
		bool num = busByIndex != null && bus != null && busByIndex.busName == bus.busName;
		float num2 = 1f;
		if (num)
		{
			num2 = bus.volume;
		}
		else if (busByIndex != null)
		{
			num2 = busByIndex.volume;
		}
		for (int i = 0; i < aGroup.Sources.Count; i++)
		{
			AudioInfo audioInfo = aGroup.Sources[i];
			AudioSource source = audioInfo.Source;
			if (audioInfo.Variation.IsPlaying)
			{
				float num3 = aGroup.Group.groupMasterVolume * num2 * Instance._masterAudioVolume;
				float volume = audioInfo.OriginalVolume * audioInfo.LastPercentageVolume * num3 + audioInfo.LastRandomVolume;
				source.volume = volume;
				audioInfo.Variation.SetGroupVolume = num3;
			}
		}
	}

	private static void SetBusVolume(GroupBus bus, float newVolume)
	{
		if (bus != null)
		{
			bus.volume = newVolume;
		}
		foreach (string allSoundGroupName in Instance.AllSoundGroupNames)
		{
			RecalculateGroupVolumes(Instance.AudioSourcesBySoundType[allSoundGroupName], bus);
		}
	}

	public static GroupBus GrabBusByName(string busName)
	{
		for (int i = 0; i < GroupBuses.Count; i++)
		{
			GroupBus groupBus = GroupBuses[i];
			if (groupBus.busName == busName)
			{
				return groupBus;
			}
		}
		return null;
	}

	public static void PauseBusOfTransform(Transform sourceTrans, string busName)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				PauseSoundGroupOfTransform(sourceTrans, masterAudioGroup.GameObjectName);
			}
		}
	}

	public static void UnpauseBusOfTransform(Transform sourceTrans, string busName)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				UnpauseSoundGroupOfTransform(sourceTrans, masterAudioGroup.GameObjectName);
			}
		}
	}

	public static void StopBusOfTransform(Transform sourceTrans, string busName)
	{
		if (!SceneHasMasterAudio || sourceTrans == null)
		{
			return;
		}
		int busIndex = GetBusIndex(busName, alertMissing: true);
		if (busIndex < 0)
		{
			return;
		}
		Instance.VariationsStartedDuringMultiStop.Clear();
		Instance._isStoppingMultiple = true;
		for (int i = 0; i < RuntimeSoundGroupNames.Count; i++)
		{
			string key = RuntimeSoundGroupNames[i];
			MasterAudioGroup masterAudioGroup = Instance.AudioSourcesBySoundType[key].Group;
			if (masterAudioGroup.busIndex == busIndex)
			{
				StopSoundGroupOfTransform(sourceTrans, masterAudioGroup.GameObjectName);
			}
		}
		Instance._isStoppingMultiple = false;
	}

	public static void AddSoundGroupToDuckList(string sType, float riseVolumeStart, float duckedVolCut, float unduckTime, bool isTemporary = false)
	{
		MasterAudio instance = Instance;
		if (!instance.duckingBySoundType.ContainsKey(sType))
		{
			DuckGroupInfo duckGroupInfo = new DuckGroupInfo
			{
				soundType = sType,
				riseVolStart = riseVolumeStart,
				duckedVolumeCut = duckedVolCut,
				unduckTime = unduckTime,
				isTemporary = isTemporary
			};
			instance.duckingBySoundType.Add(sType, duckGroupInfo);
			instance.musicDuckingSounds.Add(duckGroupInfo);
		}
	}

	public static void RemoveSoundGroupFromDuckList(string sType)
	{
		MasterAudio instance = Instance;
		if (instance.duckingBySoundType.ContainsKey(sType))
		{
			DuckGroupInfo item = instance.duckingBySoundType[sType];
			instance.musicDuckingSounds.Remove(item);
			instance.duckingBySoundType.Remove(sType);
		}
	}

	public static Playlist GrabPlaylist(string playlistName, bool logErrorIfNotFound = true)
	{
		if (playlistName == "[None]")
		{
			return null;
		}
		for (int i = 0; i < MusicPlaylists.Count; i++)
		{
			Playlist playlist = MusicPlaylists[i];
			if (playlist.playlistName == playlistName)
			{
				return playlist;
			}
		}
		if (logErrorIfNotFound)
		{
			Debug.LogWarning("Could not find Playlist '" + playlistName + "'.");
		}
		return null;
	}

	public static void ChangePlaylistPitch(string playlistName, float pitch, string songName = null)
	{
		Playlist playlist = GrabPlaylist(playlistName);
		if (playlist == null)
		{
			return;
		}
		for (int i = 0; i < playlist.MusicSettings.Count; i++)
		{
			MusicSetting musicSetting = playlist.MusicSettings[i];
			if (string.IsNullOrEmpty(songName) || !(musicSetting.alias != songName) || !(musicSetting.songName != songName))
			{
				musicSetting.pitch = pitch;
			}
		}
	}

	public static void MutePlaylist()
	{
		MutePlaylist("~only~");
	}

	public static void MutePlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToMute.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
			{
				return;
			}
			Instance.ControllersToMute.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToMute.Add(playlistController);
			}
		}
		MutePlaylists(Instance.ControllersToMute);
	}

	public static void MuteAllPlaylists()
	{
		MutePlaylists(PlaylistController.Instances);
	}

	private static void MutePlaylists(List<PlaylistController> playlists)
	{
		if (playlists.Count == PlaylistController.Instances.Count)
		{
			PlaylistsMuted = true;
		}
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].MutePlaylist();
		}
	}

	public static void UnmutePlaylist()
	{
		UnmutePlaylist("~only~");
	}

	public static void UnmutePlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToUnmute.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
			{
				return;
			}
			Instance.ControllersToUnmute.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToUnmute.Add(playlistController);
			}
		}
		UnmutePlaylists(Instance.ControllersToUnmute);
	}

	public static void UnmuteAllPlaylists()
	{
		UnmutePlaylists(PlaylistController.Instances);
	}

	private static void UnmutePlaylists(List<PlaylistController> playlists)
	{
		if (playlists.Count == PlaylistController.Instances.Count)
		{
			PlaylistsMuted = false;
		}
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].UnmutePlaylist();
		}
	}

	public static void ToggleMutePlaylist()
	{
		ToggleMutePlaylist("~only~");
	}

	public static void ToggleMutePlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToToggleMute.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
			{
				return;
			}
			Instance.ControllersToToggleMute.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToToggleMute.Add(playlistController);
			}
		}
		ToggleMutePlaylists(Instance.ControllersToToggleMute);
	}

	public static void ToggleMuteAllPlaylists()
	{
		ToggleMutePlaylists(PlaylistController.Instances);
	}

	private static void ToggleMutePlaylists(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].ToggleMutePlaylist();
		}
	}

	public static void PausePlaylist()
	{
		PausePlaylist("~only~");
	}

	public static void PausePlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToPause.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "PausePlaylist"))
			{
				return;
			}
			Instance.ControllersToPause.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToPause.Add(playlistController);
			}
		}
		PausePlaylists(Instance.ControllersToPause);
	}

	public static void PauseAllPlaylists()
	{
		PausePlaylists(PlaylistController.Instances);
	}

	private static void PausePlaylists(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].PausePlaylist();
		}
	}

	public static void UnpausePlaylist()
	{
		UnpausePlaylist("~only~");
	}

	public static void UnpausePlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToUnpause.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "UnpausePlaylist"))
			{
				return;
			}
			Instance.ControllersToUnpause.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToUnpause.Add(playlistController);
			}
		}
		UnpausePlaylists(Instance.ControllersToUnpause);
	}

	public static void UnpauseAllPlaylists()
	{
		UnpausePlaylists(PlaylistController.Instances);
	}

	private static void UnpausePlaylists(List<PlaylistController> controllers)
	{
		for (int i = 0; i < controllers.Count; i++)
		{
			controllers[i].UnpausePlaylist();
		}
	}

	public static void StopPlaylist()
	{
		StopPlaylist("~only~");
	}

	public static void StopPlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToStop.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "StopPlaylist"))
			{
				return;
			}
			Instance.ControllersToStop.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToStop.Add(playlistController);
			}
		}
		StopPlaylists(Instance.ControllersToStop);
	}

	public static void StopAllPlaylists()
	{
		StopPlaylists(PlaylistController.Instances);
	}

	private static void StopPlaylists(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].StopPlaylist();
		}
	}

	public static void TriggerNextPlaylistClip()
	{
		TriggerNextPlaylistClip("~only~");
	}

	public static void TriggerNextPlaylistClip(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToTrigNext.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerNextPlaylistClip"))
			{
				return;
			}
			Instance.ControllersToTrigNext.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToTrigNext.Add(playlistController);
			}
		}
		NextPlaylistClips(Instance.ControllersToTrigNext);
	}

	public static void TriggerNextClipAllPlaylists()
	{
		NextPlaylistClips(PlaylistController.Instances);
	}

	private static void NextPlaylistClips(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].PlayNextSong();
		}
	}

	public static void TriggerRandomPlaylistClip()
	{
		TriggerRandomPlaylistClip("~only~");
	}

	public static void TriggerRandomPlaylistClip(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToTrigRandom.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerRandomPlaylistClip"))
			{
				return;
			}
			Instance.ControllersToTrigRandom.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToTrigRandom.Add(playlistController);
			}
		}
		RandomPlaylistClips(Instance.ControllersToTrigRandom);
	}

	public static void TriggerRandomClipAllPlaylists()
	{
		RandomPlaylistClips(PlaylistController.Instances);
	}

	private static void RandomPlaylistClips(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].PlayRandomSong();
		}
	}

	public static void RestartPlaylist()
	{
		RestartPlaylist("~only~");
	}

	public static void RestartPlaylist(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "RestartPlaylist"))
			{
				return;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return;
			}
			playlistController = playlistController2;
		}
		if (playlistController != null)
		{
			RestartPlaylists(new List<PlaylistController> { playlistController });
		}
	}

	public static void RestartAllPlaylists()
	{
		RestartPlaylists(PlaylistController.Instances);
	}

	private static void RestartPlaylists(List<PlaylistController> playlists)
	{
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].RestartPlaylist();
		}
	}

	public static void StartPlaylist(string playlistName)
	{
		StartPlaylist("~only~", playlistName);
	}

	public static void StartPlaylistOnClip(string playlistName, string clipName)
	{
		StartPlaylist("~only~", playlistName, clipName);
	}

	public static void StartPlaylist(string playlistControllerName, string playlistName, string clipName = null)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToStart.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "StartPlaylist"))
			{
				return;
			}
			Instance.ControllersToStart.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToStart.Add(playlistController);
			}
		}
		for (int i = 0; i < Instance.ControllersToStart.Count; i++)
		{
			Instance.ControllersToStart[i].StartPlaylist(playlistName, clipName);
		}
	}

	public static void StopLoopingAllCurrentSongs()
	{
		StopLoopingCurrentSongs(PlaylistController.Instances);
	}

	public static void StopLoopingCurrentSong()
	{
		StopLoopingCurrentSong("~only~");
	}

	public static void StopLoopingCurrentSong(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "StopLoopingCurrentSong"))
			{
				return;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return;
			}
			playlistController = playlistController2;
		}
		if (playlistController != null)
		{
			StopLoopingCurrentSongs(new List<PlaylistController> { playlistController });
		}
	}

	private static void StopLoopingCurrentSongs(List<PlaylistController> playlistControllers)
	{
		for (int i = 0; i < playlistControllers.Count; i++)
		{
			playlistControllers[i].StopLoopingCurrentSong();
		}
	}

	public static void StopAllPlaylistsAfterCurrentSongs()
	{
		StopPlaylistAfterCurrentSongs(PlaylistController.Instances);
	}

	public static void StopPlaylistAfterCurrentSong()
	{
		StopPlaylistAfterCurrentSong("~only~");
	}

	public static void StopPlaylistAfterCurrentSong(string playlistControllerName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "StopPlaylistAfterCurrentSong"))
			{
				return;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return;
			}
			playlistController = playlistController2;
		}
		if (playlistController != null)
		{
			StopPlaylistAfterCurrentSongs(new List<PlaylistController> { playlistController });
		}
	}

	private static void StopPlaylistAfterCurrentSongs(List<PlaylistController> playlistControllers)
	{
		for (int i = 0; i < playlistControllers.Count; i++)
		{
			playlistControllers[i].StopPlaylistAfterCurrentSong();
		}
	}

	public static void QueuePlaylistClip(string clipName)
	{
		QueuePlaylistClip("~only~", clipName);
	}

	public static void QueuePlaylistClip(string playlistControllerName, string clipName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "QueuePlaylistClip"))
			{
				return;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return;
			}
			playlistController = playlistController2;
		}
		if (playlistController != null)
		{
			playlistController.QueuePlaylistClip(clipName);
		}
	}

	public static bool TriggerPlaylistClip(string clipName)
	{
		return TriggerPlaylistClip("~only~", clipName);
	}

	public static bool TriggerPlaylistClip(string playlistControllerName, string clipName)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "TriggerPlaylistClip"))
			{
				return false;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return false;
			}
			playlistController = playlistController2;
		}
		if (playlistController == null)
		{
			return false;
		}
		return playlistController.TriggerPlaylistClip(clipName);
	}

	public static void ChangePlaylistByName(string playlistName, bool playFirstClip = true)
	{
		ChangePlaylistByName("~only~", playlistName, playFirstClip);
	}

	public static void ChangePlaylistByName(string playlistControllerName, string playlistName, bool playFirstClip = true)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		PlaylistController playlistController;
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "ChangePlaylistByName"))
			{
				return;
			}
			playlistController = instances[0];
		}
		else
		{
			PlaylistController playlistController2 = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController2 == null)
			{
				return;
			}
			playlistController = playlistController2;
		}
		if (playlistController != null)
		{
			playlistController.ChangePlaylist(playlistName, playFirstClip);
		}
	}

	public static void FadePlaylistToVolume(float targetVolume, float fadeTime)
	{
		FadePlaylistToVolume("~only~", targetVolume, fadeTime);
	}

	public static void FadePlaylistToVolume(string playlistControllerName, float targetVolume, float fadeTime)
	{
		List<PlaylistController> instances = PlaylistController.Instances;
		Instance.ControllersToFade.Clear();
		if (playlistControllerName == "~only~")
		{
			if (!IsOkToCallOnlyPlaylistMethod(instances, "FadePlaylistToVolume"))
			{
				return;
			}
			Instance.ControllersToFade.Add(instances[0]);
		}
		else
		{
			PlaylistController playlistController = PlaylistController.InstanceByName(playlistControllerName);
			if (playlistController != null)
			{
				Instance.ControllersToFade.Add(playlistController);
			}
		}
		FadePlaylists(Instance.ControllersToFade, targetVolume, fadeTime);
	}

	public static void FadeAllPlaylistsToVolume(float targetVolume, float fadeTime)
	{
		FadePlaylists(PlaylistController.Instances, targetVolume, fadeTime);
	}

	private static void FadePlaylists(List<PlaylistController> playlists, float targetVolume, float fadeTime)
	{
		if (targetVolume < 0f || targetVolume > 1f)
		{
			Debug.LogError("Illegal volume passed to FadePlaylistToVolume: '" + targetVolume + "'. Legal volumes are between 0 and 1");
			return;
		}
		for (int i = 0; i < playlists.Count; i++)
		{
			playlists[i].FadeToVolume(targetVolume, fadeTime);
		}
	}

	public static void CreatePlaylist(Playlist playlist, bool errorOnDuplicate)
	{
		Playlist playlist2 = GrabPlaylist(playlist.playlistName, logErrorIfNotFound: false);
		if (playlist2 != null)
		{
			if (errorOnDuplicate)
			{
				Debug.LogError("You already have a Playlist Controller with the name '" + playlist2.playlistName + "'. You must name them all uniquely. Not adding duplicate named Playlist.");
			}
		}
		else
		{
			MusicPlaylists.Add(playlist);
		}
	}

	public static void DeletePlaylist(string playlistName)
	{
		if (SafeInstance == null)
		{
			return;
		}
		Playlist playlist = GrabPlaylist(playlistName);
		if (playlist == null)
		{
			return;
		}
		for (int i = 0; i < PlaylistController.Instances.Count; i++)
		{
			PlaylistController playlistController = PlaylistController.Instances[i];
			if (!(playlistController.PlaylistName != playlistName))
			{
				playlistController.StopPlaylist();
				break;
			}
		}
		MusicPlaylists.Remove(playlist);
	}

	public static void AddSongToPlaylist(string playlistName, AudioClip song, bool loopSong = false, float songPitch = 1f, float songVolume = 1f, string alias = null)
	{
		MusicSetting newSong = new MusicSetting
		{
			clip = song,
			alias = alias,
			isExpanded = true,
			isLoop = loopSong,
			pitch = songPitch,
			volume = songVolume
		};
		AddSongToPlaylist(playlistName, newSong);
	}

	public static void AddSongToPlaylist(string playlistName, MusicSetting newSong)
	{
		Playlist playlist = GrabPlaylist(playlistName);
		if (playlist == null)
		{
			return;
		}
		foreach (SongMetadataProperty songMetadataProp in playlist.songMetadataProps)
		{
			if (songMetadataProp.AllSongsMustContain)
			{
				switch (songMetadataProp.PropertyType)
				{
				case SongMetadataProperty.MetadataPropertyType.Boolean:
				{
					SongMetadataBoolValue item4 = new SongMetadataBoolValue(songMetadataProp);
					newSong.metadataBoolValues.Add(item4);
					break;
				}
				case SongMetadataProperty.MetadataPropertyType.String:
				{
					SongMetadataStringValue item3 = new SongMetadataStringValue(songMetadataProp);
					newSong.metadataStringValues.Add(item3);
					break;
				}
				case SongMetadataProperty.MetadataPropertyType.Integer:
				{
					SongMetadataIntValue item2 = new SongMetadataIntValue(songMetadataProp);
					newSong.metadataIntValues.Add(item2);
					break;
				}
				case SongMetadataProperty.MetadataPropertyType.Float:
				{
					SongMetadataFloatValue item = new SongMetadataFloatValue(songMetadataProp);
					newSong.metadataFloatValues.Add(item);
					break;
				}
				}
			}
		}
		playlist.MusicSettings.Add(newSong);
	}

	public static void AudioListenerChanged(AudioListener listener)
	{
		_listenerTrans = listener.GetComponent<Transform>();
		ListenerFollower listenerFollower = AmbientUtil.ListenerFollower;
		if (listenerFollower != null)
		{
			listenerFollower.StartFollowing(_listenerTrans, 0.01f);
		}
	}

	public static void FireCustomEventNextFrame(string customEventName, Transform eventOrigin)
	{
		if (!AppIsShuttingDown && !("[None]" == customEventName) && !string.IsNullOrEmpty(customEventName))
		{
			if (!CustomEventExists(customEventName) && !IsWarming)
			{
				Debug.LogError("Custom Event '" + customEventName + "' was not found in Master Audio.");
				return;
			}
			Instance.CustomEventsToFire.Enqueue(new CustomEventToFireInfo
			{
				eventName = customEventName,
				eventOrigin = eventOrigin
			});
		}
	}

	public static void AddCustomEventReceiver(ICustomEventReceiver receiver, Transform receiverTrans)
	{
		if (AppIsShuttingDown)
		{
			return;
		}
		IList<AudioEventGroup> allEvents = receiver.GetAllEvents();
		for (int i = 0; i < allEvents.Count; i++)
		{
			AudioEventGroup audioEventGroup = allEvents[i];
			if (!receiver.SubscribesToEvent(audioEventGroup.customEventName))
			{
				continue;
			}
			if (!Instance.ReceiversByEventName.ContainsKey(audioEventGroup.customEventName))
			{
				Instance.ReceiversByEventName.Add(audioEventGroup.customEventName, new Dictionary<ICustomEventReceiver, Transform> { { receiver, receiverTrans } });
				continue;
			}
			Dictionary<ICustomEventReceiver, Transform> dictionary = Instance.ReceiversByEventName[audioEventGroup.customEventName];
			if (!dictionary.ContainsKey(receiver))
			{
				dictionary.Add(receiver, receiverTrans);
			}
		}
	}

	public static void RemoveCustomEventReceiver(ICustomEventReceiver receiver)
	{
		if (AppIsShuttingDown || SafeInstance == null)
		{
			if (!(SafeInstance != null))
			{
				return;
			}
			{
				foreach (string key in Instance.ReceiversByEventName.Keys)
				{
					Instance.ReceiversByEventName[key].Remove(receiver);
				}
				return;
			}
		}
		for (int i = 0; i < Instance.customEvents.Count; i++)
		{
			CustomEvent customEvent = Instance.customEvents[i];
			if (receiver.SubscribesToEvent(customEvent.EventName))
			{
				Instance.ReceiversByEventName[customEvent.EventName].Remove(receiver);
			}
		}
	}

	public static List<Transform> ReceiversForEvent(string customEventName)
	{
		List<Transform> list = new List<Transform>();
		if (!Instance.ReceiversByEventName.ContainsKey(customEventName))
		{
			return list;
		}
		Dictionary<ICustomEventReceiver, Transform> dictionary = Instance.ReceiversByEventName[customEventName];
		foreach (ICustomEventReceiver key in dictionary.Keys)
		{
			if (key.SubscribesToEvent(customEventName))
			{
				list.Add(dictionary[key]);
			}
		}
		return list;
	}

	public static CustomEventCategory CreateCustomEventCategoryIfNotThere(string categoryName, int? actorInstanceId, bool errorOnDuplicates, bool isTemporary)
	{
		if (AppIsShuttingDown)
		{
			return null;
		}
		CustomEventCategory customEventCategory = Instance.customEventCategories.Find((CustomEventCategory cat) => cat.CatName == categoryName && cat.IsTemporary);
		if (customEventCategory == null)
		{
			customEventCategory = new CustomEventCategory
			{
				CatName = categoryName,
				ProspectiveName = categoryName,
				IsTemporary = isTemporary
			};
			Instance.customEventCategories.Add(customEventCategory);
		}
		else if (errorOnDuplicates)
		{
			Debug.LogError("You already have a Custom Event Category with the name '" + categoryName + "'. You must name them all uniquely. Not adding duplicate named Custom Event Category.");
			return null;
		}
		if (actorInstanceId.HasValue)
		{
			customEventCategory.AddActorInstanceId(actorInstanceId.Value);
		}
		return customEventCategory;
	}

	public static void CreateCustomEvent(string customEventName, CustomEventReceiveMode eventReceiveMode, float distanceThreshold, EventReceiveFilter receiveFilter, int filterModeQty, int? actorInstanceId, string categoryName = "", bool isTemporary = false, bool errorOnDuplicate = true)
	{
		if (AppIsShuttingDown)
		{
			return;
		}
		CustomEvent customEvent = null;
		for (int i = 0; i < Instance.customEvents.Count; i++)
		{
			CustomEvent customEvent2 = Instance.customEvents[i];
			if (customEvent2.EventName == customEventName)
			{
				customEvent = customEvent2;
				break;
			}
		}
		if (customEvent != null)
		{
			if (errorOnDuplicate)
			{
				Debug.LogError("You already have a Custom Event named '" + customEventName + "'. No need to add it again.");
				return;
			}
		}
		else
		{
			if (string.IsNullOrEmpty(categoryName))
			{
				categoryName = Instance.customEventCategories[0].CatName;
			}
			customEvent = new CustomEvent(customEventName)
			{
				eventReceiveMode = eventReceiveMode,
				distanceThreshold = distanceThreshold,
				eventRcvFilterMode = receiveFilter,
				filterModeQty = filterModeQty,
				categoryName = categoryName,
				isTemporary = isTemporary
			};
			Instance.customEvents.Add(customEvent);
		}
		if (actorInstanceId.HasValue)
		{
			customEvent.AddActorInstanceId(actorInstanceId.Value);
		}
	}

	public static void DeleteCustomEvent(string customEventName)
	{
		if (!AppIsShuttingDown && !(SafeInstance == null))
		{
			Instance.customEvents.RemoveAll((CustomEvent obj) => obj.EventName == customEventName);
		}
	}

	public static CustomEvent GetCustomEventByName(string customEventName)
	{
		for (int i = 0; i < Instance.customEvents.Count; i++)
		{
			CustomEvent customEvent = Instance.customEvents[i];
			if (customEvent.EventName == customEventName)
			{
				return customEvent;
			}
		}
		return null;
	}

	public static void FireCustomEvent(string customEventName, Transform originObject, bool logDupe = true)
	{
		if (AppIsShuttingDown || "[None]" == customEventName || string.IsNullOrEmpty(customEventName))
		{
			return;
		}
		if (originObject == null)
		{
			Debug.LogError("Custom Event '" + customEventName + "' cannot be fired without an originObject passed in.");
			return;
		}
		if (!CustomEventExists(customEventName) && !IsWarming)
		{
			Debug.LogError("Custom Event '" + customEventName + "' was not found in Master Audio.");
			return;
		}
		CustomEvent customEventByName = GetCustomEventByName(customEventName);
		if (customEventByName == null)
		{
			return;
		}
		if (customEventByName.frameLastFired >= AudioUtil.FrameCount)
		{
			if (logDupe)
			{
				Debug.LogWarning("Already fired Custom Event '" + customEventName + "' this frame or later. Cannot be fired twice in the same frame.");
			}
			return;
		}
		customEventByName.frameLastFired = AudioUtil.FrameCount;
		if (!Instance.disableLogging && Instance.logCustomEvents)
		{
			Debug.Log("Firing Custom Event: " + customEventName);
		}
		if (!Instance.ReceiversByEventName.ContainsKey(customEventName))
		{
			return;
		}
		Vector3 position = originObject.position;
		float? num = null;
		Dictionary<ICustomEventReceiver, Transform> dictionary = Instance.ReceiversByEventName[customEventName];
		Instance.ValidReceivers.Clear();
		bool flag = false;
		switch (customEventByName.eventReceiveMode)
		{
		case CustomEventReceiveMode.Never:
			if (Instance.LogSounds)
			{
				Debug.LogWarning("Custom Event '" + customEventName + "' not being transmitted because it is set to 'Never transmit'.");
			}
			return;
		case CustomEventReceiveMode.OnChildGameObject:
			Instance.ValidReceivers.AddRange(GetChildReceivers(originObject, customEventName, includeSelf: false));
			flag = true;
			break;
		case CustomEventReceiveMode.OnParentGameObject:
			Instance.ValidReceivers.AddRange(GetParentReceivers(originObject, customEventName, includeSelf: false));
			flag = true;
			break;
		case CustomEventReceiveMode.OnSameOrChildGameObject:
			Instance.ValidReceivers.AddRange(GetChildReceivers(originObject, customEventName, includeSelf: true));
			flag = true;
			break;
		case CustomEventReceiveMode.OnSameOrParentGameObject:
			Instance.ValidReceivers.AddRange(GetParentReceivers(originObject, customEventName, includeSelf: true));
			flag = true;
			break;
		case CustomEventReceiveMode.WhenDistanceLessThan:
		case CustomEventReceiveMode.WhenDistanceMoreThan:
			num = customEventByName.distanceThreshold * customEventByName.distanceThreshold;
			break;
		}
		if (!flag)
		{
			foreach (ICustomEventReceiver key in dictionary.Keys)
			{
				switch (customEventByName.eventReceiveMode)
				{
				case CustomEventReceiveMode.WhenDistanceLessThan:
					if ((dictionary[key].position - position).sqrMagnitude > num)
					{
						continue;
					}
					break;
				case CustomEventReceiveMode.WhenDistanceMoreThan:
					if ((dictionary[key].position - position).sqrMagnitude < num)
					{
						continue;
					}
					break;
				case CustomEventReceiveMode.OnSameGameObject:
					if (originObject != dictionary[key])
					{
						continue;
					}
					break;
				}
				Instance.ValidReceivers.Add(key);
			}
		}
		if (customEventByName.eventRcvFilterMode == EventReceiveFilter.All || customEventByName.filterModeQty >= Instance.ValidReceivers.Count || Instance.ValidReceivers.Count <= 1)
		{
			for (int i = 0; i < Instance.ValidReceivers.Count; i++)
			{
				Instance.ValidReceivers[i].ReceiveEvent(customEventName, position);
			}
			return;
		}
		Instance.ValidReceiverCandidates.Clear();
		for (int j = 0; j < Instance.ValidReceivers.Count; j++)
		{
			ICustomEventReceiver customEventReceiver = Instance.ValidReceivers[j];
			Transform transform = dictionary[customEventReceiver];
			float distance = 0f;
			int randomId = 0;
			switch (customEventByName.eventRcvFilterMode)
			{
			case EventReceiveFilter.Random:
				randomId = UnityEngine.Random.Range(0, 1000);
				break;
			case EventReceiveFilter.Closest:
				distance = (transform.position - position).sqrMagnitude;
				break;
			}
			Instance.ValidReceiverCandidates.Add(new CustomEventCandidate(distance, customEventReceiver, transform, randomId));
		}
		switch (customEventByName.eventRcvFilterMode)
		{
		case EventReceiveFilter.Closest:
		{
			Instance.ValidReceiverCandidates.Sort((CustomEventCandidate x, CustomEventCandidate y) => x.DistanceAway.CompareTo(y.DistanceAway));
			int filterModeQty = customEventByName.filterModeQty;
			int count = Instance.ValidReceiverCandidates.Count - filterModeQty;
			Instance.ValidReceiverCandidates.RemoveRange(filterModeQty, count);
			break;
		}
		case EventReceiveFilter.Random:
		{
			Instance.ValidReceiverCandidates.Sort((CustomEventCandidate x, CustomEventCandidate y) => x.RandomId.CompareTo(y.RandomId));
			int filterModeQty = customEventByName.filterModeQty;
			int count = Instance.ValidReceiverCandidates.Count - filterModeQty;
			Instance.ValidReceiverCandidates.RemoveRange(filterModeQty, count);
			break;
		}
		}
		for (int num2 = 0; num2 < Instance.ValidReceiverCandidates.Count; num2++)
		{
			Instance.ValidReceiverCandidates[num2].Receiver.ReceiveEvent(customEventName, position);
		}
	}

	public static bool CustomEventExists(string customEventName)
	{
		if (AppIsShuttingDown)
		{
			return true;
		}
		for (int i = 0; i < Instance.customEvents.Count; i++)
		{
			if (Instance.customEvents[i].EventName == customEventName)
			{
				return true;
			}
		}
		return false;
	}

	private static List<ICustomEventReceiver> GetChildReceivers(Transform origin, string eventName, bool includeSelf)
	{
		List<ICustomEventReceiver> list = origin.GetComponentsInChildren<ICustomEventReceiver>().ToList();
		list.RemoveAll((ICustomEventReceiver rec) => !rec.SubscribesToEvent(eventName));
		if (includeSelf)
		{
			return list;
		}
		return FilterOutSelf(list, origin);
	}

	private static List<ICustomEventReceiver> GetParentReceivers(Transform origin, string eventName, bool includeSelf)
	{
		List<ICustomEventReceiver> list = origin.GetComponentsInParent<ICustomEventReceiver>().ToList();
		list.RemoveAll((ICustomEventReceiver rec) => !rec.SubscribesToEvent(eventName));
		if (includeSelf)
		{
			return list;
		}
		return FilterOutSelf(list, origin);
	}

	private static List<ICustomEventReceiver> FilterOutSelf(List<ICustomEventReceiver> sourceList, Transform origin)
	{
		List<ICustomEventReceiver> list = new List<ICustomEventReceiver>();
		foreach (ICustomEventReceiver source in sourceList)
		{
			MonoBehaviour monoBehaviour = source as MonoBehaviour;
			if (!(monoBehaviour == null) && !(monoBehaviour.transform != origin))
			{
				list.Add(source);
			}
		}
		int num = 0;
		while (list.Count > 0 && num < 20)
		{
			sourceList.Remove(list[0]);
			list.RemoveAt(0);
			num++;
		}
		return sourceList;
	}

	private static bool LoggingEnabledForGroup(MasterAudioGroup grp)
	{
		if (IsWarming)
		{
			return false;
		}
		if (Instance.disableLogging)
		{
			return false;
		}
		if (grp != null && grp.logSound)
		{
			return true;
		}
		return Instance.LogSounds;
	}

	private static void LogMessage(string message)
	{
		_ = Instance.disableLogging;
	}

	public static void LogWarning(string msg)
	{
		if (!Instance.disableLogging)
		{
			Debug.LogWarning(msg);
		}
	}

	public static void LogWarningIfNeverLogged(string msg, int errorNumber)
	{
		if (!ErrorNumbersLogged.Contains(errorNumber))
		{
			Debug.LogWarning(msg);
			ErrorNumbersLogged.Add(errorNumber);
		}
	}

	public static void LogError(string msg)
	{
		if (!Instance.disableLogging)
		{
			Debug.LogError(msg);
		}
	}

	public static void LogNoPlaylist(string playlistControllerName, string methodName)
	{
		LogWarning("There is currently no Playlist assigned to Playlist Controller '" + playlistControllerName + "'. Cannot call '" + methodName + "' method.");
	}

	private static bool IsOkToCallOnlyPlaylistMethod(List<PlaylistController> pcs, string methodName)
	{
		if (pcs.Count == 0)
		{
			LogError($"You have no Playlist Controllers in the Scene. You cannot '{methodName}'.");
			return false;
		}
		if (pcs.Count > 1)
		{
			LogError($"You cannot call '{methodName}' without specifying a Playlist Controller name when you have more than one Playlist Controller.");
			return false;
		}
		return true;
	}

	public static void SetupAmbientNextFrame(AmbientSound ambient)
	{
		if (AppIsShuttingDown || ambient == null)
		{
			return;
		}
		for (int i = 0; i < Instance.AmbientsToDelayedTrigger.Count; i++)
		{
			if (Instance.AmbientsToDelayedTrigger[i].ambient == ambient)
			{
				return;
			}
		}
		Instance.AmbientsToDelayedTrigger.Add(new AmbientSoundToTriggerInfo
		{
			frameToTrigger = Time.frameCount + 1,
			ambient = ambient
		});
	}

	public static void RemoveDelayedAmbient(AmbientSound ambient)
	{
		if (AppIsShuttingDown || ambient == null)
		{
			return;
		}
		int? num = null;
		for (int i = 0; i < Instance.AmbientsToDelayedTrigger.Count; i++)
		{
			AmbientSoundToTriggerInfo ambientSoundToTriggerInfo = Instance.AmbientsToDelayedTrigger[i];
			if (ambientSoundToTriggerInfo.ambient != null && ambientSoundToTriggerInfo.ambient == ambient)
			{
				num = i;
				break;
			}
		}
		if (num.HasValue)
		{
			Instance.AmbientsToDelayedTrigger.RemoveAt(num.Value);
		}
	}

	public static void QueueTransformFollowerForColliderPositionRecalc(TransformFollower follower)
	{
		if (SafeInstance == null)
		{
			return;
		}
		foreach (TransformFollower transFollowerColliderPositionRecalc in Instance.TransFollowerColliderPositionRecalcs)
		{
			if (transFollowerColliderPositionRecalc == follower)
			{
				return;
			}
		}
		Instance.TransFollowerColliderPositionRecalcs.Enqueue(follower);
	}

	public static void AddToQueuedOcclusionRays(SoundGroupVariationUpdater updater)
	{
		if (SafeInstance == null)
		{
			return;
		}
		foreach (SoundGroupVariationUpdater queuedOcclusionRay in Instance.QueuedOcclusionRays)
		{
			if (queuedOcclusionRay == updater)
			{
				return;
			}
		}
		Instance.QueuedOcclusionRays.Enqueue(updater);
	}

	public static void AddToOcclusionInRangeSources(GameObject src)
	{
		if (Application.isEditor && !(SafeInstance == null) && Instance.occlusionShowCategories)
		{
			if (!Instance.OcclusionSourcesInRange.Contains(src))
			{
				Instance.OcclusionSourcesInRange.Add(src);
			}
			if (Instance.OcclusionSourcesOutOfRange.Contains(src))
			{
				Instance.OcclusionSourcesOutOfRange.Remove(src);
			}
		}
	}

	public static void AddToOcclusionOutOfRangeSources(GameObject src)
	{
		if (Application.isEditor && !(SafeInstance == null) && Instance.occlusionShowCategories)
		{
			if (!Instance.OcclusionSourcesOutOfRange.Contains(src))
			{
				Instance.OcclusionSourcesOutOfRange.Add(src);
			}
			if (Instance.OcclusionSourcesInRange.Contains(src))
			{
				Instance.OcclusionSourcesInRange.Remove(src);
			}
			RemoveFromBlockedOcclusionSources(src);
		}
	}

	public static void AddToBlockedOcclusionSources(GameObject src)
	{
		if (Application.isEditor && !(SafeInstance == null) && Instance.occlusionShowCategories && !Instance.OcclusionSourcesBlocked.Contains(src))
		{
			Instance.OcclusionSourcesBlocked.Add(src);
		}
	}

	public static bool HasQueuedOcclusionRays()
	{
		return Instance.QueuedOcclusionRays.Count > 0;
	}

	public static SoundGroupVariationUpdater OldestQueuedOcclusionRay()
	{
		if (SafeInstance == null)
		{
			return null;
		}
		return Instance.QueuedOcclusionRays.Dequeue();
	}

	public static bool IsOcclusionFrequencyTransitioning(SoundGroupVariation variation)
	{
		for (int i = 0; i < Instance.VariationOcclusionFreqChanges.Count; i++)
		{
			OcclusionFreqChangeInfo occlusionFreqChangeInfo = Instance.VariationOcclusionFreqChanges[i];
			if (occlusionFreqChangeInfo.ActingVariation == variation)
			{
				return occlusionFreqChangeInfo.IsActive;
			}
		}
		return false;
	}

	public static void RemoveFromOcclusionFrequencyTransitioning(SoundGroupVariation variation)
	{
		for (int i = 0; i < Instance.VariationOcclusionFreqChanges.Count; i++)
		{
			if (!(Instance.VariationOcclusionFreqChanges[i].ActingVariation != variation))
			{
				Instance.VariationOcclusionFreqChanges.RemoveAt(i);
				break;
			}
		}
	}

	public static void RemoveFromBlockedOcclusionSources(GameObject src)
	{
		if (Application.isEditor && !(SafeInstance == null) && Instance.occlusionShowCategories && Instance.OcclusionSourcesBlocked.Contains(src))
		{
			Instance.OcclusionSourcesBlocked.Remove(src);
		}
	}

	public static void StopTrackingOcclusionForSource(GameObject src)
	{
		if (Application.isEditor && !(SafeInstance == null) && Instance.occlusionShowCategories)
		{
			if (Instance.OcclusionSourcesOutOfRange.Contains(src))
			{
				Instance.OcclusionSourcesOutOfRange.Remove(src);
			}
			if (Instance.OcclusionSourcesInRange.Contains(src))
			{
				Instance.OcclusionSourcesInRange.Remove(src);
			}
			if (Instance.OcclusionSourcesBlocked.Contains(src))
			{
				Instance.OcclusionSourcesBlocked.Remove(src);
			}
		}
	}

	public static void AddAddressableForDelayedRelease(string addressableId, int unusedSecondsLifespan)
	{
		for (int i = 0; i < Instance.AddressablesToReleaseLater.Count; i++)
		{
			AddressableDelayedRelease addressableDelayedRelease = Instance.AddressablesToReleaseLater[i];
			if (addressableDelayedRelease.AddressableId == addressableId)
			{
				addressableDelayedRelease.RealtimeToRelease = Time.realtimeSinceStartup + (float)unusedSecondsLifespan;
				return;
			}
		}
		Instance.AddressablesToReleaseLater.Add(new AddressableDelayedRelease(addressableId, Time.realtimeSinceStartup + (float)unusedSecondsLifespan));
	}

	public static void RemoveAddressableFromDelayedRelease(string addressableId)
	{
		Instance.AddressablesToReleaseLater.RemoveAll((AddressableDelayedRelease adr) => adr.AddressableId == addressableId);
	}

	public static bool IsVideoPlayersGroup(string groupName)
	{
		return groupName == "_VideoPlayers";
	}

	private static bool IsLinkedGroupPlay(SoundGroupVariation variation)
	{
		if (!Instance._isStoppingMultiple)
		{
			return false;
		}
		return Instance.VariationsStartedDuringMultiStop.Contains(variation);
	}

	public static int RemainingClipsInGroup(string sType)
	{
		if (!Instance._randomizer.ContainsKey(sType))
		{
			return 0;
		}
		return Instance._randomizer[sType].Count;
	}

	public static void RescanGroupsNow()
	{
		Instance._mustRescanGroups = true;
	}

	public static void DoneRescanningGroups()
	{
		Instance._mustRescanGroups = false;
	}

	public static GameObject CreateMasterAudio()
	{
		UnityEngine.Object obj = Resources.Load("Assets/Plugins/DarkTonic/MasterAudio/Prefabs/MasterAudio.prefab", typeof(GameObject));
		if (obj == null)
		{
			Debug.LogError("Could not find MasterAudio prefab. Please update the Installation Path in the Master Audio Manager window if you have moved the folder from its default location, then try again.");
			return null;
		}
		GameObject obj2 = UnityEngine.Object.Instantiate(obj) as GameObject;
		obj2.name = "MasterAudio";
		return obj2;
	}

	public static GameObject CreatePlaylistController()
	{
		UnityEngine.Object obj = Resources.Load("Assets/Plugins/DarkTonic/MasterAudio/Prefabs/PlaylistController.prefab", typeof(GameObject));
		if (obj == null)
		{
			Debug.LogError("Could not find PlaylistController prefab. Please update the Installation Path in the Master Audio Manager window if you have moved the folder from its default location, then try again.");
			return null;
		}
		GameObject obj2 = UnityEngine.Object.Instantiate(obj) as GameObject;
		obj2.name = "PlaylistController";
		return obj2;
	}

	public static GameObject CreateDynamicSoundGroupCreator()
	{
		UnityEngine.Object obj = Resources.Load("Assets/Plugins/DarkTonic/MasterAudio/Prefabs/DynamicSoundGroupCreator.prefab", typeof(GameObject));
		if (obj == null)
		{
			Debug.LogError("Could not find DynamicSoundGroupCreator prefab. Please update the Installation Path in the Master Audio Manager window if you have moved the folder from its default location, then try again.");
			return null;
		}
		GameObject obj2 = UnityEngine.Object.Instantiate(obj) as GameObject;
		obj2.name = "DynamicSoundGroupCreator";
		return obj2;
	}

	public static GameObject CreateSoundGroupOrganizer()
	{
		UnityEngine.Object obj = Resources.Load("Assets/Plugins/DarkTonic/MasterAudio/Prefabs/SoundGroupOrganizer.prefab", typeof(GameObject));
		if (obj == null)
		{
			Debug.LogError("Could not find SoundGroupOrganizer prefab. Please update the Installation Path in the Master Audio Manager window if you have moved the folder from its default location, then try again.");
			return null;
		}
		GameObject obj2 = UnityEngine.Object.Instantiate(obj) as GameObject;
		obj2.name = "SoundGroupOrganizer";
		return obj2;
	}
}
