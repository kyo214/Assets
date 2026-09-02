using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Vector4 Signal Listener")]
public class Vector4SignalListener : SignalListener
{
	public Vector4Event OnVector4Signal = new Vector4Event();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(Vector4)))
		{
			OnVector4Signal?.Invoke((Vector4)signal.valueAsObject);
		}
	}
}
