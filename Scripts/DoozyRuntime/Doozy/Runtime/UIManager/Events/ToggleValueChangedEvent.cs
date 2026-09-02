using Doozy.Runtime.Common.Events;

namespace Doozy.Runtime.UIManager.Events;

public class ToggleValueChangedEvent : ValueChangedEventBase<bool>
{
	public ToggleValueChangedEvent(bool previousValue, bool newValue, bool animateChange)
		: base(previousValue, newValue, animateChange)
	{
	}
}
