using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DarkTonic.MasterAudio;

public static class AudioAddressableOptimizer
{
	private static readonly Dictionary<string, AddressableTracker<AudioClip>> AddressableTasksByAddressableId = new Dictionary<string, AddressableTracker<AudioClip>>();

	private static readonly object SyncRoot = new object();

	public static IEnumerator PopulateSourceWithAddressableClipAsync(AssetReference addressable, SoundGroupVariation variation, int unusedSecondsLifespan, Action successAction, Action failureAction)
	{
		bool isWarmingCall = MasterAudio.IsWarming;
		if (!IsAddressableValid(addressable))
		{
			failureAction?.Invoke();
			if (isWarmingCall)
			{
				DTMonoHelper.SetActive(variation.GameObj, isActive: false);
			}
			yield break;
		}
		string addressableId = GetAddressableId(addressable);
		bool shouldReleaseLoadedAssetNow = false;
		AsyncOperationHandle<AudioClip> loadHandle;
		AudioClip result;
		if (AddressableTasksByAddressableId.ContainsKey(addressableId))
		{
			loadHandle = AddressableTasksByAddressableId[addressableId].AssetHandle;
			result = loadHandle.Result;
		}
		else
		{
			loadHandle = Addressables.LoadAssetAsync<AudioClip>(addressable);
			while (!loadHandle.IsDone)
			{
				yield return MasterAudio.EndOfFrameDelay;
			}
			result = loadHandle.Result;
			if (result == null || loadHandle.Status != AsyncOperationStatus.Succeeded)
			{
				string text = "";
				if (loadHandle.OperationException != null)
				{
					text = " Exception: " + loadHandle.OperationException.Message;
				}
				MasterAudio.LogError("Addressable file for '" + variation.GameObjectName + "' could not be located." + text);
				failureAction?.Invoke();
				if (isWarmingCall)
				{
					DTMonoHelper.SetActive(variation.GameObj, isActive: false);
				}
				yield break;
			}
			lock (SyncRoot)
			{
				if (!AddressableTasksByAddressableId.ContainsKey(addressableId))
				{
					AddressableTasksByAddressableId.Add(addressableId, new AddressableTracker<AudioClip>(loadHandle, unusedSecondsLifespan));
				}
				else
				{
					shouldReleaseLoadedAssetNow = true;
					result = AddressableTasksByAddressableId[addressableId].AssetHandle.Result;
				}
			}
		}
		if (shouldReleaseLoadedAssetNow)
		{
			Addressables.Release(loadHandle);
		}
		if (!AudioUtil.AudioClipWillPreload(result))
		{
			MasterAudio.LogWarning("Audio Clip for Addressable file '" + result.CachedName() + "' of Sound Group '" + variation.ParentGroup.GameObjectName + "' has 'Preload Audio Data' turned off, which can cause audio glitches. Addressables should always Preload Audio Data. Please turn it on.");
		}
		variation.LoadStatus = MasterAudio.VariationLoadStatus.Loaded;
		if (!variation.IsStopRequested)
		{
			variation.VarAudio.clip = result;
			successAction?.Invoke();
		}
	}

	public static void AddAddressablePlayingClip(AssetReference addressable, AudioSource holderSource)
	{
		if (!IsAddressableValid(addressable))
		{
			return;
		}
		string addressableId = GetAddressableId(addressable);
		if (!AddressableTasksByAddressableId.ContainsKey(addressableId))
		{
			Debug.Log("Addressable not found in loaded map: id = '" + addressable?.ToString() + "'. Aborting recording play.");
			return;
		}
		MasterAudio.RemoveAddressableFromDelayedRelease(addressableId);
		AddressableTracker<AudioClip> addressableTracker = AddressableTasksByAddressableId[addressableId];
		if (!addressableTracker.AudiosSourcesUsingReference.Contains(holderSource))
		{
			addressableTracker.AudiosSourcesUsingReference.Add(holderSource);
		}
	}

	public static void RemoveAddressablePlayingClip(AssetReference addressable, AudioSource holderSource, bool forceRemove = false)
	{
		if (IsAddressableValid(addressable))
		{
			string addressableId = GetAddressableId(addressable);
			if (AddressableTasksByAddressableId.ContainsKey(addressableId))
			{
				AddressableTasksByAddressableId[addressableId].AudiosSourcesUsingReference.Remove(holderSource);
				ReleaseAddressableIfNoUses(addressable, forceRemove);
			}
		}
	}

