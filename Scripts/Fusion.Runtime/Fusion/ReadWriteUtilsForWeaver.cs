#define DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace Fusion;

public static class ReadWriteUtilsForWeaver
{
	private const int STRING_LENGTH_INDEX = 0;

	private const int STRING_HASHCODE_INDEX = 1;

	private const int STRING_DATA_INDEX = 2;

	private const int STRING_NOHASHCODE_DATA_INDEX = 1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static bool ReadBoolean(int* data)
	{
		return (*data != 0) ? true : false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static void WriteBoolean(int* data, bool value)
	{
		*data = (value ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static int ReadInt32(int* data, float accuracy)
	{
		return *data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static void WriteFloat(int* data, float accuracyInv, float value)
	{
		if (accuracyInv == 0f)
		{
			*(float*)data = value;
		}
		else
		{
			*data = ((value < 0f) ? ((int)(value * accuracyInv - 0.5f)) : ((int)(value * accuracyInv + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static int CompressFloat(float accuracyInv, float value)
	{
		if (accuracyInv == 0f)
		{
			return *(int*)(&value);
		}
		return (value < 0f) ? ((int)(value * accuracyInv - 0.5f)) : ((int)(value * accuracyInv + 0.5f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static float ReadFloat(int* data, float accuracy)
	{
		if (accuracy == 0f)
		{
			return *(float*)data;
		}
		return (float)(*data) * accuracy;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static float ReadSingle(int* data, float accuracy)
	{
		if (accuracy == 0f)
		{
			return *(float*)data;
		}
		return (float)(*data) * accuracy;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static void WriteVector2(int* data, float accuracyInv, Vector2 value)
	{
		if (accuracyInv == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * accuracyInv - 0.5f)) : ((int)(value.x * accuracyInv + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * accuracyInv - 0.5f)) : ((int)(value.y * accuracyInv + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static Vector2 ReadVector2(int* data, float accuracy)
	{
		Vector2 result = default;
		if (accuracy == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
		}
		else
		{
			result.x = (float)(*data) * accuracy;
			result.y = (float)data[1] * accuracy;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static void WriteVector3(int* data, float accuracyInv, Vector3 value)
	{
		if (accuracyInv == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
			*(float*)(data + 2) = value.z;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * accuracyInv - 0.5f)) : ((int)(value.x * accuracyInv + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * accuracyInv - 0.5f)) : ((int)(value.y * accuracyInv + 0.5f)));
			data[2] = ((value.z < 0f) ? ((int)(value.z * accuracyInv - 0.5f)) : ((int)(value.z * accuracyInv + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static Vector3 ReadVector3(int* data, float accuracy)
	{
		Vector3 result = default;
		if (accuracy == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
			result.z = *(float*)(data + 2);
		}
		else
		{
			result.x = (float)(*data) * accuracy;
			result.y = (float)data[1] * accuracy;
			result.z = (float)data[2] * accuracy;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static void WriteQuaternion(int* data, float accuracyInv, Quaternion value)
	{
		if (accuracyInv == 0f)
		{
			*(float*)data = value.x;
			*(float*)(data + 1) = value.y;
			*(float*)(data + 2) = value.z;
			*(float*)(data + 3) = value.w;
		}
		else
		{
			*data = ((value.x < 0f) ? ((int)(value.x * accuracyInv - 0.5f)) : ((int)(value.x * accuracyInv + 0.5f)));
			data[1] = ((value.y < 0f) ? ((int)(value.y * accuracyInv - 0.5f)) : ((int)(value.y * accuracyInv + 0.5f)));
			data[2] = ((value.z < 0f) ? ((int)(value.z * accuracyInv - 0.5f)) : ((int)(value.z * accuracyInv + 0.5f)));
			data[3] = ((value.w < 0f) ? ((int)(value.w * accuracyInv - 0.5f)) : ((int)(value.w * accuracyInv + 0.5f)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public unsafe static Quaternion ReadQuaternion(int* data, float accuracy)
	{
		Quaternion result = default;
		if (accuracy == 0f)
		{
			result.x = *(float*)data;
			result.y = *(float*)(data + 1);
			result.z = *(float*)(data + 2);
			result.w = *(float*)(data + 3);
		}
		else
		{
			result.x = (float)(*data) * accuracy;
			result.y = (float)data[1] * accuracy;
			result.z = (float)data[2] * accuracy;
			result.w = (float)data[3] * accuracy;
		}
		return result;
	}

	[Preserve]
	public unsafe static int WriteStringUtf8NoHash(void* destination, string str)
	{
		return Native.WriteLengthPrefixedUTF8(destination, str);
	}

	[Preserve]
	public unsafe static int ReadStringUtf8NoHash(void* source, out string result)
	{
		return Native.ReadLengthPrefixedUTF8(source, out result);
	}

	[Preserve]
	public static int GetByteCountUtf8NoHash(string value)
	{
		return Native.GetLengthPrefixedUTF8ByteCount(value);
	}

	[Preserve]
	public unsafe static int WriteStringUtf32NoHash(int* ptr, int maxLength, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			*ptr = 0;
			return 4;
		}
		UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)(ptr + 1), maxLength);
		*ptr = conversionResult.CodePointCount;
		return (conversionResult.CodePointCount + 1) * 4;
	}

	[Preserve]
	public unsafe static int ReadStringUtf32NoHash(int* ptr, int maxLength, out string result)
	{
		int num = Math.Min(*ptr, maxLength);
		int* value = ptr + 1;
		if (num == 0)
		{
			result = "";
		}
		else
		{
			result = new string((sbyte*)value, 0, num * 4, Encoding.UTF32);
		}
		return (num + 1) * 4;
	}

	[Preserve]
	public unsafe static int WriteStringUtf32WithHash(int* ptr, int maxLength, string value, ref string cache)
	{
		if (string.IsNullOrEmpty(value))
		{
			*ptr = 0;
			ptr[1] = 0;
			return 8;
		}
		UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)(ptr + 2), maxLength);
		*ptr = conversionResult.CodePointCount;
		Assert.Check(conversionResult.CharacterCount <= value.Length);
		if (conversionResult.CharacterCount < value.Length)
		{
			cache = value.Substring(0, conversionResult.CharacterCount);
		}
		else
		{
			cache = value;
		}
		ptr[1] = cache.GetHashDeterministic();
		return (conversionResult.CodePointCount + 2) * 4;
	}

	[Preserve]
	public unsafe static int ReadStringUtf32WithHash(int* ptr, int maxLength, ref string cache)
	{
		int num = Math.Min(*ptr, maxLength);
		int num2 = ptr[1];
		int* ptr2 = ptr + 2;
		if (num == 0)
		{
			cache = "";
		}
		else
		{
			if (cache != null && num >= cache.Length / 2 && num <= cache.Length && num2 == cache.GetHashCode() && UTF32Tools.CompareOrdinal(cache, (uint*)ptr2, num) == 0)
			{
				return (2 + num) * 4;
			}
			cache = new string((sbyte*)ptr2, 0, num * 4, Encoding.UTF32);
		}
		return (2 + num) * 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Preserve]
	public static int GetWordCountString(int capacity, bool withCaching)
	{
		if (withCaching)
		{
			return 2 + capacity;
		}
		return 1 + capacity;
	}

	[Preserve]
	public static int VerifyRawNetworkUnwrap<T>(int actual, int maxBytes)
	{
		if (actual > maxBytes)
		{
			throw new InvalidOperationException($"Overflow when unwrapping {typeof(T).FullName}: expected max {maxBytes}, got {actual}");
		}
		return actual;
	}

	[Preserve]
	public static int VerifyRawNetworkWrap<T>(int actual, int maxBytes)
	{
		if (actual > maxBytes)
		{
			throw new InvalidOperationException($"Overflow when wrapping {typeof(T).FullName}: expected max {maxBytes}, got {actual}");
		}
		return actual;
	}
}
