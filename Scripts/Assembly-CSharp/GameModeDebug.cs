using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameModeDebug : MonoBehaviour
{
	[SerializeField]
	private UIView view;

	private PlayerInputActions playerInputActions;

	public static GameModeDebug Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void OnEnable()
	{
	}

	private void OnDebugPress(InputAction.CallbackContext obj)
	{
		if (NetworkGameManager.Instance.isServer && GameModes.Instance.isDebug)
		{
			if (view.isHidden)
			{
				AudioManager.PlaySFX("ui_confirm");
				UIGameManager.Instance.uiPause.Hide();
				UIGameManager.Instance.uiInventory.Hide();
				UIGameManager.Instance.uiInGame.Hide();
				UIGameManager.Instance.uiOptions.Hide();
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
				view.Show();
			}
			else
			{
				EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
				AudioManager.PlaySFX("ui_cancel");
				UIGameManager.Instance.uiInGame.Show();
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
				view.Hide();
			}
		}
	}

	private void OnDisable()
	{
	}

	public void SetModeDefault()
	{
		GameModes.Instance.modeGame = "Default";
		GameModes.Instance.Init();
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.ModeGame = GameModes.Instance.modeGame;
	}

	public void SetModePVP()
	{
		GameModes.Instance.modeGame = "PVP";
		GameModes.Instance.Init();
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.ModeGame = GameModes.Instance.modeGame;
	}
}
