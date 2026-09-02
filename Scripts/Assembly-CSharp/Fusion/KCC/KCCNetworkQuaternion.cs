using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkQuaternion<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly float _readAccuracy;

	private readonly float _writeAccuracy;

	private readonly Action<TContext, Quaternion> _set;

	private readonly Func<TContext, Quaternion> _get;

	private readonly Func<TContext, float, Quaternion, Quaternion, Quaternion> _interpolate;

	public KCCNetworkQuaternion(TContext context, float accuracy, Action<TContext, Quaternion> set, Func<TContext, Quaternion> get, Func<TContext, float, Quaternion, Quaternion, Quaternion> interpolate)
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
		Quaternion arg = default;
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
		Quaternion quaternion = _get(Context);
		if (_writeAccuracy <= 0f)
		{
			*(float*)ptr = quaternion.x;
			*(float*)(ptr + 1) = quaternion.y;
			*(float*)(ptr + 2) = quaternion.z;
			*(float*)(ptr + 3) = quaternion.w;
		}
		else
		{
			*ptr = ((quaternion.x < 0f) ? ((int)(quaternion.x * _writeAccuracy - 0.5f)) : ((int)(quaternion.x * _writeAccuracy + 0.5f)));
			ptr[1] = ((quaternion.y < 0f) ? ((int)(quaternion.y * _writeAccuracy - 0.5f)) : ((int)(quaternion.y * _writeAccuracy + 0.5f)));
			ptr[2] = ((quaternion.z < 0f) ? ((int)(quaternion.z * _writeAccuracy - 0.5f)) : ((int)(quaternion.z * _writeAccuracy + 0.5f)));
			ptr[3] = ((quaternion.w < 0f) ? ((int)(quaternion.w * _writeAccuracy - 0.5f)) : ((int)(quaternion.w * _writeAccuracy + 0.5f)));
		}
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		int* ptr = interpolationData.From;
		int* to = interpolationData.To;
		Quaternion quaternion = default;
		Quaternion quaternion2 = default;
		if (_readAccuracy <= 0f)
		{
			quaternion.x = *(float*)ptr;
			quaternion.y = *(float*)(ptr + 1);
			quaternion.z = *(float*)(ptr + 2);
			quaternion.w = *(float*)(ptr + 3);
			quaternion2.x = *(float*)to;
			quaternion2.y = *(float*)(to + 1);
			quaternion2.z = *(float*)(to + 2);
			quaternion2.w = *(float*)(to + 3);
		}
		else
		{
			quaternion.x = (float)(*ptr) * _readAccuracy;
			quaternion.y = (float)ptr[1] * _readAccuracy;
			quaternion.z = (float)ptr[2] * _readAccuracy;
			quaternion.w = (float)ptr[3] * _readAccuracy;
			quaternion2.x = (float)(*to) * _readAccuracy;
			quaternion2.y = (float)to[1] * _readAccuracy;
			quaternion2.z = (float)to[2] * _readAccuracy;
			quaternion2.w = (float)to[3] * _readAccuracy;
		}
		Quaternion arg = ((_interpolate == null) ? Quaternion.Lerp(quaternion, quaternion2, interpolationData.Alpha) : _interpolate(Context, interpolationData.Alpha, quaternion, quaternion2));
		_set(Context, arg);
	}
}
