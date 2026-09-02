using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class SongMetadataFloatValue
{
	public string PropertyName;

	public float Value;

	public SongMetadataFloatValue(SongMetadataProperty prop)
	{
		if (prop != null)
		{
			PropertyName = prop.PropertyName;
		}
	}
}
