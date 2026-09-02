using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkFloat<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly float _readAccuracy;

	private readonly float _writeAccuracy;

	private readonly Action<TContext, float> _set;

	private readonly Func<TContext, float> _get;

	private readonly Func<TContext, float, float, float, float> _interpolate;

	public KCCNetworkFloat(TContext context, float accuracy, Action<TContext, float> set, Func<TContext, float> get, Func<TContext, float, float, float, float> interpolate)
		: base(context, 1)
	{
		_readAccuracy = ((accuracy > 0f) ? accuracy : 0f);
		_writeAccuracy = ((accuracy > 0f) ? (1f / accuracy) : 0f);
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		float arg = ((!(_readAccuracy <= 0f)) ? ((float)(*ptr) * _readAccuracy) : (*(float*)ptr));
		_set(Context, arg);
	}

	public unsafe override void Write(int* ptr)
	{
		float num = _get(Context);
		if (_writeAccuracy <= 0f)
		{
			*(float*)ptr = num;
		}
		else
		{
			*ptr = ((num < 0f) ? ((int)(num * _writeAccuracy - 0.5f)) : ((int)(num * _writeAccuracy + 0.5f)));
		}
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		float num = ((_readAccuracy <= 0f) ? (*(float*)interpolationData.From) : ((float)(*interpolationData.From) * _readAccuracy));
		float num2 = ((_readAccuracy <= 0f) ? (*(float*)interpolationData.To) : ((float)(*interpolationData.To) * _readAccuracy));
		float arg = ((_interpolate == null) ? Mathf.Lerp(num, num2, interpolationData.Alpha) : _interpolate(Context, interpolationData.Alpha, num, num2));
		_set(Context, arg);
	}
}
