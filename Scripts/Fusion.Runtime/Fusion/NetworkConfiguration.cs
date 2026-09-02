using System;
using Fusion.Sockets;

namespace Fusion;

[Serializable]
public class NetworkConfiguration
{
	[Flags]
	public enum ReliableDataTransfers
	{
		ClientToServer = 1,
		ClientToClientWithServerProxy = 2
	}

	[InlineHelp]
	[Unit(Units.Kilobytes)]
	[MultiPropertyDrawersFix]
	public int SocketSendBufferSize = 256;

	[InlineHelp]
	[Unit(Units.Kilobytes)]
	[MultiPropertyDrawersFix]
	public int SocketRecvBufferSize = 256;

	[InlineHelp]
	[Unit(Units.None)]
	[MultiPropertyDrawersFix]
	public int ConnectAttempts = 10;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public double ConnectInterval = 0.5;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public double ConnectionDefaultRtt = 0.1;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public double ConnectionTimeout = 10.0;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public double ConnectionPingInterval = 1.0;

	[InlineHelp]
	[Unit(Units.Seconds)]
	[MultiPropertyDrawersFix]
	public double ConnectionShutdownTime = 1.0;

	[InlineHelp]
	[Unit(Units.Bytes, 64.0, 1136.0)]
	[MultiPropertyDrawersFix]
	public int MtuDefault = 1136;

	[InlineHelp]
	public ReliableDataTransfers ReliableDataTransferModes = ReliableDataTransfers.ClientToServer | ReliableDataTransfers.ClientToClientWithServerProxy;

	public NetworkConfiguration Init()
	{
		NetworkConfiguration networkConfiguration = (NetworkConfiguration)MemberwiseClone();
		if (networkConfiguration.MtuDefault > 1136)
		{
			Log.Warn($"Invalid MTU size of {networkConfiguration.MtuDefault}, using max allowed MTU of {1136}");
		}
		networkConfiguration.MtuDefault = Math.Min(networkConfiguration.MtuDefault, 1136);
		return networkConfiguration;
	}

	internal NetConfig ToNetConfig(NetAddress address)
	{
		NetConfig defaults = NetConfig.Defaults;
		defaults.SocketSendBuffer = SocketSendBufferSize * 1024;
		defaults.SocketRecvBuffer = SocketRecvBufferSize * 1024;
		defaults.ConnectAttempts = ConnectAttempts;
		defaults.ConnectInterval = ConnectInterval;
		defaults.ConnectionDefaultRtt = ConnectionDefaultRtt;
		defaults.ConnectionTimeout = ConnectionTimeout;
		defaults.ConnectionPingInterval = ConnectionPingInterval;
		defaults.ConnectionShutdownTime = ConnectionShutdownTime;
		defaults.Address = address;
		defaults.DefaultMtu = Math.Min(MtuDefault, 1136);
		return defaults;
	}
}
