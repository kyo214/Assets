using UnityEngine;
using UnityEngine.Video;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control video players in all sorts of ways (Play, Pause, Toggle, Stop, Prepare, StepForward, StepBackward, SetPlaybackSpeed, SetDirectAudioVolume, SetDirectAudioMute, GoToFrame, ToggleLoop)")]
[FeedbackPath("UI/Video Player")]
public class MMFeedbackVideoPlayer : MMFeedback
{
	public enum VideoActions
	{
		Play = 0,
		Pause = 1,
		Toggle = 2,
		Stop = 3,
		Prepare = 4,
		StepForward = 5,
		StepBackward = 6,
		SetPlaybackSpeed = 7,
		SetDirectAudioVolume = 8,
		SetDirectAudioMute = 9,
		GoToFrame = 10,
		ToggleLoop = 11
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Video Player")]
	[Tooltip("the Video Player to control with this feedback")]
	public VideoPlayer TargetVideoPlayer;

	[Tooltip("the Video Player to control with this feedback")]
	public VideoActions VideoAction = VideoActions.Pause;

	[Tooltip("the frame at which to jump when in GoToFrame mode")]
	[MMFEnumCondition("VideoAction", new int[] { 10 })]
	public long TargetFrame = 10L;

	[Tooltip("the new playback speed (between 0 and 10)")]
	[MMFEnumCondition("VideoAction", new int[] { 7 })]
	public float PlaybackSpeed = 2f;

	[Tooltip("the track index on which to control volume")]
	[MMFEnumCondition("VideoAction", new int[] { 9, 8 })]
	public int TrackIndex;

	[Tooltip("the new volume for the specified track, between 0 and 1")]
	[MMFEnumCondition("VideoAction", new int[] { 8 })]
	public float Volume = 1f;

	[Tooltip("whether to mute the track or not when that feedback plays")]
	[MMFEnumCondition("VideoAction", new int[] { 9 })]
	public bool Mute = true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetVideoPlayer == null)
		{
			return;
		}
		switch (VideoAction)
		{
		case VideoActions.Play:
			TargetVideoPlayer.Play();
			break;
		case VideoActions.Pause:
			TargetVideoPlayer.Pause();
			break;
		case VideoActions.Toggle:
			if (TargetVideoPlayer.isPlaying)
			{
				TargetVideoPlayer.Pause();
			}
			else
			{
				TargetVideoPlayer.Play();
			}
			break;
		case VideoActions.Stop:
			TargetVideoPlayer.Stop();
			break;
		case VideoActions.Prepare:
			TargetVideoPlayer.Prepare();
			break;
		case VideoActions.StepForward:
			TargetVideoPlayer.StepForward();
			break;
		case VideoActions.StepBackward:
			TargetVideoPlayer.Pause();
			TargetVideoPlayer.frame -= 1;
			break;
		case VideoActions.SetPlaybackSpeed:
			TargetVideoPlayer.playbackSpeed = PlaybackSpeed;
			break;
		case VideoActions.SetDirectAudioVolume:
			TargetVideoPlayer.SetDirectAudioVolume((ushort)TrackIndex, Volume);
			break;
		case VideoActions.SetDirectAudioMute:
			TargetVideoPlayer.SetDirectAudioMute((ushort)TrackIndex, Mute);
			break;
		case VideoActions.GoToFrame:
			TargetVideoPlayer.frame = TargetFrame;
			break;
		case VideoActions.ToggleLoop:
			TargetVideoPlayer.isLooping = !TargetVideoPlayer.isLooping;
			break;
		}
	}
}
