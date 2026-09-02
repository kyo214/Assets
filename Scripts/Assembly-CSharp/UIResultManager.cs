using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using I2.Loc;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.UIResult.Scripts;

public class UIResultManager : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI txtTimes;

	[SerializeField]
	private int timeScore;

	[SerializeField]
	private GameObject winGameObject;

	[SerializeField]
	private GameObject loseGameObject;

	[SerializeField]
	private Image _fadeBlack;

	[SerializeField]
	private GameObject uiPressKey;

	[SerializeField]
	private bool _isClicked;

	[SerializeField]
	private GameObject _winStatusPanel;

	[SerializeField]
	private GameObject _totalLootPanel;

	[SerializeField]
	private GameObject _extraBonusPanel;

	[SerializeField]
	private GameObject _timerPanel;

	[SerializeField]
	private UIFinalResultManager _finalResultManager;

	[SerializeField]
	private RectTransform _playerResultPanel;

	[SerializeField]
	private List<UIPlayerResultPanel> _playerResultPanelUiList;

	[SerializeField]
	private UIMaterialResultPanel _materialTotalResultPanel;

	[SerializeField]
	private ResultExtractionBonusUI _resultExtractionBonusUI;

	[SerializeField]
	private UILoseResult _uiLoseResult;

	private Dictionary<string, MaterialInventoryData> _totalMaterialFromPlayer = new Dictionary<string, MaterialInventoryData>();

	private Dictionary<string, MaterialInventoryData> _totalMaterialFromMap = new Dictionary<string, MaterialInventoryData>();

	private Dictionary<string, MaterialInventoryData> _totalMaterial = new Dictionary<string, MaterialInventoryData>();

	[SerializeField]
	private int _life;

	[SerializeField]
	public SO_MissionMap _resultMission;

	[SerializeField]
	private List<int> _listPlayerID = new List<int>();

	private int _idxresultMission;

	private List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialList = new List<ItemToCraftMaterialConverter.ConvertMaterialItemData>();

	private bool _initCalculateTotalMaterialPlayerGet;

	public static UIResultManager Instance { get; private set; }

	public UILoseResult UILoseResult => _uiLoseResult;

	public bool WinCondition { get; set; }

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
		if (GameManagerPhoton.Instance.Life > 0)
		{
			SetData();
		}
	}

	private void Start()
	{
		if (GameManagerPhoton.Instance.Life <= 0 || !GameManagerPhoton.Instance.IsRandomizeMapOnDefeat || !GameManagerPhoton.Instance || WinCondition)
		{
			return;
		}
		if (GameManagerPhoton.Instance.Phase == 0)
		{
			GlobalSaveData.instance.optionData.lastSeed--;
			GlobalOptionsManager.Instance.seed--;
			if (GlobalSaveData.instance.gameData != null)
			{
				GlobalSaveData.instance.gameData.Seed--;
			}
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.Seed--;
			}
		}
		if (!MissionLobbyManager.Instance || !NetworkGameManager.Instance.isServer)
		{
			return;
		}
		foreach (MissionSelection item in MissionLobbyManager.Instance.MissionBoard.AllMissionSelection)
		{
			if (item.Phase >= GameManagerPhoton.Instance.Phase)
			{
				item.IsCleared = false;
				if (item.MissionData.IsFixedMissionObjective)
				{
					item.MissionData.IsLocked = true;
				}
				else
				{
					item.MissionData = null;
				}
			}
		}
		GameManagerPhoton.Instance.isInitializedLockedMap = false;
		GameManagerPhoton.Instance.isInitializedRandomizeWeapon = false;
		MissionLobbyManager.Instance.InitMap(isSetClearNotNullMission: false, isCheckingClearedMission: true, _life);
	}

	public void InitUI()
	{
		timeScore = 0;
		_winStatusPanel.SetActive(value: false);
		_timerPanel.SetActive(value: false);
		_extraBonusPanel.SetActive(value: false);
		txtTimes.text = "\" 00 : 00 : 00 \"";
		_totalLootPanel.SetActive(value: false);
	}

	public void ResetFadeBlack()
	{
		_fadeBlack.DOFade(0f, 0f);
	}

	public IEnumerator ShowUIResult()
	{
		if (GameManagerPhoton.Instance.Life > 0)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["status"] = "Open" };
				PhotonMultiplayerManager.Instance._runner.SessionInfo.UpdateCustomProperties(customProperties);
			}
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (item != null)
				{
					_listPlayerID.Add(item.network.GetIDX());
					item.network.isDeadResult = item.network.IsDead();
					if (NetworkGameManager.Instance.isServer)
					{
						item.network.playerPhoton.MissionVote = 0;
					}
				}
			}
			SetUI();
			if (NetworkGameManager.Instance.isServer && _life > 0 && !_resultMission.IsLastMap)
			{
				GameManagerPhoton.Instance.Mission = 0;
			}
			CalculateTotalMaterialPlayerGet();
			SetUnlockedMap();
			ResetData();
			GlobalSaveData.instance.SaveGameData(NetworkGameManager.Instance.ownPlayer, GameManagerPhoton.Instance);
			yield return new WaitForSeconds(0.5f);
			yield return _fadeBlack.DOFade(0f, 0.5f).SetDelay(0.9f).WaitForCompletion();
			yield return new WaitForSeconds(0.2f);
			_winStatusPanel.SetActive(value: true);
			yield return new WaitForSeconds(0.2f);
			_timerPanel.SetActive(value: true);
			yield return new WaitForSeconds(0.4f);
			int timeScoreTemp = 0;
			yield return DOTween.To(() => timeScore, (int x) =>
			{
				timeScore = x;
			}, ScoreManager.Instance.time, 1f).OnUpdate(() =>
			{
				if (timeScoreTemp != timeScore)
				{
					AudioManager.PlaySFX("ui_advChar");
					txtTimes.text = "\" " + MathFunc.GetHour(timeScore).ToString("00") + " : " + MathFunc.GetMinuteHour(timeScore).ToString("00") + " : " + MathFunc.GetSecond(timeScore).ToString("00") + " \"";
					timeScoreTemp = timeScore;
				}
			}).WaitForCompletion();
			yield return new WaitForSeconds(0.5f);
			_totalLootPanel.SetActive(value: true);
			_extraBonusPanel.SetActive(value: true);
			if ((WinCondition && _resultMission.Difficulty == 2 && (GameModes.Instance.isDemo || GameModes.Instance.isEvent)) || _life <= 0 || (WinCondition && _resultMission.IsLastMap))
			{
				_finalResultManager.Init(WinCondition, _life);
			}
			yield return SetExtractionBonusUI();
			yield return SetPlayerResultUI();
			yield return new WaitForSeconds(1f);
			if (NetworkGameManager.Instance.ownPlayer != null && NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton != null)
			{
				NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.SetPosition(GameManager.Instance.MapManager.GetSpawnPosition(0, NetworkGameManager.Instance.ownPlayer.network.GetIDX()));
			}
			yield return new WaitForSeconds(1.5f);
			if (NetworkGameManager.Instance.ownPlayer != null && NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton != null)
			{
				NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.SetPosition(GameManager.Instance.MapManager.GetSpawnPosition(0, NetworkGameManager.Instance.ownPlayer.network.GetIDX()));
			}
			if (NetworkGameManager.Instance.isServer)
			{
				if (WinCondition)
				{
					if ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(_resultMission.MissionID).isWinGetCheckpoint)
					{
						GameManagerPhoton.Instance.Phase = (byte)(MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(_resultMission.MissionID).Phase + 1);
					}
				}
				else
				{
					GameManagerPhoton.Instance.RemoveLife();
				}
			}
			uiPressKey.SetActive(value: true);
			yield return WaitUntilSkipped();
			if (!NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.IsRandomizeMapOnDefeat && !WinCondition)
			{
				GameManagerPhoton.Instance.isInitializedLockedMap = false;
				GameManagerPhoton.Instance.isInitializedRandomizeWeapon = false;
				MissionLobbyManager.Instance.InitMap(isSetClearNotNullMission: false, isCheckingClearedMission: true, _life);
			}
			if (WinCondition && _resultMission.Difficulty == 2 && (GameModes.Instance.isDemo || GameModes.Instance.isEvent))
			{
				_life = 0;
			}
			yield return NextScene();
		}
		else
		{
			_finalResultManager.Init(WinCondition, _life);
			ResetFadeBlack();
			LobbyManager.Instance.UIResult.SetActive(value: true);
			LobbyManager.Instance.UIResult.GetComponent<Canvas>().enabled = true;
			GlobalSaveData.instance.gameData.IsCompleted = WinCondition;
			StartCoroutine(Instance.FinalResult());
		}
	}

	public IEnumerator FinalResult()
	{
		_fadeBlack.DOFade(1f, 0f);
		_fadeBlack.DOFade(0f, 0.5f);
		_winStatusPanel.SetActive(value: false);
		_timerPanel.SetActive(value: false);
		uiPressKey.SetActive(value: false);
		_totalLootPanel.SetActive(value: false);
		_extraBonusPanel.SetActive(value: false);
		_playerResultPanel.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(0.2f);
		_finalResultManager._titleObject.SetActive(value: true);
		_finalResultManager._timerObject.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.SetUIVisibility(setActiveUI: true);
		GlobalSaveData.instance.SaveGameData(NetworkGameManager.Instance.ownPlayer, GameManagerPhoton.Instance);
		_finalResultManager.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		_finalResultManager.TxtTotalScore.text = ScoreManager.Instance.TotalGameScore.ToString("N0");
		_finalResultManager.ScoreObject.SetActive(value: true);
		_finalResultManager._btnBack.gameObject.SetActive(value: true);
		_finalResultManager._btnBack.Select();
	}

	public IEnumerator NextScene()
	{
		_fadeBlack.DOFade(1f, 0.5f);
		bool isChangeScane = false;
		bool isShowFinalResult = false;
		if (WinCondition)
		{
			if ((GameModes.Instance.isDemo || GameModes.Instance.isEvent) && _life == 0)
			{
				isShowFinalResult = true;
				yield return _uiLoseResult.Show(isShowFinalResult: true);
			}
			else if (_resultMission.IsLastMap)
			{
				isShowFinalResult = true;
				GlobalSaveData.instance.gameData.IsCompleted = WinCondition;
				StartCoroutine(FinalResult());
			}
			else
			{
				UniTaskUtil.DelayedCall(this, 0.2f, () =>
				{
					LobbyManager.Instance.UIResult.GetComponent<Canvas>().enabled = false;
					if (!WinCondition)
					{
						UIGameManager.Instance.SetUIVisibility(setActiveUI: false);
					}
				}).Forget();
			}
		}
		else
		{
			NetworkGameManager.Instance.ListPlayerTempInventory.Clear();
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				LobbyManager.Instance.UIResult.GetComponent<Canvas>().enabled = false;
				if (!WinCondition)
				{
					UIGameManager.Instance.SetUIVisibility(setActiveUI: false);
				}
			}).Forget();
			yield return _uiLoseResult.Show();
		}
		if (_life > 0 && !isChangeScane && !isShowFinalResult)
		{
			UIGameManager.Instance.SetUIVisibility(setActiveUI: true);
			GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer = true;
			LobbyManager.Instance.CanvasLobby.enabled = true;
			NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.gravity = -9.81f;
			if (!NetworkGameManager.Instance.isServer && WinCondition)
			{
				GameManagerPhoton.Instance.SetMissionClear(_idxresultMission - 1);
			}
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.Mission = 0;
				GameManagerPhoton.Instance.CurrentMission = null;
				UIGameManager.Instance.missionMapImage.sprite = UIGameManager.Instance.defaultMapSprite;
				UIGameManager.Instance.missionLocationTextField.text = LocalizationManager.GetTranslation("Menu/Mission").ToUpper() + " : ---";
			}
			else if (GameManagerPhoton.Instance.Mission == 0)
			{
				UIGameManager.Instance.missionLocationTextField.text = LocalizationManager.GetTranslation("Menu/Mission").ToUpper() + " : ---";
				UIGameManager.Instance.missionMapImage.sprite = UIGameManager.Instance.defaultMapSprite;
			}
			if (!NetworkGameManager.Instance.isServer)
			{
				NetworkGameManager.Instance.ownPlayer.network.ExecSyncInventoryLocalPlayerToAll();
			}
			LobbyManager.Instance.StartLobby();
			UIGameManager.Instance.sessionName?.transform.parent.gameObject.SetActive(value: true);
			MissionLobbyManager.Instance?.RandomizeIdxSpawnPlayer();
		}
		yield return new WaitForSeconds(0.3f);
		if ((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.Life > 0 && !isChangeScane && !isShowFinalResult)
		{
			LobbyManager.Instance.UIResult.SetActive(value: false);
			LobbyManager.Instance.UIResult.GetComponent<Canvas>().enabled = true;
		}
	}

	public IEnumerator DelaySocialGame()
	{
		yield return new WaitForSeconds(1f);
		NetworkGameManager.Instance.ownPlayer.audioListener.enabled = false;
		if (GameModes.Instance.isEvent)
		{
			GlobalUIManager.Instance.ClickGoToScene("SocialMediaNonEarlyAccess");
		}
		else
		{
			GlobalUIManager.Instance.ClickGoToScene("SocialMediaEarlyAccess");
		}
	}

	public IEnumerator GoToLastSurvivorEvent()
	{
		yield return new WaitForSeconds(1f);
		GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer = true;
		NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.gravity = -9.81f;
		GameManagerPhoton.Instance.CurrentMission = GameManagerPhoton.Instance.MissionLastSurvivor;
		LobbyManager.Instance.LoadInGameScene();
	}

	private void SetUI()
	{
		if (!GameManagerPhoton.Instance)
		{
			return;
		}
		_playerResultPanel.gameObject.SetActive(value: false);
		_materialTotalResultPanel.Init();
		_fadeBlack.enabled = true;
		AudioManager.ChangeLowPass(22000f);
		if (WinCondition)
		{
			winGameObject.SetActive(value: true);
			loseGameObject.SetActive(value: false);
			txtTimes.color = new Color(1f, 1f, 0.18f);
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.SetMissionClear(_idxresultMission - 1);
			}
		}
		else
		{
			winGameObject.SetActive(value: false);
			loseGameObject.SetActive(value: true);
			txtTimes.color = new Color(1f, 0.18f, 0.18f);
			GameManagerPhoton.Instance.ResetMissionClear(onlyNormalMap: false);
		}
	}

	private void SetData()
	{
		_resultMission = GameManagerPhoton.Instance.CurrentMission;
		_idxresultMission = GameManagerPhoton.Instance.Mission;
		_life = GameManagerPhoton.Instance.Life;
		WinCondition = false;
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if ((bool)item && (bool)item.network.playerPhoton.IsSurvive && !item.network.playerPhoton.disconnected)
			{
				WinCondition = true;
				break;
			}
		}
		if (!WinCondition && NetworkGameManager.Instance.isServer && _life <= 1)
		{
			GameManagerPhoton.Instance.EndTime = Time.time;
		}
		if (NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.TotalMissionTime += ScoreManager.Instance.time;
		}
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			InputManager.DisableInput();
		}
		if (WinCondition)
		{
			_uiLoseResult.PentagramLampController?.Init(_life);
			return;
		}
		_life--;
		_uiLoseResult.Init(_life);
	}

	private void ResetData()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer && !WinCondition && (bool)ItemBoxNetwork.instance)
		{
			ItemBoxNetwork.instance.InitItemBox();
		}
		if (NetworkGameManager.Instance.isServer)
		{
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (item != null)
				{
					item.network.SetHealth(item.data.GetMaxHealth());
				}
			}
		}
		for (int i = 0; i < _listPlayerID.Count; i++)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(_listPlayerID[i]);
			player.DizzinessManager.ClearPoints();
			if (player.network.isDeadResult)
			{
				player.data.initialized = false;
				player.data.arrInventory.Clear();
				player.data.InitInventory(out var _, isInitInventoryPerks: false);
				if (player.network.isLocalPlayer)
				{
					player.data.AddInventory(1);
				}
				if (NetworkGameManager.Instance.isServer)
				{
					player.network.SetWeapon0(1);
				}
			}
		}
	}

	private IEnumerator SetExtractionBonusUI()
	{
		if (_resultExtractionBonusUI.Init(convertMaterialList))
		{
			yield return new WaitForSeconds(1.5f);
			_materialTotalResultPanel.Set(_totalMaterialFromMap);
			yield return new WaitForSeconds(2.5f);
			uiPressKey.SetActive(value: true);
			yield return WaitUntilSkipped();
			uiPressKey.SetActive(value: false);
			_resultExtractionBonusUI.Hide();
			yield return new WaitForSeconds(1.5f);
		}
	}

	private void CalculateTotalAllPlayerMaterial()
	{
		_totalMaterialFromPlayer = new Dictionary<string, MaterialInventoryData>();
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item == null)
			{
				continue;
			}
			item.data.MaterialInventoryManager.SyncInGameMaterialInventory();
			if (WinCondition)
			{
				item.data.AddSkillPoint(_resultMission.SkillPointReward);
			}
			byte iDX = item.network.GetIDX();
			_playerResultPanelUiList[iDX].Init(item);
			if (!GetPlayerAliveStatus(iDX))
			{
				continue;
			}
			foreach (KeyValuePair<string, MaterialInventoryData> inGameMaterialDatum in item.data.MaterialInventoryManager.GetInGameMaterialData())
			{
				if (_totalMaterialFromPlayer.TryGetValue(inGameMaterialDatum.Key, out var value))
				{
					value.Amount += inGameMaterialDatum.Value.Amount;
				}
				else
				{
					_totalMaterialFromPlayer.Add(inGameMaterialDatum.Key, inGameMaterialDatum.Value);
				}
			}
		}
	}

	private void CalculateTotalMaterialFromMap()
	{
		convertMaterialList = new List<ItemToCraftMaterialConverter.ConvertMaterialItemData>();
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item != null)
			{
				convertMaterialList.AddRange(ItemToCraftMaterialConverter.ConvertItemToCraftMaterial(item));
			}
		}
		if (WinCondition)
		{
			AddBonusMapItem(convertMaterialList);
		}
		_totalMaterialFromMap = new Dictionary<string, MaterialInventoryData>();
		if (convertMaterialList.Count > 0)
		{
			_totalMaterialFromMap = CalculateTotalMaterial(convertMaterialList);
		}
		void AddBonusMapItem(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialItemList)
		{
			ItemToCraftMaterialConverter.ConvertMaterialItemData convertMaterialItemData = ItemToCraftMaterialConverter.ConvertItemToCraftMaterial(_resultMission.MissionObjective.MissionKeyItem);
			if (convertMaterialItemData != null)
			{
				convertMaterialItemList.Add(convertMaterialItemData);
			}
		}
		static Dictionary<string, MaterialInventoryData> CalculateTotalMaterial(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialItemList)
		{
			Dictionary<string, MaterialInventoryData> dictionary = new Dictionary<string, MaterialInventoryData>();
			foreach (ItemToCraftMaterialConverter.ConvertMaterialItemData item2 in new List<ItemToCraftMaterialConverter.ConvertMaterialItemData>(convertMaterialItemList))
			{
				foreach (KeyValuePair<string, MaterialInventoryData> item3 in item2.Material)
				{
					if (dictionary.ContainsKey(item3.Key))
					{
						dictionary[item3.Key].Amount += item3.Value.Amount;
					}
					else
					{
						dictionary.Add(item3.Key, new MaterialInventoryData(item3.Value));
					}
				}
			}
			return dictionary;
		}
	}

	private void SetUnlockedMap()
	{
		if (!GameManagerPhoton.Instance.IsRandomizeMapOnDefeat || WinCondition)
		{
			return;
		}
		foreach (MissionSelection item in MissionLobbyManager.Instance.MissionBoard.AllMissionSelection)
		{
			if (item.Phase < GameManagerPhoton.Instance.Phase || !(item.MissionData != null))
			{
				continue;
			}
			item.IsCleared = false;
			if (item.MissionData.IsFixedMissionObjective)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					GameManagerPhoton.Instance.ArrMissionLocked.Set(item.MissionData.MissionID - 1, value: true);
				}
				else
				{
					item.MissionData.IsLocked = true;
				}
			}
			else if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.ArrMissionLocked.Set(item.MissionData.MissionID - 1, value: false);
			}
			else
			{
				item.MissionData = null;
			}
		}
	}

	private Dictionary<string, MaterialInventoryData> CalculateTotalMaterialPlayerGet()
	{
		if (_initCalculateTotalMaterialPlayerGet)
		{
			return _totalMaterial;
		}
		CalculateTotalAllPlayerMaterial();
		CalculateTotalMaterialFromMap();
		_totalMaterial = new Dictionary<string, MaterialInventoryData>();
		foreach (KeyValuePair<string, MaterialInventoryData> item in _totalMaterialFromPlayer)
		{
			if (_totalMaterial.TryGetValue(item.Key, out var value))
			{
				value.Amount += item.Value.Amount;
			}
			else
			{
				_totalMaterial.Add(item.Key, item.Value);
			}
		}
		foreach (KeyValuePair<string, MaterialInventoryData> item2 in _totalMaterialFromMap)
		{
			if (_totalMaterial.TryGetValue(item2.Key, out var value2))
			{
				value2.Amount += item2.Value.Amount;
			}
			else
			{
				_totalMaterial.Add(item2.Key, item2.Value);
			}
		}
		foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			item3?.data.TransferMaterialToMainInventory(_totalMaterial);
		}
		_initCalculateTotalMaterialPlayerGet = true;
		return _totalMaterial;
	}

	private IEnumerator SetPlayerResultUI()
	{
		if ((bool)NetworkGameManager.Instance)
		{
			_playerResultPanel.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(1.5f);
			if (_totalMaterialFromPlayer.Count > 0)
			{
				_materialTotalResultPanel.Set(_totalMaterial);
			}
		}
		else
		{
			yield return new WaitForSeconds(0f);
		}
	}

	private IEnumerator WaitUntilSkipped()
	{
		do
		{
			yield return null;
		}
		while (!GetSkipInput());
	}

	public bool GetSkipInput()
	{
		if (Gamepad.current == null || (!Gamepad.current.buttonWest.wasPressedThisFrame && !Gamepad.current.buttonNorth.wasPressedThisFrame && !Gamepad.current.buttonSouth.wasPressedThisFrame && !Gamepad.current.buttonEast.wasPressedThisFrame))
		{
			if (!Input.GetKey(KeyCode.F3) && Input.anyKey)
			{
				if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
				{
					return !Input.GetMouseButtonDown(2);
				}
				return false;
			}
			return false;
		}
		return true;
	}

	private bool GetPlayerAliveStatus(int index)
	{
		return !NetworkGameManager.Instance.GetPlayer(index).network.isDeadResult;
	}
}
