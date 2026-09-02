using System;
using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Reactions;

public static class IntReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<int> getter) where T : IntReaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : IntReaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<int> setter) where T : IntReaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : IntReaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : IntReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : IntReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<int>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : IntReaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}
}
