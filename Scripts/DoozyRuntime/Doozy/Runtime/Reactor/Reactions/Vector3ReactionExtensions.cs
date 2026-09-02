using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

public static class Vector3ReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<Vector3> getter) where T : Vector3Reaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : Vector3Reaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<Vector3> setter) where T : Vector3Reaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : Vector3Reaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<Vector3> callback) where T : Vector3Reaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<Vector3> callback) where T : Vector3Reaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<Vector3>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : Vector3Reaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}
}
