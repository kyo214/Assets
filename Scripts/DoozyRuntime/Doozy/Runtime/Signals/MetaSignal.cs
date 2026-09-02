using UnityEngine;

namespace Doozy.Runtime.Signals;

public class MetaSignal<T> : Signal
{
	public T value { get; private set; }

	public MetaSignal()
		: base(null, hasValue: true, typeof(T))
	{
		SetSignalValue(default);
	}

	internal MetaSignal(SignalStream stream, GameObject signalSource)
		: base(stream, signalSource, hasValue: true, typeof(T))
	{
		SetSignalValue(default);
	}

	internal MetaSignal(SignalStream stream, SignalProvider signalProvider)
		: base(stream, signalProvider, hasValue: true, typeof(T))
	{
		SetSignalValue(default);
	}

	internal MetaSignal(SignalStream stream, Object senderObject)
		: base(stream, senderObject, hasValue: true, typeof(T))
	{
		SetSignalValue(default);
	}

	internal void SetSignalValue(T signalValue)
	{
		this.SetValueType(hasValue: true, typeof(T));
		value = signalValue;
		base.valueAsObject = signalValue;
	}

	internal void ResetValue()
	{
		base.hasValue = false;
		base.valueType = null;
		value = default;
		base.valueAsObject = null;
	}
}
