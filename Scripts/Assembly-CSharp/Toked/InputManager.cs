using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Toked;

public class InputManager : MonoBehaviour
{
	public enum PlayerInputToggleAction
	{
		ENABLE_PLAYER_INPUT = 0,
		DISABLE_PLAYER_INPUT = 1,
		NONE = 2
	}

	public static PlayerInputActions inputActions = new PlayerInputActions();

	public static Gamepad CurrentGamepad => Gamepad.current;

	public static void ToggleActionMap(PlayerInputToggleAction playerInput, params InputActionMap[] actionMaps)
	{
		switch (playerInput)
		{
		case PlayerInputToggleAction.ENABLE_PLAYER_INPUT:
			EnableInput();
			break;
		case PlayerInputToggleAction.DISABLE_PLAYER_INPUT:
			DisableInput();
			break;
		}
		inputActions.Disable();
		for (int i = 0; i < actionMaps.Length; i++)
		{
			actionMaps[i].Enable();
		}
	}

	public static void DisableInput()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.playerInput.DeactivateInput();
		}
		if ((bool)UITitleMenuManager.Instance)
		{
			UITitleMenuManager.Instance.playerInput.DeactivateInput();
		}
	}

	public static void EnableInput()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer && NetworkGameManager.Instance.ownPlayer.playerInput.currentActionMap != null)
		{
			NetworkGameManager.Instance.ownPlayer.playerInput.ActivateInput();
		}
		if ((bool)UITitleMenuManager.Instance)
		{
			UITitleMenuManager.Instance.playerInput.ActivateInput();
		}
	}

	public static bool CheckAnyInput()
	{
		if (!Keyboard.current.anyKey.wasPressedThisFrame)
		{
			return Gamepad.current.IsPressed();
		}
		return true;
	}

	public static IEnumerator Vibrate(float lowFrequency, float highFrequency, float duration)
	{
		StartRumble(lowFrequency, highFrequency);
		float elapsedTime = duration;
		while (elapsedTime >= 0f)
		{
			elapsedTime -= Time.deltaTime;
			yield return null;
		}
		StopRumble();
	}

	public static void StartRumble(float lowFrequency, float highFrequency, Gamepad gamepad = null)
	{
		if (gamepad == null)
		{
			gamepad = CurrentGamepad;
		}
		gamepad?.SetMotorSpeeds(lowFrequency, highFrequency);
	}

	public static void StopRumble(Gamepad gamepad = null)
	{
		if (gamepad == null)
		{
			gamepad = CurrentGamepad;
		}
		gamepad?.SetMotorSpeeds(0f, 0f);
	}
}
