namespace Doozy.Runtime.Common;

public interface IValueChangedEvent<out T>
{
	T previousValue { get; }

	T newValue { get; }

	bool animateChange { get; }

	bool used { get; set; }

	float timestamp { get; }
}
