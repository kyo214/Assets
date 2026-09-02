using System;
using UnityEngine;

namespace Doozy.Runtime.Signals;

public static class SignalExtensions
{
	internal static T Reset<T>(this T target) where T : Signal
	{
		target.stream = null;
		target.signalProvider = null;
		target.providerType = null;
		target.signalSenderObject = null;
		target.senderType = null;
		target.sourceGameObject = null;
		target.used = false;
		target.timestamp = Time.time;
		target.hasValue = false;
		target.valueType = null;
		target.valueAsObject = null;
		if (target is MetaSignal<T> metaSignal)
		{
			metaSignal.ResetValue();
		}
		return target;
	}

	internal static T SetValueType<T>(this T target, bool hasValue = false, Type valueType = null) where T : Signal
	{
		target.hasValue = hasValue;
		target.valueType = valueType;
		return target;
	}

	internal static T SetStream<T>(this T target, SignalStream stream) where T : Signal
	{
		target.stream = stream;
		return target;
	}

	internal static T SetSignalProvider<T>(this T target, SignalProvider signalProvider) where T : Signal
	{
		target.signalProvider = signalProvider;
		target.providerType = ((signalProvider != null) ? signalProvider.GetType() : null);
		return target;
	}

	internal static T SetSignalSender<T>(this T target, UnityEngine.Object signalSender) where T : Signal
	{
		target.signalSenderObject = signalSender;
		target.senderType = ((signalSender != null) ? signalSender.GetType() : null);
		return target;
	}

	internal static T SetSignalSource<T>(this T target, GameObject sourceGameObject) where T : Signal
	{
		target.sourceGameObject = sourceGameObject;
		return target;
	}

	internal static T SetTimestamp<T>(this T target) where T : Signal
	{
		target.timestamp = Time.time;
		return target;
	}

	internal static T SetMessage<T>(this T target, string message) where T : Signal
	{
		target.message = message;
		return target;
	}
}
