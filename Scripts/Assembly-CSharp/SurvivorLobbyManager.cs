using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.UIMission.Scripts;

public class SurvivorLobbyManager : MonoBehaviour
{
	public UIView UIMenu;

	public Localize _txtReady;

	[SerializeField]
	private TextMeshProUGUI _txtTimer;

	[SerializeField]
	private List<SurvivorLobby> _listSurvivorLobby = new List<SurvivorLobby>();

	[SerializeField]
	private UIButton _btnBack;

	[SerializeField]
	private UIButton _btnReady;

	[SerializeField]
	private GameObject _btnInviteFriends;

	public Localize _txtLocation;

	public Localize _txtMission;

	[SerializeField]
	public SteamFriendView _steamFriendView;

	[SerializeField]
	private Image _objectiveImg;

	[SerializeField]
	private List<UIIconModifier> _listIconModifier = new List<UIIconModifier>();

	public static SurvivorLobbyManager Instance { get; private set; }

	public SteamFriendView GetSteamFriendView()
	{
		return _steamFriendView;
	}

	private void Awake()
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

	private void Start()
	{
		if ((object)UIMenu == null)
		{
			UIMenu = GetComponent<UIView>();
		}
	}

	public void Show()
	{
		_txtTimer.enabled = false;
		SetActiveInviteFriends(seActive: true);
		ShowBoard();
	}

	public void SetMission()
	{
		UIGameManager.Instance.SetMissionLocation(_txtLocation, _txtMission);
	}

	public void ShowBoard()
	{
		if (UIMenu.isHidden)
		{
			return;
		}
		SetMission();
		for (int i = 0; i < 4; i++)
		{
			_listSurvivorLobby[i].gameObject.SetActive(value: false);
			_listSurvivorLobby[i].BtnKick.gameObject.SetActive(value: true);
			_listSurvivorLobby[i].BtnWarn.gameObject.SetActive(value: true);
			_listSurvivorLobby[i].BtnWarnDisable.gameObject.SetActive(value: false);
			if (i == 0)
			{
				_listSurvivorLobby[i].BtnKick.gameObject.SetActive(value: false);
				_listSurvivorLobby[i].BtnWarn.gameObject.SetActive(value: false);
				_listSurvivorLobby[i].BtnWarnDisable.gameObject.SetActive(value: false);
			}
			if (NetworkGameManager.Instance.isServer)
			{
				Navigation navigation = _listSurvivorLobby[i].BtnWarn.navigation;
				navigation.selectOnRight = _listSurvivorLobby[i].BtnKick;
				_listSurvivorLobby[i].BtnWarn.navigation = navigation;
				Navigation navigation2 = _listSurvivorLobby[i].BtnKick.navigation;
				navigation2.selectOnLeft = _listSurvivorLobby[i].BtnWarn;
				_listSurvivorLobby[i].BtnKick.navigation = navigation2;
			}
			if (!NetworkGameManager.Instance.isServer)
			{
				_listSurvivorLobby[i].BtnKick.gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < NetworkGameManager.Instance.arrPlayerNetworkController.Count; j++)
		{
			PlayerController playerController = NetworkGameManager.Instance.arrPlayerNetworkController[j];
			if ((bool)playerController)
			{
				int num = playerController.network.GetIDX();
				if (playerController.network.isLocalPlayer)
				{
					num = 0;
				}
				else if (num == 0)
				{
					num = NetworkGameManager.Instance.ownPlayer.network.GetIDX();
				}
				SurvivorLobby survivorLobby = _listSurvivorLobby[num];
				survivorLobby.gameObject.SetActive(value: true);
				survivorLobby.Init(playerController, j);
				SetNavigation(j);
			}
		}
		Navigation navigation3 = _btnBack.navigation;
		for (int num2 = NetworkGameManager.Instance.arrPlayerNetworkController.Count - 1; num2 >= 0; num2--)
		{
			if ((bool)_listSurvivorLobby[num2].Player && !_listSurvivorLobby[num2].Player.network.GetReadyLobby() && !_listSurvivorLobby[num2].Player.network.isLocalPlayer)
			{
				navigation3.selectOnUp = _listSurvivorLobby[num2].BtnWarn;
				break;
			}
		}
		_btnBack.navigation = navigation3;
		if (NetworkGameManager.Instance.isServer)
		{
			Navigation navigation4 = _btnReady.navigation;
			for (int num3 = NetworkGameManager.Instance.arrPlayerNetworkController.Count - 1; num3 >= 0; num3--)
			{
				if ((bool)_listSurvivorLobby[num3].Player)
				{
					navigation4.selectOnUp = _listSurvivorLobby[num3].BtnKick;
					break;
				}
			}
			_btnReady.navigation = navigation4;
		}
		else
		{
			Navigation navigation5 = _btnReady.navigation;
			for (int num4 = NetworkGameManager.Instance.arrPlayerNetworkController.Count - 1; num4 >= 0; num4--)
			{
				if ((bool)_listSurvivorLobby[num4].Player && !_listSurvivorLobby[num4].Player.network.GetReadyLobby() && !_listSurvivorLobby[num4].Player.network.isLocalPlayer)
				{
					navigation5.selectOnUp = _listSurvivorLobby[num4].BtnWarn;
					break;
				}
			}
			_btnReady.navigation = navigation5;
		}
		if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
		{
			_objectiveImg.sprite = GameManagerPhoton.Instance.CurrentMission.MissionObjective.IconSticker;
			for (int k = 0; k < _listIconModifier.Count; k++)
			{
				_listIconModifier[k].gameObject.SetActive(value: false);
			}
			for (int l = 0; l < GameManagerPhoton.Instance.CurrentMission.ListModifier.Count; l++)
			{
				_listIconModifier[l].gameObject.SetActive(value: true);
				_listIconModifier[l].Init(GameManagerPhoton.Instance.CurrentMission.ListModifier[l]);
			}
		}
		void SetNavigation(int num5)
		{
			Navigation navigation6 = _listSurvivorLobby[num5].BtnWarn.navigation;
			if (num5 > 0)
			{
				for (int num6 = num5 - 1; num6 >= 0; num6--)
				{
					if ((bool)_listSurvivorLobby[num6].Player && !_listSurvivorLobby[num6].Player.network.GetReadyLobby())
					{
						navigation6.selectOnUp = _listSurvivorLobby[num6].BtnWarn;
						break;
					}
				}
			}
			if (num5 != 0)
			{
				bool flag = false;
				for (int m = num5 + 1; m < NetworkGameManager.Instance.arrPlayerNetworkController.Count; m++)
				{
					if ((bool)_listSurvivorLobby[m].Player && !_listSurvivorLobby[m].Player.network.GetReadyLobby())
					{
						navigation6.selectOnDown = _listSurvivorLobby[m].BtnWarn;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (NetworkGameManager.Instance.isServer)
					{
						navigation6.selectOnDown = _btnBack;
					}
					else
					{
						navigation6.selectOnDown = _btnReady;
					}
				}
			}
			if (num5 == NetworkGameManager.Instance.arrPlayerController.Count + NetworkGameManager.Instance.arrPlayerDisconnected.Count - 1)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					navigation6.selectOnDown = _btnBack;
				}
				else
				{
					navigation6.selectOnDown = _btnReady;
				}
			}
			_listSurvivorLobby[num5].BtnWarn.navigation = navigation6;
			if (NetworkGameManager.Instance.isServer)
			{
				Navigation navigation7 = _listSurvivorLobby[num5].BtnKick.navigation;
				if (num5 > 0)
				{
					navigation7.selectOnUp = _listSurvivorLobby[num5 - 1].BtnKick;
				}
				if (num5 < NetworkGameManager.Instance.arrPlayerNetworkController.Count - 2 && num5 != 0)
				{
					navigation7.selectOnDown = _listSurvivorLobby[num5 + 1].BtnKick;
				}
				if (num5 >= 1)
				{
					if (num5 == NetworkGameManager.Instance.arrPlayerController.Count + NetworkGameManager.Instance.arrPlayerDisconnected.Count - 1)
					{
						navigation7.selectOnDown = _btnReady;
					}
					else if (num5 < _listSurvivorLobby.Count - 1)
					{
						navigation7.selectOnDown = _listSurvivorLobby[num5 + 1].BtnKick;
					}
				}
				_listSurvivorLobby[num5].BtnKick.navigation = navigation7;
			}
		}
	}

