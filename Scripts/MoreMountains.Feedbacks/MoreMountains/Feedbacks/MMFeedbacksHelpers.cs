using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
public class MMFeedbacksHelpers : MonoBehaviour
{
	public static float Remap(float x, float A, float B, float C, float D)
	{
		return C + (x - A) / (B - A) * (D - C);
	}
}
