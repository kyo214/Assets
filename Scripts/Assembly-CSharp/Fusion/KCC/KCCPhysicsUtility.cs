using UnityEngine;

namespace Fusion.KCC;

public static class KCCPhysicsUtility
{
	public static bool ProjectOnGround(Vector3 groundNormal, Vector3 vector, out Vector3 projectedVector)
	{
		float num = Vector3.Dot(Vector3.up, groundNormal);
		float num2 = 0f - Vector3.Dot(vector, groundNormal);
		if (!num.IsAlmostZero(0.001f))
		{
			projectedVector = new Vector3(vector.x, vector.y + num2 / num, vector.z);
			return true;
		}
		projectedVector = default;
		return false;
	}

	public static void ProjectVerticalPenetration(ref Vector3 direction, ref float distance)
	{
		Vector3 vector = direction * distance;
		float num = distance * (1f - Vector3.Dot(direction, Vector3.up));
		float num2 = vector.y * vector.y / num;
		vector = vector.OnlyXZ() * (1f + num2 / num);
		float num3 = Vector3.Magnitude(vector);
		if (num3 >= 1E-06f)
		{
			direction = vector / num3;
			distance = num3;
		}
	}

	public static void ProjectHorizontalPenetration(ref Vector3 direction, ref float distance)
	{
		Vector3 vector = direction * distance;
		direction = Vector3.up;
		distance = 0f;
		if (!(vector.y > -1E-06f) || !(vector.y < 1E-06f))
		{
			distance = vector.y + (vector.x * vector.x + vector.z * vector.z) / vector.y;
			if (distance < 0f)
			{
				direction = -direction;
				distance = 0f - distance;
			}
		}
	}

	public static bool CheckGround(Collider collider, Vector3 position, Collider groundCollider, Vector3 groundPosition, Quaternion groundRotation, float radius, float height, float extent, float minGroundDot, out Vector3 groundNormal, out float groundDistance, out bool isWithinExtent)
	{
		isWithinExtent = false;
		if (groundCollider is MeshCollider || groundCollider is TerrainCollider)
		{
			if (Physics.ComputePenetration(collider, position - new Vector3(0f, extent, 0f), Quaternion.identity, groundCollider, groundPosition, groundRotation, out var direction, out var distance))
			{
				isWithinExtent = true;
				float num = Vector3.Dot(direction, Vector3.up);
				if (num >= minGroundDot)
				{
					Vector3 direction2 = direction;
					float distance2 = distance;
					ProjectHorizontalPenetration(ref direction2, ref distance2);
					float num2 = Mathf.Max(0f, extent - distance2);
					groundNormal = direction;
					groundDistance = num2 * num;
					return true;
				}
			}
		}
		else
		{
			float num3 = radius + extent;
			float num4 = num3 * num3;
			Vector3 vector = position + new Vector3(0f, radius, 0f);
			Vector3 vector2 = Physics.ClosestPoint(vector, groundCollider, groundPosition, groundRotation) - vector;
			if (vector2.OnlyXZ().sqrMagnitude <= num4)
			{
				if (vector2.y < 0f)
				{
					float num5 = Vector3.Magnitude(vector2);
					if (num5 <= num3)
					{
						isWithinExtent = true;
						Vector3 vector3 = -(vector2 / num5);
						if (Vector3.Dot(vector3, Vector3.up) >= minGroundDot)
						{
							groundNormal = vector3;
							groundDistance = Mathf.Max(0f, num5 - radius);
							return true;
						}
					}
				}
				else if (vector2.y < height - radius * 2f)
				{
					isWithinExtent = true;
				}
			}
		}
		groundNormal = Vector3.up;
		groundDistance = 0f;
		return false;
	}

	public static Vector3 GetAcceleration(Vector3 velocity, Vector3 direction, Vector3 axis, float maxSpeed, bool clampSpeed, float inputAcceleration, float constantAcceleration, float relativeAcceleration, float proportionalAcceleration, float deltaTime, float fixedDeltaTime)
	{
		if (inputAcceleration <= 0f)
		{
			return Vector3.zero;
		}
		if (constantAcceleration <= 0f && relativeAcceleration <= 0f && proportionalAcceleration <= 0f)
		{
			return Vector3.zero;
		}
		if (direction.IsZero())
		{
			return Vector3.zero;
		}
		float magnitude = new Vector3(velocity.x * axis.x, velocity.y * axis.y, velocity.z * axis.z).magnitude;
		Vector3 normalized = new Vector3(direction.x * axis.x, direction.y * axis.y, direction.z * axis.z).normalized;
		float num = Mathf.Max(0f, maxSpeed - magnitude);
		if (constantAcceleration < 0f)
		{
			constantAcceleration = 0f;
		}
		if (relativeAcceleration < 0f)
		{
			relativeAcceleration = 0f;
		}
		if (proportionalAcceleration < 0f)
		{
			proportionalAcceleration = 0f;
		}
		constantAcceleration *= inputAcceleration;
		relativeAcceleration *= inputAcceleration;
		proportionalAcceleration *= inputAcceleration;
		float num2 = (constantAcceleration + maxSpeed * relativeAcceleration + num * proportionalAcceleration) * GetCompensatedDeltaTime(deltaTime, fixedDeltaTime);
		if (num2 <= 0f)
		{
			return Vector3.zero;
		}
		if (clampSpeed && num2 > num)
		{
			num2 = num;
		}
		return normalized.normalized * num2;
	}

