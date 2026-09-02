using UnityEngine;

namespace Fusion.KCC;

public static class KCCUtility
{
	public static void ClampLookRotationAngles(ref float pitch, ref float yaw)
	{
		pitch = Mathf.Clamp(pitch, -90f, 90f);
		while (yaw > 180f)
		{
			yaw -= 360f;
		}
		while (yaw < -180f)
		{
			yaw += 360f;
		}
	}

	public static void GetLookRotationAngles(Quaternion lookRotation, out float pitch, out float yaw)
	{
		Vector3 eulerAngles = lookRotation.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		pitch = Mathf.Clamp(eulerAngles.x, -90f, 90f);
		yaw = Mathf.Clamp(eulerAngles.y, -180f, 180f);
	}

	public static Vector3 GetEulerLookRotation(Quaternion lookRotation)
	{
		Vector3 eulerAngles = lookRotation.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		eulerAngles.x = Mathf.Clamp(eulerAngles.x, -90f, 90f);
		eulerAngles.y = Mathf.Clamp(eulerAngles.y, -180f, 180f);
		return eulerAngles;
	}

	public static Vector2 GetClampedLookRotation(Vector2 lookRotation, float minPitch, float maxPitch)
	{
		lookRotation.x = Mathf.Clamp(lookRotation.x, minPitch, maxPitch);
		return lookRotation;
	}

	public static Vector2 GetClampedLookRotation(Vector2 lookRotation, Vector2 lookRotationDelta, float minPitch, float maxPitch)
	{
		return lookRotation + GetClampedLookRotationDelta(lookRotation, lookRotationDelta, minPitch, maxPitch);
	}

	public static Vector2 GetClampedLookRotationDelta(Vector2 lookRotation, Vector2 lookRotationDelta, float minPitch, float maxPitch)
	{
		lookRotationDelta.x = Mathf.Clamp(lookRotation.x + lookRotationDelta.x, minPitch, maxPitch) - lookRotation.x;
		return lookRotationDelta;
	}

	public static Vector3 EasyLerpDirection(Vector3 fromDirection, Vector3 toDirection, float time, float responsivity)
	{
		Vector3 vector = Vector3.Lerp(fromDirection, toDirection, time);
		float num = Mathf.Clamp01(responsivity) * 2f;
		if (num <= 1f)
		{
			float t = KCCMathUtility.EasyOut4(num);
			return Vector3.Lerp(fromDirection, vector, t);
		}
		float t2 = KCCMathUtility.EasyIn4(num - 1f);
		return Vector3.Lerp(vector, toDirection, t2);
	}

	public static void GetPositionAndRotation(Transform transform, out Vector3 position, out Quaternion rotation)
	{
		transform.GetPositionAndRotation(out position, out rotation);
	}
}
