using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.UIManager.Listeners.Internal;

public abstract class BaseListener : BaseStreamListener
{
	public ModyEvent Callback;

	protected BaseListener()
	{
		Callback = new ModyEvent("Callback invoked every time the listener is triggered");
	}

	protected override void ProcessSignal(Signal signal)
	{
		Callback?.Execute();
	}
}
