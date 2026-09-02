using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

public static class SignalReceiverExtensions
{
	public static T SetStreamConnection<T>(this T target, StreamConnection streamConnection) where T : SignalReceiver
	{
		target.streamConnection = streamConnection;
		return target;
	}

	public static T SetProviderId<T>(this T target, ProviderId providerId, bool updateStreamConnection = true) where T : SignalReceiver
	{
		target.providerId = providerId;
		if (!updateStreamConnection)
		{
			return target;
		}
		return target.SetStreamConnection(StreamConnection.ProviderId);
	}

	public static T SetProviderReference<T>(this T target, SignalProvider providerReference, bool updateStreamConnection = true) where T : SignalReceiver
	{
		target.providerReference = providerReference;
		if (!updateStreamConnection)
		{
			return target;
		}
		return target.SetStreamConnection(StreamConnection.ProviderReference);
	}

	public static T SetStreamId<T>(this T target, StreamId streamId, bool updateStreamConnection = true) where T : SignalReceiver
	{
		target.streamId = streamId;
		if (!updateStreamConnection)
		{
			return target;
		}
		return target.SetStreamConnection(StreamConnection.StreamId);
	}

	public static T SetStreamId<T>(this T target, string category, string name, bool updateStreamConnection = true) where T : SignalReceiver
	{
		target.streamId = new StreamId(category, name);
		if (!updateStreamConnection)
		{
			return target;
		}
		return target.SetStreamConnection(StreamConnection.StreamId);
	}

	public static T SetSignalSource<T>(this T target, GameObject signalSource) where T : SignalReceiver
	{
		if (target.isConnected)
		{
			return target;
		}
		target.signalSource = signalSource;
		return target;
	}

	public static T SetOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
	{
		target.onSignal = callback;
		return target;
	}

	public static T AddOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
	{
		UnityAction<Signal> onSignal = (UnityAction<Signal>)Delegate.Combine(target.onSignal, callback);
		target.onSignal = onSignal;
		return target;
	}

	public static T RemoveOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
	{
		UnityAction<Signal> onSignal = (UnityAction<Signal>)Delegate.Remove(target.onSignal, callback);
		target.onSignal = onSignal;
		return target;
	}

	public static T ClearOnSignalCallback<T>(this T target) where T : SignalReceiver
	{
		target.onSignal = null;
		return target;
	}
}
