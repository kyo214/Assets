using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCNetworkUtility
{
	public const int WORD_COUNT_BOOL = 1;

	public const int WORD_COUNT_INT = 1;

	public const int WORD_COUNT_FLOAT = 1;

	public const int WORD_COUNT_VECTOR3 = 3;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool ReadBool(int* ptr)
	{
		if (*ptr == 0)
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteBool(int* ptr, bool value)
	{
		*ptr = (value ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int ReadInt(int* ptr)
	{
		return *ptr;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteInt(int* ptr, int value)
	{
		*ptr = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadFloat(int* ptr, float accuracy = 0f)
	{
		if (accuracy <= 0f)
		{
			return *(float*)ptr;
		}
		return (float)(*ptr) * accuracy;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteFloat(int* ptr, float value, float inverseAccuracy = 0f)
	{
		if (inverseAccuracy <= 0f)
		{
			*(float*)ptr = value;
		}
		else
		{
			*ptr = ((value < 0f) ? ((int)(value * inverseAccuracy - 0.5f)) : ((int)(value * inverseAccuracy + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadVector3(int* ptr, float accuracy = 0f)
	{
		if (accuracy <= 0f)
		{
			Vector3 result = default;
			result.x = *(float*)ptr;
			result.y = *(float*)(ptr + 1);
			result.z = *(float*)(ptr + 2);
			return result;
		}
		Vector3 result2 = default;
		result2.x = (float)(*ptr) * accuracy;
		result2.y = (float)ptr[1] * accuracy;
		result2.z = (float)ptr[2] * accuracy;
		return result2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteVector3(int* ptr, Vector3 value, float inverseAccuracy = 0f)
	{
		if (inverseAccuracy <= 0f)
		{
			*(float*)ptr = value.x;
			*(float*)(ptr + 1) = value.y;
			*(float*)(ptr + 2) = value.z;
		}
		else
		{
			*ptr = ((value.x < 0f) ? ((int)(value.x * inverseAccuracy - 0.5f)) : ((int)(value.x * inverseAccuracy + 0.5f)));
			ptr[1] = ((value.y < 0f) ? ((int)(value.y * inverseAccuracy - 0.5f)) : ((int)(value.y * inverseAccuracy + 0.5f)));
			ptr[2] = ((value.z < 0f) ? ((int)(value.z * inverseAccuracy - 0.5f)) : ((int)(value.z * inverseAccuracy + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static KCCNetworkID ReadNetworkID(int* ptr)
	{
		return new KCCNetworkID
		{
			A = *ptr,
			B = ptr[1],
			C = ptr[2],
			D = ptr[3]
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteNetworkID(int* ptr, KCCNetworkID networkID)
	{
		*ptr = networkID.A;
		ptr[1] = networkID.B;
		ptr[2] = networkID.C;
		ptr[3] = networkID.D;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float InterpolateRange(float from, float to, float min, float max, float alpha)
	{
		return KCCMathUtility.InterpolateRange(from, to, min, max, alpha);
	}
}
