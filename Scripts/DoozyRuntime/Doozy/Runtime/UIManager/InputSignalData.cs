using System;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine.InputSystem;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct InputSignalData
{
	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public int playerIndex { get; }

	public InputAction.CallbackContext callbackContext { get; }

	public PlayerInput playerInput { get; }

	public bool hasPlayerInput => playerInput != null;

	public bool ignorePlayerIndex => playerIndex == inputSettings.defaultPlayerIndex;

	public UIInputActionName inputActionName { get; }

	public InputSignalData(UIInputActionName inputActionName, int playerIndex)
		: this(inputActionName, default, playerIndex)
	{
	}

	public InputSignalData(UIInputActionName inputActionName, InputAction.CallbackContext callbackContext, int playerIndex, PlayerInput playerInput = null)
	{
		this.inputActionName = inputActionName;
		this.callbackContext = callbackContext;
		this.playerIndex = playerIndex;
		this.playerInput = playerInput;
	}

	public override string ToString()
	{
		string text = ((callbackContext.action != null) ? ("'" + callbackContext.action.name + "'") : inputActionName.ToString());
		if (multiplayerMode && !ignorePlayerIndex)
		{
			text += $" called by Player {playerIndex}";
		}
		return text;
	}
}
