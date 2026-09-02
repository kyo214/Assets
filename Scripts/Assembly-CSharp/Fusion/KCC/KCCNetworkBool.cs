using System;

namespace Fusion.KCC;

public sealed class KCCNetworkBool<TContext> : KCCNetworkProperty<TContext> where TContext : class
{
	private readonly Action<TContext, bool> _set;

	private readonly Func<TContext, bool> _get;

	private readonly Func<TContext, float, bool, bool, bool> _interpolate;

	public KCCNetworkBool(TContext context, Action<TContext, bool> set, Func<TContext, bool> get, Func<TContext, float, bool, bool, bool> interpolate)
		: base(context, 1)
	{
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		_set(Context, (*ptr != 0) ? true : false);
	}

	public unsafe override void Write(int* ptr)
	{
		*ptr = (_get(Context) ? 1 : 0);
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		bool flag = ((*interpolationData.From != 0) ? true : false);
		bool flag2 = ((*interpolationData.To != 0) ? true : false);
		bool arg = ((_interpolate == null) ? ((interpolationData.Alpha < 0.5f) ? flag : flag2) : _interpolate(Context, interpolationData.Alpha, flag, flag2));
		_set(Context, arg);
	}
}
