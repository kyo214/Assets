using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio;

[Serializable]
public class GroupBus
{
	public string busName;

	public float volume = 1f;

	public bool isSoloed;

	public bool isMuted;

	public int voiceLimit = -1;

	public bool isExisting;

	public bool isTemporary;

	public bool isUsingOcclusion;

	public MasterAudio.BusVoiceLimitExceededMode busVoiceLimitExceededMode;

	public Color busColor = Color.white;

	public AudioMixerGroup mixerChannel;

	public bool forceTo2D;

	private readonly List<int> _activeAudioSourcesIds = new List<int>(50);

	private readonly List<int> _actorInstanceIds = new List<int>();

	private float _originalVolume = 1f;

	public int ActiveVoices => _activeAudioSourcesIds.Count;

	public bool HasLiveActors => _actorInstanceIds.Count > 0;

	public bool BusVoiceLimitReached
	{
		get
		{
			if (voiceLimit <= 0)
			{
				return false;
			}
			return _activeAudioSourcesIds.Count >= voiceLimit;
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

	public void AddActiveAudioSourceId(int id)
	{
		if (!_activeAudioSourcesIds.Contains(id))
		{
			_activeAudioSourcesIds.Add(id);
		}
	}

	public void RemoveActiveAudioSourceId(int id)
	{
		_activeAudioSourcesIds.Remove(id);
	}
}
