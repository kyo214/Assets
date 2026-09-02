using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[Serializable]
public class MMInputExecutionBinding
{
	public Key TargetInputKey = Key.Space;

	public UnityEvent OnKeyDown;

	public UnityEvent OnKey;

	public UnityEvent OnKeyUp;

	public virtual void ProcessInput()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		flag = Keyboard.current[TargetInputKey].isPressed;
		flag2 = Keyboard.current[TargetInputKey].wasPressedThisFrame;
		flag3 = Keyboard.current[TargetInputKey].wasReleasedThisFrame;
		if (OnKey != null && flag)
		{
			OnKey.Invoke();
		}
		if (OnKeyDown != null && flag2)
		{
			OnKeyDown.Invoke();
		}
		if (OnKeyUp != null && flag3)
		{
			OnKeyUp.Invoke();
		}
	}
}
