using UnityEngine;

namespace MoreMountains.Tools;

public class MMInput : MonoBehaviour
{
	public enum ButtonStates
	{
		Off = 0,
		ButtonDown = 1,
		ButtonPressed = 2,
		ButtonUp = 3
	}

	public enum AxisTypes
	{
		Positive = 0,
		Negative = 1
	}

	public class IMButton
	{
		public delegate void ButtonDownMethodDelegate();

		public delegate void ButtonPressedMethodDelegate();

		public delegate void ButtonUpMethodDelegate();

		public string ButtonID;

		public ButtonDownMethodDelegate ButtonDownMethod;

		public ButtonPressedMethodDelegate ButtonPressedMethod;

		public ButtonUpMethodDelegate ButtonUpMethod;

		protected float _lastButtonDownAt;

		protected float _lastButtonUpAt;

		public MMStateMachine<ButtonStates> State { get; protected set; }

		public float TimeSinceLastButtonDown => Time.unscaledTime - _lastButtonDownAt;

		public float TimeSinceLastButtonUp => Time.unscaledTime - _lastButtonUpAt;

		public bool ButtonDownRecently(float time)
		{
			return Time.unscaledTime - TimeSinceLastButtonDown <= time;
		}

		public bool ButtonUpRecently(float time)
		{
			return Time.unscaledTime - TimeSinceLastButtonUp <= time;
		}

		public IMButton(string playerID, string buttonID, ButtonDownMethodDelegate btnDown = null, ButtonPressedMethodDelegate btnPressed = null, ButtonUpMethodDelegate btnUp = null)
		{
			ButtonID = playerID + "_" + buttonID;
			ButtonDownMethod = btnDown;
			ButtonUpMethod = btnUp;
			ButtonPressedMethod = btnPressed;
			State = new MMStateMachine<ButtonStates>(null, triggerEvents: false);
			State.ChangeState(ButtonStates.Off);
		}

		public virtual void TriggerButtonDown()
		{
			_lastButtonDownAt = Time.unscaledTime;
			if (ButtonDownMethod == null)
			{
				State.ChangeState(ButtonStates.ButtonDown);
			}
			else
			{
				ButtonDownMethod();
			}
		}

		public virtual void TriggerButtonPressed()
		{
			if (ButtonPressedMethod == null)
			{
				State.ChangeState(ButtonStates.ButtonPressed);
			}
			else
			{
				ButtonPressedMethod();
			}
		}

		public virtual void TriggerButtonUp()
		{
			_lastButtonUpAt = Time.unscaledTime;
			if (ButtonUpMethod == null)
			{
				State.ChangeState(ButtonStates.ButtonUp);
			}
			else
			{
				ButtonUpMethod();
			}
		}
	}

	public static ButtonStates ProcessAxisAsButton(string axisName, float threshold, ButtonStates currentState, AxisTypes AxisType = AxisTypes.Positive)
	{
		float axis = Input.GetAxis(axisName);
		if ((AxisType == AxisTypes.Positive) ? (axis < threshold) : (axis > threshold))
		{
			if (currentState == ButtonStates.ButtonPressed)
			{
				return ButtonStates.ButtonUp;
			}
			return ButtonStates.Off;
		}
		if (currentState == ButtonStates.Off)
		{
			return ButtonStates.ButtonDown;
		}
		return ButtonStates.ButtonPressed;
	}
}
