using Toked;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScene : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	public void ClickButton(string sceneName)
	{
		GlobalUIManager.Instance.ClickGoToScene(sceneName);
		AudioManager.PlaySFX("ui_cancel");
	}

	public void BackToMainMenu(bool isRemovePlayer)
	{
		if (isRemovePlayer)
		{
			if (NetworkGameManager.Instance.photonNetworking != null)
			{
				NetworkGameManager.Instance.Shutdown();
				Object.Destroy(NetworkGameManager.Instance.photonNetworking.gameObject);
			}
			else if (!NetworkGameManager.Instance.isServer)
			{
				GlobalUIManager.Instance.ClickGoToScene("MainMenu");
			}
		}
		GlobalSaveData.instance.optionData.lastRoomCode = "";
		GlobalSaveData.instance.optionData.lastSeed = 0;
		GlobalSaveData.instance.SaveOptionData();
		AudioManager.PlaySFX("ui_cancel");
	}

	public void BackToLobby()
	{
		GlobalUIManager.Instance.ClickGoToScene("Lobby");
		AudioManager.PlaySFX("ui_confirm");
		NetworkGameManager.Instance.ownPlayer.playerInput.enabled = true;
	}

	public void QuitButton()
	{
		Debug.Log("Quitting");
		Application.Quit();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		AudioManager.PlaySFX("ui_select");
	}
}
