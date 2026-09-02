using System;
using UnityEngine;

namespace DarkTonic.MasterAudio.Multiplayer;

public class MasterAudioMultiplayerAdapter : MonoBehaviour
{
	public static bool CanSendRPCs => false;

	public void Awake()
	{
		Debug.LogError("You must install one of the multiplayer packages in order for MAM to work. The default can compile but doesn't do anything.");
	}

	public static void FireCustomEvent(string enterCustomEvent, Transform _actorTrans)
	{
	}

	public static void AudioListenerPause(Transform _actorTrans)
	{
	}

	public static void AudioListenerUnpause(Transform _actorTrans)
	{
	}

	public static void StopSoundGroupOfTransform(Transform _actorTrans, string timedSoundGroup)
	{
	}

	public static void PlaySound3DFollowTransformAndForget(string enterSoundGroup, Transform _actorTrans)
	{
	}

	public static void PlaySound3DFollowTransformAndForget(string enterSoundGroup, Transform _actorTrans, float volume, float? pitch, float delay, string varName)
	{
	}

	public static void PlaySound3DAtTransformAndForget(string enterSoundGroup, Transform _actorTrans)
	{
	}

	public static void PlaySound3DAtTransformAndForget(string enterSoundGroup, Transform _actorTrans, float volume, float? pitch, float delay, string varName)
	{
	}

	public static PlaySoundResult PlaySound3DAtTransform(string sType, Transform trans, float volume, float? pitch, float delay, string variationName)
	{
		return null;
	}

	public static PlaySoundResult PlaySound3DFollowTransform(string sType, Transform trans, float volume, float? pitch, float delay, string variationName)
	{
		return null;
	}

	public static PlaySoundResult PlaySound(Transform trans, string sType, float volume, float? pitch, float delay, string variationName)
	{
		return null;
	}

	public static void FadeOutAllOfSound(Transform trans, string soundType, float fadeTime)
	{
	}

	public static void PlaySound3DAtTransformAndForget(string sType, Transform trans, float volume, float? pitch, float delaySound)
	{
	}

	public static void PlaySound3DFollowTransformAndForget(string sType, Transform trans, float volume, float? pitch, float delaySound)
	{
	}

	public static void PlaySoundAndForget(Transform trans, string sType, float volume, float? pitch, float delaySound)
	{
	}

	public static void PlaySoundAndForget(Transform trans, string sType, float volume, float? pitch, float delay, string variationName)
	{
	}

	public static void FadeOutSoundGroupOfTransform(Transform trans, string soundType, float fadeTime)
	{
	}

	public static void RefillSoundGroupPool(Transform trans, string soundType)
	{
	}

	public static void FadeSoundGroupToVolume(Transform trans, string soundType, float targetVol, float fadeTime, Action completionCallback, bool stopAfterFade, bool restoreVolumeAfterFade)
	{
	}

	public static void MuteGroup(Transform trans, string soundType)
	{
	}

	public static void PauseSoundGroup(Transform trans, string soundType)
	{
	}

	public static void SoloGroup(Transform trans, string soundType)
	{
	}

	public static void StopAllOfSound(Transform trans, string soundType)
	{
	}

	public static void UnmuteGroup(Transform trans, string soundType)
	{
	}

	public static void UnpauseSoundGroup(Transform trans, string soundType)
	{
	}

	public static void UnsoloGroup(Transform trans, string soundType)
	{
	}

	public static void StopAllSoundsOfTransform(Transform trans)
	{
	}

	public static void PauseAllSoundsOfTransform(Transform trans)
	{
	}

	public static void PauseSoundGroupOfTransform(Transform trans, string soundType)
	{
	}

	public static void UnpauseAllSoundsOfTransform(Transform trans)
	{
	}

	public static void UnpauseSoundGroupOfTransform(Transform trans, string soundType)
	{
	}

	public static void FadeOutAllSoundsOfTransform(Transform trans, float fadeTime)
	{
	}

	public static void RouteGroupToBus(Transform trans, string soundType, string busName)
	{
	}

	public static void GlideSoundGroupByPitch(Transform trans, string soundType, float targetGlidePitch, float pitchGlideTime, Action completionCallback)
	{
	}

	public static void StopOldSoundGroupVoices(Transform trans, string soundType, float minAge)
	{
	}

	public static void FadeOutOldSoundGroupVoices(Transform trans, string soundType, float minAge, float fadeTime)
	{
	}

