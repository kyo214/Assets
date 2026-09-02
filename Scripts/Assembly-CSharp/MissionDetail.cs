using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionDetail : MonoBehaviour
{
	[SerializeField]
	private Localize _termMapName;

	[SerializeField]
	private Image _iconMission;

	[SerializeField]
	private Localize _termMissionMode;

	[SerializeField]
	private Localize _termMissionModeDesc;

	[SerializeField]
	private TMP_Text _textMissionModeDesc;

	[SerializeField]
	private Localize _termObjective;

	[SerializeField]
	private Localize _termMissionBriefing;

	[SerializeField]
	private List<Image> _listPossibleLoot = new List<Image>();

	[SerializeField]
	private GameObject _content;

	[SerializeField]
	private SO_MissionMap _selectedMissionData;

	[SerializeField]
	private MissionSelection _selectedMissionUI;

	[SerializeField]
	private List<Image> _listSkullImages = new List<Image>();

	[SerializeField]
	private UIButton _buttonConfirm;

	[SerializeField]
	private Localize _termBtnConfirm;

	[SerializeField]
	private RectTransform _mapImage;

	public bool IsVisible;

	public void ShowUI(SO_MissionMap missionData, MissionSelection missionUI)
	{
		_buttonConfirm.Select();
		if (!NetworkGameManager.Instance.isServer)
		{
			_termBtnConfirm.SetTerm("Menu/Vote");
		}
		IsVisible = true;
		_content.SetActive(value: true);
		_termMapName.SetTerm(missionData.MapNameLocalization);
		MissionLobbyManager.Instance.MapDesign.SetActive(value: true);
		Transform transform = null;
		foreach (MapDesignSelection item in MissionLobbyManager.Instance.ListMapDesign)
		{
			item.mapDesign.SetActive(value: false);
		}
		foreach (MapDesignSelection item2 in MissionLobbyManager.Instance.ListMapDesign)
		{
			if (!(item2.missionData.SceneName == missionData.SceneName))
			{
				continue;
			}
			if ((bool)item2.mapDesign)
			{
				if (item2.ObjectiveMark == null)
				{
					item2.ObjectiveMark = item2.mapDesign.GetComponentInChildren<ObjectiveMark>()?.gameObject;
				}
				if (missionData.MissionObjective.IsCarRepairingOnStart || missionData.MissionObjective.MinTargetDestroy > 0)
				{
					item2.ObjectiveMark?.SetActive(value: false);
				}
				else
				{
					item2.ObjectiveMark?.SetActive(value: true);
				}
				item2.mapDesign.SetActive(value: true);
				transform = item2.mapDesign.transform;
			}
			break;
		}
		if (transform != null && transform.childCount > 1)
		{
			if (missionData.MissionObjective.IsSpawnEndlessHordeFromBeginning)
			{
				for (int i = 0; i < transform.GetChild(1).childCount; i++)
				{
					if (transform.GetChild(1).GetChild(i).name.IndexOf("Spawning", StringComparison.Ordinal) >= 0)
					{
						if (transform.GetChild(1).GetChild(i).name.IndexOf("DEFEND", StringComparison.Ordinal) >= 0)
						{
							transform.GetChild(1).GetChild(i).gameObject.SetActive(value: true);
						}
						else
						{
							transform.GetChild(1).GetChild(i).gameObject.SetActive(value: false);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < transform.GetChild(1).childCount; j++)
				{
					if (transform.GetChild(1).GetChild(j).name.IndexOf("Spawning", StringComparison.Ordinal) >= 0)
					{
						if (j != missionData.PlayerSpawningIdx)
						{
							transform.GetChild(1).GetChild(j).gameObject.SetActive(value: false);
						}
						else
						{
							transform.GetChild(1).GetChild(j).gameObject.SetActive(value: true);
						}
					}
				}
			}
		}
		_iconMission.sprite = missionData.MissionObjective.IconSmall;
		_termMissionMode.SetTerm(missionData.MissionObjective.MissionModeLocalization);
		_termMissionModeDesc.SetTerm(missionData.MissionObjective.MissionModeDescLocalization);
		if (missionData.MissionObjective.IsCountdownEndlessHordeEnable)
		{
			int num = missionData.MissionObjective.GetCountdownTimerEndlessHorde(NetworkGameManager.Instance.arrPlayerController.Count) / 60;
			_textMissionModeDesc.text = _textMissionModeDesc.text.Replace("(x)", num.ToString());
		}
		if (missionData.MissionObjective.MinTargetDestroy > 0 && missionData.MissionObjective.TargetType != "")
		{
			_textMissionModeDesc.text = _textMissionModeDesc.text.Replace("(x)", missionData.MissionObjective.MinTargetDestroy.ToString());
		}
		_termObjective.SetTerm(missionData.MissionObjective.MissionObjectiveLocalization);
		_termMissionBriefing.SetTerm(missionData.DescLocalization);
		for (int k = 0; k < 6; k++)
		{
			_listPossibleLoot[k].sprite = null;
			_listPossibleLoot[k].color = new Color(1f, 1f, 1f, 0f);
		}
		for (int l = 0; l < missionData.ListWeapon.Count; l++)
		{
			Image image = _listPossibleLoot[l];
			DataManager instance = DataManager.Instance;
			int weapon = (int)missionData.ListWeapon[l].Weapon;
			image.sprite = instance.GetItemSprite(weapon.ToString());
			_listPossibleLoot[l].color = new Color(1f, 1f, 1f, 1f);
		}
		for (int m = 0; m < _listSkullImages.Count; m++)
		{
			_listSkullImages[m].gameObject.SetActive(value: false);
			if (m < missionData.Difficulty)
			{
				_listSkullImages[m].gameObject.SetActive(value: true);
				_listSkullImages[m].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}
		_selectedMissionData = missionData;
		_selectedMissionUI = missionUI;
		_mapImage.anchoredPosition = new Vector2(0f, 0f);
		_mapImage.localScale = new Vector3(1f, 1f, 1f);
	}

	public void ConfirmMission()
	{
		bool flag = false;
		if (NetworkGameManager.Instance.isServer)
		{
			if (LobbyManager.Instance.LobbyState != LobbyManager.LobbyStateEnum.Car)
			{
				flag = true;
			}
			LobbyManager.Instance.SetLobbyState(LobbyManager.LobbyStateEnum.Car);
			_selectedMissionUI.SetMissionGlobal();
		}
		else if (!NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RPCVoteMission((byte)_selectedMissionData.MissionID);
		}
		CloseUI();
		MissionLobbyManager.Instance.MissionSelect();
		if (flag && NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			MissionLobbyManager.Instance.BackBtnClicked();
		}
	}

	public void CloseUI()
	{
		IsVisible = false;
		foreach (MapDesignSelection item in MissionLobbyManager.Instance.ListMapDesign)
		{
			if ((bool)_selectedMissionUI && item.missionData.MissionID == _selectedMissionUI.MissionData.MissionID)
			{
				if ((bool)item.mapDesign)
				{
					item.mapDesign.SetActive(value: false);
				}
				break;
			}
		}
		MissionLobbyManager.Instance.MapDesign.SetActive(value: false);
		_content.SetActive(value: false);
		MissionLobbyManager.Instance.MissionSelect();
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}
}
