using System;

namespace Fusion.KCC;

public sealed class KCCNetworkEnum<TContext, TEnum> : KCCNetworkProperty<TContext> where TContext : class where TEnum : unmanaged, Enum
{
	private static class EnumConvertor
	{
		public unsafe static int ToInt<T>(T value) where T : unmanaged, Enum
		{
			return *(int*)(&value);
		}

		public unsafe static T ToEnum<T>(int value) where T : unmanaged, Enum
		{
			return *(T*)(&value);
		}
	}

	private readonly Action<TContext, TEnum> _set;

	private readonly Func<TContext, TEnum> _get;

	private readonly Func<TContext, float, TEnum, TEnum, TEnum> _interpolate;

	public KCCNetworkEnum(TContext context, Action<TContext, TEnum> set, Func<TContext, TEnum> get, Func<TContext, float, TEnum, TEnum, TEnum> interpolate)
		: base(context, 1)
	{
		_set = set;
		_get = get;
		_interpolate = interpolate;
	}

	public unsafe override void Read(int* ptr)
	{
		_set(Context, EnumConvertor.ToEnum<TEnum>(*ptr));
	}

	public unsafe override void Write(int* ptr)
	{
		*ptr = EnumConvertor.ToInt(_get(Context));
	}

	public unsafe override void Interpolate(InterpolationData interpolationData)
	{
		int num = *interpolationData.From;
		int to = *interpolationData.To;
		int value = ((_interpolate == null) ? ((interpolationData.Alpha < 0.5f) ? num : to) : EnumConvertor.ToInt(_interpolate(Context, interpolationData.Alpha, EnumConvertor.ToEnum<TEnum>(num), EnumConvertor.ToEnum<TEnum>(to))));
		_set(Context, EnumConvertor.ToEnum<TEnum>(value));
	}
}
