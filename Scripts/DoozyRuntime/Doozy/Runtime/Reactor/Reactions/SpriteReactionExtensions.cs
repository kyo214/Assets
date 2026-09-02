using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

public static class SpriteReactionExtensions
{
	public static T SetGetter<T>(this T target, PropertyGetter<Sprite> getter) where T : SpriteReaction
	{
		target.getter = getter;
		return target;
	}

	public static T ClearGetter<T>(this T target) where T : SpriteReaction
	{
		return SetGetter(target, null);
	}

	public static T SetSetter<T>(this T target, PropertySetter<Sprite> setter) where T : SpriteReaction
	{
		target.setter = setter;
		return target;
	}

	public static T ClearSetter<T>(this T target) where T : SpriteReaction
	{
		return SetSetter(target, null);
	}

	public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : SpriteReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = callback;
		return target;
	}

	public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : SpriteReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnValueChangedCallback = (ReactionCallback<int>)Delegate.Combine(target.OnValueChangedCallback, callback);
		return target;
	}

	public static T ClearOnValueChangedCallback<T>(this T target) where T : SpriteReaction
	{
		target.OnValueChangedCallback = null;
		return target;
	}

	public static T SetOnFrameChangedCallback<T>(this T target, ReactionCallback<Sprite> callback) where T : SpriteReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnFrameChangedCallback = callback;
		return target;
	}

	public static T AddOnFrameChangedCallback<T>(this T target, ReactionCallback<Sprite> callback) where T : SpriteReaction
	{
		if (callback == null)
		{
			return target;
		}
		target.OnFrameChangedCallback = (ReactionCallback<Sprite>)Delegate.Combine(target.OnFrameChangedCallback, callback);
		return target;
	}

	public static T ClearOnFrameChangedCallback<T>(this T target) where T : SpriteReaction
	{
		target.OnFrameChangedCallback = null;
		return target;
	}
}
