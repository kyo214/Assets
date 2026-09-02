using UnityEngine;

namespace Dissonance.Demo;

public class ToggleInvertMute : MonoBehaviour
{
	public VoiceBroadcastTrigger Trigger;

	public bool IsUnmuted
	{
		set
		{
			if ((bool)Trigger)
			{
				Trigger.IsMuted = !value;
			}
		}
	}
}
