using UnityEngine;

namespace Doozy.Runtime.Signals;

public abstract class BaseStreamListener : MonoBehaviour
{
	public SignalReceiver receiver { get; protected set; }

	public bool isConnected { get; protected set; }

	protected BaseStreamListener()
	{
		isConnected = false;
		receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
	}

	public virtual void Connect()
	{
		if (!isConnected)
		{
			ConnectReceiver();
			isConnected = true;
		}
	}

	public virtual void Disconnect()
	{
		if (isConnected)
		{
			DisconnectReceiver();
			isConnected = false;
		}
	}

	protected abstract void ConnectReceiver();

	protected abstract void DisconnectReceiver();

	protected abstract void ProcessSignal(Signal signal);
}
