using System;

[Serializable]
public class AddressableDelayedRelease
{
	public string AddressableId { get; private set; }

	public float RealtimeToRelease { get; set; }

	public AddressableDelayedRelease(string addressableId, float realtimeToRelease)
	{
		AddressableId = addressableId;
		RealtimeToRelease = realtimeToRelease;
	}
}
