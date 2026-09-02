using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Ticker;

public class RuntimeHeartbeat : Heartbeat
{
	public override double timeSinceStartup => RuntimeTicker.timeSinceStartup;

	public RuntimeHeartbeat()
		: base(null)
	{
	}

	public RuntimeHeartbeat(ReactionCallback onTickCallback)
		: base(onTickCallback)
	{
	}

	public override void RegisterToTickService()
	{
		base.RegisterToTickService();
		RuntimeTicker.service?.Register(this);
	}

	public override void UnregisterFromTickService()
	{
		base.UnregisterFromTickService();
		RuntimeTicker.service?.Unregister(this);
	}
}
