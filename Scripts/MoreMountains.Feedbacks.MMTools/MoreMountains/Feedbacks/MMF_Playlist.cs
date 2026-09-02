using System;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you pilot a MMPlaylist")]
[FeedbackPath("Audio/MMPlaylist")]
public class MMF_Playlist : MMF_Feedback
{
	public enum Modes
	{
		Play = 0,
		PlayNext = 1,
		PlayPrevious = 2,
		Stop = 3,
		Pause = 4,
		PlaySongAt = 5
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("MMPlaylist", true, 13, false, false)]
	[Tooltip("the action to call on the playlist")]
	public Modes Mode = Modes.PlayNext;

	[Tooltip("the index of the song to play")]
	[MMEnumCondition("Mode", new int[] { 5 })]
	public int SongIndex;

	protected Coroutine _coroutine;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			switch (Mode)
			{
			case Modes.Play:
				MMPlaylistPlayEvent.Trigger(Channel);
				break;
			case Modes.PlayNext:
				MMPlaylistPlayNextEvent.Trigger(Channel);
				break;
			case Modes.PlayPrevious:
				MMPlaylistPlayPreviousEvent.Trigger(Channel);
				break;
			case Modes.Stop:
				MMPlaylistStopEvent.Trigger(Channel);
				break;
			case Modes.Pause:
				MMPlaylistPauseEvent.Trigger(Channel);
				break;
			case Modes.PlaySongAt:
				MMPlaylistPlayIndexEvent.Trigger(Channel, SongIndex);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
