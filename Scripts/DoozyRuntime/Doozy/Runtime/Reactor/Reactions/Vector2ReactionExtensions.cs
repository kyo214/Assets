using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

public static class Vector2ReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<Vector2> getter) where T : Vector2Reaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : Vector2Reaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<Vector2> setter) where T : Vector2Reaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : Vector2Reaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<Vector2> callback) where T : Vector2Reaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<Vector2> callback) where T : Vector2Reaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<Vector2>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : Vector2Reaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}
}
