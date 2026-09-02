using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Float Signal Listener")]
public class FloatSignalListener : SignalListener
{
	public FloatEvent OnFloatSignal = new FloatEvent();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(float)))
		{
			OnFloatSignal?.Invoke((float)signal.valueAsObject);
		}
	}
}
