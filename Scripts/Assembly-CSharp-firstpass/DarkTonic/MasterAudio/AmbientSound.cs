using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

[AddComponentMenu("Dark Tonic/Master Audio/Ambient Sound")]
[AudioScriptOrder(-20)]
public class AmbientSound : MonoBehaviour
{
	[SoundGroup]
	public string AmbientSoundGroup = "[None]";

	public EventSounds.VariationType variationType = EventSounds.VariationType.PlayRandom;

	public string variationName = string.Empty;

	public float playVolume = 1f;

	public MasterAudio.AmbientSoundExitMode exitMode;

	public float exitFadeTime = 0.5f;

	public MasterAudio.AmbientSoundReEnterMode reEnterMode;

	public float reEnterFadeTime = 0.5f;

	[Tooltip("This option is useful if your caller ever moves, as it will make the Audio Source follow to the location of the caller every frame.")]
	public bool FollowCaller;

	[Tooltip("Using this option, the Audio Source will be updated every frame to the closest position on the caller's collider, if any. This will override the Follow Caller option above and happen instead.")]
	public bool UseClosestColliderPosition;

	public bool UseTopCollider = true;

	public bool IncludeChildColliders;

	[Tooltip("This is for diagnostic purposes only. Do not change or assign this field.")]
	public Transform RuntimeFollower;

	private Transform _trans;

	public float colliderMaxDistance;

	public long lastTimeMaxDistanceCalced;

	public bool IsValidSoundGroup => !MasterAudio.SoundGroupHardCodedNames.Contains(AmbientSoundGroup);

	public Transform Trans
	{
		get
		{
			if (_trans == null)
			{
				_trans = base.transform;
			}
			return _trans;
		}
	}

	private void OnEnable()
	{
		MasterAudio.SetupAmbientNextFrame(this);
	}

	private void OnDisable()
	{
		if (!MasterAudio.AppIsShuttingDown && IsValidSoundGroup && !(MasterAudio.SafeInstance == null))
		{
			MasterAudio.RemoveDelayedAmbient(this);
			StopTrackers();
		}
	}

	private void StopTrackers()
	{
		if (MasterAudio.GrabGroup(AmbientSoundGroup, logIfMissing: false) != null)
		{
			switch (exitMode)
			{
			case MasterAudio.AmbientSoundExitMode.StopSound:
				MasterAudio.StopSoundGroupOfTransform(Trans, AmbientSoundGroup);
				break;
			case MasterAudio.AmbientSoundExitMode.FadeSound:
				MasterAudio.FadeOutSoundGroupOfTransform(Trans, AmbientSoundGroup, exitFadeTime);
				break;
			}
		}
		RuntimeFollower = null;
	}

	public void CalculateRadius()
	{
		AudioSource namedOrFirstAudioSource = GetNamedOrFirstAudioSource();
		if (namedOrFirstAudioSource == null)
		{
			colliderMaxDistance = 0f;
			return;
		}
		colliderMaxDistance = namedOrFirstAudioSource.maxDistance;
		lastTimeMaxDistanceCalced = DateTime.Now.Ticks;
	}

	public AudioSource GetNamedOrFirstAudioSource()
	{
		if (string.IsNullOrEmpty(AmbientSoundGroup))
		{
			colliderMaxDistance = 0f;
			return null;
		}
		if (MasterAudio.SafeInstance == null)
		{
			colliderMaxDistance = 0f;
			return null;
		}
		Transform transform = MasterAudio.Instance.transform.Find(AmbientSoundGroup);
		if (transform == null)
		{
			colliderMaxDistance = 0f;
			return null;
		}
		Transform transform2 = null;
		switch (variationType)
		{
		case EventSounds.VariationType.PlayRandom:
			transform2 = transform.GetChild(0);
			break;
		case EventSounds.VariationType.PlaySpecific:
			transform2 = transform.transform.Find(variationName);
			break;
		}
		if (transform2 == null)
		{
			colliderMaxDistance = 0f;
			return null;
		}
		return transform2.GetComponent<AudioSource>();
	}

	public List<AudioSource> GetAllVariationAudioSources()
	{
		if (string.IsNullOrEmpty(AmbientSoundGroup))
		{
			colliderMaxDistance = 0f;
			return null;
		}
		if (MasterAudio.SafeInstance == null)
		{
			colliderMaxDistance = 0f;
			return null;
		}
		Transform transform = MasterAudio.Instance.transform.Find(AmbientSoundGroup);
		if (transform == null)
		{
			colliderMaxDistance = 0f;
			return null;
		}
		List<AudioSource> list = new List<AudioSource>(transform.childCount);
		for (int i = 0; i < transform.childCount; i++)
		{
			AudioSource component = transform.GetChild(i).GetComponent<AudioSource>();
			list.Add(component);
		}
		return list;
	}

	private void OnDrawGizmos()
	{
		if (MasterAudio.SafeInstance == null || !MasterAudio.Instance.showRangeSoundGizmos)
		{
			return;
		}
		if (lastTimeMaxDistanceCalced < DateTime.Now.AddHours(-1.0).Ticks)
		{
			lastTimeMaxDistanceCalced = DateTime.Now.Ticks;
			CalculateRadius();
		}
		if (colliderMaxDistance != 0f)
		{
			Color color = Color.green;
			if (MasterAudio.SafeInstance != null)
			{
				color = MasterAudio.Instance.rangeGizmoColor;
			}
			Gizmos.color = color;
			Gizmos.DrawWireSphere(base.transform.position, colliderMaxDistance);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (MasterAudio.SafeInstance == null || !MasterAudio.Instance.showSelectedRangeSoundGizmos)
		{
			return;
		}
		if (lastTimeMaxDistanceCalced < DateTime.Now.AddHours(-1.0).Ticks)
		{
			lastTimeMaxDistanceCalced = DateTime.Now.Ticks;
			CalculateRadius();
		}
		if (colliderMaxDistance != 0f)
		{
			Color color = Color.green;
			if (MasterAudio.SafeInstance != null)
			{
				color = MasterAudio.Instance.selectedRangeGizmoColor;
			}
			Gizmos.color = color;
			Gizmos.DrawWireSphere(base.transform.position, colliderMaxDistance);
		}
	}

	public void StartTrackers()
	{
		if (!IsValidSoundGroup)
		{
			return;
		}
		if (Physics.GetIgnoreLayerCollision(2, 2))
		{
			MasterAudio.LogWarningIfNeverLogged("You have disabled collisions between Ignore Raycast layer and itself on the Physics Layer Collision matrix. This must be turned back on or Ambient Sounds script will not function.", 1);
			return;
		}
		if (!AmbientUtil.InitListenerFollower())
		{
			MasterAudio.LogWarning("Your Ambient Sound script on Game Object '" + base.name + "' will not function because you have no Audio Listener component in any active Game Object in the Scene.");
			return;
		}
		if (!AmbientUtil.HasListenerFolowerRigidBody)
		{
			MasterAudio.LogWarning("Your Ambient Sound script on Game Object '" + base.name + "' will not function because you have turned off the Listener Follower RigidBody in Advanced Settings.");
		}
		string followerName = base.name + "_" + AmbientSoundGroup + "_Follower_" + Guid.NewGuid().ToString();
		RuntimeFollower = AmbientUtil.InitAudioSourceFollower(Trans, followerName, AmbientSoundGroup, variationName, playVolume, FollowCaller, UseClosestColliderPosition, UseTopCollider, IncludeChildColliders, exitMode, exitFadeTime, reEnterMode, reEnterFadeTime);
	}
}
