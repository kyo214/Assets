using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCNetworkInt<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly Action<TContext, int> _set;

	private readonly Func<TContext, int> _get;

	private readonly Func<TContext, float, int, int, int> _interpolate;

	public KCCNetworkInt(TContext context, Action<TContext, int> set, Func<TContext, int> get, Func<TContext, float, int, int, int> interpolate)
		: base(context, 1)
	{
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		_set(Context, *ptr);
	}

	public unsafe override void Write(int* ptr)
	{
		*ptr = _get(Context);
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		int num = *interpolationData.From;
		int to = *interpolationData.To;
		int arg = ((_interpolate == null) ? ((int)Mathf.Lerp(num, to, interpolationData.Alpha)) : _interpolate(Context, interpolationData.Alpha, num, to));
		_set(Context, arg);
	}
}
