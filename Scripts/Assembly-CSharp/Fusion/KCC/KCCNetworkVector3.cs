using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkVector3<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly float _readAccuracy;

	private readonly float _writeAccuracy;

	private readonly Action<TContext, Vector3> _set;

	private readonly Func<TContext, Vector3> _get;

	private readonly Func<TContext, float, Vector3, Vector3, Vector3> _interpolate;

	public KCCNetworkVector3(TContext context, float accuracy, Action<TContext, Vector3> set, Func<TContext, Vector3> get, Func<TContext, float, Vector3, Vector3, Vector3> interpolate)
		: base(context, 3)
	{
		_readAccuracy = ((accuracy > 0f) ? accuracy : 0f);
		_writeAccuracy = ((accuracy > 0f) ? (1f / accuracy) : 0f);
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		Vector3 arg = default;
		if (_readAccuracy <= 0f)
		{
			arg.x = *(float*)ptr;
			arg.y = *(float*)(ptr + 1);
			arg.z = *(float*)(ptr + 2);
		}
		else
		{
			arg.x = (float)(*ptr) * _readAccuracy;
			arg.y = (float)ptr[1] * _readAccuracy;
			arg.z = (float)ptr[2] * _readAccuracy;
		}
		_set(Context, arg);
	}

	public unsafe override void Write(int* ptr)
	{
		Vector3 vector = _get(Context);
		if (_writeAccuracy <= 0f)
		{
			*(float*)ptr = vector.x;
			*(float*)(ptr + 1) = vector.y;
			*(float*)(ptr + 2) = vector.z;
		}
		else
		{
			*ptr = ((vector.x < 0f) ? ((int)(vector.x * _writeAccuracy - 0.5f)) : ((int)(vector.x * _writeAccuracy + 0.5f)));
			ptr[1] = ((vector.y < 0f) ? ((int)(vector.y * _writeAccuracy - 0.5f)) : ((int)(vector.y * _writeAccuracy + 0.5f)));
			ptr[2] = ((vector.z < 0f) ? ((int)(vector.z * _writeAccuracy - 0.5f)) : ((int)(vector.z * _writeAccuracy + 0.5f)));
		}
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		int* ptr = interpolationData.From;
		int* to = interpolationData.To;
		Vector3 vector = default;
		Vector3 vector2 = default;
		if (_readAccuracy <= 0f)
		{
			vector.x = *(float*)ptr;
			vector.y = *(float*)(ptr + 1);
			vector.z = *(float*)(ptr + 2);
			vector2.x = *(float*)to;
			vector2.y = *(float*)(to + 1);
			vector2.z = *(float*)(to + 2);
		}
		else
		{
			vector.x = (float)(*ptr) * _readAccuracy;
			vector.y = (float)ptr[1] * _readAccuracy;
			vector.z = (float)ptr[2] * _readAccuracy;
			vector2.x = (float)(*to) * _readAccuracy;
			vector2.y = (float)to[1] * _readAccuracy;
			vector2.z = (float)to[2] * _readAccuracy;
		}
		Vector3 arg = ((_interpolate == null) ? Vector3.Lerp(vector, vector2, interpolationData.Alpha) : _interpolate(Context, interpolationData.Alpha, vector, vector2));
		_set(Context, arg);
	}
}
