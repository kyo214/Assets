using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

public class MasterAudioGroup : MonoBehaviour
{
	public enum TargetDespawnedBehavior
	{
		None = 0,
		Stop = 1,
		FadeOut = 2
	}

	public enum VariationSequence
	{
		Randomized = 0,
		TopToBottom = 1
	}

	public enum VariationMode
	{
		Normal = 0,
		LoopedChain = 1,
		Dialog = 2
	}

	public enum ChainedLoopLoopMode
	{
		Endless = 0,
		NumberOfLoops = 1
	}

	public enum LimitMode
	{
		None = 0,
		FrameBased = 1,
		TimeBased = 2
	}

	public const float UseCurveSpatialBlend = -99f;

	public const string NoBus = "[NO BUS]";

	public const int MinNoRepeatVariations = 3;

	public int busIndex = -1;

	public MasterAudio.ItemSpatialBlendType spatialBlendType = MasterAudio.ItemSpatialBlendType.ForceTo3D;

	public float spatialBlend = 1f;

	public MasterAudio.DefaultGroupPlayType groupPlayType;

	public bool isSelected;

	public bool isExpanded = true;

	public float groupMasterVolume = 1f;

	public int retriggerPercentage = 100;

	public VariationMode curVariationMode;

	public bool alwaysHighestPriority;

	public bool ignoreListenerPause;

	[Range(0f, 10f)]
	public int importance = 5;

	public bool isUninterruptible;

	public float chainLoopDelayMin;

	public float chainLoopDelayMax;

	public ChainedLoopLoopMode chainLoopMode;

	public int chainLoopNumLoops;

	public bool useDialogFadeOut;

	public float dialogFadeOutTime = 0.5f;

	public VariationSequence curVariationSequence;

	public bool useNoRepeatRefill = true;

	public bool useInactivePeriodPoolRefill;

	public float inactivePeriodSeconds = 5f;

	public List<SoundGroupVariation> groupVariations = new List<SoundGroupVariation>();

	public MasterAudio.AudioLocation bulkVariationMode;

	public string comments;

	public bool logSound;

	public bool copySettingsExpanded;

	public bool expandLinkedGroups;

	public List<string> childSoundGroups = new List<string>();

	public List<string> endLinkedGroups = new List<string>();

	public MasterAudio.LinkedGroupSelectionType linkedStartGroupSelectionType;

	public MasterAudio.LinkedGroupSelectionType linkedStopGroupSelectionType;

	public LimitMode limitMode;

	public int limitPerXFrames = 1;

	public float minimumTimeBetween = 0.1f;

	public bool useClipAgePriority;

	public bool limitPolyphony;

	public int voiceLimitCount = 1;

	public TargetDespawnedBehavior targetDespawnedBehavior = TargetDespawnedBehavior.FadeOut;

	public float despawnFadeTime = 0.3f;

	public bool isUsingOcclusion;

	public bool willOcclusionOverrideRaycastOffset;

	public float occlusionRayCastOffset;

	public bool willOcclusionOverrideFrequencies;

	public float occlusionMaxCutoffFreq;

	public float occlusionMinCutoffFreq = 22000f;

	public bool isSoloed;

	public bool isMuted;

	public bool soundPlayedEventActive;

	public string soundPlayedCustomEvent = string.Empty;

	public bool willCleanUpDelegatesAfterStop = true;

	public int addressableUnusedSecondsLifespan;

	public int frames;

	private List<int> _activeAudioSourcesIds = new List<int>();

	private string _objectName = string.Empty;

	private Transform _trans;

	private float _originalVolume = 1f;

	private readonly List<int> _actorInstanceIds = new List<int>();

	public float SpatialBlendForGroup => MasterAudio.Instance.mixerSpatialBlendType switch
	{
		MasterAudio.AllMixerSpatialBlendType.ForceAllTo2D => 0f, 
		MasterAudio.AllMixerSpatialBlendType.ForceAllTo3D => 1f, 
		MasterAudio.AllMixerSpatialBlendType.ForceAllToCustom => MasterAudio.Instance.mixerSpatialBlend, 
		_ => spatialBlendType switch
		{
			MasterAudio.ItemSpatialBlendType.ForceTo2D => 0f, 
			MasterAudio.ItemSpatialBlendType.ForceTo3D => 1f, 
			MasterAudio.ItemSpatialBlendType.ForceToCustom => spatialBlend, 
			_ => -99f, 
		}, 
	};

