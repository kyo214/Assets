using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DarkTonic.MasterAudio;

[Serializable]
public class MusicSetting
{
	public string alias = string.Empty;

	public MasterAudio.AudioLocation audLocation;

	public AudioClip clip;

	public string songName = string.Empty;

	public string resourceFileName = string.Empty;

	public AssetReference audioClipAddressable;

	public float volume = 1f;

	public float pitch = 1f;

	public bool isExpanded = true;

	public bool isLoop;

	public bool isChecked = true;

	public List<SongMetadataStringValue> metadataStringValues = new List<SongMetadataStringValue>();

	public List<SongMetadataBoolValue> metadataBoolValues = new List<SongMetadataBoolValue>();

	public List<SongMetadataIntValue> metadataIntValues = new List<SongMetadataIntValue>();

	public List<SongMetadataFloatValue> metadataFloatValues = new List<SongMetadataFloatValue>();

	public bool metadataExpanded = true;

	public MasterAudio.CustomSongStartTimeMode songStartTimeMode;

	public float customStartTime;

	public float customStartTimeMax;

	public int lastKnownTimePoint;

	public bool wasLastKnownTimePointSet;

	public int songIndex;

	public float sectionStartTime;

	public float sectionEndTime;

	public bool songStartedEventExpanded;

	public string songStartedCustomEvent = string.Empty;

	public bool songChangedEventExpanded;

	public string songChangedCustomEvent = string.Empty;

	public bool HasMetadataProperties => MetadataPropertyCount > 0;

	public int MetadataPropertyCount => metadataStringValues.Count + metadataBoolValues.Count + metadataIntValues.Count + metadataFloatValues.Count;

	public float SongStartTime => songStartTimeMode switch
	{
		MasterAudio.CustomSongStartTimeMode.SpecificTime => customStartTime, 
		MasterAudio.CustomSongStartTimeMode.RandomTime => UnityEngine.Random.Range(customStartTime, customStartTimeMax), 
		MasterAudio.CustomSongStartTimeMode.Section => sectionStartTime, 
		_ => 0f, 
	};

	public MusicSetting()
	{
		songChangedEventExpanded = false;
	}

	public static MusicSetting Clone(MusicSetting mus, MasterAudio.Playlist aList)
	{
		MusicSetting musicSetting = new MusicSetting
		{
			alias = mus.alias,
			audLocation = mus.audLocation,
			clip = mus.clip,
			songName = mus.songName,
			resourceFileName = mus.resourceFileName,
			volume = mus.volume,
			pitch = mus.pitch,
			isExpanded = mus.isExpanded,
			isLoop = mus.isLoop,
			isChecked = mus.isChecked,
			customStartTime = mus.customStartTime,
			songStartedEventExpanded = mus.songStartedEventExpanded,
			songStartedCustomEvent = mus.songStartedCustomEvent,
			songChangedEventExpanded = mus.songChangedEventExpanded,
			songChangedCustomEvent = mus.songChangedCustomEvent,
			metadataExpanded = mus.metadataExpanded
		};
		for (int i = 0; i < mus.metadataStringValues.Count; i++)
		{
			SongMetadataStringValue valToClone = mus.metadataStringValues[i];
			SongMetadataStringValue songMetadataStringValue = new SongMetadataStringValue(aList.songMetadataProps.Find((SongMetadataProperty p) => p.PropertyName == valToClone.PropertyName));
			songMetadataStringValue.Value = valToClone.Value;
			musicSetting.metadataStringValues.Add(songMetadataStringValue);
		}
		for (int num = 0; num < mus.metadataFloatValues.Count; num++)
		{
			SongMetadataFloatValue valToClone2 = mus.metadataFloatValues[num];
			SongMetadataFloatValue songMetadataFloatValue = new SongMetadataFloatValue(aList.songMetadataProps.Find((SongMetadataProperty p) => p.PropertyName == valToClone2.PropertyName));
			songMetadataFloatValue.Value = valToClone2.Value;
			musicSetting.metadataFloatValues.Add(songMetadataFloatValue);
		}
		for (int num2 = 0; num2 < mus.metadataBoolValues.Count; num2++)
		{
			SongMetadataBoolValue valToClone3 = mus.metadataBoolValues[num2];
			SongMetadataBoolValue songMetadataBoolValue = new SongMetadataBoolValue(aList.songMetadataProps.Find((SongMetadataProperty p) => p.PropertyName == valToClone3.PropertyName));
			songMetadataBoolValue.Value = valToClone3.Value;
			musicSetting.metadataBoolValues.Add(songMetadataBoolValue);
		}
		for (int num3 = 0; num3 < mus.metadataIntValues.Count; num3++)
		{
			SongMetadataIntValue valToClone4 = mus.metadataIntValues[num3];
			SongMetadataIntValue songMetadataIntValue = new SongMetadataIntValue(aList.songMetadataProps.Find((SongMetadataProperty p) => p.PropertyName == valToClone4.PropertyName));
			songMetadataIntValue.Value = valToClone4.Value;
			musicSetting.metadataIntValues.Add(songMetadataIntValue);
		}
		return musicSetting;
	}
}
