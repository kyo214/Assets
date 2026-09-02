using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Audio/MMPlaylistRemote")]
public class MMPlaylistRemote : MonoBehaviour
{
	public int Channel;

	public int TrackNumber;

	[Header("Triggers")]
	public bool PlaySelectedTrackOnTriggerEnter = true;

	public bool PlaySelectedTrackOnTriggerExit;

	public string TriggerTag = "Player";

	[Header("Test")]
	[MMInspectorButton("Play")]
	public bool PlayButton;

	[MMInspectorButton("Pause")]
	public bool PauseButton;

	[MMInspectorButton("Stop")]
	public bool StopButton;

	[MMInspectorButton("PlayNextTrack")]
	public bool NextButton;

	[MMInspectorButton("PlaySelectedTrack")]
	public bool SelectedTrackButton;

	public virtual void Play()
	{
		MMPlaylistPlayEvent.Trigger(Channel);
	}

	public virtual void Pause()
	{
		MMPlaylistPauseEvent.Trigger(Channel);
	}

	public virtual void Stop()
	{
		MMPlaylistStopEvent.Trigger(Channel);
	}

	public virtual void PlayNextTrack()
	{
		MMPlaylistPlayNextEvent.Trigger(Channel);
	}

	public virtual void PlaySelectedTrack()
	{
		MMPlaylistPlayIndexEvent.Trigger(Channel, TrackNumber);
	}

	public virtual void PlayTrack(int trackIndex)
	{
		MMPlaylistPlayIndexEvent.Trigger(Channel, trackIndex);
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (PlaySelectedTrackOnTriggerEnter && collider.CompareTag(TriggerTag))
		{
			PlaySelectedTrack();
		}
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (PlaySelectedTrackOnTriggerExit && collider.CompareTag(TriggerTag))
		{
			PlaySelectedTrack();
		}
	}
}
