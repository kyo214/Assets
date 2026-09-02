using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

[Serializable]
public class SignalReceiver : ISignalReceiver
{
	[SerializeField]
	private StreamConnection ConnectionMode;

	[SerializeField]
	private ProviderId SignalProviderId;

	[SerializeField]
	private SignalProvider ProviderReference;

	[SerializeField]
	private StreamId StreamId;

	public UnityAction<Signal> onSignal { get; set; }

	public StreamConnection streamConnection
	{
		get
		{
			return ConnectionMode;
		}
		protected internal set
		{
			ConnectionMode = value;
		}
	}

	public ProviderId providerId
	{
		get
		{
			return SignalProviderId;
		}
		protected internal set
		{
			SignalProviderId = value;
		}
	}

	public SignalProvider providerReference
	{
		get
		{
			return ProviderReference;
		}
		protected internal set
		{
			ProviderReference = value;
		}
	}

	public StreamId streamId
	{
		get
		{
			return StreamId;
		}
		protected internal set
		{
			StreamId = value;
		}
	}

	public GameObject signalSource { get; protected internal set; }

	public SignalStream stream { get; private set; }

	public bool isConnected { get; private set; }

	public bool isDisconnecting { get; private set; }

	public SignalReceiver()
	{
		Reset();
	}

	public ISignalReceiver Reset()
	{
		ConnectionMode = StreamConnection.None;
		SignalProviderId = default;
		ProviderReference = null;
		StreamId = new StreamId();
		signalSource = null;
		stream = null;
		isConnected = false;
		isDisconnecting = false;
		return this;
	}

	public virtual void OnSignal(Signal signal)
	{
		onSignal?.Invoke(signal);
	}

	public void Connect()
	{
		if (!Application.isPlaying)
		{
			Disconnect();
		}
		else
		{
			if (isConnected)
			{
				return;
			}
			switch (streamConnection)
			{
			case StreamConnection.None:
				return;
			case StreamConnection.ProviderId:
				providerReference = (SignalProvider)SignalsService.GetProvider(providerId, signalSource);
				if (providerReference == null)
				{
					Debug.Log("Provider not found!");
					return;
				}
				if (!providerReference.isConnected)
				{
					providerReference.OpenStream();
				}
				stream = providerReference.stream;
				break;
			case StreamConnection.ProviderReference:
				if (providerReference == null)
				{
					Debug.Log("Provider not referenced!");
					return;
				}
				if (!providerReference.isConnected)
				{
					providerReference.OpenStream();
				}
				stream = providerReference.stream;
				break;
			case StreamConnection.StreamId:
				if (streamId.Category.Equals("None") || streamId.Name.Equals("None"))
				{
					Debug.Log("Will not connect to " + streamId.Category + " > " + streamId.Name);
					return;
				}
				stream = SignalsService.GetStream(streamId.Category, streamId.Name);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (stream != null)
			{
				stream.ConnectReceiver(this);
				streamId.SetStream(stream);
				isConnected = true;
			}
		}
	}

	public void Disconnect()
	{
		if (isConnected)
		{
			SignalStream signalStream = stream;
			stream = null;
			isConnected = false;
			signalStream.DisconnectReceiver(this);
		}
	}
}