	public static void MaybeReleaseAddressable(string addressableId, bool forceRelease = false)
	{
		if (AddressableTasksByAddressableId.ContainsKey(addressableId))
		{
			AddressableTracker<AudioClip> addressableTracker = AddressableTasksByAddressableId[addressableId];
			if (forceRelease || addressableTracker.UnusedSecondsLifespan == 0)
			{
				AsyncOperationHandle<AudioClip> assetHandle = addressableTracker.AssetHandle;
				AddressableTasksByAddressableId.Remove(addressableId);
				Addressables.Release(assetHandle);
			}
			else
			{
				MasterAudio.AddAddressableForDelayedRelease(addressableId, addressableTracker.UnusedSecondsLifespan);
			}
		}
	}

	public static bool IsAddressableValid(AssetReference addressable)
	{
		return addressable?.RuntimeKeyIsValid() ?? false;
	}

	public static IEnumerator PopulateAddressableSongToPlaylistControllerAsync(MusicSetting setting, AssetReference addressable, PlaylistController playlistController, PlaylistController.AudioPlayType playType)
	{
		if (!IsAddressableValid(addressable))
		{
			yield break;
		}
		string addressableId = GetAddressableId(addressable);
		bool shouldReleaseLoadedAssetNow = false;
		AsyncOperationHandle<AudioClip> loadHandle;
		AudioClip result;
		if (AddressableTasksByAddressableId.ContainsKey(addressableId))
		{
			loadHandle = AddressableTasksByAddressableId[addressableId].AssetHandle;
			result = loadHandle.Result;
		}
		else
		{
			loadHandle = Addressables.LoadAssetAsync<AudioClip>(addressable);
			while (!loadHandle.IsDone)
			{
				yield return MasterAudio.EndOfFrameDelay;
			}
			result = loadHandle.Result;
			if (result == null || loadHandle.Status != AsyncOperationStatus.Succeeded)
			{
				string text = "";
				if (loadHandle.OperationException != null)
				{
					text = " Exception: " + loadHandle.OperationException.Message;
				}
				MasterAudio.LogError("Addressable file for PlaylistController '" + playlistController.ControllerName + "' could not be located." + text);
				yield break;
			}
			lock (SyncRoot)
			{
				if (!AddressableTasksByAddressableId.ContainsKey(addressableId))
				{
					AddressableTasksByAddressableId.Add(addressableId, new AddressableTracker<AudioClip>(loadHandle, 0));
				}
				else
				{
					shouldReleaseLoadedAssetNow = true;
					result = AddressableTasksByAddressableId[addressableId].AssetHandle.Result;
				}
			}
		}
		if (shouldReleaseLoadedAssetNow)
		{
			Addressables.Release(loadHandle);
		}
		if (!AudioUtil.AudioClipWillPreload(result))
		{
			MasterAudio.LogWarning("Audio Clip for Addressable file '" + result.CachedName() + "' of Playlist Controller '" + playlistController.ControllerName + "' has 'Preload Audio Data' turned off, which can cause audio glitches. Addressables should always Preload Audio Data. Please turn it on.");
		}
		if (0 == 0)
		{
			playlistController.FinishLoadingNewSong(setting, result, playType);
		}
	}

	private static bool IsAnyOfAddressableClipPlaying(AssetReference addressable)
	{
		string addressableId = GetAddressableId(addressable);
		if (!AddressableTasksByAddressableId.ContainsKey(addressableId))
		{
			return false;
		}
		return AddressableTasksByAddressableId[addressableId].AudiosSourcesUsingReference.Count > 0;
	}

	private static void ReleaseAddressableIfNoUses(AssetReference addressable, bool forceRemove = false)
	{
		if (!IsAnyOfAddressableClipPlaying(addressable))
		{
			MaybeReleaseAddressable(GetAddressableId(addressable), forceRemove);
		}
	}

	private static string GetAddressableId(AssetReference addressable)
	{
		return addressable.RuntimeKey.ToString();
	}
}