	public int ActiveVoices => ActiveAudioSourceIds.Count;

	public int AddressableUnusedSecondsLifespan => addressableUnusedSecondsLifespan;

	public int TotalVoices => base.transform.childCount;

	public bool WillCleanUpDelegatesAfterStop
	{
		set
		{
			willCleanUpDelegatesAfterStop = value;
		}
	}

	public GroupBus BusForGroup
	{
		get
		{
			if (busIndex < 2)
			{
				return null;
			}
			int num = busIndex - 2;
			if (num >= MasterAudio.GroupBuses.Count)
			{
				return null;
			}
			return MasterAudio.GroupBuses[num];
		}
	}

	public float OriginalVolume
	{
		get
		{
			return _originalVolume;
		}
		set
		{
			_originalVolume = value;
		}
	}

	public bool LoggingEnabledForGroup
	{
		get
		{
			if (!logSound)
			{
				return MasterAudio.LogSoundsEnabled;
			}
			return true;
		}
	}

	public int ChainLoopCount { get; set; }

	public string GameObjectName
	{
		get
		{
			if (string.IsNullOrEmpty(_objectName))
			{
				_objectName = base.name;
			}
			return _objectName;
		}
	}

	public MasterAudio.GroupPlayType GroupPlayType
	{
		get
		{
			if (MasterAudio.Instance.groupPlayType != MasterAudio.GroupPlayType.AllowDifferentPerGroup)
			{
				return MasterAudio.Instance.groupPlayType;
			}
			return groupPlayType switch
			{
				MasterAudio.DefaultGroupPlayType.Always => MasterAudio.GroupPlayType.Always, 
				MasterAudio.DefaultGroupPlayType.WhenActorInAudibleRange => MasterAudio.GroupPlayType.WhenActorInAudibleRange, 
				_ => throw new Exception("Illegal Group Play Type: " + groupPlayType), 
			};
		}
	}

	public bool HasLiveActors => _actorInstanceIds.Count > 0;

	public bool UsesNoRepeat
	{
		get
		{
			if (curVariationSequence == VariationSequence.Randomized && groupVariations.Count >= 3)
			{
				return useNoRepeatRefill;
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

	private List<int> ActiveAudioSourceIds
	{
		get
		{
			if (_activeAudioSourcesIds != null)
			{
				return _activeAudioSourcesIds;
			}
			_activeAudioSourcesIds = new List<int>(Trans.childCount);
			return _activeAudioSourcesIds;
		}
	}

	public event Action LastVariationFinishedPlay;

	private void Start()
	{
		_objectName = base.name;
		_ = ActiveAudioSourceIds.Count;
		_ = 0;
		if (Trans.parent != null)
		{
			base.gameObject.layer = Trans.parent.gameObject.layer;
		}
	}

	public void AddActiveAudioSourceId(int varInstanceId)
	{
		if (!ActiveAudioSourceIds.Contains(varInstanceId))
		{
			ActiveAudioSourceIds.Add(varInstanceId);
			BusForGroup?.AddActiveAudioSourceId(varInstanceId);
		}
	}

	public void RemoveActiveAudioSourceId(int varInstanceId)
	{
		ActiveAudioSourceIds.Remove(varInstanceId);
		BusForGroup?.RemoveActiveAudioSourceId(varInstanceId);
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

	public void FireLastVariationFinishedPlay()
	{
		if (LastVariationFinishedPlay != null)
		{
			LastVariationFinishedPlay();
		}
	}

	public void SubscribeToLastVariationFinishedPlay(Action finishedCallback)
	{
		LastVariationFinishedPlay = null;
		LastVariationFinishedPlay += finishedCallback;
	}

	public void UnsubscribeFromLastVariationFinishedPlay()
	{
		LastVariationFinishedPlay = null;
	}
}
