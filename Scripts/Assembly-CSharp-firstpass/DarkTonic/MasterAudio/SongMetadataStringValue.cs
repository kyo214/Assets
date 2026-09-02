using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class SongMetadataStringValue
{
	public string PropertyName;

	public string Value;

	public SongMetadataStringValue(SongMetadataProperty prop)
	{
		if (prop != null)
		{
			PropertyName = prop.PropertyName;
		}
	}
}
