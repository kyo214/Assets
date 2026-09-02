namespace Doozy.Runtime.UIManager.Components;

public static class UIToggleExtensions
{
	public static T SetIsOn<T>(this T target, bool newValue, bool animateChange = true, bool triggerValueChanged = true) where T : UIToggle
	{
		if (target.isLocked)
		{
			return target;
		}
		bool isOn = target.isOn;
		target.IsOn = newValue;
		if (target.inToggleGroup)
		{
			target.toggleGroup.ToggleChangedValue(target, animateChange, triggerValueChanged);
			return target;
		}
		target.ValueChanged(isOn, newValue, animateChange, triggerValueChanged);
		return target;
	}

	public static T Lock<T>(this T target) where T : UIToggle
	{
		target.isLocked = true;
		return target;
	}

	public static T Unlock<T>(this T target) where T : UIToggle
	{
		target.isLocked = false;
		return target;
	}
}
