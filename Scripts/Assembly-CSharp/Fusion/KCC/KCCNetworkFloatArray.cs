using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkFloatArray<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly int _count;

	private readonly float _readAccuracy;

	private readonly float _writeAccuracy;

	private readonly Action<TContext, int, float> _set;

	private readonly Func<TContext, int, float> _get;

	private readonly Func<TContext, int, float, float, float, float> _interpolate;

	public KCCNetworkFloatArray(TContext context, int count, float accuracy, Action<TContext, int, float> set, Func<TContext, int, float> get, Func<TContext, int, float, float, float, float> interpolate)
		: base(context, count)
	{
		_count = count;
		_readAccuracy = ((accuracy > 0f) ? accuracy : 0f);
		_writeAccuracy = ((accuracy > 0f) ? (1f / accuracy) : 0f);
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		for (int i = 0; i < _count; i++)
		{
			float arg = ((!(_readAccuracy <= 0f)) ? ((float)(*ptr) * _readAccuracy) : (*(float*)ptr));
			_set(Context, i, arg);
			ptr++;
		}
	}

	public unsafe override void Write(int* ptr)
	{
		for (int i = 0; i < _count; i++)
		{
			float num = _get(Context, i);
			if (_writeAccuracy <= 0f)
			{
				*(float*)ptr = num;
			}
			else
			{
				*ptr = ((num < 0f) ? ((int)(num * _writeAccuracy - 0.5f)) : ((int)(num * _writeAccuracy + 0.5f)));
			}
			ptr++;
		}
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		for (int i = 0; i < _count; i++)
		{
			float num = ((_readAccuracy <= 0f) ? (*(float*)interpolationData.From) : ((float)(*interpolationData.From) * _readAccuracy));
			float num2 = ((_readAccuracy <= 0f) ? (*(float*)interpolationData.To) : ((float)(*interpolationData.To) * _readAccuracy));
			float arg = ((_interpolate == null) ? Mathf.Lerp(num, num2, interpolationData.Alpha) : _interpolate(Context, i, interpolationData.Alpha, num, num2));
			_set(Context, i, arg);
			interpolationData.From++;
			interpolationData.To++;
		}
	}
}
