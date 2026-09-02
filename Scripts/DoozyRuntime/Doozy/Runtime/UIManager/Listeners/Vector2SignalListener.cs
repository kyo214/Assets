using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Vector2 Signal Listener")]
public class Vector2SignalListener : SignalListener
{
	public Vector2Event OnVector2Signal = new Vector2Event();

	protected override void ProcessSignal(Signal signal)
	{
		base.ProcessSignal(signal);
		if (signal != null && !(signal.valueType != typeof(Vector2)))
		{
			OnVector2Signal?.Invoke((Vector2)signal.valueAsObject);
		}
	}
}
