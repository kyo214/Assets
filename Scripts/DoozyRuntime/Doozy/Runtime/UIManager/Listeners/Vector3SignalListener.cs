using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Vector3 Signal Listener")]
public class Vector3SignalListener : SignalListener
{
	public Vector3Event OnVector3Signal = new Vector3Event();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(Vector3)))
		{
			OnVector3Signal?.Invoke((Vector3)signal.valueAsObject);
		}
	}
}
