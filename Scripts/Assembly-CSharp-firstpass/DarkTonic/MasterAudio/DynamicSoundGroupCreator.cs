using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

[AudioScriptOrder(-35)]
public class DynamicSoundGroupCreator : MonoBehaviour
{
	public enum CreateItemsWhen
	{
		FirstEnableOnly = 0,
		EveryEnable = 1
	}

	public const int ExtraHardCodedBusOptions = 1;

	public SystemLanguage previewLanguage = SystemLanguage.English;

	public MasterAudio.DragGroupMode curDragGroupMode;

	public GameObject groupTemplate;

	public GameObject variationTemplate;

	public bool errorOnDuplicates;

	public bool createOnAwake = true;

	public bool soundGroupsAreExpanded = true;

	public bool removeGroupsOnSceneChange = true;

	public CreateItemsWhen reUseMode;

	public bool showCustomEvents = true;

	public MasterAudio.AudioLocation bulkVariationMode;

	public List<CustomEvent> customEventsToCreate = new List<CustomEvent>();

	public List<CustomEventCategory> customEventCategories = new List<CustomEventCategory>
	{
		new CustomEventCategory()
	};

	public string newEventName = "my event";

	public string newCustomEventCategoryName = "New Category";

	public string addToCustomEventCategoryName = "New Category";

	public bool showMusicDucking = true;

	public List<DuckGroupInfo> musicDuckingSounds = new List<DuckGroupInfo>();

	public List<GroupBus> groupBuses = new List<GroupBus>();

	public bool playListExpanded;

	public bool playlistEditorExp = true;

	public List<MasterAudio.Playlist> musicPlaylists = new List<MasterAudio.Playlist>();

	public List<GameObject> audioSourceTemplates = new List<GameObject>(10);

	public string audioSourceTemplateName = "Max Distance 500";

	public bool groupByBus;

	public bool itemsCreatedEventExpanded;

	public string itemsCreatedCustomEvent = string.Empty;

	public bool showUnityMixerGroupAssignment = true;

	private bool _hasCreated;

	private readonly List<Transform> _groupsToRemove = new List<Transform>();

	private Transform _trans;

	private int _instanceId = -1;

	private readonly List<DynamicSoundGroup> _groupsToCreate = new List<DynamicSoundGroup>();

	public static int HardCodedBusOptions => 3;

	public List<DynamicSoundGroup> GroupsToCreate => _groupsToCreate;

	public int InstanceId
	{
		get
		{
			if (_instanceId < 0)
			{
				_instanceId = GetInstanceID();
			}
			return _instanceId;
		}
	}

	public bool ShouldShowUnityAudioMixerGroupAssignments => showUnityMixerGroupAssignment;

