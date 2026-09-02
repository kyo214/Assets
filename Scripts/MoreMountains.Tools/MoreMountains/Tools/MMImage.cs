using System.Collections;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMImage : MonoBehaviour
{
	public static IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
	{
		if (!(renderer == null) && renderer.material.HasProperty("_Color") && !(initialColor == flickerColor))
		{
			float flickerStop = Time.time + flickerDuration;
			while (Time.time < flickerStop)
			{
				renderer.material.color = flickerColor;
				yield return MMCoroutine.WaitFor(flickerSpeed);
				renderer.material.color = initialColor;
				yield return MMCoroutine.WaitFor(flickerSpeed);
			}
			renderer.material.color = initialColor;
		}
	}
}