	public static Vector3 GetAcceleration(Vector3 velocity, Vector3 direction, Vector3 axis, Vector3 normal, float targetSpeed, bool clampSpeed, float inputAcceleration, float constantAcceleration, float relativeAcceleration, float proportionalAcceleration, float deltaTime, float fixedDeltaTime)
	{
		float num = 1f - Mathf.Clamp01(Vector3.Dot(direction.normalized, normal));
		constantAcceleration *= num;
		relativeAcceleration *= num;
		proportionalAcceleration *= num;
		return GetAcceleration(velocity, direction, axis, targetSpeed, clampSpeed, inputAcceleration, constantAcceleration, relativeAcceleration, proportionalAcceleration, deltaTime, fixedDeltaTime);
	}

	public static Vector3 GetFriction(Vector3 velocity, Vector3 direction, Vector3 axis, float maxSpeed, bool clampSpeed, float constantFriction, float relativeFriction, float proportionalFriction, float deltaTime, float fixedDeltaTime)
	{
		if (constantFriction <= 0f && relativeFriction <= 0f && proportionalFriction <= 0f)
		{
			return Vector3.zero;
		}
		if (direction.IsZero())
		{
			return Vector3.zero;
		}
		float magnitude = new Vector3(velocity.x * axis.x, velocity.y * axis.y, velocity.z * axis.z).magnitude;
		Vector3 normalized = new Vector3(direction.x * axis.x, direction.y * axis.y, direction.z * axis.z).normalized;
		if (constantFriction < 0f)
		{
			constantFriction = 0f;
		}
		if (relativeFriction < 0f)
		{
			relativeFriction = 0f;
		}
		if (proportionalFriction < 0f)
		{
			proportionalFriction = 0f;
		}
		float num = (constantFriction + maxSpeed * relativeFriction + magnitude * proportionalFriction) * GetCompensatedDeltaTime(deltaTime, fixedDeltaTime);
		if (num <= 0f)
		{
			return Vector3.zero;
		}
		if (clampSpeed && num > magnitude)
		{
			num = magnitude;
		}
		return -normalized * num;
	}

	public static Vector3 GetFriction(Vector3 velocity, Vector3 direction, Vector3 axis, Vector3 normal, float maxSpeed, bool clampSpeed, float constantFriction, float relativeFriction, float proportionalFriction, float deltaTime, float fixedDeltaTime)
	{
		float num = 1f - Mathf.Clamp01(Vector3.Dot(direction.normalized, normal));
		constantFriction *= num;
		relativeFriction *= num;
		proportionalFriction *= num;
		return GetFriction(velocity, direction, axis, maxSpeed, clampSpeed, constantFriction, relativeFriction, proportionalFriction, deltaTime, fixedDeltaTime);
	}

	public static Vector3 CombineAccelerationAndFriction(Vector3 velocity, Vector3 acceleration, Vector3 friction)
	{
		velocity.x = CombineAxis(velocity.x, acceleration.x, friction.x);
		velocity.y = CombineAxis(velocity.y, acceleration.y, friction.y);
		velocity.z = CombineAxis(velocity.z, acceleration.z, friction.z);
		return velocity;
		static float CombineAxis(float axisVelocity, float axisAcceleration, float axisFriction)
		{
			float num = axisAcceleration + axisFriction;
			if (Mathf.Abs(axisAcceleration) >= Mathf.Abs(axisFriction))
			{
				axisVelocity += num;
			}
			else if ((double)axisVelocity > 0.0)
			{
				axisVelocity = Mathf.Max(0f, axisVelocity + num);
			}
			else if ((double)axisVelocity < 0.0)
			{
				axisVelocity = Mathf.Min(axisVelocity + num, 0f);
			}
			return axisVelocity;
		}
	}

	public static Vector3 AccumulatePenetrationCorrection(Vector3 accumulatedCorrection, Vector3 contactCorrectionDirection, float contactCorrectionMagnitude)
	{
		float num = Vector3.Magnitude(accumulatedCorrection);
		Vector3 vector = ((num > 1E-10f) ? (accumulatedCorrection / num) : Vector3.zero);
		float num2 = 0f;
		Vector3 vector2 = Vector3.Cross(Vector3.Cross(vector, contactCorrectionDirection), vector).normalized;
		float num3 = Vector3.Dot(contactCorrectionDirection, vector);
		float num4 = 1f - num3 * num3;
		if (num4 > 0.001f)
		{
			num2 = (contactCorrectionMagnitude - num * num3) / Mathf.Sqrt(num4);
		}
		else if (contactCorrectionMagnitude > num)
		{
			num2 = contactCorrectionMagnitude - num;
			vector2 = vector;
		}
		accumulatedCorrection += vector2 * num2;
		return accumulatedCorrection;
	}

	private static float GetCompensatedDeltaTime(float deltaTime, float fixedDeltaTime)
	{
		return deltaTime * Mathf.Clamp(fixedDeltaTime / deltaTime, 1f, 8f);
	}
}
