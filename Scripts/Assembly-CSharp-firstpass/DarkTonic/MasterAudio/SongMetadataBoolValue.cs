using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class SongMetadataBoolValue
{
	public string PropertyName;

	public bool Value;

	public SongMetadataBoolValue(SongMetadataProperty prop)
	{
		if (prop != null)
		{
			PropertyName = prop.PropertyName;
		}
	}
}
