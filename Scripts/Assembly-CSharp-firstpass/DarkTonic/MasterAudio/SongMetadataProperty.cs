using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class SongMetadataProperty
{
	public enum MetadataPropertyType
	{
		Boolean = 0,
		String = 1,
		Integer = 2,
		Float = 3
	}

	public MetadataPropertyType PropertyType;

	public string PropertyName;

	public string ProspectiveName;

	public bool IsEditing;

	public bool PropertyExpanded = true;

	public bool AllSongsMustContain = true;

	public bool CanSongHaveMultiple;

	public SongMetadataProperty(string propertyName, MetadataPropertyType propertyType, bool allSongsMustContain, bool canSongHaveMultiple)
	{
		PropertyName = propertyName;
		ProspectiveName = propertyName;
		PropertyType = propertyType;
		AllSongsMustContain = allSongsMustContain;
		CanSongHaveMultiple = canSongHaveMultiple;
	}
}
