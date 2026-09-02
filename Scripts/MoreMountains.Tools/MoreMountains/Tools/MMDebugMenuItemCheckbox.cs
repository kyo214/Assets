using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMDebugMenuItemCheckbox : MonoBehaviour
{
	[Header("Bindings")]
	public MMDebugMenuSwitch Switch;

	public Text SwitchText;

	public string CheckboxEventName = "Checkbox";

	protected bool _valueSetThisFrame;

	protected bool _listening;

	public virtual void TriggerCheckboxEvent()
	{
		if (_valueSetThisFrame)
		{
			_valueSetThisFrame = false;
		}
		else
		{
			MMDebugMenuCheckboxEvent.Trigger(CheckboxEventName, Switch.SwitchState);
		}
	}

	public virtual void TriggerCheckboxEventTrue()
	{
		if (_valueSetThisFrame)
		{
			_valueSetThisFrame = false;
		}
		else
		{
			MMDebugMenuCheckboxEvent.Trigger(CheckboxEventName, value: true);
		}
	}

	public virtual void TriggerCheckboxEventFalse()
	{
		if (_valueSetThisFrame)
		{
			_valueSetThisFrame = false;
		}
		else
		{
			MMDebugMenuCheckboxEvent.Trigger(CheckboxEventName, value: false);
		}
	}

	protected virtual void OnMMDebugMenuCheckboxEvent(string checkboxEventName, bool value, MMDebugMenuCheckboxEvent.EventModes eventMode)
	{
		if (eventMode == MMDebugMenuCheckboxEvent.EventModes.SetCheckbox && checkboxEventName == CheckboxEventName)
		{
			_valueSetThisFrame = true;
			if (value)
			{
				Switch.SetTrue();
			}
			else
			{
				Switch.SetFalse();
			}
		}
	}

	public virtual void OnEnable()
	{
		if (!_listening)
		{
			_listening = true;
			MMDebugMenuCheckboxEvent.Register(OnMMDebugMenuCheckboxEvent);
		}
	}

	public virtual void OnDestroy()
	{
		_listening = false;
		MMDebugMenuCheckboxEvent.Unregister(OnMMDebugMenuCheckboxEvent);
	}
}
