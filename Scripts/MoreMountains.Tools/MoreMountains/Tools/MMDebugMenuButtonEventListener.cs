using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuButtonEventListener : MonoBehaviour
{
	[Header("Event")]
	public string ButtonEventName = "Button";

	public MMDButtonPressedEvent MMDEvent;

	[Header("Test")]
	public bool TestValue = true;

	[MMInspectorButton("TestSetValue")]
	public bool TestSetValueButton;

	protected virtual void TestSetValue()
	{
		MMDebugMenuButtonEvent.Trigger(ButtonEventName, TestValue, MMDebugMenuButtonEvent.EventModes.SetButton);
	}

	protected virtual void OnMMDebugMenuButtonEvent(string buttonEventName, bool value, MMDebugMenuButtonEvent.EventModes eventMode)
	{
		if (eventMode == MMDebugMenuButtonEvent.EventModes.FromButton && buttonEventName == ButtonEventName && MMDEvent != null)
		{
			MMDEvent.Invoke();
		}
	}

	public virtual void OnEnable()
	{
		MMDebugMenuButtonEvent.Register(OnMMDebugMenuButtonEvent);
	}

	public virtual void OnDisable()
	{
		MMDebugMenuButtonEvent.Unregister(OnMMDebugMenuButtonEvent);
	}
}
