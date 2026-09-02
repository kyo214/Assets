using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

[Serializable]
public abstract class MultiSignalsReceiver<T> where T : SignalReceiver
{
	public List<T> SignalsReceivers = new List<T>();

	public bool isConnected
	{
		get
		{
			if (SignalsReceivers != null && SignalsReceivers.Count != 0)
			{
				return (from SignalReceiver receiver in SignalsReceivers
					where receiver != null
					select receiver).Any((SignalReceiver receiver) => receiver.isConnected);
			}
			return false;
		}
	}

	protected abstract void OnSignal(Signal signal);

	public virtual void ConnectReceivers()
	{
		foreach (T signalsReceiver in SignalsReceivers)
		{
			T current = signalsReceiver;
			UnityAction<Signal> onSignal = (UnityAction<Signal>)Delegate.Combine(current.onSignal, new UnityAction<Signal>(OnSignal));
			current.onSignal = onSignal;
			current.Connect();
		}
	}

	public virtual void DisconnectReceivers()
	{
		foreach (T signalsReceiver in SignalsReceivers)
		{
			T current = signalsReceiver;
			UnityAction<Signal> onSignal = (UnityAction<Signal>)Delegate.Remove(current.onSignal, new UnityAction<Signal>(OnSignal));
			current.onSignal = onSignal;
			current.Disconnect();
		}
	}
}
