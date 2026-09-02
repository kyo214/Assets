using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

namespace HighlightPlus;

public static class InputProxy
{
	private static Vector3 lastPointerPosition;

	public static Vector3 mousePosition
	{
		get
		{
			if (touchCount > 0)
			{
				lastPointerPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
			}
			else
			{
				Mouse current = Mouse.current;
				if (current != null)
				{
					lastPointerPosition = current.position.ReadValue();
				}
			}
			return lastPointerPosition;
		}
	}

	public static int touchCount => UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;

	public static void Init()
	{
		if (!EnhancedTouchSupport.enabled)
		{
			EnhancedTouchSupport.Enable();
		}
	}

	public static bool GetMouseButtonDown(int buttonIndex)
	{
		if (touchCount > 0)
		{
			return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began;
		}
		Mouse current = Mouse.current;
		if (current == null)
		{
			return false;
		}
		return buttonIndex switch
		{
			1 => current.rightButton.wasPressedThisFrame, 
			2 => current.middleButton.wasPressedThisFrame, 
			_ => current.leftButton.wasPressedThisFrame, 
		};
	}

	public static int GetFingerIdFromTouch(int touchIndex)
	{
		return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[touchIndex].finger.index;
	}

	public static bool GetKeyDown(string name)
	{
		return ((KeyControl)Keyboard.current[name]).wasPressedThisFrame;
	}
}
