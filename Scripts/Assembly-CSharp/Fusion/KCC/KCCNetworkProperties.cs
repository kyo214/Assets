using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkProperties : KCCNetworkProperty<KCCNetworkContext>
{
	private const int WORD_COUNT_DEFAULT = 12;

	private const int WORD_COUNT_POSITION_OFFSET = 3;

	private const int WORD_COUNT_WITH_POSITION_OFFSET = 15;

	private static readonly float _defaultPositionReadAccuracy = new Accuracy("Position").Value;

	private readonly bool _hasPositionOffset;

	private readonly float _positionReadAccuracy;

	private readonly float _positionWriteAccuracy;

	private readonly float _positionOffsetReadAccuracy;

	private readonly float _positionOffsetWriteAccuracy;

	private readonly float _rotationReadAccuracy;

	private readonly float _rotationWriteAccuracy;

	public KCCNetworkProperties(KCCNetworkContext context, Accuracy positionAccuracy, Accuracy rotationAccuracy)
		: base(context, HasPositionOffset(positionAccuracy) ? 15 : 12)
	{
		_hasPositionOffset = HasPositionOffset(positionAccuracy);
		_positionReadAccuracy = ((_defaultPositionReadAccuracy > 0f) ? _defaultPositionReadAccuracy : 0f);
		_positionWriteAccuracy = ((_defaultPositionReadAccuracy > 0f) ? (1f / _defaultPositionReadAccuracy) : 0f);
		float num = Mathf.Min(positionAccuracy.Value, _defaultPositionReadAccuracy);
		_positionOffsetReadAccuracy = ((num > 0f) ? num : 0f);
		_positionOffsetWriteAccuracy = ((num > 0f) ? (1f / num) : 0f);
		float value = rotationAccuracy.Value;
		_rotationReadAccuracy = ((value > 0f) ? value : 0f);
		_rotationWriteAccuracy = ((value > 0f) ? (1f / value) : 0f);
	}

	public unsafe Vector3 ReadPosition(int* ptr)
	{
		Vector3 result = ReadVector3(_positionReadAccuracy, ref ptr);
		if (_hasPositionOffset)
		{
			result += ReadVector3(_positionOffsetReadAccuracy, ref ptr);
		}
		return result;
	}

	public unsafe override void Read(int* ptr)
	{
		KCCData data = Context.Data;
		KCCSettings settings = Context.Settings;
		data.TargetPosition = ReadVector3(_positionReadAccuracy, ref ptr);
		if (_hasPositionOffset)
		{
			data.TargetPosition += ReadVector3(_positionOffsetReadAccuracy, ref ptr);
		}
		data.LookPitch = ReadFloat(_rotationReadAccuracy, ref ptr);
		data.LookYaw = ReadFloat(_rotationReadAccuracy, ref ptr);
		int num = *ptr;
		ptr++;
		settings.Shape = (EKCCShape)(num & 3);
		settings.IsTrigger = (num & 4) == 4;
		settings.ColliderLayer = (num & 0xF8) >> 3;
		settings.CollisionLayerMask = *ptr;
		ptr++;
		int num2 = *ptr;
		ptr++;
		settings.RenderBehavior = (EKCCRenderBehavior)(num2 & 3);
		settings.Features = (EKCCFeatures)((num2 & 0xFC) >> 2);
		settings.Radius = ReadFloat(0f, ref ptr);
		settings.Height = ReadFloat(0f, ref ptr);
		settings.Extent = ReadFloat(0f, ref ptr);
		settings.Mass = ReadFloat(0f, ref ptr);
	}

	public unsafe override void Write(int* ptr)
	{
		KCCData data = Context.Data;
		KCCSettings settings = Context.Settings;
		if (_hasPositionOffset)
		{
			Vector3 vector = WriteAndReadVector3(data.TargetPosition, _positionWriteAccuracy, _positionReadAccuracy, ref ptr);
			WriteVector3(data.TargetPosition - vector, _positionOffsetWriteAccuracy, ref ptr);
		}
		else
		{
			WriteVector3(data.TargetPosition, _positionWriteAccuracy, ref ptr);
		}
		WriteFloat(data.LookPitch, _rotationWriteAccuracy, ref ptr);
		WriteFloat(data.LookYaw, _rotationWriteAccuracy, ref ptr);
		int num = (int)(settings.Shape & (EKCCShape)3);
		num |= (settings.IsTrigger ? 4 : 0);
		num |= (settings.ColliderLayer << 3) & 0xF8;
		*ptr = num;
		ptr++;
		*ptr = settings.CollisionLayerMask;
		ptr++;
		int num2 = (int)(settings.RenderBehavior & (EKCCRenderBehavior)3);
		num2 |= ((int)settings.Features << 2) & 0xFC;
		*ptr = num2;
		ptr++;
		WriteFloat(settings.Radius, 0f, ref ptr);
		WriteFloat(settings.Height, 0f, ref ptr);
		WriteFloat(settings.Extent, 0f, ref ptr);
		WriteFloat(settings.Mass, 0f, ref ptr);
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		KCCData data = Context.Data;
		KCCSettings settings = Context.Settings;
		int* ptr = interpolationData.From;
		int* ptr2 = interpolationData.To;
		Vector3 a = ReadVector3(_positionReadAccuracy, ref ptr);
		Vector3 b = ReadVector3(_positionReadAccuracy, ref ptr2);
		if (_hasPositionOffset)
		{
			a += ReadVector3(_positionOffsetReadAccuracy, ref ptr);
			b += ReadVector3(_positionOffsetReadAccuracy, ref ptr2);
		}
		data.TargetPosition = Vector3.Lerp(a, b, interpolationData.Alpha);
		float a2 = ReadFloat(_rotationReadAccuracy, ref ptr);
		float b2 = ReadFloat(_rotationReadAccuracy, ref ptr2);
		data.LookPitch = Mathf.Lerp(a2, b2, interpolationData.Alpha);
		float num = ReadFloat(_rotationReadAccuracy, ref ptr);
		float to = ReadFloat(_rotationReadAccuracy, ref ptr2);
		data.LookYaw = KCCMathUtility.InterpolateRange(num, to, -180f, 180f, interpolationData.Alpha);
		int* ptr3 = ((interpolationData.Alpha < 0.5f) ? ptr : ptr2);
		int num2 = *ptr3;
		ptr3++;
		settings.Shape = (EKCCShape)(num2 & 3);
		settings.IsTrigger = (num2 & 4) == 4;
		settings.ColliderLayer = (num2 & 0xF8) >> 3;
		settings.CollisionLayerMask = *ptr3;
		ptr3++;
		int num3 = *ptr3;
		ptr3++;
		settings.RenderBehavior = (EKCCRenderBehavior)(num3 & 3);
		settings.Features = (EKCCFeatures)((num3 & 0xFC) >> 2);
		settings.Radius = ReadFloat(0f, ref ptr3);
		settings.Height = ReadFloat(0f, ref ptr3);
		settings.Extent = ReadFloat(0f, ref ptr3);
		settings.Mass = ReadFloat(0f, ref ptr3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static float ReadFloat(float accuracy, ref int* ptr)
	{
		float result;
		if (accuracy <= 0f)
		{
			result = *(float*)ptr;
			ptr++;
		}
		else
		{
			result = (float)(*ptr) * accuracy;
			ptr++;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void WriteFloat(float value, float accuracy, ref int* ptr)
	{
		if (accuracy <= 0f)
		{
			*(float*)ptr = value;
			ptr++;
		}
		else
		{
			*ptr = ((value < 0f) ? ((int)(value * accuracy - 0.5f)) : ((int)(value * accuracy + 0.5f)));
			ptr++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static Vector3 ReadVector3(float accuracy, ref int* ptr)
	{
		Vector3 result = default;
		if (accuracy <= 0f)
		{
			result.x = *(float*)ptr;
			ptr++;
			result.y = *(float*)ptr;
			ptr++;
			result.z = *(float*)ptr;
			ptr++;
		}
		else
		{
			result.x = (float)(*ptr) * accuracy;
			ptr++;
			result.y = (float)(*ptr) * accuracy;
			ptr++;
			result.z = (float)(*ptr) * accuracy;
			ptr++;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void WriteVector3(Vector3 value, float accuracy, ref int* ptr)
	{
		if (accuracy <= 0f)
		{
			*(float*)ptr = value.x;
			ptr++;
			*(float*)ptr = value.y;
			ptr++;
			*(float*)ptr = value.z;
			ptr++;
		}
		else
		{
			*ptr = ((value.x < 0f) ? ((int)(value.x * accuracy - 0.5f)) : ((int)(value.x * accuracy + 0.5f)));
			ptr++;
			*ptr = ((value.y < 0f) ? ((int)(value.y * accuracy - 0.5f)) : ((int)(value.y * accuracy + 0.5f)));
			ptr++;
			*ptr = ((value.z < 0f) ? ((int)(value.z * accuracy - 0.5f)) : ((int)(value.z * accuracy + 0.5f)));
			ptr++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static Vector3 WriteAndReadVector3(Vector3 value, float writeAccuracy, float readAccuracy, ref int* ptr)
	{
		if (writeAccuracy <= 0f)
		{
			*(float*)ptr = value.x;
			ptr++;
			*(float*)ptr = value.y;
			ptr++;
			*(float*)ptr = value.z;
			ptr++;
		}
		else
		{
			*ptr = ((value.x < 0f) ? ((int)(value.x * writeAccuracy - 0.5f)) : ((int)(value.x * writeAccuracy + 0.5f)));
			value.x = (float)(*ptr) * readAccuracy;
			ptr++;
			*ptr = ((value.y < 0f) ? ((int)(value.y * writeAccuracy - 0.5f)) : ((int)(value.y * writeAccuracy + 0.5f)));
			value.y = (float)(*ptr) * readAccuracy;
			ptr++;
			*ptr = ((value.z < 0f) ? ((int)(value.z * writeAccuracy - 0.5f)) : ((int)(value.z * writeAccuracy + 0.5f)));
			value.z = (float)(*ptr) * readAccuracy;
			ptr++;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool HasPositionOffset(Accuracy positionAccuracy)
	{
		return positionAccuracy.Value < _defaultPositionReadAccuracy;
	}
}
