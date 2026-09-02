using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Feel;

public static class FeelDemosInputHelper
{
	private const string _horizontalAxis = "Horizontal";

	private const string _verticalAxis = "Vertical";

	public static bool CheckMainActionInputPressedThisFrame()
	{
		if (!Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			return Mouse.current.leftButton.wasPressedThisFrame;
		}
		return true;
	}

	public static bool CheckMainActionInputPressed()
	{
		if (!Keyboard.current.spaceKey.isPressed)
		{
			return Mouse.current.leftButton.isPressed;
		}
		return true;
	}

	public static bool CheckMainActionInputUpThisFrame()
	{
		if (!Keyboard.current.spaceKey.wasReleasedThisFrame)
		{
			return Mouse.current.leftButton.wasReleasedThisFrame;
		}
		return true;
	}

	public static bool CheckEnterPressedThisFrame()
	{
		return Keyboard.current.enterKey.wasPressedThisFrame;
	}

	public static bool CheckMouseDown()
	{
		return Mouse.current.leftButton.wasReleasedThisFrame;
	}

	public static Vector2 MousePosition()
	{
		return Mouse.current.position.ReadValue();
	}

	public static Vector2 GetDirectionAxis(ref Vector2 direction)
	{
		direction.x = 0f;
		direction.y = 0f;
		if (Keyboard.current.leftArrowKey.isPressed)
		{
			direction.x = -1f;
		}
		else if (Keyboard.current.rightArrowKey.isPressed)
		{
			direction.x = 1f;
		}
		if (Keyboard.current.downArrowKey.isPressed)
		{
			direction.y = -1f;
		}
		else if (Keyboard.current.upArrowKey.isPressed)
		{
			direction.y = 1f;
		}
		return direction;
	}

	public static bool CheckAlphaInputPressedThisFrame(int alpha)
	{
		bool result = false;
		switch (alpha)
		{
		case 1:
			result = Keyboard.current.digit1Key.wasPressedThisFrame;
			break;
		case 2:
			result = Keyboard.current.digit2Key.wasPressedThisFrame;
			break;
		case 3:
			result = Keyboard.current.digit3Key.wasPressedThisFrame;
			break;
		case 4:
			result = Keyboard.current.digit4Key.wasPressedThisFrame;
			break;
		}
		return result;
	}
}
