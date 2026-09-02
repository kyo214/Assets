using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

internal class RayNodeTraversalTest : IBoundsTraversalTest
{
	private Vector3 _origin;

	private Vector3 _direction;

	private float _length;

	internal RayNodeTraversalTest(Vector3 origin, Vector3 direction, float length)
	{
		SetTestSettings(origin, direction, length);
	}

	internal void SetTestSettings(Vector3 origin, Vector3 direction, float length)
	{
		_origin = origin;
		_direction = direction;
		_length = length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Check(ref BVHNode.CachedBounds bounds)
	{
		Vector3 min = bounds.Min;
		Vector3 max = bounds.Max;
		float num = _length * _length;
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		Vector3 vector = default;
		Vector3 vector2 = default;
		Vector3 vector3 = default;
		if (_origin.x < min.x)
		{
			vector2.x = min.x;
			flag = false;
		}
		else if (_origin.x > max.x)
		{
			vector2.x = max.x;
			flag = false;
		}
		else
		{
			flag2 = true;
		}
		if (_origin.y < min.y)
		{
			vector2.y = min.y;
			flag = false;
		}
		else if (_origin.y > max.y)
		{
			vector2.y = max.y;
			flag = false;
		}
		else
		{
			flag3 = true;
		}
		if (_origin.z < min.z)
		{
			vector2.z = min.z;
			flag = false;
		}
		else if (_origin.z > max.z)
		{
			vector2.z = max.z;
			flag = false;
		}
		else
		{
			flag4 = true;
		}
		if (flag)
		{
			vector3 = _origin;
			return true;
		}
		if (_direction.x != 0f && !flag2)
		{
			vector.x = (vector2.x - _origin.x) / _direction.x;
		}
		else
		{
			vector.x = -1f;
		}
		if (_direction.y != 0f && !flag3)
		{
			vector.y = (vector2.y - _origin.y) / _direction.y;
		}
		else
		{
			vector.y = -1f;
		}
		if (_direction.z != 0f && !flag4)
		{
			vector.z = (vector2.z - _origin.z) / _direction.z;
		}
		else
		{
			vector.z = -1f;
		}
		int num2 = 0;
		float num3 = vector.x;
		if (num3 < vector.y)
		{
			num2 = 1;
			num3 = vector.y;
		}
		if (num3 < vector.z)
		{
			num2 = 2;
			num3 = vector.z;
		}
		if (num3 < 0f)
		{
			return false;
		}
		if (num2 != 0)
		{
			vector3.x = _origin.x + num3 * _direction.x;
			if (vector3.x < min.x || vector3.x > max.x)
			{
				return false;
			}
		}
		else
		{
			vector3.x = vector2.x;
		}
		if (num2 != 1)
		{
			vector3.y = _origin.y + num3 * _direction.y;
			if (vector3.y < min.y || vector3.y > max.y)
			{
				return false;
			}
		}
		else
		{
			vector3.y = vector2.y;
		}
		if (num2 != 2)
		{
			vector3.z = _origin.z + num3 * _direction.z;
			if (vector3.z < min.z || vector3.z > max.z)
			{
				return false;
			}
		}
		else
		{
			vector3.z = vector2.z;
		}
		Vector3 origin = _origin;
		origin.x -= vector3.x;
		origin.y -= vector3.y;
		origin.z -= vector3.z;
		if (origin.sqrMagnitude <= num)
		{
			return true;
		}
		return false;
	}
}
