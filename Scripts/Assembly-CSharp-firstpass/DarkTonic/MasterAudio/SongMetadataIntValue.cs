using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class SongMetadataIntValue
{
	public string PropertyName;

	public int Value;

	public SongMetadataIntValue(SongMetadataProperty prop)
	{
		if (prop != null)
		{
			PropertyName = prop.PropertyName;
		}
	}
}
