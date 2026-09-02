using Doozy.Runtime.Colors;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Color Signal Listener")]
public class ColorSignalListener : SignalListener
{
	public ColorEvent OnColorSignal = new ColorEvent();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(Color)))
		{
			OnColorSignal?.Invoke((Color)signal.valueAsObject);
		}
	}
}
