using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using Toked;
using UnityEngine;
using _Modules.UIResult.Scripts;

public class UIFinalResultManager : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TxtPlayTime;

	[SerializeField]
	public TextMeshProUGUI TxtTotalScore;

	[SerializeField]
	public List<UIPlayerFinalResultPanel> listPlayerResult = new List<UIPlayerFinalResultPanel>();

	[SerializeField]
	public GameObject _titleObject;

	[SerializeField]
	public GameObject _timerObject;

	[SerializeField]
	public GameObject ScoreObject;

	[SerializeField]
	public UIButton _btnBack;

	[SerializeField]
	public GameObject _NonExpoTitleObject;

	[SerializeField]
	public GameObject _ExpoTitleObject;

	[SerializeField]
	public GameObject _QrCode;

	private bool _winCondition;

	public static UIFinalResultManager Instance { get; private set; }

	public void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void Init(bool winCondition, int life)
	{
		if (GameModes.Instance.isEvent)
		{
			_NonExpoTitleObject.SetActive(value: false);
			_ExpoTitleObject.SetActive(value: true);
			_QrCode.SetActive(value: true);
		}
		_winCondition = winCondition;
		if ((bool)GameManagerPhoton.Instance)
		{
			int num = 0;
			float totalMissionTime = GameManagerPhoton.Instance.TotalMissionTime;
			TxtPlayTime.text = "\" " + MathFunc.GetHour(totalMissionTime).ToString("00") + " : " + MathFunc.GetMinuteHour(totalMissionTime).ToString("00") + " : " + MathFunc.GetSecond(totalMissionTime).ToString("00") + " \"";
			{
				foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
				{
					listPlayerResult[num].gameObject.SetActive(value: true);
					listPlayerResult[num].Init(item, life);
					num++;
				}
				return;
			}
		}
		UIGameManager.Instance.uiFailedConnect.Hide();
		ChangeScene();
	}

	public void ChangeScene()
	{
		if (SteamManager.Initialized)
		{
			SteamManager.Instance.SteamLeaderBoard.UpdateRankIndividual();
		}
		if (NetworkGameManager.Instance.photonNetworking != null)
		{
			NetworkGameManager.Instance.Shutdown();
			Object.Destroy(NetworkGameManager.Instance.photonNetworking.gameObject);
		}
		if (_winCondition && !GameModes.Instance.isDemo && !GameModes.Instance.isEvent)
		{
			UIGameManager.Instance.uiFailedConnect.Hide();
			GlobalUIManager.Instance.ClickGoToScene("AfterCreditResult");
		}
		else if (GameModes.Instance.isInitDemo)
		{
			GlobalUIManager.Instance.ClickGoToScene("MainMenuFriendPass");
		}
		else
		{
			GlobalUIManager.Instance.ClickGoToScene("MainMenu");
		}
		GlobalSaveData.instance.optionData.lastRoomCode = "";
		GlobalSaveData.instance.optionData.lastSeed = 0;
		GlobalSaveData.instance.SaveOptionData();
		AudioManager.PlaySFX("ui_cancel");
	}
}
