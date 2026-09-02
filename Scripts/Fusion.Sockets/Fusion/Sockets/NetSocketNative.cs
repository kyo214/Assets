using System;
using Fusion.Sockets.Stun;
using NanoSockets;

namespace Fusion.Sockets;

internal class NetSocketNative : INetSocket
{
	public bool SupportsMultiThreading => true;

	public void Initialize(NetConfig config)
	{
		Assert.Always(UDP.Initialize() == Status.Ok, "Unable to initialize Socket");
	}

	public NetSocket Create(NetConfig config)
	{
		Socket socket = UDP.Create(config.SocketSendBuffer, config.SocketRecvBuffer);
		Assert.Always(socket.IsCreated, "Unable to create Socket");
		Assert.Always(UDP.SetNonBlocking(socket) == Status.Ok, "Unable to set Socket as NonBlocking");
		return new NetSocket
		{
			NativeSocket = socket
		};
	}

	public NetAddress Bind(NetSocket socket, NetConfig config)
	{
		Address address = config.Address.NativeAddress;
		if (UDP.Bind(socket.NativeSocket.handle, ref address) != 0)
		{
			UDP.Destroy(ref socket.NativeSocket.handle);
			throw new InvalidOperationException($"Failed to bind socket to {config.Address.NativeAddress}");
		}
		address = default;
		if (UDP.GetAddress(socket.NativeSocket.handle, ref address) != Status.Ok)
		{
			UDP.Destroy(ref socket.NativeSocket.handle);
			throw new InvalidOperationException("Failed to resolve address for bound socket");
		}
		address._address0 = config.Address.NativeAddress._address0;
		address._address1 = config.Address.NativeAddress._address1;
		return new NetAddress
		{
			NativeAddress = address
		};
	}

	public bool CanFragment(NetAddress address)
	{
		return true;
	}

	public bool Poll(NetSocket socket, long timeout)
	{
		return UDP.Poll(socket.NativeSocket.handle, timeout) > 0;
	}

	public unsafe int Receive(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength)
	{
		int num = UDP.Receive(socket.NativeSocket.handle, &address->NativeAddress, buffer, bufferLength);
		if (num > 0 && StunMessage.IsStunMessage(buffer, bufferLength))
		{
			StunClient.TryParse(address, buffer, num);
			return -1;
		}
		return num;
	}

	public unsafe int Send(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength, bool reliable = false)
	{
		return UDP.Send(socket.NativeSocket.handle, &address->NativeAddress, buffer, bufferLength);
	}

	public void Destroy(NetSocket netSocket)
	{
		UDP.Destroy(ref netSocket.NativeSocket.handle);
	}
}