	private void FixedUpdate()
	{
		if (UIMenu.isHidden)
		{
			return;
		}
		if (LobbyManager.Instance.timerCountDown.isRunning)
		{
			_txtTimer.enabled = true;
			SetActiveInviteFriends(seActive: false);
			if (_txtTimer.text != Mathf.FloorToInt(LobbyManager.Instance.timerCountDown.interval).ToString())
			{
				_txtTimer.text = LocalizationManager.GetTranslation("Menu/MissionStart") + " " + Mathf.FloorToInt(LobbyManager.Instance.timerCountDown.interval);
			}
		}
		else
		{
			SetActiveInviteFriends(seActive: true);
			_txtTimer.enabled = false;
		}
	}

	public void ReadyBtnClicked(UIButton button)
	{
		if (NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby())
		{
			_txtReady.SetTerm("Menu/Ready");
		}
		else
		{
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				_txtReady.SetTerm("Menu/NotReady");
			}).Forget();
		}
		UniTaskUtil.DelayedCall(this, 0.2f, () =>
		{
			SelectButton(button);
		}).Forget();
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerReady(!NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby());
	}

	public void BackBtnClicked()
	{
		UIMenu.Hide();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		EventSystem.current.SetSelectedGameObject(null);
		NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
		if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: true);
		}
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}

	public void ShowFriendView()
	{
		_steamFriendView?.Show();
	}

	public void ChangeAvatar(int indexPlayer, PlayerSkinData playerSkinData)
	{
		_listSurvivorLobby[indexPlayer]?.ChangeAvatar(playerSkinData);
	}

	private void SetActiveInviteFriends(bool seActive)
	{
	}
}
