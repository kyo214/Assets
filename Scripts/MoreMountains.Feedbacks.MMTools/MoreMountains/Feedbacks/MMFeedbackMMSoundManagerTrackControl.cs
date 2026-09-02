using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Track Control")]
[FeedbackHelp("This feedback will let you control all sounds playing on a specific track (master, UI, music, sfx), and play, pause, mute, unmute, resume, stop, free them all at once. You will need a MMSoundManager in your scene for this to work.")]
public class MMFeedbackMMSoundManagerTrackControl : MMFeedback
{
	public enum ControlModes
	{
		Mute = 0,
		UnMute = 1,
		SetVolume = 2,
		Pause = 3,
		Play = 4,
		Stop = 5,
		Free = 6
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("MMSoundManager Track Control")]
	[Tooltip("the track to mute/unmute/pause/play/stop/free/etc")]
	public MMSoundManager.MMSoundManagerTracks Track;

	[Tooltip("the selected control mode to interact with the track. Free will stop all sounds and return them to the pool")]
	public ControlModes ControlMode = ControlModes.Pause;

	[Tooltip("if setting the volume, the volume to assign to the track")]
	[MMEnumCondition("ControlMode", new int[] { 2 })]
	public float Volume = 0.5f;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			switch (ControlMode)
			{
			case ControlModes.Mute:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack, Track);
				break;
			case ControlModes.UnMute:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack, Track);
				break;
			case ControlModes.SetVolume:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.SetVolumeTrack, Track, Volume);
				break;
			case ControlModes.Pause:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PauseTrack, Track);
				break;
			case ControlModes.Play:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PlayTrack, Track);
				break;
			case ControlModes.Stop:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.StopTrack, Track);
				break;
			case ControlModes.Free:
				MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.FreeTrack, Track);
				break;
			}
		}
	}
}
