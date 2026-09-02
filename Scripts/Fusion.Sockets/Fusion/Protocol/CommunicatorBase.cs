#define DEBUG
using System;
using System.Collections.Generic;

namespace Fusion.Protocol;

internal abstract class CommunicatorBase : ICommunicator
{
	protected readonly Dictionary<Type, Action<int, IMessage>> _callbacks = new Dictionary<Type, Action<int, IMessage>>();

	protected readonly List<Message> _messageList;

	protected readonly Queue<(int, Message)> _messageSendQueue;

	protected readonly Queue<(int, object)> _recvQueue;

	private ProtocolSerializer _protocolSerializer = new ProtocolSerializer();

	public abstract int CommunicatorID { get; }

	public CommunicatorBase()
	{
		_messageList = new List<Message>(64);
		_messageSendQueue = new Queue<(int, Message)>(64);
		_recvQueue = new Queue<(int, object)>();
	}

	public bool Poll()
	{
		return _recvQueue.Count > 0;
	}

	public void PushPackage(int senderActor, int eventCode, object data)
	{
		senderActor = ((senderActor > 0) ? senderActor : 0);
		switch (eventCode)
		{
		case 101:
			_recvQueue.Enqueue((senderActor, data));
			break;
		case 100:
			HandleProtocolPackage(senderActor, data);
			break;
		case 102:
			break;
		}
	}

	public unsafe void SendMessage(int targetActor, IMessage message)
	{
		if (_protocolSerializer.ConvertToBuffer((Message)message, out var buffer))
		{
			fixed (byte* data = buffer.Data)
			{
				if (SendPackage(100, targetActor, reliable: true, data, buffer.BytesRequired))
				{
					Log.Debug($"Sending to [{targetActor}]: {message}");
				}
				else
				{
					_messageSendQueue.Enqueue((targetActor, (Message)message));
				}
			}
		}
		else
		{
			_messageSendQueue.Enqueue((targetActor, (Message)message));
		}
	}

	public virtual void Service()
	{
		if (_messageSendQueue.Count > 0)
		{
			var (targetActor, message) = _messageSendQueue.Dequeue();
			SendMessage(targetActor, message);
		}
	}

	private void HandleProtocolPackage(int actorNr, object data)
	{
		ConvertData(data, out var dataBuffer, out var _);
		if (dataBuffer == null || !_protocolSerializer.ConvertToMessages(dataBuffer, _messageList))
		{
			return;
		}
		foreach (Message message in _messageList)
		{
			if (_callbacks.TryGetValue(message.GetType(), out var value))
			{
				Log.Debug($"Received from [{actorNr}] :: {message}");
				value(actorNr, message);
			}
		}
	}

	public unsafe int ReceivePackage(out int senderActor, byte* buffer, int bufferLength)
	{
		if (Poll())
		{
			var (num, data) = _recvQueue.Dequeue();
			ConvertData(data, out var dataBuffer, out var maxLength);
			if (maxLength > 0)
			{
				senderActor = num;
				Assert.Always(maxLength <= bufferLength, "ReceivePackage overflow");
				fixed (byte* source = dataBuffer)
				{
					Native.MemCpy(buffer, source, maxLength);
				}
				return maxLength;
			}
		}
		senderActor = -1;
		return -1;
	}

	public unsafe abstract bool SendPackage(byte code, int targetActor, bool reliable, byte* buffer, int bufferLength);

	protected abstract void ConvertData(object data, out byte[] dataBuffer, out int maxLength);

	public void RegisterPackageCallback<K>(Action<int, K> callback) where K : IMessage
	{
		_callbacks.Add(typeof(K), (int actor, IMessage msg) =>
		{
			callback(actor, (K)msg);
		});
	}
}
