using System;
using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Ticker;

public static class HeartbeatExtensions
{
	public static T ClearOnTickCallback<T>(this T target) where T : Heartbeat
	{
		target.onTickCallback = null;
		return target;
	}

	public static T SetOnTickCallback<T>(this T target, ReactionCallback callback) where T : Heartbeat
	{
		target.onTickCallback = callback;
		return target;
	}

	public static T AddOnTickCallback<T>(this T target, ReactionCallback callback) where T : Heartbeat
	{
		ref T reference = ref target;
		ref T reference2 = ref reference;
		ReactionCallback onTickCallback = (ReactionCallback)Delegate.Remove(reference.onTickCallback, callback);
		reference2.onTickCallback = onTickCallback;
		reference = ref target;
		ref T reference3 = ref reference;
		ReactionCallback onTickCallback2 = (ReactionCallback)Delegate.Combine(reference.onTickCallback, callback);
		reference3.onTickCallback = onTickCallback2;
		return target;
	}

	public static T RemoveOnTickCallback<T>(this T target, ReactionCallback callback) where T : Heartbeat
	{
		ReactionCallback onTickCallback = (ReactionCallback)Delegate.Remove(target.onTickCallback, callback);
		target.onTickCallback = onTickCallback;
		return target;
	}
}
