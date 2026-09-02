using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.CustomTrack.DeactivateTrack;

[Serializable]
[TrackClipType(typeof(DeactivationClip))]
[TrackBindingType(typeof(GameObject))]
[ExcludeFromPreset]
public class DeactivationTrack : TrackAsset
{
	public enum PostPlaybackState
	{
		Active = 0,
		Inactive = 1,
		Revert = 2,
		LeaveAsIs = 3
	}

	[SerializeField]
	private PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;

	private DeactivationMixerPlayable _mDeactivationMixerPlayable;

	public PostPlaybackState postPlaybackState
	{
		get
		{
			return m_PostPlaybackState;
		}
		set
		{
			m_PostPlaybackState = value;
			UpdateTrackMode();
		}
	}

	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		ScriptPlayable<DeactivationMixerPlayable> scriptPlayable = DeactivationMixerPlayable.Create(graph, inputCount);
		_mDeactivationMixerPlayable = scriptPlayable.GetBehaviour();
		UpdateTrackMode();
		return scriptPlayable;
	}

	internal void UpdateTrackMode()
	{
		if (_mDeactivationMixerPlayable != null)
		{
			_mDeactivationMixerPlayable.postPlaybackState = m_PostPlaybackState;
		}
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		GameObject gameObjectBinding = GetGameObjectBinding(director);
		if (gameObjectBinding != null)
		{
			driver.AddFromName(gameObjectBinding, "m_IsActive");
		}
	}

	protected override void OnCreateClip(TimelineClip clip)
	{
		clip.displayName = "Deactivate";
		base.OnCreateClip(clip);
	}

	private GameObject GetGameObjectBinding(PlayableDirector director)
	{
		if (director == null)
		{
			return null;
		}
		UnityEngine.Object genericBinding = director.GetGenericBinding(this);
		GameObject gameObject = genericBinding as GameObject;
		if (gameObject != null)
		{
			return gameObject;
		}
		Component component = genericBinding as Component;
		if (component != null)
		{
			return component.gameObject;
		}
		return null;
	}
}