	public static void FadeBusToVolume(Transform trans, string busName, float targetVol, float fadeTime, Action completionCallback, bool stopAfterFade, bool restoreVolumeAfterFade)
	{
	}

	public static void GlideBusByPitch(Transform trans, string busName, float targetGlidePitch, float pitchGlideTime, Action completionCallback)
	{
	}

	public static void PauseBus(Transform trans, string busName)
	{
	}

	public static void StopBus(Transform trans, string busName)
	{
	}

	public static void UnpauseBus(Transform trans, string busName)
	{
	}

	public static void MuteBus(Transform trans, string busName)
	{
	}

	public static void UnmuteBus(Transform trans, string busName)
	{
	}

	public static void ToggleMuteBus(Transform trans, string busName)
	{
	}

	public static void SoloBus(Transform trans, string busName)
	{
	}

	public static void UnsoloBus(Transform trans, string busName)
	{
	}

	public static void ChangeBusPitch(Transform trans, string busName, float pitch)
	{
	}

	public static void PauseBusOfTransform(Transform trans, string busName)
	{
	}

	public static void UnpauseBusOfTransform(Transform trans, string busName)
	{
	}

	public static void RestartAllPlaylists(Transform trans)
	{
	}

	public static void StopBusOfTransform(Transform trans, string busName)
	{
	}

	public static void StopOldBusVoices(Transform trans, string busName, float minAge)
	{
	}

	public static void FadeOutOldBusVoices(Transform trans, string busName, float minAge, float fadeTime)
	{
	}

	public static void SetMasterMixerVolume(Transform trans, float targetVol)
	{
	}

	public static void SetPlaylistMasterVolume(Transform trans, float tgtVol)
	{
	}

	public static void PauseMixer(Transform trans)
	{
	}

	public static void UnpauseMixer(Transform trans)
	{
	}

	public static void StopMixer(Transform trans)
	{
	}

	public static void MuteEverything(Transform trans)
	{
	}

	public static void UnmuteEverything(Transform trans)
	{
	}

	public static void PauseEverything(Transform trans)
	{
	}

	public static void UnpauseEverything(Transform trans)
	{
	}

	public static void StopEverything(Transform trans)
	{
	}

	public static void RestartPlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static void StartPlaylist(Transform trans, string playlistControllerName, string playlistName)
	{
	}

	public static void ChangePlaylistByName(Transform trans, string playlistControllerName, string playlistName, bool startPlaylist)
	{
	}

	public static void StopLoopingAllCurrentSongs(Transform trans)
	{
	}

	public static void StopLoopingCurrentSong(Transform trans, string playlistControllerName)
	{
	}

	public static void StopAllPlaylistsAfterCurrentSongs(Transform trans)
	{
	}

	public static void StopPlaylistAfterCurrentSong(Transform trans, string playlistControllerName)
	{
	}

	public static void FadeAllPlaylistsToVolume(Transform trans, float targetVol, float fadeTime)
	{
	}

	public static void FadePlaylistToVolume(Transform trans, string playlistControllerName, float targetVol, float fadeTime)
	{
	}

	public static void MuteAllPlaylists(Transform trans)
	{
	}

	public static void MutePlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static void UnmuteAllPlaylists(Transform trans)
	{
	}

	public static void UnmutePlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static void ToggleMuteAllPlaylists(Transform trans)
	{
	}

	public static void ToggleMutePlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static bool TriggerPlaylistClip(Transform trans, string playlistControllerName, string clipName)
	{
		return false;
	}

	public static void QueuePlaylistClip(Transform trans, string playlistControllerName, string clipName)
	{
	}

	public static void TriggerRandomClipAllPlaylists(Transform trans)
	{
	}

	public static void TriggerRandomPlaylistClip(Transform trans, string playlistControllerName)
	{
	}

	public static void TriggerNextClipAllPlaylists(Transform trans)
	{
	}

	public static void TriggerNextPlaylistClip(Transform trans, string playlistControllerName)
	{
	}

	public static void PauseAllPlaylists(Transform trans)
	{
	}

	public static void PausePlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static void StopAllPlaylists(Transform trans)
	{
	}

	public static void StopPlaylist(Transform trans, string playlistControllerName)
	{
	}

	public static void UnpauseAllPlaylists(Transform trans)
	{
	}

	public static void UnpausePlaylist(Transform trans, string playlistControllerName)
	{
	}
}
