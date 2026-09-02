using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Doozy.Runtime.UIManager.Input;

[AddComponentMenu("Input/Multiplayer Info")]
public class MultiplayerInfo : MonoBehaviour
{
	[SerializeField]
	private bool AutoUpdate = true;

	[SerializeField]
	private int CustomPlayerIndex;

	[SerializeField]
	private bool UseCustomPlayerIndex;

	[SerializeField]
	private PlayerInput PlayerInput;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public bool hasPlayerInput => playerInput != null;

	public PlayerInput playerInput
	{
		get
		{
			return PlayerInput;
		}
		set
		{
			PlayerInput = value;
		}
	}

	public int playerIndex
	{
		get
		{
			if (!useCustomPlayerIndex)
			{
				if (!hasPlayerInput)
				{
					return inputSettings.defaultPlayerIndex;
				}
				return playerInput.playerIndex;
			}
			return customPlayerIndex;
		}
	}

	public bool ignorePlayerIndex => playerIndex == inputSettings.defaultPlayerIndex;

	public int customPlayerIndex
	{
		get
		{
			return CustomPlayerIndex;
		}
		set
		{
			UseCustomPlayerIndex = true;
			CustomPlayerIndex = value;
		}
	}

	public bool useCustomPlayerIndex
	{
		get
		{
			return UseCustomPlayerIndex;
		}
		set
		{
			UseCustomPlayerIndex = value;
		}
	}

	public bool autoUpdate
	{
		get
		{
			return AutoUpdate;
		}
		set
		{
			AutoUpdate = value;
		}
	}

	private void GetReferences()
	{
		if ((object)PlayerInput == null)
		{
			PlayerInput = GetComponent<PlayerInput>();
		}
	}

	private void Reset()
	{
		GetReferences();
	}

	private void Awake()
	{
		if (AutoUpdate)
		{
			UpdateReferences();
		}
	}

	public void UpdateReferences()
	{
		IUseMultiplayerInfo[] componentsInChildren = GetComponentsInChildren<IUseMultiplayerInfo>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			IUseMultiplayerInfo[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetMultiplayerInfo(this);
			}
		}
	}

	public MultiplayerInfo SetAutoUpdate(bool value)
	{
		autoUpdate = value;
		return this;
	}

	public MultiplayerInfo SetPlayerInput(PlayerInput value)
	{
		PlayerInput = value;
		return this;
	}

	public MultiplayerInfo SetCustomPlayerIndex(int value)
	{
		customPlayerIndex = value;
		return this;
	}

	public MultiplayerInfo SetUseCustomPlayerIndex(bool value)
	{
		useCustomPlayerIndex = value;
		return this;
	}
}
