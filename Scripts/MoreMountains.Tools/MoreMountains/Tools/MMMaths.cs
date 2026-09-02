using System;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMMaths
{
	private static float SpringVelocity(float currentValue, float targetValue, float velocity, float damping, float frequency, float speed, float deltaTime)
	{
		frequency = frequency * 2f * MathF.PI;
		return velocity + deltaTime * frequency * frequency * (targetValue - currentValue) + -2f * deltaTime * frequency * damping * velocity;
	}

	public static void Spring(ref float currentValue, float targetValue, ref float velocity, float damping, float frequency, float speed, float deltaTime)
	{
		float value = velocity;
		velocity = SpringVelocity(currentValue, targetValue, velocity, damping, frequency, speed, deltaTime);
		velocity = Lerp(value, velocity, speed, Time.deltaTime);
		currentValue += deltaTime * velocity;
	}

	public static void Spring(ref Vector2 currentValue, Vector2 targetValue, ref Vector2 velocity, float damping, float frequency, float speed, float deltaTime)
	{
		Vector2 vector = velocity;
		velocity.x = SpringVelocity(currentValue.x, targetValue.x, velocity.x, damping, frequency, speed, deltaTime);
		velocity.y = SpringVelocity(currentValue.y, targetValue.y, velocity.y, damping, frequency, speed, deltaTime);
		velocity.x = Lerp(vector.x, velocity.x, speed, Time.deltaTime);
		velocity.y = Lerp(vector.y, velocity.y, speed, Time.deltaTime);
		currentValue += deltaTime * velocity;
	}

	public static void Spring(ref Vector3 currentValue, Vector3 targetValue, ref Vector3 velocity, float damping, float frequency, float speed, float deltaTime)
	{
		Vector3 vector = velocity;
		velocity.x = SpringVelocity(currentValue.x, targetValue.x, velocity.x, damping, frequency, speed, deltaTime);
		velocity.y = SpringVelocity(currentValue.y, targetValue.y, velocity.y, damping, frequency, speed, deltaTime);
		velocity.z = SpringVelocity(currentValue.z, targetValue.z, velocity.z, damping, frequency, speed, deltaTime);
		velocity.x = Lerp(vector.x, velocity.x, speed, Time.deltaTime);
		velocity.y = Lerp(vector.y, velocity.y, speed, Time.deltaTime);
		velocity.z = Lerp(vector.z, velocity.z, speed, Time.deltaTime);
		currentValue += deltaTime * velocity;
	}

	public static void Spring(ref Vector4 currentValue, Vector4 targetValue, ref Vector4 velocity, float damping, float frequency, float speed, float deltaTime)
	{
		Vector4 vector = velocity;
		velocity.x = SpringVelocity(currentValue.x, targetValue.x, velocity.x, damping, frequency, speed, deltaTime);
		velocity.y = SpringVelocity(currentValue.y, targetValue.y, velocity.y, damping, frequency, speed, deltaTime);
		velocity.z = SpringVelocity(currentValue.z, targetValue.z, velocity.z, damping, frequency, speed, deltaTime);
		velocity.w = SpringVelocity(currentValue.w, targetValue.w, velocity.w, damping, frequency, speed, deltaTime);
		velocity.x = Lerp(vector.x, velocity.x, speed, Time.deltaTime);
		velocity.y = Lerp(vector.y, velocity.y, speed, Time.deltaTime);
		velocity.z = Lerp(vector.z, velocity.z, speed, Time.deltaTime);
		velocity.w = Lerp(vector.w, velocity.w, speed, Time.deltaTime);
		currentValue += deltaTime * velocity;
	}

	private static float LerpRate(float rate, float deltaTime)
	{
		rate = Mathf.Clamp01(rate);
		float num = (0f - Mathf.Log(1f - rate, 2f)) * 60f;
		return Mathf.Pow(2f, (0f - num) * deltaTime);
	}

	public static float Lerp(float value, float target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Mathf.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Vector2 Lerp(Vector2 value, Vector2 target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Vector2.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Vector3 Lerp(Vector3 value, Vector3 target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Vector3.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Vector4 Lerp(Vector4 value, Vector4 target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Vector4.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Quaternion Lerp(Quaternion value, Quaternion target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Quaternion.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Color Lerp(Color value, Color target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Color.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static Color32 Lerp(Color32 value, Color32 target, float rate, float deltaTime)
	{
		if (deltaTime == 0f)
		{
			return value;
		}
		return Color32.Lerp(target, value, LerpRate(rate, deltaTime));
	}

	public static float Clamp(float value, float min, float max, bool clampMin, bool clampMax)
	{
		float num = value;
		if (clampMin && num < min)
		{
			num = min;
		}
		if (clampMax && num > max)
		{
			num = max;
		}
		return num;
	}

	public static float RoundToNearestHalf(float a)
	{
		return a -= a % 0.5f;
	}

	public static Quaternion LookAt2D(Vector2 direction)
	{
		return Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 57.29578f, Vector3.forward);
	}

	public static Vector2 Vector3ToVector2(Vector3 target)
	{
		return new Vector2(target.x, target.y);
	}

	public static Vector3 Vector2ToVector3(Vector2 target)
	{
		return new Vector3(target.x, target.y, 0f);
	}

	public static Vector3 Vector2ToVector3(Vector2 target, float newZValue)
	{
		return new Vector3(target.x, target.y, newZValue);
	}

	public static Vector3 RoundVector3(Vector3 vector)
	{
		return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
	}

	public static Vector2 RandomVector2(Vector2 minimum, Vector2 maximum)
	{
		return new Vector2(UnityEngine.Random.Range(minimum.x, maximum.x), UnityEngine.Random.Range(minimum.y, maximum.y));
	}

	public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
	{
		return new Vector3(UnityEngine.Random.Range(minimum.x, maximum.x), UnityEngine.Random.Range(minimum.y, maximum.y), UnityEngine.Random.Range(minimum.z, maximum.z));
	}

	public static Vector2 RandomPointOnCircle(float circleRadius)
	{
		return UnityEngine.Random.insideUnitCircle.normalized * circleRadius;
	}

	public static Vector3 RandomPointOnSphere(float sphereRadius)
	{
		return UnityEngine.Random.onUnitSphere * sphereRadius;
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
	{
		angle *= MathF.PI / 180f;
		float x = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y - pivot.y) + pivot.x;
		float y = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
		return new Vector3(x, y, 0f);
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angle) * vector;
		point = vector + pivot;
		return point;
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion quaternion)
	{
		Vector3 vector = point - pivot;
		vector = quaternion * vector;
		point = vector + pivot;
		return point;
	}

	public static Vector2 RotateVector2(Vector2 vector, float angle)
	{
		if (angle == 0f)
		{
			return vector;
		}
		float num = Mathf.Sin(angle * (MathF.PI / 180f));
		float num2 = Mathf.Cos(angle * (MathF.PI / 180f));
		float x = vector.x;
		float y = vector.y;
		vector.x = num2 * x - num * y;
		vector.y = num * x + num2 * y;
		return vector;
	}

	public static float AngleBetween(Vector2 vectorA, Vector2 vectorB)
	{
		float num = Vector2.Angle(vectorA, vectorB);
		if (Vector3.Cross(vectorA, vectorB).z > 0f)
		{
			num = 360f - num;
		}
		return num;
	}

	public static float AngleDirection(Vector3 vectorA, Vector3 vectorB, Vector3 up)
	{
		return Vector3.Dot(Vector3.Cross(vectorA, vectorB), up);
	}

	public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
	}

	public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector;
		if (magnitude > 1E-06f)
		{
			vector2 /= magnitude;
		}
		float num = Mathf.Clamp(Vector3.Dot(vector2, rhs), 0f, magnitude);
		return lineStart + vector2 * num;
	}

	public static int Sum(params int[] thingsToAdd)
	{
		int num = 0;
		for (int i = 0; i < thingsToAdd.Length; i++)
		{
			num += thingsToAdd[i];
		}
		return num;
	}

	public static int RollADice(int numberOfSides)
	{
		return UnityEngine.Random.Range(1, numberOfSides + 1);
	}

	public static bool Chance(int percent)
	{
		return UnityEngine.Random.Range(0, 100) <= percent;
	}

	public static float Approach(float from, float to, float amount)
	{
		if (from < to)
		{
			from += amount;
			if (from > to)
			{
				return to;
			}
		}
		else
		{
			from -= amount;
			if (from < to)
			{
				return to;
			}
		}
		return from;
	}

	public static float Remap(float x, float A, float B, float C, float D)
	{
		return C + (x - A) / (B - A) * (D - C);
	}

	public static float ClampAngle(float angle, float minimumAngle, float maximumAngle)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, minimumAngle, maximumAngle);
	}

	public static float RoundToDecimal(float value, int numberOfDecimals)
	{
		if (numberOfDecimals <= 0)
		{
			return Mathf.Round(value);
		}
		return Mathf.Round(value * 10f * (float)numberOfDecimals) / (10f * (float)numberOfDecimals);
	}

	public static float RoundToClosest(float value, float[] possibleValues, bool pickSmallestDistance = false)
	{
		if (possibleValues.Length == 0)
		{
			return 0f;
		}
		float num = possibleValues[0];
		foreach (float num2 in possibleValues)
		{
			float num3 = Mathf.Abs(num - value);
			float num4 = Mathf.Abs(num2 - value);
			if (num3 > num4)
			{
				num = num2;
			}
			else if (num3 == num4 && ((pickSmallestDistance && num > num2) || (!pickSmallestDistance && num < num2)))
			{
				num = ((value < 0f) ? num : num2);
			}
		}
		return num;
	}

	public static Vector3 DirectionFromAngle(float angle, float additionalAngle)
	{
		angle += additionalAngle;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(angle * (MathF.PI / 180f));
		zero.y = 0f;
		zero.z = Mathf.Cos(angle * (MathF.PI / 180f));
		return zero;
	}

	public static Vector3 DirectionFromAngle2D(float angle, float additionalAngle)
	{
		angle += additionalAngle;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Cos(angle * (MathF.PI / 180f));
		zero.y = Mathf.Sin(angle * (MathF.PI / 180f));
		zero.z = 0f;
		return zero;
	}
}
