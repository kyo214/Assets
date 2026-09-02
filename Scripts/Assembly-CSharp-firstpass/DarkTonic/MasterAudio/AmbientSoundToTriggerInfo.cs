using System;

namespace DarkTonic.MasterAudio;

[Serializable]
public class AmbientSoundToTriggerInfo
{
	public int frameToTrigger;

	public AmbientSound ambient;
}
