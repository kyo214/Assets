using System;
using Doozy.Runtime.Signals;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody;

[Serializable]
public class ModyEvent : ModyEventBase
{
	public UnityEvent Event = new UnityEvent();

	public bool hasEvents
	{
		get
		{
			if (Event != null)
			{
				return Event.GetPersistentEventCount() > 0;
			}
			return false;
		}
	}

	public override bool hasCallbacks => base.hasRunners | hasEvents;

	public ModyEvent()
		: this("Unnamed")
	{
	}

	public ModyEvent(string eventName)
		: base(eventName)
	{
	}

	public override void Execute(Signal signal = null)
	{
		base.Execute(signal);
		Event?.Invoke();
	}
}
