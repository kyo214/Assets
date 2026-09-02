using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Bool Signal Listener")]
public class BoolSignalListener : SignalListener
{
	public BoolEvent OnBoolSignal = new BoolEvent();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(bool)))
		{
			OnBoolSignal?.Invoke((bool)signal.valueAsObject);
		}
	}
}
