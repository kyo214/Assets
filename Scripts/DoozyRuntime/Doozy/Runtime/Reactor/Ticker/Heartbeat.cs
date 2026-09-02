using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Ticker;

public abstract class Heartbeat : IUseTickService
{
	public bool isActive { get; private set; }

	public virtual double timeSinceStartup => 0.0;

	public double lastUpdateTime { get; set; }

	public double deltaTime
	{
		get
		{
			double result = timeSinceStartup - lastUpdateTime;
			lastUpdateTime = timeSinceStartup;
			return result;
		}
	}

	public ReactionCallback onTickCallback { get; internal set; }

	protected Heartbeat(ReactionCallback onTickCallback)
	{
		this.onTickCallback = onTickCallback;
	}

	public virtual void Tick()
	{
		onTickCallback?.Invoke();
	}

	public virtual void RegisterToTickService()
	{
		isActive = true;
		lastUpdateTime = timeSinceStartup;
	}

	public virtual void UnregisterFromTickService()
	{
		isActive = false;
	}
}
