#define DEBUG
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Fusion.Sockets.Stun;

internal static class StunClient
{
	private static class TestIPs
	{
		public static readonly IPEndPoint TestNetIpv4 = new IPEndPoint(IPAddress.Parse("203.0.113.0"), 65530);

		public static readonly IPEndPoint TestNetIpv6 = new IPEndPoint(IPAddress.Parse("2001:db8::"), 65530);
	}

	internal static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<int, NetAddress>> PendingRequests = new ConcurrentDictionary<Guid, ConcurrentDictionary<int, NetAddress>>();

	public unsafe static bool TryParse(NetAddress* origin, byte* buffer, int bufferLength)
	{
		StunMessage stunMessage = StunMessage.TryParse(buffer, bufferLength);
		if (stunMessage != null && stunMessage.MappedAddress != null)
		{
			if (PendingRequests.TryGetValue(stunMessage.ID, out var value))
			{
				int port = stunMessage.MappedAddress.Port;
				string ip = stunMessage.MappedAddress.Address.ToString();
				NetAddress value2 = NetAddress.CreateFromIpPort(ip, (ushort)port);
				if (value2.IsValid && !value.TryAdd(origin->GetHashCode(), value2))
				{
				}
			}
			return true;
		}
		return false;
	}

	public unsafe static bool QueryLocalAddress(NetPeer* peer, INetSocket socket, out AddressFamily addressFamily, out NetAddress localAddress)
	{
		localAddress = NetAddress.AnyIPv4Addr;
		AddressFamily addressFamily2 = (peer->Address.IsIPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);
		ushort port = peer->Address.NativeAddress.Port;
		addressFamily = addressFamily2;
		if (GetLocalAddress(ref addressFamily, out var localIP))
		{
			if (addressFamily != addressFamily2)
			{
			}
			NetAddress netAddress = NetAddress.CreateFromIpPort(localIP.ToString(), port);
			if (netAddress.IsValid)
			{
				localAddress = netAddress;
				return true;
			}
		}
		Log.Warn("[STUN] Unable to resolve Local Address");
		return false;
	}

	public unsafe static bool QueryPublicAddress(NetPeer* peer, INetSocket socket, AddressFamily originalFamily, ref Guid requestID, out bool skipNATDiscovery)
	{
		skipNATDiscovery = false;
		bool flag = originalFamily == AddressFamily.InterNetworkV6;
		List<StunServers.StunServer> stunServer = StunServers.GetStunServer(flag);
		if (stunServer.Count == 0)
		{
			Log.Warn("[STUN] Unable to find any valid STUN Server, aborting Reflexive Address query.");
			return false;
		}
		if (stunServer.Count == 1)
		{
			Log.Debug("[STUN] Only one STUN Server found, skip NAT Type Discovery.");
			skipNATDiscovery = true;
		}
		StunMessage stunMessage = new StunMessage(requestID);
		byte[] array = stunMessage.Serialize();
		int bufferLength = array.Length;
		bool flag2 = false;
		foreach (StunServers.StunServer item in stunServer)
		{
			try
			{
				NetAddress netAddress = (flag ? item.IPv6Addr : item.IPv4Addr);
				if (!netAddress.IsValid)
				{
					continue;
				}
				fixed (byte* buffer = array)
				{
					if (socket.Send(peer->_socket, &netAddress, buffer, bufferLength) > 0)
					{
						flag2 = true;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		if (!flag2)
		{
			return false;
		}
		requestID = stunMessage.ID;
		return true;
	}

	private static bool GetLocalAddress(ref AddressFamily addressFamily, out IPAddress localIP)
	{
		while (true)
		{
			localIP = null;
			try
			{
				using Socket socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.IP);
				socket.Connect((addressFamily == AddressFamily.InterNetwork) ? TestIPs.TestNetIpv4 : TestIPs.TestNetIpv6);
				localIP = (socket.LocalEndPoint as IPEndPoint).Address;
			}
			catch
			{
				if (addressFamily == AddressFamily.InterNetworkV6)
				{
					addressFamily = AddressFamily.InterNetwork;
					continue;
				}
				return false;
			}
			break;
		}
		return true;
	}
}
