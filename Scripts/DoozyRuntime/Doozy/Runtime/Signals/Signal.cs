using System;
using UnityEngine;

namespace Doozy.Runtime.Signals;

public class Signal
{
	public SignalStream stream { get; protected internal set; }

	public SignalProvider signalProvider { get; protected internal set; }

	public Type providerType { get; protected internal set; }

	public bool hasProvider => signalProvider != null;

	public UnityEngine.Object signalSenderObject { get; protected internal set; }

	public Type senderType { get; protected internal set; }

	public bool hasSenderObject => signalSenderObject != null;

	public GameObject sourceGameObject { get; protected internal set; }

	public bool hasSourceGameObject => sourceGameObject != null;

	public bool hasValue { get; protected internal set; }

	public Type valueType { get; protected internal set; }

	public object valueAsObject { get; protected internal set; }

	public bool used { get; protected internal set; }

	public float timestamp { get; protected internal set; }

	public string message { get; protected internal set; }

	public Signal()
	{
		stream = null;
		signalProvider = null;
		providerType = null;
		signalSenderObject = null;
		senderType = null;
		sourceGameObject = null;
		hasValue = false;
		valueType = null;
		valueAsObject = null;
		used = false;
		timestamp = Time.time;
		message = string.Empty;
	}

	internal Signal(SignalStream stream, bool hasValue = false, Type valueType = null)
		: this(stream, null, null, null, hasValue, valueType)
	{
	}

	internal Signal(SignalStream stream, GameObject signalSource, bool hasValue = false, Type valueType = null)
		: this(stream, signalSource, null, null, hasValue, valueType)
	{
	}

	internal Signal(SignalStream stream, SignalProvider signalProvider, bool hasValue = false, Type valueType = null)
		: this(stream, null, signalProvider, null, hasValue, valueType)
	{
	}

	internal Signal(SignalStream stream, UnityEngine.Object senderObject, bool hasValue = false, Type valueType = null)
		: this(stream, null, null, senderObject, hasValue, valueType)
	{
	}

	internal Signal(SignalStream stream, GameObject signalSource, SignalProvider signalProvider, UnityEngine.Object signalSender, bool hasValue = false, Type valueType = null)
	{
		this.Reset().SetStream(stream).SetValueType(hasValue, valueType);
		if (signalProvider != null)
		{
			SignalExtensions.SetSignalSource(this, signalProvider.gameObject);
			this.SetSignalProvider(signalProvider);
			this.SetSignalSender(signalProvider);
		}
		if (signalSource != null)
		{
			SignalExtensions.SetSignalSource(this, signalSource);
		}
		if (signalSender != null)
		{
			this.SetSignalSender(signalSender);
		}
	}

	internal void Recycle()
	{
		this.AddToPool();
	}

	public void Use()
	{
		used = true;
	}

	public bool TryGetValue<T>(out T value)
	{
		if (hasValue)
		{
			try
			{
				value = ((MetaSignal<T>)this).value;
				return true;
			}
			catch
			{
			}
		}
		value = default;
		return false;
	}

	public T GetValueUnsafe<T>()
	{
		return ((MetaSignal<T>)this).value;
	}

	public bool TryGetValueType(out Type type)
	{
		if (hasValue)
		{
			type = valueType;
			return true;
		}
		type = null;
		return false;
	}

	public static bool Send(string streamCategory, string streamName, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, message);
	}

	public static bool Send(string streamCategory, string streamName, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalSource, message);
	}

	public static bool Send(string streamCategory, string streamName, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalProvider, message);
	}

	public static bool Send(string streamCategory, string streamName, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalSender, message);
	}

	public static bool Send(Guid streamKey, string message = "")
	{
		return SignalsService.SendSignal(streamKey, message);
	}

	public static bool Send(Guid streamKey, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalSource, message);
	}

	public static bool Send(Guid streamKey, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalProvider, message);
	}

	public static bool Send(Guid streamKey, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalSender, message);
	}

	public static bool Send(SignalStream stream, string message = "")
	{
		return SignalsService.SendSignal(stream, message);
	}

	public static bool Send(SignalStream stream, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(stream, signalSource, message);
	}

	public static bool Send(SignalStream stream, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(stream, signalProvider, message);
	}

	public static bool Send(SignalStream stream, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(stream, signalSender, message);
	}

	public static bool Send<T>(string streamCategory, string streamName, T signalValue, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalValue, message);
	}

	public static bool Send<T>(string streamCategory, string streamName, T signalValue, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalValue, signalSource, message);
	}

	public static bool Send<T>(string streamCategory, string streamName, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalValue, signalProvider, message);
	}

	public static bool Send<T>(string streamCategory, string streamName, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(streamCategory, streamName, signalValue, signalSender, message);
	}

	public static bool Send<T>(Guid streamKey, T signalValue, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalValue, message);
	}

	public static bool Send<T>(Guid streamKey, T signalValue, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalValue, signalSource, message);
	}

	public static bool Send<T>(Guid streamKey, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalValue, signalProvider, message);
	}

	public static bool Send<T>(Guid streamKey, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(streamKey, signalValue, signalSender, message);
	}

	public static bool Send<T>(SignalStream stream, T signalValue, string message = "")
	{
		return SignalsService.SendSignal(stream, signalValue, message);
	}

	public static bool Send<T>(SignalStream stream, T signalValue, GameObject signalSource, string message = "")
	{
		return SignalsService.SendSignal(stream, signalValue, signalSource, message);
	}

	public static bool Send<T>(SignalStream stream, T signalValue, SignalProvider signalProvider, string message = "")
	{
		return SignalsService.SendSignal(stream, signalValue, signalProvider, message);
	}

	public static bool Send<T>(SignalStream stream, T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SignalsService.SendSignal(stream, signalValue, signalSender, message);
	}
}
