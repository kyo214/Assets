using UnityEngine;

namespace Doozy.Runtime.Common.Events;

public abstract class ValueChangedEventBase<T> : IValueChangedEvent<T>
{
	public T previousValue { get; }

	public T newValue { get; }

	public bool animateChange { get; }

	public bool used { get; set; }

	public float timestamp { get; }

	protected ValueChangedEventBase(T previousValue, T newValue, bool animateChange)
	{
		this.previousValue = previousValue;
		this.newValue = newValue;
		this.animateChange = animateChange;
		used = false;
		timestamp = Time.time;
	}
}
