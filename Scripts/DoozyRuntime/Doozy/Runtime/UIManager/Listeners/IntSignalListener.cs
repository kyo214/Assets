using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Int Signal Listener")]
public class IntSignalListener : SignalListener
{
	public IntEvent OnIntSignal = new IntEvent();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(int)))
		{
			OnIntSignal?.Invoke((int)signal.valueAsObject);
		}
	}
}
