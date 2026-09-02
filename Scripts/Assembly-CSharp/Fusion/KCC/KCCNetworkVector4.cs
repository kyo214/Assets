using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkVector4<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly float _readAccuracy;

	private readonly float _writeAccuracy;

	private readonly Action<TContext, Vector4> _set;

	private readonly Func<TContext, Vector4> _get;

	private readonly Func<TContext, float, Vector4, Vector4, Vector4> _interpolate;

	public KCCNetworkVector4(TContext context, float accuracy, Action<TContext, Vector4> set, Func<TContext, Vector4> get, Func<TContext, float, Vector4, Vector4, Vector4> interpolate)
		: base(context, 4)
	{
		_readAccuracy = ((accuracy > 0f) ? accuracy : 0f);
		_writeAccuracy = ((accuracy > 0f) ? (1f / accuracy) : 0f);
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		Vector4 arg = default;
		if (_readAccuracy <= 0f)
		{
			arg.x = *(float*)ptr;
			arg.y = *(float*)(ptr + 1);
			arg.z = *(float*)(ptr + 2);
			arg.w = *(float*)(ptr + 3);
		}
		else
		{
			arg.x = (float)(*ptr) * _readAccuracy;
			arg.y = (float)ptr[1] * _readAccuracy;
			arg.z = (float)ptr[2] * _readAccuracy;
			arg.w = (float)ptr[3] * _readAccuracy;
		}
		_set(Context, arg);
	}

	public unsafe override void Write(int* ptr)
	{
		Vector4 vector = _get(Context);
		if (_writeAccuracy <= 0f)
		{
			*(float*)ptr = vector.x;
			*(float*)(ptr + 1) = vector.y;
			*(float*)(ptr + 2) = vector.z;
			*(float*)(ptr + 3) = vector.w;
		}
		else
		{
			*ptr = ((vector.x < 0f) ? ((int)(vector.x * _writeAccuracy - 0.5f)) : ((int)(vector.x * _writeAccuracy + 0.5f)));
			ptr[1] = ((vector.y < 0f) ? ((int)(vector.y * _writeAccuracy - 0.5f)) : ((int)(vector.y * _writeAccuracy + 0.5f)));
			ptr[2] = ((vector.z < 0f) ? ((int)(vector.z * _writeAccuracy - 0.5f)) : ((int)(vector.z * _writeAccuracy + 0.5f)));
			ptr[3] = ((vector.w < 0f) ? ((int)(vector.w * _writeAccuracy - 0.5f)) : ((int)(vector.w * _writeAccuracy + 0.5f)));
		}
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		int* ptr = interpolationData.From;
		int* to = interpolationData.To;
		Vector4 vector = default;
		Vector4 vector2 = default;
		if (_readAccuracy <= 0f)
		{
			vector.x = *(float*)ptr;
			vector.y = *(float*)(ptr + 1);
			vector.z = *(float*)(ptr + 2);
			vector.w = *(float*)(ptr + 3);
			vector2.x = *(float*)to;
			vector2.y = *(float*)(to + 1);
			vector2.z = *(float*)(to + 2);
			vector2.w = *(float*)(to + 3);
		}
		else
		{
			vector.x = (float)(*ptr) * _readAccuracy;
			vector.y = (float)ptr[1] * _readAccuracy;
			vector.z = (float)ptr[2] * _readAccuracy;
			vector.w = (float)ptr[3] * _readAccuracy;
			vector2.x = (float)(*to) * _readAccuracy;
			vector2.y = (float)to[1] * _readAccuracy;
			vector2.z = (float)to[2] * _readAccuracy;
			vector2.w = (float)to[3] * _readAccuracy;
		}
		Vector4 arg = ((_interpolate == null) ? Vector4.Lerp(vector, vector2, interpolationData.Alpha) : _interpolate(Context, interpolationData.Alpha, vector, vector2));
		_set(Context, arg);
	}
}
