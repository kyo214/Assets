using System.Collections;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMMovement
{
	public static IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float duration, AnimationCurve curve = null)
	{
		float journey = 0f;
		while (journey < duration)
		{
			float time = Mathf.Clamp01(journey / duration);
			Vector3 position = Vector3.Lerp(pointA, pointB, curve.Evaluate(time));
			movingObject.transform.position = position;
			journey += Time.deltaTime;
			yield return null;
		}
	}

	public static IEnumerator AnimateScale(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float multiplier = 1f)
	{
		if (!(targetTransform == null) && curveX != null && curveY != null && curveZ != null && duration != 0f)
		{
			float journey = 0f;
			while (journey < duration)
			{
				float time = Mathf.Clamp01(journey / duration);
				vector.x = curveX.Evaluate(time);
				vector.y = curveY.Evaluate(time);
				vector.z = curveZ.Evaluate(time);
				targetTransform.localScale = multiplier * vector;
				journey += Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
	}

	public static IEnumerator AnimateRotation(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float multiplier)
	{
		if (!(targetTransform == null) && curveX != null && curveY != null && curveZ != null && duration != 0f)
		{
			float journey = 0f;
			while (journey < duration)
			{
				float time = Mathf.Clamp01(journey / duration);
				vector.x = curveX.Evaluate(time) * multiplier;
				vector.y = curveY.Evaluate(time) * multiplier;
				vector.z = curveZ.Evaluate(time) * multiplier;
				targetTransform.localEulerAngles = vector;
				journey += Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
	}
}
