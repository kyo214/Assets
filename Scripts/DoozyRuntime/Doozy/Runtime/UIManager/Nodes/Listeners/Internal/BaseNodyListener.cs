using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager.Nodes.Listeners.Internal;

public abstract class BaseNodyListener
{
	protected static bool multiplayerMode => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance.multiplayerMode;

	protected SignalReceiver receiver { get; }

	protected FlowNode node { get; }

	protected bool isConnected { get; private set; }

	protected BaseNodyListener(FlowNode node)
	{
		this.node = node;
		isConnected = false;
		receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
	}

	public virtual void Start()
	{
		Stop();
		if (!isConnected)
		{
			ConnectReceiver();
			isConnected = true;
		}
	}

	public virtual void Stop()
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
