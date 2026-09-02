using System;
using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Reactions;

public static class FloatReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<float> getter) where T : FloatReaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : FloatReaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<float> setter) where T : FloatReaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : FloatReaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<float> callback) where T : FloatReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<float> callback) where T : FloatReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<float>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : FloatReaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}
}
