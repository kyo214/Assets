using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

public static class Texture2DReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<Texture2D> getter) where T : Texture2DReaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : Texture2DReaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<Texture2D> setter) where T : Texture2DReaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : Texture2DReaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : Texture2DReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : Texture2DReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<int>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : Texture2DReaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}

	public static T SetOnFrameChangedCallback<T>(this T target, ReactionCallback<Texture2D> callback) where T : Texture2DReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnFrameChangedCallback = callback;
		return target;
	}

	public static T AddOnFrameChangedCallback<T>(this T target, ReactionCallback<Texture2D> callback) where T : Texture2DReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnFrameChangedCallback = (ReactionCallback<Texture2D>)Delegate.Combine(target.OnFrameChangedCallback, callback);
		return target;
	}

	public static T ClearOnFrameChangedCallback<T>(this T target) where T : Texture2DReaction
	{
		target.OnFrameChangedCallback = null;
		return target;
	}
}
