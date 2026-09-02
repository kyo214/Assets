using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

public static class ReadWriteUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteFloat(int* data, WriteAccuracy writeAccuracy, float value)
	{
		float value2 = writeAccuracy.Value;
		if (value2 == 0f)
		{
			*(float*)data = value;
		}
		else
		{
			*data = ((value < 0f) ? ((int)(value * value2 - 0.5f)) : ((int)(value * value2 + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static float ReadFloat(int* data, ReadAccuracy readAccuracy)
	{
		float value = readAccuracy.Value;
		if (value == 0f)
		{
			return *(float*)data;
		}
		return (float)(*data) * value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteVector2(int* data, WriteAccuracy writeAccuracy, Vector2 value)
	{
		float value2 = writeAccuracy.Value;
		if (value2 == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * value2 - 0.5f)) : ((int)(value.x * value2 + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * value2 - 0.5f)) : ((int)(value.y * value2 + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector2 ReadVector2(int* data, ReadAccuracy readAccuracy)
	{
		float value = readAccuracy.Value;
		Vector2 result = default;
		if (value == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
		}
		else
		{
			result.x = (float)(*data) * value;
			result.y = (float)data[1] * value;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteVector3(int* data, WriteAccuracy writeAccuracy, Vector3 value)
	{
		float value2 = writeAccuracy.Value;
		if (value2 == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
			*(float*)(data + 2) = value.z;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * value2 - 0.5f)) : ((int)(value.x * value2 + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * value2 - 0.5f)) : ((int)(value.y * value2 + 0.5f)));
			data[2] = ((value.z < 0f) ? ((int)(value.z * value2 - 0.5f)) : ((int)(value.z * value2 + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadVector3(int* data, ReadAccuracy readAccuracy)
	{
		float value = readAccuracy.Value;
		Vector3 result = default;
		if (value == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
			result.z = *(float*)(data + 2);
		}
		else
		{
			result.x = (float)(*data) * value;
			result.y = (float)data[1] * value;
			result.z = (float)data[2] * value;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteQuaternion(int* data, WriteAccuracy writeAccuracy, Quaternion value)
	{
		float value2 = writeAccuracy.Value;
		if (value2 == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
			*(float*)(data + 2) = value.z;
			*(float*)(data + 3) = value.w;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * value2 - 0.5f)) : ((int)(value.x * value2 + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * value2 - 0.5f)) : ((int)(value.y * value2 + 0.5f)));
			data[2] = ((value.z < 0f) ? ((int)(value.z * value2 - 0.5f)) : ((int)(value.z * value2 + 0.5f)));
			data[3] = ((value.w < 0f) ? ((int)(value.w * value2 - 0.5f)) : ((int)(value.w * value2 + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Quaternion ReadQuaternion(int* data, ReadAccuracy readAccuracy)
	{
		float value = readAccuracy.Value;
		Quaternion result = default;
		if (value == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
			result.z = *(float*)(data + 2);
			result.w = *(float*)(data + 3);
		}
		else
		{
			result.x = (float)(*data) * value;
			result.y = (float)data[1] * value;
			result.z = (float)data[2] * value;
			result.w = (float)data[3] * value;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteEmptyNetworkBehaviourRef(int* data)
	{
		*data = 0;
		data[1] = 0;
	}

	public unsafe static void WriteNullkBehaviourRef(int* data)
	{
		*data = 0;
		data[1] = 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteNetworkBehaviourRef(int* data, NetworkRunner runner, NetworkBehaviour reference)
	{
		*(NetworkBehaviourId*)data = NetworkBehaviour.NetworkWrap(runner, reference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static NetworkBehaviour ReadNetworkBehaviourRef(int* data, NetworkRunner runner, out bool isValid)
	{
		if (*data == 0)
		{
			switch (data[1])
			{
			case 0:
				isValid = false;
				return null;
			case 1:
				isValid = true;
				return null;
			}
		}
		isValid = true;
		return NetworkBehaviour.NetworkUnwrap(runner, *(NetworkBehaviourId*)data);
	}
}
