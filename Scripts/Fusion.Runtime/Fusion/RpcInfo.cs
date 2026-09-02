namespace Fusion;

public struct RpcInfo
{
	public Tick Tick;

	public PlayerRef Source;

	public RpcChannel Channel;

	public bool IsInvokeLocal;

	public static RpcInfo FromLocal(NetworkRunner runner, RpcChannel channel, RpcHostMode hostMode)
	{
		RpcInfo result = new RpcInfo
		{
			Tick = runner.Simulation.Tick,
			Source = runner.Simulation.LocalPlayer,
			IsInvokeLocal = true,
			Channel = channel
		};
		if (hostMode == RpcHostMode.SourceIsServer && runner.IsHostPlayer(result.Source))
		{
			result.Source = default;
		}
		return result;
	}

	public unsafe static RpcInfo FromMessage(NetworkRunner runner, SimulationMessage* message, RpcHostMode hostMode)
	{
		RpcInfo result = new RpcInfo
		{
			Tick = message->Tick,
			Source = message->Source,
			IsInvokeLocal = false,
			Channel = (message->GetFlag(8) ? RpcChannel.Unreliable : RpcChannel.Reliable)
		};
		if (message->Source.IsNone && hostMode == RpcHostMode.SourceIsHostPlayer && runner.TryGetHostPlayer(out var player))
		{
			result.Source = player;
		}
		return result;
	}

	public override string ToString()
	{
		return string.Format("[RpcInfo: {0}={1}, {2}={3}, {4}={5}, {6}={7}]", "Tick", Tick, "Source", Source, "IsInvokeLocal", IsInvokeLocal, "Channel", Channel);
	}
}