	private void Awake()
	{
		_trans = base.transform;
		_hasCreated = false;
		AudioSource component = GetComponent<AudioSource>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void OnEnable()
	{
		CreateItemsIfReady();
	}

	private void Start()
	{
		CreateItemsIfReady();
	}

	private void OnDisable()
	{
		if (!MasterAudio.AppIsShuttingDown && removeGroupsOnSceneChange && MasterAudio.SafeInstance != null)
		{
			RemoveItems();
		}
	}

	private void CreateItemsIfReady()
	{
		if (!(MasterAudio.SafeInstance == null) && createOnAwake && MasterAudio.SoundsReady && !_hasCreated)
		{
			CreateItems();
		}
	}

	public void RemoveItems()
	{
		for (int i = 0; i < groupBuses.Count; i++)
		{
			GroupBus groupBus = groupBuses[i];
			if (groupBus.isExisting)
			{
				continue;
			}
			GroupBus groupBus2 = MasterAudio.GrabBusByName(groupBus.busName);
			if (groupBus2 != null && !groupBus2.isTemporary)
			{
				continue;
			}
			if (groupBus2 != null)
			{
				groupBus2.RemoveActorInstanceId(InstanceId);
				if (groupBus2.HasLiveActors)
				{
					continue;
				}
			}
			MasterAudio.DeleteBusByName(groupBus.busName);
		}
		for (int j = 0; j < _groupsToRemove.Count; j++)
		{
			string sType = _groupsToRemove[j].name;
			MasterAudioGroup masterAudioGroup = MasterAudio.GrabGroup(sType, logIfMissing: false);
			if (!(masterAudioGroup == null))
			{
				masterAudioGroup.RemoveActorInstanceId(InstanceId);
				if (!masterAudioGroup.HasLiveActors)
				{
					MasterAudio.RemoveSoundGroupFromDuckList(sType);
					MasterAudio.DeleteSoundGroup(sType);
				}
			}
		}
		_groupsToRemove.Clear();
		for (int k = 0; k < customEventsToCreate.Count; k++)
		{
			CustomEvent anEvent = customEventsToCreate[k];
			CustomEvent customEvent = MasterAudio.Instance.customEvents.Find((CustomEvent cEvent) => cEvent.EventName == anEvent.EventName && cEvent.isTemporary);
			if (customEvent != null)
			{
				customEvent.RemoveActorInstanceId(InstanceId);
				if (!customEvent.HasLiveActors)
				{
					MasterAudio.DeleteCustomEvent(anEvent.EventName);
				}
			}
		}
		for (int num = 0; num < customEventCategories.Count; num++)
		{
			CustomEventCategory aCat = customEventCategories[num];
			CustomEventCategory customEventCategory = MasterAudio.Instance.customEventCategories.Find((CustomEventCategory category) => category.CatName == aCat.CatName && category.IsTemporary);
			if (customEventCategory != null)
			{
				customEventCategory.RemoveActorInstanceId(InstanceId);
				if (!customEventCategory.HasLiveActors)
				{
					MasterAudio.Instance.customEventCategories.Remove(customEventCategory);
				}
			}
		}
		for (int num2 = 0; num2 < musicPlaylists.Count; num2++)
		{
			MasterAudio.Playlist playlist = musicPlaylists[num2];
			MasterAudio.Playlist playlist2 = MasterAudio.GrabPlaylist(playlist.playlistName);
			if (playlist2 != null)
			{
				playlist2.RemoveActorInstanceId(InstanceId);
				if (!playlist2.HasLiveActors)
				{
					MasterAudio.DeletePlaylist(playlist.playlistName);
				}
			}
		}
		if (reUseMode == CreateItemsWhen.EveryEnable)
		{
			_hasCreated = false;
		}
		MasterAudio.SilenceOrUnsilenceGroupsFromSoloChange();
	}

	public void CreateItems()
	{
		if (_hasCreated)
		{
			Debug.LogWarning("DynamicSoundGroupCreator '" + base.transform.name + "' has already created its items. Cannot create again.");
		}
		else
		{
			if (MasterAudio.Instance == null)
			{
				return;
			}
			PopulateGroupData();
			for (int i = 0; i < groupBuses.Count; i++)
			{
				GroupBus groupBus = groupBuses[i];
				if (groupBus.isExisting)
				{
					if (MasterAudio.GrabBusByName(groupBus.busName) == null)
					{
						MasterAudio.LogWarning("Existing bus '" + groupBus.busName + "' was not found, specified in prefab '" + base.name + "'.");
					}
					continue;
				}
				GroupBus groupBus2 = MasterAudio.GrabBusByName(groupBus.busName);
				if (groupBus2 == null)
				{
					if (MasterAudio.CreateBus(groupBus.busName, InstanceId, errorOnDuplicates, isTemporary: true))
					{
						groupBus2 = MasterAudio.GrabBusByName(groupBus.busName);
					}
				}
				else
				{
					groupBus2.AddActorInstanceId(InstanceId);
				}
				if (groupBus2 != null)
				{
					if (!PersistentAudioSettings.GetBusVolume(groupBus.busName).HasValue)
					{
						groupBus2.volume = groupBus.volume;
						groupBus2.OriginalVolume = groupBus2.volume;
					}
					groupBus2.voiceLimit = groupBus.voiceLimit;
					groupBus2.busVoiceLimitExceededMode = groupBus.busVoiceLimitExceededMode;
					groupBus2.forceTo2D = groupBus.forceTo2D;
					groupBus2.mixerChannel = groupBus.mixerChannel;
					groupBus2.busColor = groupBus.busColor;
					groupBus2.isUsingOcclusion = groupBus.isUsingOcclusion;
				}
			}
			for (int j = 0; j < _groupsToCreate.Count; j++)
			{
				DynamicSoundGroup dynamicSoundGroup = _groupsToCreate[j];
				string busName = string.Empty;
				int num = ((dynamicSoundGroup.busIndex != -1) ? dynamicSoundGroup.busIndex : 0);
				if (num >= HardCodedBusOptions)
				{
					busName = groupBuses[num - HardCodedBusOptions].busName;
				}
				dynamicSoundGroup.busName = busName;
				MasterAudioGroup masterAudioGroup = MasterAudio.GrabGroup(dynamicSoundGroup.name, logIfMissing: false);
				Transform transform;
				if (masterAudioGroup != null)
				{
					masterAudioGroup.AddActorInstanceId(InstanceId);
					transform = masterAudioGroup.transform;
				}
				else
				{
					transform = MasterAudio.CreateSoundGroup(dynamicSoundGroup, InstanceId, errorOnDuplicates);
				}
				for (int k = 0; k < dynamicSoundGroup.groupVariations.Count; k++)
				{
					DynamicGroupVariation dynamicGroupVariation = dynamicSoundGroup.groupVariations[k];
					if (dynamicGroupVariation.LowPassFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.LowPassFilter);
					}
					if (dynamicGroupVariation.HighPassFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.HighPassFilter);
					}
					if (dynamicGroupVariation.DistortionFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.DistortionFilter);
					}
					if (dynamicGroupVariation.ChorusFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.ChorusFilter);
					}
					if (dynamicGroupVariation.EchoFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.EchoFilter);
					}
					if (dynamicGroupVariation.ReverbFilter != null)
					{
						Object.Destroy(dynamicGroupVariation.ReverbFilter);
					}
				}
				if (!(transform == null))
				{
					_groupsToRemove.Add(transform);
				}
			}
			for (int l = 0; l < musicDuckingSounds.Count; l++)
			{
				DuckGroupInfo duckGroupInfo = musicDuckingSounds[l];
				if (!(duckGroupInfo.soundType == "[None]"))
				{
					MasterAudio.AddSoundGroupToDuckList(duckGroupInfo.soundType, duckGroupInfo.riseVolStart, duckGroupInfo.duckedVolumeCut, duckGroupInfo.unduckTime, isTemporary: true);
				}
			}
			for (int m = 0; m < customEventCategories.Count; m++)
			{
				MasterAudio.CreateCustomEventCategoryIfNotThere(customEventCategories[m].CatName, InstanceId, errorOnDuplicates, isTemporary: true);
			}
			for (int n = 0; n < customEventsToCreate.Count; n++)
			{
				CustomEvent customEvent = customEventsToCreate[n];
				MasterAudio.CreateCustomEvent(customEvent.EventName, customEvent.eventReceiveMode, customEvent.distanceThreshold, customEvent.eventRcvFilterMode, customEvent.filterModeQty, InstanceId, customEvent.categoryName, isTemporary: true, errorOnDuplicates);
			}
			for (int num2 = 0; num2 < musicPlaylists.Count; num2++)
			{
				MasterAudio.Playlist aPlaylist = musicPlaylists[num2];
				aPlaylist.isTemporary = true;
				MasterAudio.Playlist playlist = MasterAudio.Instance.musicPlaylists.Find((MasterAudio.Playlist playlist2) => playlist2.playlistName == aPlaylist.playlistName && aPlaylist.isTemporary);
				if (playlist != null)
				{
					playlist.AddActorInstanceId(InstanceId);
					continue;
				}
				MasterAudio.CreatePlaylist(aPlaylist, errorOnDuplicates);
				aPlaylist.AddActorInstanceId(InstanceId);
			}
			MasterAudio.SilenceOrUnsilenceGroupsFromSoloChange();
			_hasCreated = true;
			if (itemsCreatedEventExpanded)
			{
				FireEvents();
			}
		}
	}

	private void FireEvents()
	{
		MasterAudio.FireCustomEventNextFrame(itemsCreatedCustomEvent, _trans);
	}

	public void PopulateGroupData()
	{
		if (_trans == null)
		{
			_trans = base.transform;
		}
		_groupsToCreate.Clear();
		for (int i = 0; i < _trans.childCount; i++)
		{
			DynamicSoundGroup component = _trans.GetChild(i).GetComponent<DynamicSoundGroup>();
			if (component == null)
			{
				continue;
			}
			component.groupVariations.Clear();
			for (int j = 0; j < component.transform.childCount; j++)
			{
				DynamicGroupVariation component2 = component.transform.GetChild(j).GetComponent<DynamicGroupVariation>();
				if (!(component2 == null))
				{
					component.groupVariations.Add(component2);
				}
			}
			_groupsToCreate.Add(component);
		}
	}
}
