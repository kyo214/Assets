using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/String Signal Listener")]
public class StringSignalListener : SignalListener
{
	public StringEvent OnStringSignal = new StringEvent();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(string)))
		{
			OnStringSignal?.Invoke((string)signal.valueAsObject);
		}
	}
}
