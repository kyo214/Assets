using System;
using System.Collections.Generic;
using Doozy.Runtime.Pooler;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

public class SignalStream
{
	public const string k_None = "None";

	public const string k_DefaultCategory = "None";

	public const string k_DefaultName = "None";

	public UnityAction<ISignalReceiver> OnReceiverConnected;

	public UnityAction<ISignalReceiver> OnReceiverDisconnected;

	public UnityAction<Signal> OnSignal;

	public Guid key { get; }

	public string category { get; private set; } = "None";

	public string name { get; private set; } = "None";

	public int signalsCounter { get; private set; }

	public List<ISignalReceiver> receivers { get; } = new List<ISignalReceiver>();

	public int receiversCount => receivers.Count;

	public Signal previousSignal { get; protected set; }

	public Signal currentSignal { get; protected set; }

	public SignalProvider signalProvider { get; protected set; }

	public bool hasProvider => signalProvider != null;

	public string infoMessage { get; protected set; }

	private HashSet<ISignalReceiver> sendTempList { get; } = new HashSet<ISignalReceiver>();

	private HashSet<ISignalReceiver> disconnectTempList { get; } = new HashSet<ISignalReceiver>();

	internal SignalStream(Guid streamKey)
	{
		key = streamKey;
	}

	internal SignalStream SetCategory(string streamCategory)
	{
		category = streamCategory;
		return this;
	}

	internal SignalStream SetName(string streamName)
	{
		name = streamName;
		return this;
	}

	internal SignalStream SetSignalProvider(SignalProvider provider)
	{
		signalProvider = provider;
		return this;
	}

	internal SignalStream SetInfoMessage(string message)
	{
		infoMessage = message;
		return this;
	}

	public virtual SignalStream ConnectReceiver(ISignalReceiver receiver)
	{
		if (receiver == null)
		{
			return this;
		}
		if (receivers.Contains(receiver))
		{
			return this;
		}
		receivers.Add(receiver);
		OnReceiverConnected?.Invoke(receiver);
		return this;
	}

	public virtual void DisconnectReceiver(ISignalReceiver receiver)
	{
		if (receiver != null && receivers.Contains(receiver))
		{
			receivers.Remove(receiver);
			if (receiver.stream == this)
			{
				receiver.Disconnect();
				OnReceiverDisconnected?.Invoke(receiver);
			}
		}
	}

	public virtual void DisconnectAllReceivers()
	{
		receivers.Remove(null);
		ISignalReceiver[] array = receivers.ToArray();
		foreach (ISignalReceiver signalReceiver in array)
		{
			if (signalReceiver != null)
			{
				DisconnectReceiver(signalReceiver);
			}
		}
		receivers.Clear();
	}

	public virtual void ClearCallbacks()
	{
		OnSignal = null;
	}

	public virtual void Close()
	{
		DisconnectAllReceivers();
		ClearCallbacks();
	}

	public virtual bool SendSignal(string message = "")
	{
		return SendSignal(null, null, null, message);
	}

	public virtual bool SendSignal(GameObject signalSource, string message = "")
	{
		return SendSignal(signalSource, null, null, message);
	}

	public virtual bool SendSignal(SignalProvider provider, string message = "")
	{
		return SendSignal(null, provider, null, message);
	}

	public virtual bool SendSignal(UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(null, null, signalSender, message);
	}

	public virtual bool SendSignal(GameObject signalSource, SignalProvider provider, UnityEngine.Object signalSender, string message = "")
	{
		return InternalSendSignal(signalSource, provider, signalSender, message);
	}

	private bool InternalSendSignal(GameObject signalSource, SignalProvider provider, UnityEngine.Object signalSender, string message = "")
	{
		Signal signal = SignalPool.Get<Signal>().Reset();
		if (provider != null)
		{
			SignalExtensions.SetSignalSource(signal, provider.gameObject);
			signal.SetSignalProvider(provider);
			signal.SetSignalSender(provider);
		}
		if (signalSource != null)
		{
			SignalExtensions.SetSignalSource(signal, signalSource);
		}
		if (signalSender != null)
		{
			signal.SetSignalSender(signalSender);
		}
		signal.SetMessage(message);
		return Send(signal);
	}

	public virtual bool SendSignal<T>(T signalValue, string message = "")
	{
		return SendSignal(signalValue, null, null, null, message);
	}

	public virtual bool SendSignal<T>(T signalValue, GameObject signalSource, string message = "")
	{
		return SendSignal(signalValue, signalSource, null, null, message);
	}

	public virtual bool SendSignal<T>(T signalValue, SignalProvider provider, string message = "")
	{
		return SendSignal(signalValue, null, provider, null, message);
	}

	public virtual bool SendSignal<T>(T signalValue, UnityEngine.Object signalSender, string message = "")
	{
		return SendSignal(signalValue, null, null, signalSender, message);
	}

	public virtual bool SendSignal<T>(T signalValue, GameObject signalSource, SignalProvider provider, UnityEngine.Object signalSender, string message = "")
	{
		return InternalSendSignal(signalValue, signalSource, provider, signalSender, message);
	}

	private bool InternalSendSignal<T>(T signalValue, GameObject signalSource, SignalProvider provider, UnityEngine.Object signalSender, string message = "")
	{
		MetaSignal<T> metaSignal = SignalPool.Get<MetaSignal<T>>().Reset();
		metaSignal.SetSignalValue(signalValue);
		if (provider != null)
		{
			SignalExtensions.SetSignalSource(metaSignal, provider.gameObject);
			metaSignal.SetSignalProvider(provider);
			metaSignal.SetSignalSender(provider);
		}
		if (signalSource != null)
		{
			SignalExtensions.SetSignalSource(metaSignal, signalSource);
		}
		if (signalSender != null)
		{
			metaSignal.SetSignalSender(signalSender);
		}
		metaSignal.SetMessage(message);
		return Send(metaSignal);
	}

	private bool Send(Signal signal)
	{
		signal.SetStream(this);
		signalsCounter++;
		if (previousSignal != null)
		{
			if (previousSignal.hasValue && previousSignal.valueAsObject is IPoolable poolable)
			{
				poolable.Recycle();
			}
			previousSignal.Recycle();
		}
		previousSignal = currentSignal;
		currentSignal = signal;
		OnSignal?.Invoke(currentSignal);
		SignalsService.OnSignal?.Invoke(currentSignal);
		receivers.Remove(null);
		ISignalReceiver[] array = receivers.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.OnSignal(currentSignal);
		}
		return true;
	}

	public static SignalStream Get()
	{
		return SignalsService.GetStream();
	}

	public static SignalStream Get(string streamCategory, string streamName)
	{
		return SignalsService.GetStream(streamCategory, streamName);
	}

	public static SignalStream Get(Guid streamKey)
	{
		return SignalsService.FindStream(streamKey);
	}
}
